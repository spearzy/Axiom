using Axiom.Vectors;

namespace Axiom.Tests.Vectors.HaveReciprocalRank;

public sealed class HaveReciprocalRankTests
{
    [Fact]
    public void HaveReciprocalRank_Passes_WhenTargetAppearsAtExpectedRank()
    {
        var results = new[] { "doc-2", "doc-7", "doc-5" };

        var continuation = results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 0.5);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void HaveReciprocalRank_UsesFirstOccurrence_WhenResultsContainDuplicates()
    {
        var results = new[] { "doc-7", "doc-1", "doc-7" };

        var continuation = results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 1d);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void HaveReciprocalRank_ReportsFirstOccurrence_WhenResultsContainDuplicates()
    {
        var results = new[] { "doc-7", "doc-1", "doc-7" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 1d / 3d));

        Assert.Contains("Expected results to have reciprocal rank for item \"doc-7\" equal to 0.333333333333333", ex.Message);
        Assert.Contains("actual reciprocal rank for item \"doc-7\" was 1", ex.Message);
        Assert.Contains("first found at rank 1", ex.Message);
    }

    [Fact]
    public void HaveReciprocalRank_Throws_WhenTargetAppearsAtDifferentRank()
    {
        var results = new[] { "doc-1", "doc-2", "doc-7", "doc-5" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 0.5d, tolerance: 0.001d));

        Assert.Contains("Expected results to have reciprocal rank for item \"doc-7\" equal to 0.5 within tolerance 0.001", ex.Message);
        Assert.Contains("actual reciprocal rank for item \"doc-7\" was 0.333333333333333", ex.Message);
        Assert.Contains("first found at rank 3", ex.Message);
    }

    [Fact]
    public void HaveReciprocalRank_Throws_WhenTargetIsMissing()
    {
        var results = new[] { "doc-1", "doc-2", "doc-5" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 1d, tolerance: 0.001d));

        Assert.Contains("Expected results to have reciprocal rank for item \"doc-7\" equal to 1 within tolerance 0.001", ex.Message);
        Assert.Contains("item \"doc-7\" was missing entirely; actual reciprocal rank was 0", ex.Message);
    }

    [Fact]
    public void HaveReciprocalRank_Throws_WhenResultsAreEmpty()
    {
        string[] results = [];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 1d, tolerance: 0.001d));

        Assert.Contains("actual reciprocal rank was 0", ex.Message);
    }

    [Fact]
    public void HaveReciprocalRank_ThrowsForOutOfRangeExpectedReciprocalRank()
    {
        var results = new[] { "doc-7" };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 1.5d, tolerance: 0.001d));

        Assert.Equal("expectedReciprocalRank", ex.ParamName);
    }

    [Fact]
    public void HaveReciprocalRank_ThrowsForNegativeTolerance()
    {
        var results = new[] { "doc-7" };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 1d, tolerance: -0.1d));

        Assert.Equal("tolerance", ex.ParamName);
    }

    [Fact]
    public void HaveReciprocalRank_Throws_WhenSubjectIsNull()
    {
        List<string> results = null!;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 1d, tolerance: 0.001d));

        Assert.Contains("Expected results to have reciprocal rank for item \"doc-7\" equal to 1 within tolerance 0.001", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }
}
