using Axiom.Vectors;

namespace Axiom.Tests.Vectors.HaveRecallAt;

public sealed class HaveRecallAtTests
{
    [Fact]
    public void HaveRecallAt_Passes_WhenActualRecallMatchesExpectedValue()
    {
        var results = new[] { "doc-2", "doc-7", "doc-5", "doc-9" };
        var relevantItems = new[] { "doc-2", "doc-5" };

        var continuation = results.Should().HaveRecallAt(2, relevantItems, expectedRecall: 0.5);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void HaveRecallAt_Passes_WhenFewerThanKResultsAreReturned()
    {
        var results = new[] { "doc-2" };
        var relevantItems = new[] { "doc-2", "doc-5" };

        var continuation = results.Should().HaveRecallAt(3, relevantItems, expectedRecall: 0.5);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void HaveRecallAt_DoesNotDoubleCountDuplicateResults()
    {
        var results = new[] { "doc-2", "doc-2", "doc-5", "doc-9" };
        var relevantItems = new[] { "doc-2", "doc-5", "doc-8" };

        var continuation = results.Should().HaveRecallAt(3, relevantItems, expectedRecall: 0.6666666666666666d, tolerance: 0.001d);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void HaveRecallAt_Throws_WhenActualRecallDiffers()
    {
        var results = new[] { "doc-2", "doc-7", "doc-9" };
        var relevantItems = new[] { "doc-2", "doc-5" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            results.Should().HaveRecallAt(2, relevantItems, expectedRecall: 1d, tolerance: 0.001d));

        Assert.Contains("Expected results to have recall@2 equal to 1 within tolerance 0.001", ex.Message);
        Assert.Contains("actual recall@2 was 0.5", ex.Message);
        Assert.Contains("matched 1 of 2 relevant item(s)", ex.Message);
    }

    [Fact]
    public void HaveRecallAt_Throws_WhenResultsAreEmpty()
    {
        string[] results = [];
        var relevantItems = new[] { "doc-2" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            results.Should().HaveRecallAt(3, relevantItems, expectedRecall: 1d));

        Assert.Contains("Expected results to have recall@3 equal to 1", ex.Message);
        Assert.DoesNotContain("within tolerance", ex.Message);
        Assert.Contains("actual recall@3 was 0", ex.Message);
    }

    [Fact]
    public void HaveRecallAt_ThrowsForEmptyRelevantSet()
    {
        var results = new[] { "doc-2" };

        var ex = Assert.Throws<ArgumentException>(() =>
            results.Should().HaveRecallAt(1, Array.Empty<string>(), expectedRecall: 0d, tolerance: 0.001d));

        Assert.Equal("relevantItems", ex.ParamName);
    }

    [Fact]
    public void HaveRecallAt_ThrowsForNonPositiveK()
    {
        var results = new[] { "doc-2" };
        var relevantItems = new[] { "doc-2" };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            results.Should().HaveRecallAt(0, relevantItems, expectedRecall: 1d, tolerance: 0.001d));

        Assert.Equal("k", ex.ParamName);
    }

    [Fact]
    public void HaveRecallAt_ThrowsForOutOfRangeExpectedRecall()
    {
        var results = new[] { "doc-2" };
        var relevantItems = new[] { "doc-2" };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            results.Should().HaveRecallAt(1, relevantItems, expectedRecall: 1.5d, tolerance: 0.001d));

        Assert.Equal("expectedRecall", ex.ParamName);
    }

    [Fact]
    public void HaveRecallAt_ThrowsForNegativeTolerance()
    {
        var results = new[] { "doc-2" };
        var relevantItems = new[] { "doc-2" };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            results.Should().HaveRecallAt(1, relevantItems, expectedRecall: 1d, tolerance: -0.1d));

        Assert.Equal("tolerance", ex.ParamName);
    }

    [Fact]
    public void HaveRecallAt_Throws_WhenSubjectIsNull()
    {
        List<string> results = null!;
        var relevantItems = new[] { "doc-2" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            results.Should().HaveRecallAt(1, relevantItems, expectedRecall: 1d, tolerance: 0.001d));

        Assert.Contains("Expected results to have recall@1 equal to 1 within tolerance 0.001", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }
}
