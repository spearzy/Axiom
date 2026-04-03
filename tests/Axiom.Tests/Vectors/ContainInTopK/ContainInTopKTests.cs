using Axiom.Vectors;

namespace Axiom.Tests.Vectors.ContainInTopK;

public sealed class ContainInTopKTests
{
    [Fact]
    public void ContainInTopK_Passes_WhenTargetAppearsWithinTopK()
    {
        var results = new[] { "doc-2", "doc-7", "doc-5" };

        var continuation = results.Should().ContainInTopK("doc-7", 2);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void ContainInTopK_Passes_WhenAnEarlyDuplicateIsWithinTopK()
    {
        var results = new[] { "doc-7", "doc-1", "doc-7", "doc-9" };

        var continuation = results.Should().ContainInTopK("doc-7", 1);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void ContainInTopK_ReportsEarliestFoundRank_WhenResultsContainDuplicates()
    {
        var results = new[] { "doc-1", "doc-2", "doc-7", "doc-3", "doc-7" };

        var ex = Assert.Throws<InvalidOperationException>(() => results.Should().ContainInTopK("doc-7", 2));

        Assert.Contains("Expected results to contain item \"doc-7\" in the top 2 result(s)", ex.Message);
        Assert.Contains("item \"doc-7\" was found at rank 3", ex.Message);
        Assert.DoesNotContain("rank 5", ex.Message);
    }

    [Fact]
    public void ContainInTopK_Throws_WhenTargetAppearsBelowTopK()
    {
        var results = new[] { "doc-1", "doc-2", "doc-7", "doc-5" };

        var ex = Assert.Throws<InvalidOperationException>(() => results.Should().ContainInTopK("doc-7", 2));

        Assert.Contains("Expected results to contain item \"doc-7\" in the top 2 result(s)", ex.Message);
        Assert.Contains("item \"doc-7\" was found at rank 3", ex.Message);
    }

    [Fact]
    public void ContainInTopK_Throws_WhenTargetIsMissing()
    {
        var results = new[] { "doc-1", "doc-2", "doc-5" };

        var ex = Assert.Throws<InvalidOperationException>(() => results.Should().ContainInTopK("doc-7", 3));

        Assert.Contains("Expected results to contain item \"doc-7\" in the top 3 result(s)", ex.Message);
        Assert.Contains("item \"doc-7\" was missing entirely", ex.Message);
    }

    [Fact]
    public void ContainInTopK_Throws_WhenResultsAreEmpty()
    {
        string[] results = [];

        var ex = Assert.Throws<InvalidOperationException>(() => results.Should().ContainInTopK("doc-7", 3));

        Assert.Contains("item \"doc-7\" was missing entirely", ex.Message);
    }

    [Fact]
    public void ContainInTopK_ThrowsForNonPositiveK()
    {
        var results = new[] { "doc-7" };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => results.Should().ContainInTopK("doc-7", 0));

        Assert.Equal("k", ex.ParamName);
    }

    [Fact]
    public void ContainInTopK_Throws_WhenSubjectIsNull()
    {
        List<string> results = null!;

        var ex = Assert.Throws<InvalidOperationException>(() => results.Should().ContainInTopK("doc-7", 2));

        Assert.Contains("Expected results to contain item \"doc-7\" in the top 2 result(s)", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }
}
