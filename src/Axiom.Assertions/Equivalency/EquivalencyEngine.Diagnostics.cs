using Axiom.Assertions.AssertionTypes;

namespace Axiom.Assertions.Equivalency;

internal static partial class EquivalencyEngine
{
    private static void AddDifference(
        List<EquivalencyDifference> differences,
        string path,
        string expectedPath,
        object? expected,
        object? actual,
        EquivalencyDifferenceKind kind,
        string? detail = null)
    {
        differences.Add(new EquivalencyDifference(path, expectedPath, expected, actual, kind, detail));
    }

    private static string BuildStringMismatchDetail(
        string expected,
        string actual,
        StringComparison comparison)
    {
        var detail = StringDifferenceDiagnostics.BuildEqualityFailureDetail(expected, actual);
        if (comparison == StringComparison.Ordinal)
        {
            return detail;
        }

        return $"comparison {comparison}; {detail}";
    }
}
