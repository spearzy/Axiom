namespace Axiom.Core.Comparison;

public sealed class DefaultComparerProvider : IComparerProvider
{
    public static DefaultComparerProvider Instance { get; } = new();

    public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
    {
        comparer = EqualityComparer<T>.Default;
        return true;
    }
}
