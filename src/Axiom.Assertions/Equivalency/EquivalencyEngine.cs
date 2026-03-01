namespace Axiom.Assertions.Equivalency;

internal static class EquivalencyEngine
{
    public static IReadOnlyList<EquivalencyDifference> Compare(
        object? actual,
        object? expected,
        string rootPath,
        EquivalencyOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);
        ArgumentNullException.ThrowIfNull(options);

        var differences = new List<EquivalencyDifference>();
        CompareNode(actual, expected, rootPath, options, differences);
        return differences;
    }

    private static void CompareNode(
        object? actual,
        object? expected,
        string path,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences)
    {
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
        if (options.RequireStrictRuntimeTypes && actualType != expectedType)
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

        if (IsLeafType(actualType) || IsLeafType(expectedType))
        {
            if (!Equals(actual, expected))
            {
                differences.Add(new EquivalencyDifference(path, expected, actual, "Values differ."));
            }

            return;
        }

        if (!Equals(actual, expected))
        {
            differences.Add(new EquivalencyDifference(
                path,
                expected,
                actual,
                "Complex member comparison is not implemented yet."));
        }
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
}
