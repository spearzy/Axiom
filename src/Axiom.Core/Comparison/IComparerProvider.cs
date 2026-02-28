namespace Axiom.Core.Comparison;

public interface IComparerProvider
{
    bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer);
}
