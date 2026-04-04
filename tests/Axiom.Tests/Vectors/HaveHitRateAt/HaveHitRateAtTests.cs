using Axiom.Vectors;

namespace Axiom.Tests.Vectors.HaveHitRateAt;

public sealed class HaveHitRateAtTests
{
    [Fact]
    public void HaveHitRateAt_Passes_WhenActualHitRateMatchesExpectedValue()
    {
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2", "doc-7", "doc-5"], ["doc-2"]),
            new RankingQuery<string>(["doc-8", "doc-5", "doc-3"], ["doc-5"]),
        };

        var continuation = queries.Should().HaveHitRateAt(k: 1, expectedHitRate: 0.5);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<RankingQuery<string>[]>>(continuation.And);
    }

    [Fact]
    public void HaveHitRateAt_TreatsMissingRelevantHitsAsMisses()
    {
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2", "doc-7"], ["doc-2"]),
            new RankingQuery<string>(["doc-8", "doc-3"], ["doc-5"]),
        };

        var continuation = queries.Should().HaveHitRateAt(k: 2, expectedHitRate: 0.5);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<RankingQuery<string>[]>>(continuation.And);
    }

    [Fact]
    public void HaveHitRateAt_UsesAvailableResults_WhenFewerThanKResultsAreReturned()
    {
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2"], ["doc-2"]),
            new RankingQuery<string>(["doc-8"], ["doc-5"]),
        };

        var continuation = queries.Should().HaveHitRateAt(k: 5, expectedHitRate: 0.5);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<RankingQuery<string>[]>>(continuation.And);
    }

    [Fact]
    public void HaveHitRateAt_Throws_WhenActualHitRateDiffers()
    {
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2", "doc-7", "doc-5"], ["doc-2"]),
            new RankingQuery<string>(["doc-8", "doc-5", "doc-3"], ["doc-5"]),
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            queries.Should().HaveHitRateAt(k: 1, expectedHitRate: 1d, tolerance: 0.001d));

        Assert.Contains("Expected queries to have hit rate@1 equal to 1 within tolerance 0.001", ex.Message);
        Assert.Contains("actual hit rate@1 was 0.5 (1 of 2 queries had a relevant hit)", ex.Message);
    }

    [Fact]
    public void HaveHitRateAt_Throws_WhenQuerySetIsEmpty()
    {
        RankingQuery<string>[] queries = [];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            queries.Should().HaveHitRateAt(k: 1, expectedHitRate: 1d));

        Assert.Contains("Expected queries to have hit rate@1 equal to 1", ex.Message);
        Assert.DoesNotContain("within tolerance", ex.Message);
        Assert.Contains("query set was empty", ex.Message);
    }

    [Fact]
    public void HaveHitRateAt_ThrowsForNonPositiveK()
    {
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2"], ["doc-2"]),
        };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            queries.Should().HaveHitRateAt(k: 0, expectedHitRate: 1d, tolerance: 0.001d));

        Assert.Equal("k", ex.ParamName);
    }

    [Fact]
    public void HaveHitRateAt_ThrowsForOutOfRangeExpectedHitRate()
    {
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2"], ["doc-2"]),
        };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            queries.Should().HaveHitRateAt(k: 1, expectedHitRate: 1.1d));

        Assert.Equal("expectedHitRate", ex.ParamName);
    }

    [Fact]
    public void HaveHitRateAt_ThrowsForNegativeTolerance()
    {
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2"], ["doc-2"]),
        };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            queries.Should().HaveHitRateAt(k: 1, expectedHitRate: 1d, tolerance: -0.1d));

        Assert.Equal("tolerance", ex.ParamName);
    }

    [Fact]
    public void HaveHitRateAt_Throws_WhenSubjectIsNull()
    {
        List<RankingQuery<string>> queries = null!;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            queries.Should().HaveHitRateAt(k: 1, expectedHitRate: 1d, tolerance: 0.001d));

        Assert.Contains("Expected queries to have hit rate@1 equal to 1 within tolerance 0.001", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }
}
