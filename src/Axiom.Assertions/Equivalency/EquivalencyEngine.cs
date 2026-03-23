using System.Collections;
using System.Collections.Concurrent;
using System.Numerics;
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
        CompareNode(actual, expected, rootPath, rootPath, string.Empty, options, differences, visitedPairs);
        return differences;
    }

    private static void CompareNode(
        object? actual,
        object? expected,
        string path,
        string rootPath,
        string expectedPath,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
    {
        if (IsPathIgnored(path, rootPath, options))
        {
            return;
        }

        if (!IsPathIncluded(path, rootPath, options))
        {
            return;
        }

        var hasConfiguredPathOrCollectionComparers = options.HasPathComparers || options.HasCollectionItemComparers;

        // Preserve the fastest path when no per-path comparers are configured.
        if (!hasConfiguredPathOrCollectionComparers && ReferenceEquals(actual, expected))
        {
            return;
        }

        if (actual is null || expected is null)
        {
            if (actual is null && expected is null)
            {
                return;
            }

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

        var hasPathComparers = options.HasPathComparers;
        var isLeafComparison = IsLeafType(actualType) || IsLeafType(expectedType);
        if (hasPathComparers &&
            isLeafComparison &&
            TryCompareWithTolerance(actual, expected, options, out var toleranceResult))
        {
            if (!toleranceResult)
            {
                differences.Add(new EquivalencyDifference(path, expected, actual, "Values differ."));
            }

            return;
        }

        if (hasPathComparers &&
            TryCompareWithConfiguredPathComparer(actual, expected, path, rootPath, options, out var pathComparerResult))
        {
            if (!pathComparerResult)
            {
                differences.Add(new EquivalencyDifference(path, expected, actual, "Values differ."));
            }

            return;
        }

        if (ReferenceEquals(actual, expected) && !hasConfiguredPathOrCollectionComparers)
        {
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
            CompareEnumerable(actualEnumerable, expectedEnumerable, path, rootPath, expectedPath, options, differences, visitedPairs);
            return;
        }

        if (isLeafComparison)
        {
            var areEquivalent = hasPathComparers
                ? LeafValuesEquivalentWithoutTolerance(actual, expected, actualType, expectedType, options)
                : LeafValuesEquivalent(actual, expected, actualType, expectedType, options);

            if (!areEquivalent)
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

        CompareMembers(actual, expected, actualType, expectedType, path, rootPath, expectedPath, options, differences, visitedPairs);
    }

    private static void CompareMembers(
        object actual,
        object expected,
        Type actualType,
        Type expectedType,
        string path,
        string rootPath,
        string expectedPath,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
    {
        var actualMembers = GetComparableMembers(actualType, options);
        var expectedMembers = GetComparableMembers(expectedType, options);
        var usedExpectedMembers = new HashSet<string>(StringComparer.Ordinal);
        var actualRelativePath = ToRelativePath(path, rootPath);

        // Compare by actual members first so the rendered paths stay anchored to the actual object shape.
        foreach (var actualMemberName in actualMembers.Keys.OrderBy(static name => name, StringComparer.Ordinal))
        {
            var actualMemberPath = AppendPath(actualRelativePath, actualMemberName);
            var expectedMemberPath = ResolveExpectedMemberPath(actualMemberPath, expectedPath, actualMemberName, options);
            var expectedMemberName = GetDirectChildMemberName(expectedMemberPath, expectedPath);
            if (options.IgnoredMemberNames.Contains(actualMemberName) ||
                options.IgnoredMemberNames.Contains(expectedMemberName))
            {
                continue;
            }

            var memberPath = $"{path}.{actualMemberName}";
            if (IsPathIgnored(memberPath, rootPath, options))
            {
                continue;
            }

            var actualGetter = actualMembers[actualMemberName];
            var hasExpected = expectedMembers.TryGetValue(expectedMemberName, out var expectedGetter);
            if (hasExpected && !usedExpectedMembers.Add(expectedMemberName))
            {
                hasExpected = false;
            }

            if (!hasExpected)
            {
                if (!options.FailOnExtraMembers)
                {
                    continue;
                }

                var actualValue = actualGetter(actual);
                if (options.IgnoreActualNullMemberValues && actualValue is null)
                {
                    continue;
                }

                differences.Add(new EquivalencyDifference(memberPath, null, actualValue, "Member missing on expected type."));
                continue;
            }

            var actualValueAtMember = actualGetter(actual);
            var expectedValueAtMember = expectedGetter!(expected);
            if (options.IgnoreActualNullMemberValues && actualValueAtMember is null)
            {
                continue;
            }

            if (options.IgnoreExpectedNullMemberValues && expectedValueAtMember is null)
            {
                continue;
            }

            CompareNode(actualValueAtMember, expectedValueAtMember, memberPath, rootPath, expectedMemberPath, options, differences, visitedPairs);
        }

        // Then account for expected members that were never matched to an actual member.
        foreach (var expectedMemberName in expectedMembers.Keys.OrderBy(static name => name, StringComparer.Ordinal))
        {
            if (usedExpectedMembers.Contains(expectedMemberName))
            {
                continue;
            }

            var expectedMemberPath = AppendPath(expectedPath, expectedMemberName);
            var actualMemberPath = ResolveActualMemberPath(expectedMemberPath, actualRelativePath, expectedMemberName, options);
            var actualMemberName = GetDirectChildMemberName(actualMemberPath, actualRelativePath);
            if (options.IgnoredMemberNames.Contains(expectedMemberName) ||
                options.IgnoredMemberNames.Contains(actualMemberName))
            {
                continue;
            }

            var memberPath = $"{path}.{actualMemberName}";
            if (IsPathIgnored(memberPath, rootPath, options))
            {
                continue;
            }

            if (!options.FailOnMissingMembers)
            {
                continue;
            }

            var expectedValue = expectedMembers[expectedMemberName](expected);
            if (options.IgnoreExpectedNullMemberValues && expectedValue is null)
            {
                continue;
            }

            differences.Add(new EquivalencyDifference(memberPath, expectedValue, null, "Member missing on actual type."));
        }
    }

    private static void CompareEnumerable(
        IEnumerable actualEnumerable,
        IEnumerable expectedEnumerable,
        string path,
        string rootPath,
        string expectedPath,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
    {
        IEqualityComparer? collectionItemComparer = null;
        var hasCollectionItemComparer =
            options.HasCollectionItemComparers &&
            TryGetConfiguredCollectionItemComparer(path, rootPath, options, out collectionItemComparer);

        if (options.CollectionOrder == EquivalencyCollectionOrder.Any)
        {
            // Any-order matching needs random access and "used index" tracking.
            // Keep list materialisation only for this mode.
            var actualItems = actualEnumerable.Cast<object?>().ToList();
            var expectedItems = expectedEnumerable.Cast<object?>().ToList();
            CompareEnumerableAnyOrder(
                actualItems,
                expectedItems,
                path,
                rootPath,
                expectedPath,
                options,
                differences,
                hasCollectionItemComparer ? collectionItemComparer : null);
            return;
        }

        // Ordered mode walks both sequences once to avoid eager list allocations.
        var actualEnumerator = actualEnumerable.GetEnumerator();
        var expectedEnumerator = expectedEnumerable.GetEnumerator();
        try
        {
            var index = 0;
            while (true)
            {
                var hasActual = actualEnumerator.MoveNext();
                var hasExpected = expectedEnumerator.MoveNext();
                if (!hasActual || !hasExpected)
                {
                    if (!hasActual && !hasExpected)
                    {
                        return;
                    }

                    if (hasExpected)
                    {
                        differences.Add(new EquivalencyDifference(
                            $"{path}[{index}]",
                            expectedEnumerator.Current,
                            null,
                            "Item missing on actual collection."));
                        index++;

                        while (expectedEnumerator.MoveNext())
                        {
                            differences.Add(new EquivalencyDifference(
                                $"{path}[{index}]",
                                expectedEnumerator.Current,
                                null,
                                "Item missing on actual collection."));
                            index++;
                        }
                    }
                    else
                    {
                        differences.Add(new EquivalencyDifference(
                            $"{path}[{index}]",
                            null,
                            actualEnumerator.Current,
                            "Extra item on actual collection."));
                        index++;

                        while (actualEnumerator.MoveNext())
                        {
                            differences.Add(new EquivalencyDifference(
                                $"{path}[{index}]",
                                null,
                                actualEnumerator.Current,
                                "Extra item on actual collection."));
                            index++;
                        }
                    }

                    return;
                }

                var itemPath = $"{path}[{index}]";
                var actualItem = actualEnumerator.Current;
                var expectedItem = expectedEnumerator.Current;
                if (hasCollectionItemComparer)
                {
                    if (!collectionItemComparer!.Equals(actualItem, expectedItem))
                    {
                        differences.Add(new EquivalencyDifference(itemPath, expectedItem, actualItem, "Values differ."));
                    }
                }
                else
                {
                    CompareNode(
                        actualItem,
                        expectedItem,
                        itemPath,
                        rootPath,
                        AppendIndex(expectedPath, index),
                        options,
                        differences,
                        visitedPairs);
                }

                index++;
            }
        }
        finally
        {
            (actualEnumerator as IDisposable)?.Dispose();
            (expectedEnumerator as IDisposable)?.Dispose();
        }
    }

    private static void CompareEnumerableAnyOrder(
        List<object?> actualItems,
        List<object?> expectedItems,
        string path,
        string rootPath,
        string expectedPath,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        IEqualityComparer? collectionItemComparer)
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

                var isEquivalent = collectionItemComparer is not null
                    ? collectionItemComparer.Equals(actualItems[actualIndex], expectedItem)
                    // In Any-order mode we still require deep equivalency for each candidate pair.
                    : ItemsEquivalentDeep(
                        actualItems[actualIndex],
                        expectedItem,
                        $"{path}[{expectedIndex}]",
                        rootPath,
                        AppendIndex(expectedPath, expectedIndex),
                        options);
                if (!isEquivalent)
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
        string rootPath,
        string expectedPath,
        EquivalencyOptions options)
    {
        var localDifferences = new List<EquivalencyDifference>();
        var localVisitedPairs = new HashSet<ReferencePair>(ReferencePairComparer.Instance);
        CompareNode(actual, expected, path, rootPath, expectedPath, options, localDifferences, localVisitedPairs);
        return localDifferences.Count == 0;
    }

    private static bool LeafValuesEquivalent(
        object actual,
        object expected,
        Type actualType,
        Type expectedType,
        EquivalencyOptions options)
    {
        // Per-assertion tolerance settings take priority for supported leaf types.
        if (TryCompareWithTolerance(actual, expected, options, out var toleranceResult))
        {
            return toleranceResult;
        }

        if (TryCompareWithConfiguredComparer(actual, expected, actualType, expectedType, options, out var comparerResult))
        {
            return comparerResult;
        }

        return Equals(actual, expected);
    }

    private static bool LeafValuesEquivalentWithoutTolerance(
        object actual,
        object expected,
        Type actualType,
        Type expectedType,
        EquivalencyOptions options)
    {
        if (TryCompareWithConfiguredComparer(actual, expected, actualType, expectedType, options, out var comparerResult))
        {
            return comparerResult;
        }

        return Equals(actual, expected);
    }

    private static bool TryCompareWithTolerance(
        object actual,
        object expected,
        EquivalencyOptions options,
        out bool areEquivalent)
    {
        switch (actual)
        {
            // Absolute delta check for float with NaN/Infinity-safe handling.
            case float actualFloat when expected is float expectedFloat && options.FloatTolerance.HasValue:
                areEquivalent = AreFloatsEquivalent(actualFloat, expectedFloat, Math.Abs(options.FloatTolerance.Value));
                return true;

            // Absolute delta check for double with NaN/Infinity-safe handling.
            case double actualDouble when expected is double expectedDouble && options.DoubleTolerance.HasValue:
                areEquivalent = AreDoublesEquivalent(actualDouble, expectedDouble, Math.Abs(options.DoubleTolerance.Value));
                return true;

            // Compare Half values via float arithmetic to keep tolerance behaviour consistent.
            case Half actualHalf when expected is Half expectedHalf && options.HalfTolerance.HasValue:
                areEquivalent = AreFloatsEquivalent((float)actualHalf, (float)expectedHalf, Math.Abs(options.HalfTolerance.Value));
                return true;

            // Absolute delta check for decimal values.
            case decimal actualDecimal when expected is decimal expectedDecimal && options.DecimalTolerance.HasValue:
                areEquivalent = decimal.Abs(actualDecimal - expectedDecimal) <= decimal.Abs(options.DecimalTolerance.Value);
                return true;

            // DateOnly has day precision, so compare day gap against the configured window.
            case DateOnly actualDateOnly when expected is DateOnly expectedDateOnly && options.DateOnlyTolerance.HasValue:
                areEquivalent = TimeSpan.FromDays(Math.Abs(actualDateOnly.DayNumber - expectedDateOnly.DayNumber)) <=
                                NormaliseTemporalTolerance(options.DateOnlyTolerance.Value, nameof(EquivalencyOptions.DateOnlyTolerance));
                return true;

            // Compare clock instants by permitted time window.
            case DateTime actualDateTime when expected is DateTime expectedDateTime && options.DateTimeTolerance.HasValue:
                areEquivalent = (actualDateTime - expectedDateTime).Duration() <=
                                NormaliseTemporalTolerance(options.DateTimeTolerance.Value, nameof(EquivalencyOptions.DateTimeTolerance));
                return true;

            // Compare offset-aware instants by permitted time window.
            case DateTimeOffset actualDateTimeOffset when expected is DateTimeOffset expectedDateTimeOffset && options.DateTimeOffsetTolerance.HasValue:
                areEquivalent = (actualDateTimeOffset - expectedDateTimeOffset).Duration() <=
                                NormaliseTemporalTolerance(options.DateTimeOffsetTolerance.Value, nameof(EquivalencyOptions.DateTimeOffsetTolerance));
                return true;

            // Compare time-of-day values by permitted time window.
            case TimeOnly actualTimeOnly when expected is TimeOnly expectedTimeOnly && options.TimeOnlyTolerance.HasValue:
                areEquivalent = AbsoluteTimeOnlyDifference(actualTimeOnly, expectedTimeOnly) <=
                                NormaliseTemporalTolerance(options.TimeOnlyTolerance.Value, nameof(EquivalencyOptions.TimeOnlyTolerance));
                return true;

            // Compare durations by permitted time window.
            case TimeSpan actualTimeSpan when expected is TimeSpan expectedTimeSpan && options.TimeSpanTolerance.HasValue:
                areEquivalent = (actualTimeSpan - expectedTimeSpan).Duration() <=
                                NormaliseTemporalTolerance(options.TimeSpanTolerance.Value, nameof(EquivalencyOptions.TimeSpanTolerance));
                return true;
        }

        areEquivalent = false;
        return false;
    }

    private static bool AreFloatsEquivalent(float actual, float expected, float tolerance)
    {
        if (float.IsNaN(actual) || float.IsNaN(expected))
        {
            return float.IsNaN(actual) && float.IsNaN(expected);
        }

        if (float.IsInfinity(actual) || float.IsInfinity(expected))
        {
            return actual.Equals(expected);
        }

        return Math.Abs(actual - expected) <= tolerance;
    }

    private static bool AreDoublesEquivalent(double actual, double expected, double tolerance)
    {
        if (double.IsNaN(actual) || double.IsNaN(expected))
        {
            return double.IsNaN(actual) && double.IsNaN(expected);
        }

        if (double.IsInfinity(actual) || double.IsInfinity(expected))
        {
            return actual.Equals(expected);
        }

        return Math.Abs(actual - expected) <= tolerance;
    }

    private static TimeSpan NormaliseTemporalTolerance(TimeSpan tolerance, string optionName)
    {
        if (tolerance == TimeSpan.MinValue)
        {
            throw new ArgumentOutOfRangeException(optionName, "Tolerance must not be TimeSpan.MinValue.");
        }

        return tolerance.Duration();
    }

    private static TimeSpan AbsoluteTimeOnlyDifference(TimeOnly left, TimeOnly right)
    {
        var directTicks = Math.Abs(left.Ticks - right.Ticks);
        var wrappedTicks = TimeSpan.TicksPerDay - directTicks;
        return TimeSpan.FromTicks(Math.Min(directTicks, wrappedTicks));
    }

    private static bool TryCompareWithConfiguredComparer(
        object actual,
        object expected,
        Type actualType,
        Type expectedType,
        EquivalencyOptions options,
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

        // Per-assertion type comparer should win over global provider behaviour.
        if (options.TryCompareWithTypeComparer(comparisonType, actual, expected, out var localComparerResult))
        {
            areEquivalent = localComparerResult;
            return true;
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

    private static bool TryCompareWithConfiguredPathComparer(
        object actual,
        object expected,
        string path,
        string rootPath,
        EquivalencyOptions options,
        out bool areEquivalent)
    {
        if (options.TryCompareWithPathComparer(path, actual, expected, out areEquivalent))
        {
            return true;
        }

        var relativePath = ToRelativePath(path, rootPath);
        if (relativePath.Length > 0 &&
            options.TryCompareWithPathComparer(relativePath, actual, expected, out areEquivalent))
        {
            return true;
        }

        areEquivalent = false;
        return false;
    }

    private static bool TryGetConfiguredCollectionItemComparer(
        string path,
        string rootPath,
        EquivalencyOptions options,
        out IEqualityComparer comparer)
    {
        if (options.TryGetCollectionItemComparer(path, out comparer))
        {
            return true;
        }

        var relativePath = ToRelativePath(path, rootPath);
        if (relativePath.Length > 0 &&
            options.TryGetCollectionItemComparer(relativePath, out comparer))
        {
            return true;
        }

        comparer = null!;
        return false;
    }

    private static string ResolveExpectedMemberName(string actualMemberName, EquivalencyOptions options)
    {
        return options.TryGetMappedExpectedMemberName(actualMemberName, out var expectedMemberName)
            ? expectedMemberName
            : actualMemberName;
    }

    private static string ResolveActualMemberName(string expectedMemberName, EquivalencyOptions options)
    {
        return options.TryGetMappedActualMemberName(expectedMemberName, out var actualMemberName)
            ? actualMemberName
            : expectedMemberName;
    }

    private static string ResolveExpectedMemberPath(
        string actualMemberPath,
        string currentExpectedPath,
        string actualMemberName,
        EquivalencyOptions options)
    {
        if (options.TryGetMappedExpectedMemberPath(actualMemberPath, out var expectedMemberPath))
        {
            return expectedMemberPath;
        }

        return AppendPath(currentExpectedPath, ResolveExpectedMemberName(actualMemberName, options));
    }

    private static string ResolveActualMemberPath(
        string expectedMemberPath,
        string currentActualPath,
        string expectedMemberName,
        EquivalencyOptions options)
    {
        if (options.TryGetMappedActualMemberPath(expectedMemberPath, out var actualMemberPath))
        {
            return actualMemberPath;
        }

        return AppendPath(currentActualPath, ResolveActualMemberName(expectedMemberName, options));
    }

    private static string AppendPath(string parentPath, string childSegment)
    {
        return parentPath.Length == 0 ? childSegment : $"{parentPath}.{childSegment}";
    }

    private static string AppendIndex(string path, int index)
    {
        return path.Length == 0 ? $"[{index}]" : $"{path}[{index}]";
    }

    private static string GetDirectChildMemberName(string fullPath, string parentPath)
    {
        if (parentPath.Length == 0)
        {
            return ExtractFirstMemberSegment(fullPath);
        }

        if (fullPath.StartsWith($"{parentPath}.", StringComparison.Ordinal))
        {
            return ExtractFirstMemberSegment(fullPath[(parentPath.Length + 1)..]);
        }

        throw new InvalidOperationException(
            $"Configured member mapping '{fullPath}' does not align with expected parent path '{parentPath}'.");
    }

    private static string ExtractFirstMemberSegment(string path)
    {
        var separatorIndex = path.IndexOf('.');
        return separatorIndex >= 0 ? path[..separatorIndex] : path;
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

    private static bool IsPathIgnored(string path, string rootPath, EquivalencyOptions options)
    {
        if (options.IgnoredPaths.Contains(path))
        {
            return true;
        }

        var relativePath = ToRelativePath(path, rootPath);
        if (relativePath.Length > 0 && options.IgnoredPaths.Contains(relativePath))
        {
            return true;
        }

        foreach (var ignoredPath in options.IgnoredPaths)
        {
            if (PathMatchesOrIsChild(path, ignoredPath))
            {
                return true;
            }

            if (relativePath.Length > 0 && PathMatchesOrIsChild(relativePath, ignoredPath))
            {
                return true;
            }
        }

        return false;
    }

    private static bool PathMatchesOrIsChild(string currentPath, string configuredPath)
    {
        if (currentPath.Equals(configuredPath, StringComparison.Ordinal))
        {
            return true;
        }

        return currentPath.StartsWith($"{configuredPath}.", StringComparison.Ordinal) ||
               currentPath.StartsWith($"{configuredPath}[", StringComparison.Ordinal);
    }

    private static bool IsPathIncluded(string path, string rootPath, EquivalencyOptions options)
    {
        if (options.OnlyComparedMembers.Count == 0)
        {
            return true;
        }

        var relativePath = ToRelativePath(path, rootPath);
        if (relativePath.Length == 0)
        {
            return true;
        }

        foreach (var includedMember in options.OnlyComparedMembers)
        {
            if (PathMatchesOrContains(relativePath, includedMember))
            {
                return true;
            }
        }

        return false;
    }

    private static bool PathMatchesOrContains(string currentPath, string includedMember)
    {
        if (currentPath.Equals(includedMember, StringComparison.Ordinal))
        {
            return true;
        }

        if (currentPath.StartsWith($"{includedMember}.", StringComparison.Ordinal) ||
            currentPath.StartsWith($"{includedMember}[", StringComparison.Ordinal))
        {
            return true;
        }

        return includedMember.StartsWith($"{currentPath}.", StringComparison.Ordinal) ||
               includedMember.StartsWith($"{currentPath}[", StringComparison.Ordinal);
    }

    private static string ToRelativePath(string path, string rootPath)
    {
        if (path.Equals(rootPath, StringComparison.Ordinal))
        {
            return string.Empty;
        }

        if (path.StartsWith($"{rootPath}.", StringComparison.Ordinal))
        {
            return path[(rootPath.Length + 1)..];
        }

        if (path.StartsWith($"{rootPath}[", StringComparison.Ordinal))
        {
            return path[rootPath.Length..];
        }

        // Fallback for unusual path roots; preserving current path keeps matching predictable.
        return path;
    }

    private static bool AreTypesCompatible(Type actualType, Type expectedType, EquivalencyOptions options)
    {
        if (options.RequireStrictRuntimeTypes)
        {
            return actualType == expectedType;
        }

        // Structural mode: compare members/values regardless of runtime type identity.
        // MatchMemberName(...) remains an optional rename tool, not a type-compatibility gate.
        return true;
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
               nonNullableType == typeof(DateOnly) ||
               nonNullableType == typeof(TimeOnly) ||
               nonNullableType == typeof(Half) ||
               nonNullableType == typeof(Int128) ||
               nonNullableType == typeof(UInt128) ||
               nonNullableType == typeof(BigInteger) ||
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
