using Axiom.Vectors;

namespace Axiom.Tests.Vectors.HaveMeanReciprocalRank;

public sealed class HaveMeanReciprocalRankTests
{
    [Fact]
    public void HaveMeanReciprocalRank_Passes_WhenActualMrrMatchesExpectedValue()
    {
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2", "doc-7", "doc-5"], ["doc-2"]),
            new RankingQuery<string>(["doc-8", "doc-5", "doc-3"], ["doc-5"]),
        };

        var continuation = queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 0.75);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<RankingQuery<string>[]>>(continuation.And);
    }

    [Fact]
    public void HaveMeanReciprocalRank_TreatsMissingRelevantHitsAsZero()
    {
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2", "doc-7"], ["doc-2"]),
            new RankingQuery<string>(["doc-8", "doc-3"], ["doc-5"]),
        };

        var continuation = queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 0.5, tolerance: 0.001);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<RankingQuery<string>[]>>(continuation.And);
    }

    [Fact]
    public void HaveMeanReciprocalRank_UsesFirstRelevantHit_WhenResultsContainDuplicates()
    {
        var queries = new[]
        {
            new RankingQuery<string>(["doc-5", "doc-5", "doc-2"], ["doc-5"]),
        };

        var continuation = queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 1d, tolerance: 0.001);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<RankingQuery<string>[]>>(continuation.And);
    }

    [Fact]
    public void HaveMeanReciprocalRank_Throws_WhenActualMrrDiffers()
    {
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2", "doc-7", "doc-5"], ["doc-2"]),
            new RankingQuery<string>(["doc-8", "doc-5", "doc-3"], ["doc-5"]),
            new RankingQuery<string>(["doc-4", "doc-6"], ["doc-9"]),
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 1d, tolerance: 0.001d));

        Assert.Contains("Expected queries to have mean reciprocal rank equal to 1 within tolerance 0.001", ex.Message);
        Assert.Contains("actual mean reciprocal rank was 0.5 across 3 queries", ex.Message);
    }

    [Fact]
    public void HaveMeanReciprocalRank_Throws_WhenQuerySetIsEmpty()
    {
        RankingQuery<string>[] queries = [];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 1d, tolerance: 0.001d));

        Assert.Contains("query set was empty", ex.Message);
    }

    [Fact]
    public void HaveMeanReciprocalRank_ThrowsForEmptyRelevantItemsAtQueryConstruction()
    {
        var ex = Assert.Throws<ArgumentException>(() => new RankingQuery<string>(["doc-2"], Array.Empty<string>()));

        Assert.Equal("relevantItems", ex.ParamName);
    }

    [Fact]
    public void HaveMeanReciprocalRank_Throws_WhenSubjectIsNull()
    {
        List<RankingQuery<string>> queries = null!;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 1d, tolerance: 0.001d));

        Assert.Contains("Expected queries to have mean reciprocal rank equal to 1 within tolerance 0.001", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }
}
