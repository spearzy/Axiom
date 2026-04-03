using Axiom.Vectors;

namespace Axiom.Tests.Vectors.HaveRank;

public sealed class HaveRankTests
{
    [Fact]
    public void HaveRank_Passes_WhenTargetAppearsAtExpectedRank()
    {
        var results = new[] { "doc-2", "doc-7", "doc-5" };

        var continuation = results.Should().HaveRank("doc-7", 2);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void HaveRank_UsesFirstOccurrence_WhenResultsContainDuplicates()
    {
        var results = new[] { "doc-7", "doc-1", "doc-7" };

        var continuation = results.Should().HaveRank("doc-7", 1);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void HaveRank_Throws_WhenTargetAppearsAtDifferentRank()
    {
        var results = new[] { "doc-1", "doc-2", "doc-7", "doc-5" };

        var ex = Assert.Throws<InvalidOperationException>(() => results.Should().HaveRank("doc-7", 2));

        Assert.Contains("Expected results to have item \"doc-7\" at rank 2", ex.Message);
        Assert.Contains("item \"doc-7\" was found at rank 3", ex.Message);
    }

    [Fact]
    public void HaveRank_Throws_WhenTargetIsMissing()
    {
        var results = new[] { "doc-1", "doc-2", "doc-5" };

        var ex = Assert.Throws<InvalidOperationException>(() => results.Should().HaveRank("doc-7", 2));

        Assert.Contains("Expected results to have item \"doc-7\" at rank 2", ex.Message);
        Assert.Contains("item \"doc-7\" was missing entirely", ex.Message);
    }

    [Fact]
    public void HaveRank_ThrowsForNonPositiveExpectedRank()
    {
        var results = new[] { "doc-7" };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => results.Should().HaveRank("doc-7", 0));

        Assert.Equal("expectedRank", ex.ParamName);
    }

    [Fact]
    public void HaveRank_Throws_WhenSubjectIsNull()
    {
        List<string> results = null!;

        var ex = Assert.Throws<InvalidOperationException>(() => results.Should().HaveRank("doc-7", 2));

        Assert.Contains("Expected results to have item \"doc-7\" at rank 2", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }
}
