using System.Collections;

namespace Axiom.Assertions.Equivalency;

public enum EquivalencyCollectionOrder
{
    Strict,
    Any,
}

public sealed partial class EquivalencyOptions
{
    private readonly HashSet<string> _ignoredMemberNames = new(StringComparer.Ordinal);
    private readonly HashSet<string> _ignoredPaths = new(StringComparer.Ordinal);
    private readonly HashSet<string> _onlyComparedMembers = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IEqualityComparer> _pathComparers = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IEqualityComparer> _collectionItemComparers = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _actualToExpectedMemberNames = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _expectedToActualMemberNames = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _actualToExpectedMemberPaths = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _expectedToActualMemberPaths = new(StringComparer.Ordinal);
    private readonly Dictionary<Type, Func<object, object, bool>> _typeComparers = new();

    public EquivalencyCollectionOrder CollectionOrder { get; set; } = EquivalencyCollectionOrder.Strict;
    public bool RequireStrictRuntimeTypes { get; set; } = true;
    public int MaxDifferences { get; set; } = 20;
    public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;
    public bool IncludePublicProperties { get; set; } = true;
    public bool IncludePublicFields { get; set; } = true;
    public bool FailOnMissingMembers { get; set; } = true;
    public bool FailOnExtraMembers { get; set; } = true;
    public bool IgnoreExpectedNullMemberValues { get; private set; }
    public bool IgnoreActualNullMemberValues { get; private set; }
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
    internal bool HasCollectionItemComparers => _collectionItemComparers.Count > 0;
    internal bool HasMemberNameMappings => _actualToExpectedMemberNames.Count > 0;
    internal bool HasTypedMemberMappings => _actualToExpectedMemberPaths.Count > 0;

    public EquivalencyOptions IgnoreExpectedNullMembers()
    {
        IgnoreExpectedNullMemberValues = true;
        return this;
    }

    public EquivalencyOptions IgnoreActualNullMembers()
    {
        IgnoreActualNullMemberValues = true;
        return this;
    }
}
