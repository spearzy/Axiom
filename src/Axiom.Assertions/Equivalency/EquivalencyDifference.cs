namespace Axiom.Assertions.Equivalency;

internal enum EquivalencyDifferenceKind
{
    ValueMismatch,
    StringMismatch,
    NullMismatch,
    TypeMismatch,
    MissingMemberOnActual,
    MissingMemberOnExpected,
    CollectionItemMissingOnActual,
    CollectionItemExtraOnActual,
    ExpectedCollectionItemNotFound,
    ActualCollectionContainsExtraItem,
}

internal readonly record struct EquivalencyDifference(
    string Path,
    string ExpectedPath,
    object? Expected,
    object? Actual,
    EquivalencyDifferenceKind Kind,
    string? Detail = null);
