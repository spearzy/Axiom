using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using Axiom.Core.Comparison;
using Axiom.Core.Configuration;

namespace Axiom.Assertions.Equivalency;

internal static class EquivalencyEngine
{
    // Cache per runtime comparison type so we pay reflection cost only once per type.
    private static readonly ConcurrentDictionary<Type, Func<IComparerProvider, object, object, bool?>> ComparerInvokers = new();

    public static IReadOnlyList<EquivalencyDifference> Compare(
        object? actual,
        object? expected,
        string rootPath,
        EquivalencyOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);
        ArgumentNullException.ThrowIfNull(options);

        var differences = new List<EquivalencyDifference>();
        var visitedPairs = new HashSet<ReferencePair>(ReferencePairComparer.Instance);
        CompareNode(actual, expected, rootPath, options, differences, visitedPairs);
        return differences;
    }

    private static void CompareNode(
        object? actual,
        object? expected,
        string path,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
    {
        if (IsPathIgnored(path, options))
        {
            return;
        }

        if (ReferenceEquals(actual, expected))
        {
            return;
        }

        if (actual is null || expected is null)
        {
            differences.Add(new EquivalencyDifference(path, expected, actual, "One value was <null> and the other was not."));
            return;
        }

        var actualType = actual.GetType();
        var expectedType = expected.GetType();
        if (!AreTypesCompatible(actualType, expectedType, options))
        {
            differences.Add(new EquivalencyDifference(
                path,
                expected,
                actual,
                $"Runtime types differ: expected {expectedType.FullName}, but found {actualType.FullName}."));
            return;
        }

        if (actual is string actualString && expected is string expectedString)
        {
            if (!string.Equals(actualString, expectedString, options.StringComparison))
            {
                differences.Add(new EquivalencyDifference(path, expected, actual, "String values differ."));
            }

            return;
        }

        if (actual is IEnumerable actualEnumerable &&
            expected is IEnumerable expectedEnumerable &&
            actual is not string &&
            expected is not string)
        {
            CompareEnumerable(actualEnumerable, expectedEnumerable, path, options, differences, visitedPairs);
            return;
        }

        if (IsLeafType(actualType) || IsLeafType(expectedType))
        {
            if (!LeafValuesEquivalent(actual, expected, actualType, expectedType))
            {
                differences.Add(new EquivalencyDifference(path, expected, actual, "Values differ."));
            }

            return;
        }

        if (!actualType.IsValueType && !expectedType.IsValueType)
        {
            // Cycles should not fail: if this pair was already compared, stop walking here.
            var pair = new ReferencePair(actual, expected);
            if (!visitedPairs.Add(pair))
            {
                return;
            }
        }

        CompareMembers(actual, expected, actualType, expectedType, path, options, differences, visitedPairs);
    }

    private static void CompareMembers(
        object actual,
        object expected,
        Type actualType,
        Type expectedType,
        string path,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
    {
        var actualMembers = GetComparableMembers(actualType, options);
        var expectedMembers = GetComparableMembers(expectedType, options);

        var memberNames = actualMembers.Keys
            .Concat(expectedMembers.Keys)
            .Distinct(StringComparer.Ordinal)
            // Stable ordering keeps failure output deterministic across runtimes.
            .OrderBy(static name => name, StringComparer.Ordinal);

        foreach (var memberName in memberNames)
        {
            if (options.IgnoredMemberNames.Contains(memberName))
            {
                continue;
            }

            var memberPath = $"{path}.{memberName}";
            if (IsPathIgnored(memberPath, options))
            {
                continue;
            }

            var hasActual = actualMembers.TryGetValue(memberName, out var actualGetter);
            var hasExpected = expectedMembers.TryGetValue(memberName, out var expectedGetter);

            if (!hasActual && hasExpected)
            {
                if (!options.FailOnMissingMembers)
                {
                    continue;
                }

                var expectedValue = expectedGetter!(expected);
                differences.Add(new EquivalencyDifference(memberPath, expectedValue, null, "Member missing on actual type."));
                continue;
            }

            if (hasActual && !hasExpected)
            {
                if (!options.FailOnExtraMembers)
                {
                    continue;
                }

                var actualValue = actualGetter!(actual);
                differences.Add(new EquivalencyDifference(memberPath, null, actualValue, "Member missing on expected type."));
                continue;
            }

            var actualValueAtMember = actualGetter!(actual);
            var expectedValueAtMember = expectedGetter!(expected);
            CompareNode(actualValueAtMember, expectedValueAtMember, memberPath, options, differences, visitedPairs);
        }
    }

    private static void CompareEnumerable(
        IEnumerable actualEnumerable,
        IEnumerable expectedEnumerable,
        string path,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
    {
        var actualItems = actualEnumerable.Cast<object?>().ToList();
        var expectedItems = expectedEnumerable.Cast<object?>().ToList();

        if (options.CollectionOrder == EquivalencyCollectionOrder.Any)
        {
            CompareEnumerableAnyOrder(actualItems, expectedItems, path, options, differences);
            return;
        }

        var sharedCount = Math.Min(actualItems.Count, expectedItems.Count);
        for (var index = 0; index < sharedCount; index++)
        {
            CompareNode(actualItems[index], expectedItems[index], $"{path}[{index}]", options, differences, visitedPairs);
        }

        for (var index = sharedCount; index < expectedItems.Count; index++)
        {
            differences.Add(new EquivalencyDifference(
                $"{path}[{index}]",
                expectedItems[index],
                null,
                "Item missing on actual collection."));
        }

        for (var index = sharedCount; index < actualItems.Count; index++)
        {
            differences.Add(new EquivalencyDifference(
                $"{path}[{index}]",
                null,
                actualItems[index],
                "Extra item on actual collection."));
        }
    }

    private static void CompareEnumerableAnyOrder(
        List<object?> actualItems,
        List<object?> expectedItems,
        string path,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences)
    {
        // Simple one-pass matching: each actual element can satisfy at most one expected element.
        var usedActualIndexes = new bool[actualItems.Count];

        for (var expectedIndex = 0; expectedIndex < expectedItems.Count; expectedIndex++)
        {
            var expectedItem = expectedItems[expectedIndex];
            var matched = false;
            for (var actualIndex = 0; actualIndex < actualItems.Count; actualIndex++)
            {
                if (usedActualIndexes[actualIndex])
                {
                    continue;
                }

                // In Any-order mode we still require deep equivalency for each candidate pair.
                if (!ItemsEquivalentDeep(
                        actualItems[actualIndex],
                        expectedItem,
                        $"{path}[{expectedIndex}]",
                        options))
                {
                    continue;
                }

                usedActualIndexes[actualIndex] = true;
                matched = true;
                break;
            }

            if (!matched)
            {
                differences.Add(new EquivalencyDifference(
                    $"{path}[{expectedIndex}]",
                    expectedItem,
                    null,
                    "Expected item was not found in actual collection."));
            }
        }

        for (var actualIndex = 0; actualIndex < actualItems.Count; actualIndex++)
        {
            if (usedActualIndexes[actualIndex])
            {
                continue;
            }

            differences.Add(new EquivalencyDifference(
                $"{path}[{actualIndex}]",
                null,
                actualItems[actualIndex],
                "Actual collection contains an extra item."));
        }
    }

    private static bool ItemsEquivalentDeep(
        object? actual,
        object? expected,
        string path,
        EquivalencyOptions options)
    {
        var localDifferences = new List<EquivalencyDifference>();
        var localVisitedPairs = new HashSet<ReferencePair>(ReferencePairComparer.Instance);
        CompareNode(actual, expected, path, options, localDifferences, localVisitedPairs);
        return localDifferences.Count == 0;
    }

    private static bool LeafValuesEquivalent(
        object actual,
        object expected,
        Type actualType,
        Type expectedType)
    {
        if (TryCompareWithConfiguredComparer(actual, expected, actualType, expectedType, out var comparerResult))
        {
            return comparerResult;
        }

        return Equals(actual, expected);
    }

    private static bool TryCompareWithConfiguredComparer(
        object actual,
        object expected,
        Type actualType,
        Type expectedType,
        out bool areEquivalent)
    {
        areEquivalent = false;
        // We can only ask the provider for one generic T, so first choose a shared T
        // that can represent both runtime values (same type or assignable base/interface).
        var comparisonType = GetSharedComparisonType(actualType, expectedType);
        if (comparisonType is null)
        {
            return false;
        }

        var provider = AxiomServices.Configuration.ComparerProvider;
        // Build (or reuse) a strongly-typed comparer call delegate for this runtime type.
        var invoker = ComparerInvokers.GetOrAdd(comparisonType, static type => BuildComparerInvoker(type));
        var comparerResult = invoker(provider, actual, expected);
        if (!comparerResult.HasValue)
        {
            return false;
        }

        areEquivalent = comparerResult.Value;
        return true;
    }

    private static Type? GetSharedComparisonType(Type actualType, Type expectedType)
    {
        if (actualType == expectedType)
        {
            return actualType;
        }

        if (actualType.IsAssignableFrom(expectedType))
        {
            return actualType;
        }

        if (expectedType.IsAssignableFrom(actualType))
        {
            return expectedType;
        }

        return null;
    }

    private static Func<IComparerProvider, object, object, bool?> BuildComparerInvoker(Type comparisonType)
    {
        // Convert CompareWithProviderCore<T> into a callable delegate where T is only known at runtime.
        // This avoids repeated reflection inside hot comparison paths.
        var compareMethod = typeof(EquivalencyEngine)
            .GetMethod(nameof(CompareWithProviderCore), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(comparisonType);

        return (Func<IComparerProvider, object, object, bool?>)compareMethod
            .CreateDelegate(typeof(Func<IComparerProvider, object, object, bool?>));
    }

    private static bool? CompareWithProviderCore<T>(IComparerProvider provider, object actual, object expected)
    {
        // If no comparer exists for T, return null so caller can fall back to default equality.
        if (!provider.TryGetEqualityComparer<T>(out var comparer) || comparer is null)
        {
            return null;
        }

        // Delegate is only built for compatible types, so these casts are intentional and safe here.
        return comparer.Equals((T)actual, (T)expected);
    }

    private static Dictionary<string, Func<object, object?>> GetComparableMembers(Type type, EquivalencyOptions options)
    {
        var members = new Dictionary<string, Func<object, object?>>(StringComparer.Ordinal);

        if (options.IncludePublicProperties)
        {
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.GetMethod is null || property.GetMethod.IsStatic)
                {
                    continue;
                }

                if (property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                members.TryAdd(property.Name, property.GetValue);
            }
        }

        if (options.IncludePublicFields)
        {
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsStatic)
                {
                    continue;
                }

                members.TryAdd(field.Name, field.GetValue);
            }
        }

        return members;
    }

    private static bool IsPathIgnored(string path, EquivalencyOptions options)
    {
        if (options.IgnoredPaths.Contains(path))
        {
            return true;
        }

        // Ignoring "a.b" also ignores children like "a.b.c" and "a.b[0]".
        foreach (var ignoredPath in options.IgnoredPaths)
        {
            if (path.StartsWith($"{ignoredPath}.", StringComparison.Ordinal) ||
                path.StartsWith($"{ignoredPath}[", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool AreTypesCompatible(Type actualType, Type expectedType, EquivalencyOptions options)
    {
        if (options.RequireStrictRuntimeTypes)
        {
            return actualType == expectedType;
        }

        return actualType.IsAssignableFrom(expectedType) || expectedType.IsAssignableFrom(actualType);
    }

    private static bool IsLeafType(Type type)
    {
        var nonNullableType = Nullable.GetUnderlyingType(type) ?? type;

        if (nonNullableType.IsPrimitive || nonNullableType.IsEnum)
        {
            return true;
        }

        return nonNullableType == typeof(decimal) ||
               nonNullableType == typeof(string) ||
               nonNullableType == typeof(DateTime) ||
               nonNullableType == typeof(DateTimeOffset) ||
               nonNullableType == typeof(TimeSpan) ||
               nonNullableType == typeof(Guid);
    }

    private readonly record struct ReferencePair(object Actual, object Expected);

    private sealed class ReferencePairComparer : IEqualityComparer<ReferencePair>
    {
        public static ReferencePairComparer Instance { get; } = new();

        public bool Equals(ReferencePair x, ReferencePair y)
            => ReferenceEquals(x.Actual, y.Actual) && ReferenceEquals(x.Expected, y.Expected);

        public int GetHashCode(ReferencePair pair)
            => HashCode.Combine(RuntimeHelpers.GetHashCode(pair.Actual), RuntimeHelpers.GetHashCode(pair.Expected));
    }
}
