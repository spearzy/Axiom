namespace Axiom.Assertions.Equivalency;

internal readonly record struct EquivalencyDifference(
    string Path,
    object? Expected,
    object? Actual,
    string? Detail = null);
