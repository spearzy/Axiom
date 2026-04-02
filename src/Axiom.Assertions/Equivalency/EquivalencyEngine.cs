using System.Collections.Concurrent;
using Axiom.Core.Comparison;

namespace Axiom.Assertions.Equivalency;

internal static partial class EquivalencyEngine
{
    private static readonly ConcurrentDictionary<Type, Func<IComparerProvider, object, object, bool?>> ComparerInvokers = new();
    private static int _appendPathProbeCount;
    private static int _appendIndexProbeCount;
    private static int _resolveExpectedMemberPathProbeCount;
    private static int _resolveActualMemberPathProbeCount;
    private static int _getDirectChildMemberNameProbeCount;
    private static volatile bool _expectedPathHelperProbeEnabled;

    internal static void SetExpectedPathHelperProbeEnabled(bool enabled)
    {
        _expectedPathHelperProbeEnabled = enabled;
    }

    internal static void ResetExpectedPathHelperProbe()
    {
        _appendPathProbeCount = 0;
        _appendIndexProbeCount = 0;
        _resolveExpectedMemberPathProbeCount = 0;
        _resolveActualMemberPathProbeCount = 0;
        _getDirectChildMemberNameProbeCount = 0;
    }

    internal static int[] SnapshotExpectedPathHelperProbe()
    {
        return
        [
            _appendPathProbeCount,
            _appendIndexProbeCount,
            _resolveExpectedMemberPathProbeCount,
            _resolveActualMemberPathProbeCount,
            _getDirectChildMemberNameProbeCount,
        ];
    }

    public static IReadOnlyList<EquivalencyDifference> Compare(
        object? actual,
        object? expected,
        string rootPath,
        EquivalencyOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);
        ArgumentNullException.ThrowIfNull(options);

        var differences = new List<EquivalencyDifference>();
        var visitedPairs = new HashSet<ReferencePair>(ReferencePairComparer.Instance);
        if (HasMemberMappings(options))
        {
            CompareNode(actual, expected, rootPath, rootPath, MappedPathMode.Root, options, differences, visitedPairs);
        }
        else
        {
            CompareNode(actual, expected, rootPath, rootPath, default(NoMappingPathMode), options, differences, visitedPairs);
        }

        return differences;
    }

    private static bool HasMemberMappings(EquivalencyOptions options)
    {
        return options.HasTypedMemberMappings || options.HasMemberNameMappings;
    }
}
