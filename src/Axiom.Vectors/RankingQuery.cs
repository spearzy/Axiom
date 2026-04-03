namespace Axiom.Vectors;

public sealed class RankingQuery<T>
{
    private readonly T[] _rankedResults;
    private readonly HashSet<T> _relevantItems;

    public RankingQuery(IEnumerable<T> rankedResults, IEnumerable<T> relevantItems)
    {
        ArgumentNullException.ThrowIfNull(rankedResults);

        _rankedResults = rankedResults as T[] ?? rankedResults.ToArray();
        _relevantItems = RankingAssertionHelpers.CreateRelevantSet(relevantItems, nameof(relevantItems));
    }

    public IReadOnlyList<T> RankedResults => _rankedResults;

    public IReadOnlyCollection<T> RelevantItems => _relevantItems;

    internal IReadOnlySet<T> RelevantSet => _relevantItems;
}
