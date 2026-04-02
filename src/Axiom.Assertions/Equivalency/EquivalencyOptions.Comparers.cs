using System.Collections;
using System.Linq.Expressions;

namespace Axiom.Assertions.Equivalency;

public sealed partial class EquivalencyOptions
{
    public EquivalencyOptions UseComparerForType<T>(IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        // Store once per configured type so each equivalency comparison can invoke directly.
        var key = NormaliseComparerType(typeof(T));
        _typeComparers[key] = (actual, expected) => comparer.Equals((T)actual, (T)expected);
        return this;
    }

    public EquivalencyOptions UseComparerForPath(string path, IEqualityComparer comparer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(comparer);

        // Supports both absolute paths (e.g. "actual.Address.Postcode") and relative paths ("Address.Postcode").
        AddPathComparer(path, comparer);
        return this;
    }

    public EquivalencyOptions UseComparerForMember(string memberPath, IEqualityComparer comparer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(memberPath);
        ArgumentNullException.ThrowIfNull(comparer);

        // Member alias for UseComparerForPath to keep call sites intention-revealing.
        AddPathComparer(memberPath, comparer);
        return this;
    }

    public EquivalencyOptions UseComparer<TSubject>(
        Expression<Func<TSubject, object?>> memberSelector,
        IEqualityComparer comparer)
    {
        ArgumentNullException.ThrowIfNull(memberSelector);
        ArgumentNullException.ThrowIfNull(comparer);

        AddPathComparer(EquivalencySelectorPath.Create(memberSelector, nameof(memberSelector)), comparer);
        return this;
    }

    public EquivalencyOptions UseCollectionItemComparerForPath(string path, IEqualityComparer comparer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(comparer);

        _collectionItemComparers[path.Trim()] = comparer;
        return this;
    }

    public EquivalencyOptions UseCollectionItemComparer<TSubject>(
        Expression<Func<TSubject, object?>> collectionSelector,
        IEqualityComparer comparer)
    {
        ArgumentNullException.ThrowIfNull(collectionSelector);
        ArgumentNullException.ThrowIfNull(comparer);

        _collectionItemComparers[EquivalencySelectorPath.Create(collectionSelector, nameof(collectionSelector))] = comparer;
        return this;
    }
    internal bool TryCompareWithPathComparer(string path, object actual, object expected, out bool areEquivalent)
    {
        if (_pathComparers.TryGetValue(path, out var comparer))
        {
            areEquivalent = comparer.Equals(actual, expected);
            return true;
        }

        areEquivalent = false;
        return false;
    }

    internal bool TryGetCollectionItemComparer(string path, out IEqualityComparer comparer)
    {
        return _collectionItemComparers.TryGetValue(path, out comparer!);
    }

    internal bool TryCompareWithTypeComparer(Type comparisonType, object actual, object expected, out bool areEquivalent)
    {
        var key = NormaliseComparerType(comparisonType);
        if (_typeComparers.TryGetValue(key, out var compare))
        {
            areEquivalent = compare(actual, expected);
            return true;
        }

        areEquivalent = false;
        return false;
    }

    private static Type NormaliseComparerType(Type type)
    {
        // Nullable<T> values are boxed as their underlying T when they have a value.
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    private void AddPathComparer(string path, IEqualityComparer comparer)
    {
        _pathComparers[path.Trim()] = comparer;
    }
}
