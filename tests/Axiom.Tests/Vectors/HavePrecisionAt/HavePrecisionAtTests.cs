using Axiom.Vectors;

namespace Axiom.Tests.Vectors.HavePrecisionAt;

public sealed class HavePrecisionAtTests
{
    [Fact]
    public void HavePrecisionAt_Passes_WhenActualPrecisionMatchesExpectedValue()
    {
        var results = new[] { "doc-2", "doc-7", "doc-5", "doc-9" };
        var relevantItems = new[] { "doc-2", "doc-5" };

        var continuation = results.Should().HavePrecisionAt(2, relevantItems, expectedPrecision: 0.5);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void HavePrecisionAt_UsesRequestedKAsDenominator_WhenFewerThanKResultsAreReturned()
    {
        var results = new[] { "doc-2" };
        var relevantItems = new[] { "doc-2", "doc-5" };

        var continuation = results.Should().HavePrecisionAt(5, relevantItems, expectedPrecision: 0.2, tolerance: 0.001);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void HavePrecisionAt_DoesNotDoubleCountDuplicateResults()
    {
        var results = new[] { "doc-2", "doc-2", "doc-5" };
        var relevantItems = new[] { "doc-2", "doc-5" };

        var continuation = results.Should().HavePrecisionAt(2, relevantItems, expectedPrecision: 0.5, tolerance: 0.001);

        Assert.IsType<Axiom.Assertions.AssertionTypes.ValueAssertions<string[]>>(continuation.And);
    }

    [Fact]
    public void HavePrecisionAt_Throws_WhenActualPrecisionDiffers()
    {
        var results = new[] { "doc-2", "doc-7", "doc-9" };
        var relevantItems = new[] { "doc-2", "doc-5" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            results.Should().HavePrecisionAt(2, relevantItems, expectedPrecision: 1d, tolerance: 0.001d));

        Assert.Contains("Expected results to have precision@2 equal to 1 within tolerance 0.001", ex.Message);
        Assert.Contains("actual precision@2 was 0.5", ex.Message);
        Assert.Contains("matched 1 unique relevant item(s); denominator 2", ex.Message);
    }

    [Fact]
    public void HavePrecisionAt_Throws_WhenResultsAreEmpty()
    {
        string[] results = [];
        var relevantItems = new[] { "doc-2" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            results.Should().HavePrecisionAt(3, relevantItems, expectedPrecision: 1d, tolerance: 0.001d));

        Assert.Contains("actual precision@3 was 0", ex.Message);
    }

    [Fact]
    public void HavePrecisionAt_ThrowsForEmptyRelevantSet()
    {
        var results = new[] { "doc-2" };

        var ex = Assert.Throws<ArgumentException>(() =>
            results.Should().HavePrecisionAt(1, Array.Empty<string>(), expectedPrecision: 0d, tolerance: 0.001d));

        Assert.Equal("relevantItems", ex.ParamName);
    }

    [Fact]
    public void HavePrecisionAt_ThrowsForOutOfRangeExpectedPrecision()
    {
        var results = new[] { "doc-2" };
        var relevantItems = new[] { "doc-2" };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            results.Should().HavePrecisionAt(1, relevantItems, expectedPrecision: -0.1d, tolerance: 0.001d));

        Assert.Equal("expectedPrecision", ex.ParamName);
    }

    [Fact]
    public void HavePrecisionAt_Throws_WhenSubjectIsNull()
    {
        List<string> results = null!;
        var relevantItems = new[] { "doc-2" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            results.Should().HavePrecisionAt(1, relevantItems, expectedPrecision: 1d, tolerance: 0.001d));

        Assert.Contains("Expected results to have precision@1 equal to 1 within tolerance 0.001", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }
}
