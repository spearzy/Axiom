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

        return clone;
    }
}
