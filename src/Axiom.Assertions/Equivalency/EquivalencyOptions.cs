using System.Collections;

namespace Axiom.Assertions.Equivalency;

public enum EquivalencyCollectionOrder
{
    Strict,
    Any,
}

public sealed class EquivalencyOptions
{
    private readonly HashSet<string> _ignoredMemberNames = new(StringComparer.Ordinal);
    private readonly HashSet<string> _ignoredPaths = new(StringComparer.Ordinal);
    private readonly HashSet<string> _onlyComparedMembers = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IEqualityComparer> _pathComparers = new(StringComparer.Ordinal);
    private readonly Dictionary<Type, Func<object, object, bool>> _typeComparers = new();

    public EquivalencyCollectionOrder CollectionOrder { get; set; } = EquivalencyCollectionOrder.Strict;
    public bool RequireStrictRuntimeTypes { get; set; } = true;
    public int MaxDifferences { get; set; } = 20;
    public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;
    public bool IncludePublicProperties { get; set; } = true;
    public bool IncludePublicFields { get; set; } = true;
    public bool FailOnMissingMembers { get; set; } = true;
    public bool FailOnExtraMembers { get; set; } = true;
    public float? FloatTolerance { get; set; }
    public double? DoubleTolerance { get; set; }
    public float? HalfTolerance { get; set; }
    public decimal? DecimalTolerance { get; set; }
    public TimeSpan? DateOnlyTolerance { get; set; }
    public TimeSpan? DateTimeTolerance { get; set; }
    public TimeSpan? DateTimeOffsetTolerance { get; set; }
    public TimeSpan? TimeOnlyTolerance { get; set; }
    public TimeSpan? TimeSpanTolerance { get; set; }

    public IReadOnlySet<string> IgnoredMemberNames => _ignoredMemberNames;
    public IReadOnlySet<string> IgnoredPaths => _ignoredPaths;
    internal IReadOnlySet<string> OnlyComparedMembers => _onlyComparedMembers;
    internal bool HasPathComparers => _pathComparers.Count > 0;

    public EquivalencyOptions IgnoreMember(string memberName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(memberName);
        _ignoredMemberNames.Add(memberName);
        return this;
    }

    public EquivalencyOptions IgnorePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _ignoredPaths.Add(path);
        return this;
    }

    // Restrict equivalency to one member branch, e.g. "Name" or "Address.Postcode".
    public EquivalencyOptions OnlyCompareMember(string memberPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(memberPath);
        _onlyComparedMembers.Add(memberPath.Trim());
        return this;
    }

    public EquivalencyOptions OnlyCompareMembers(params string[] memberPaths)
    {
        ArgumentNullException.ThrowIfNull(memberPaths);
        foreach (var memberPath in memberPaths)
        {
            OnlyCompareMember(memberPath);
        }

        return this;
    }

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

    // Snapshot copy for deterministic comparison settings during one assertion run.
    internal EquivalencyOptions Clone()
    {
        var clone = new EquivalencyOptions
        {
            CollectionOrder = CollectionOrder,
            RequireStrictRuntimeTypes = RequireStrictRuntimeTypes,
            MaxDifferences = MaxDifferences,
            StringComparison = StringComparison,
            IncludePublicProperties = IncludePublicProperties,
            IncludePublicFields = IncludePublicFields,
            FailOnMissingMembers = FailOnMissingMembers,
            FailOnExtraMembers = FailOnExtraMembers,
            FloatTolerance = FloatTolerance,
            DoubleTolerance = DoubleTolerance,
            HalfTolerance = HalfTolerance,
            DecimalTolerance = DecimalTolerance,
            DateOnlyTolerance = DateOnlyTolerance,
            DateTimeTolerance = DateTimeTolerance,
            DateTimeOffsetTolerance = DateTimeOffsetTolerance,
            TimeOnlyTolerance = TimeOnlyTolerance,
            TimeSpanTolerance = TimeSpanTolerance,
        };

        foreach (var member in _ignoredMemberNames)
        {
            clone._ignoredMemberNames.Add(member);
        }

        foreach (var path in _ignoredPaths)
        {
            clone._ignoredPaths.Add(path);
        }

        foreach (var memberPath in _onlyComparedMembers)
        {
            clone._onlyComparedMembers.Add(memberPath);
        }

        foreach (var typeComparer in _typeComparers)
        {
            clone._typeComparers[typeComparer.Key] = typeComparer.Value;
        }

        foreach (var pathComparer in _pathComparers)
        {
            clone._pathComparers[pathComparer.Key] = pathComparer.Value;
        }

        return clone;
    }
}
