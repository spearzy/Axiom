using System.Collections;
using System.Linq.Expressions;

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

    public EquivalencyOptions Ignore<TSubject>(Expression<Func<TSubject, object?>> memberSelector)
    {
        ArgumentNullException.ThrowIfNull(memberSelector);

        _ignoredPaths.Add(EquivalencySelectorPath.Create(memberSelector, nameof(memberSelector)));
        return this;
    }

    public EquivalencyOptions Ignore<TSubject>(params Expression<Func<TSubject, object?>>[] memberSelectors)
    {
        ArgumentNullException.ThrowIfNull(memberSelectors);

        foreach (var memberSelector in memberSelectors)
        {
            if (memberSelector is null)
            {
                throw new ArgumentNullException(nameof(memberSelectors), "memberSelectors must not contain null entries.");
            }

            _ignoredPaths.Add(EquivalencySelectorPath.Create(memberSelector, nameof(memberSelectors)));
        }

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

    public EquivalencyOptions OnlyCompare<TSubject>(Expression<Func<TSubject, object?>> memberSelector)
    {
        ArgumentNullException.ThrowIfNull(memberSelector);

        _onlyComparedMembers.Add(EquivalencySelectorPath.Create(memberSelector, nameof(memberSelector)));
        return this;
    }

    public EquivalencyOptions OnlyCompare<TSubject>(params Expression<Func<TSubject, object?>>[] memberSelectors)
    {
        ArgumentNullException.ThrowIfNull(memberSelectors);

        foreach (var memberSelector in memberSelectors)
        {
            if (memberSelector is null)
            {
                throw new ArgumentNullException(nameof(memberSelectors), "memberSelectors must not contain null entries.");
            }

            _onlyComparedMembers.Add(EquivalencySelectorPath.Create(memberSelector, nameof(memberSelectors)));
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

    public EquivalencyOptions MatchMemberName(string actualMember, string expectedMember)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(actualMember);
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedMember);

        AddMemberNameMapping(actualMember.Trim(), expectedMember.Trim());
        return this;
    }

    public EquivalencyOptions MatchMember<TActual, TExpected>(
        Expression<Func<TActual, object?>> actualMemberSelector,
        Expression<Func<TExpected, object?>> expectedMemberSelector)
    {
        ArgumentNullException.ThrowIfNull(actualMemberSelector);
        ArgumentNullException.ThrowIfNull(expectedMemberSelector);

        var actualPath = EquivalencySelectorPath.Create(actualMemberSelector, nameof(actualMemberSelector));
        var expectedPath = EquivalencySelectorPath.Create(expectedMemberSelector, nameof(expectedMemberSelector));

        ValidateTypedMemberMappingDepth(actualPath, expectedPath);
        AddTypedMemberMapping(actualPath, expectedPath);
        return this;
    }

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

    internal bool TryGetMappedExpectedMemberName(string actualMember, out string expectedMember)
    {
        return _actualToExpectedMemberNames.TryGetValue(actualMember, out expectedMember!);
    }

    internal bool TryGetMappedActualMemberName(string expectedMember, out string actualMember)
    {
        return _expectedToActualMemberNames.TryGetValue(expectedMember, out actualMember!);
    }

    internal bool TryGetMappedExpectedMemberPath(string actualPath, out string expectedPath)
    {
        return _actualToExpectedMemberPaths.TryGetValue(actualPath, out expectedPath!);
    }

    internal bool TryGetMappedActualMemberPath(string expectedPath, out string actualPath)
    {
        return _expectedToActualMemberPaths.TryGetValue(expectedPath, out actualPath!);
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

    private void AddMemberNameMapping(string actualMember, string expectedMember)
    {
        // Keep mappings one-to-one so comparisons stay deterministic.
        if (_actualToExpectedMemberNames.TryGetValue(actualMember, out var existingExpected))
        {
            _expectedToActualMemberNames.Remove(existingExpected);
        }

        if (_expectedToActualMemberNames.TryGetValue(expectedMember, out var existingActual))
        {
            _actualToExpectedMemberNames.Remove(existingActual);
        }

        _actualToExpectedMemberNames[actualMember] = expectedMember;
        _expectedToActualMemberNames[expectedMember] = actualMember;
    }

    private void AddTypedMemberMapping(string actualPath, string expectedPath)
    {
        var actualSegments = actualPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var expectedSegments = expectedPath.Split('.', StringSplitOptions.RemoveEmptyEntries);

        for (var index = 0; index < actualSegments.Length; index++)
        {
            AddTypedPathMapping(
                string.Join(".", actualSegments[..(index + 1)]),
                string.Join(".", expectedSegments[..(index + 1)]));
        }
    }

    private void AddTypedPathMapping(string actualPath, string expectedPath)
    {
        if (_actualToExpectedMemberPaths.TryGetValue(actualPath, out var existingExpected))
        {
            _expectedToActualMemberPaths.Remove(existingExpected);
        }

        if (_expectedToActualMemberPaths.TryGetValue(expectedPath, out var existingActual))
        {
            _actualToExpectedMemberPaths.Remove(existingActual);
        }

        _actualToExpectedMemberPaths[actualPath] = expectedPath;
        _expectedToActualMemberPaths[expectedPath] = actualPath;
    }

    private static void ValidateTypedMemberMappingDepth(string actualPath, string expectedPath)
    {
        var actualDepth = actualPath.Split('.', StringSplitOptions.RemoveEmptyEntries).Length;
        var expectedDepth = expectedPath.Split('.', StringSplitOptions.RemoveEmptyEntries).Length;
        if (actualDepth != expectedDepth)
        {
            throw new ArgumentException(
                "actualMemberSelector and expectedMemberSelector must select member paths with the same depth.");
        }
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
            IgnoreExpectedNullMemberValues = IgnoreExpectedNullMemberValues,
            IgnoreActualNullMemberValues = IgnoreActualNullMemberValues,
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

        foreach (var collectionItemComparer in _collectionItemComparers)
        {
            clone._collectionItemComparers[collectionItemComparer.Key] = collectionItemComparer.Value;
        }

        foreach (var memberNameMapping in _actualToExpectedMemberNames)
        {
            clone._actualToExpectedMemberNames[memberNameMapping.Key] = memberNameMapping.Value;
        }

        foreach (var reverseMapping in _expectedToActualMemberNames)
        {
            clone._expectedToActualMemberNames[reverseMapping.Key] = reverseMapping.Value;
        }

        foreach (var pathMapping in _actualToExpectedMemberPaths)
        {
            clone._actualToExpectedMemberPaths[pathMapping.Key] = pathMapping.Value;
        }

        foreach (var reversePathMapping in _expectedToActualMemberPaths)
        {
            clone._expectedToActualMemberPaths[reversePathMapping.Key] = reversePathMapping.Value;
        }

        return clone;
    }
}
