using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Axiom.Assertions.Equivalency;

internal static partial class EquivalencyEngine
{
    private static void CompareNode<TPathMode>(
        object? actual,
        object? expected,
        string path,
        string rootPath,
        TPathMode pathMode,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
        where TPathMode : struct, IExpectedPathMode<TPathMode>
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

            AddDifference(
                differences,
                path,
                pathMode.DifferenceExpectedPath,
                expected,
                actual,
                EquivalencyDifferenceKind.NullMismatch,
                "one value was <null> and the other was not");
            return;
        }

        var actualType = actual.GetType();
        var expectedType = expected.GetType();
        if (!AreTypesCompatible(actualType, expectedType, options))
        {
            AddDifference(
                differences,
                path,
                pathMode.DifferenceExpectedPath,
                expected,
                actual,
                EquivalencyDifferenceKind.TypeMismatch,
                $"runtime types differ: expected {expectedType.FullName}, but found {actualType.FullName}");
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
                AddDifference(
                    differences,
                    path,
                    pathMode.DifferenceExpectedPath,
                    expected,
                    actual,
                    EquivalencyDifferenceKind.ValueMismatch,
                    "values differ");
            }

            return;
        }

        if (hasPathComparers &&
            TryCompareWithConfiguredPathComparer(actual, expected, path, rootPath, options, out var pathComparerResult))
        {
            if (!pathComparerResult)
            {
                AddDifference(
                    differences,
                    path,
                    pathMode.DifferenceExpectedPath,
                    expected,
                    actual,
                    EquivalencyDifferenceKind.ValueMismatch,
                    "values differ");
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
                AddDifference(
                    differences,
                    path,
                    pathMode.DifferenceExpectedPath,
                    expected,
                    actual,
                    EquivalencyDifferenceKind.StringMismatch,
                    BuildStringMismatchDetail(expectedString, actualString, options.StringComparison));
            }

            return;
        }

        if (actual is IEnumerable actualEnumerable &&
            expected is IEnumerable expectedEnumerable &&
            actual is not string &&
            expected is not string)
        {
            CompareEnumerable(actualEnumerable, expectedEnumerable, path, rootPath, pathMode, options, differences, visitedPairs);
            return;
        }

        if (isLeafComparison)
        {
            var areEquivalent = hasPathComparers
                ? LeafValuesEquivalentWithoutTolerance(actual, expected, actualType, expectedType, options)
                : LeafValuesEquivalent(actual, expected, actualType, expectedType, options);

            if (!areEquivalent)
            {
                AddDifference(
                    differences,
                    path,
                    pathMode.DifferenceExpectedPath,
                    expected,
                    actual,
                    EquivalencyDifferenceKind.ValueMismatch,
                    "values differ");
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

        CompareMembers(actual, expected, actualType, expectedType, path, rootPath, pathMode, options, differences, visitedPairs);
    }

    private static void CompareMembers<TPathMode>(
        object actual,
        object expected,
        Type actualType,
        Type expectedType,
        string path,
        string rootPath,
        TPathMode pathMode,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
        where TPathMode : struct, IExpectedPathMode<TPathMode>
    {
        var actualMemberSet = GetComparableMembers(actualType, options);
        var expectedMemberSet = GetComparableMembers(expectedType, options);
        var actualMembers = actualMemberSet.Members;
        var expectedMembers = expectedMemberSet.Members;
        var usedExpectedMembers = new HashSet<string>(StringComparer.Ordinal);
        foreach (var actualMemberName in actualMemberSet.OrderedNames)
        {
            if (options.IgnoredMemberNames.Contains(actualMemberName))
            {
                continue;
            }

            var memberPath = $"{path}.{actualMemberName}";
            if (IsPathIgnored(memberPath, rootPath, options))
            {
                continue;
            }

            var childMode = pathMode.GetExpectedMemberMode(path, rootPath, actualMemberName, options, out var expectedMemberName);
            if (options.IgnoredMemberNames.Contains(expectedMemberName))
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

                AddDifference(
                    differences,
                    memberPath,
                    childMode.DifferenceExpectedPath,
                    null,
                    actualValue,
                    EquivalencyDifferenceKind.MissingMemberOnExpected);
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

            CompareNode(actualValueAtMember, expectedValueAtMember, memberPath, rootPath, childMode, options, differences, visitedPairs);
        }

        foreach (var expectedMemberName in expectedMemberSet.OrderedNames)
        {
            if (usedExpectedMembers.Contains(expectedMemberName) ||
                options.IgnoredMemberNames.Contains(expectedMemberName))
            {
                continue;
            }

            var childMode = pathMode.GetActualMemberMode(path, rootPath, expectedMemberName, options, out var actualMemberName);
            if (options.IgnoredMemberNames.Contains(actualMemberName))
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

            AddDifference(
                differences,
                memberPath,
                childMode.DifferenceExpectedPath,
                expectedValue,
                null,
                EquivalencyDifferenceKind.MissingMemberOnActual);
        }
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
