namespace Axiom.Assertions.AssertionTypes;

// Shared collection assertion logic invoked by extension methods to avoid per-call wrapper allocations.
internal static partial class CollectionAssertionEngine
{
    public readonly record struct ContainSingleResult(
        bool HasSingleItem,
        object? SingleItem,
        string? FailureMessage);

    public readonly record struct ContainSingleResult<TItem>(
        bool HasSingleItem,
        TItem SingleItem,
        string? FailureMessage);

    public readonly record struct ContainKeyResult<TValue>(
        bool HasValue,
        TValue Value,
        string? FailureMessage);
}
