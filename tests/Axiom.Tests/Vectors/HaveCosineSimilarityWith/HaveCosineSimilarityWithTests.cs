using Axiom.Vectors;

namespace Axiom.Tests.Vectors.HaveCosineSimilarityWith;

public sealed class HaveCosineSimilarityWithTests
{
    [Fact]
    public void HaveCosineSimilarityWith_AtLeast_Passes_WhenThresholdIsMet()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [0.9999f, 0.01f];

        var continuation = embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.999f);

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void HaveCosineSimilarityWith_AtMost_Passes_WhenThresholdIsMet()
    {
        float[] embedding = [1f, 0f];
        float[] unrelated = [0f, 1f];

        var continuation = embedding.Should().HaveCosineSimilarityWith(unrelated).AtMost(0.2f);

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void HaveCosineSimilarityWith_Between_Passes_WhenSimilarityFallsInsideInclusiveRange()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [0.99f, 0.1f];

        var continuation = embedding.Should().HaveCosineSimilarityWith(expected).Between(0.99f, 1f);

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void HaveCosineSimilarityWith_ThresholdAssertion_And_ReturnsOriginalVectorAssertions()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [1f, 0f];

        var continuation = embedding.Should()
            .HaveCosineSimilarityWith(expected)
            .AtLeast(0.999f)
            .And
            .BeNormalized();

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void HaveCosineSimilarityWith_Between_And_BeNormalized_ChainsBackToVectorAssertions()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [0.99f, 0.1f];

        var continuation = embedding.Should()
            .HaveCosineSimilarityWith(expected)
            .Between(0.99f, 1f)
            .And
            .BeNormalized();

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void HaveCosineSimilarityWith_ExposesComputedSimilarity()
    {
        double[] embedding = [1d, 0d];
        double[] expected = [1d, 0d];

        var similarity = embedding.Should().HaveCosineSimilarityWith(expected).ActualSimilarity;

        Assert.Equal(1d, similarity);
    }

    [Fact]
    public void HaveCosineSimilarityWith_AtLeast_Throws_WhenThresholdIsNotMet()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [0f, 1f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.5f));

        Assert.Contains("Expected embedding to have cosine similarity with expected at least 0.5", ex.Message);
        Assert.Contains("computed cosine similarity 0", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityWith_AtMost_Throws_WhenThresholdIsExceeded()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [1f, 0f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).AtMost(0.2f));

        Assert.Contains("Expected embedding to have cosine similarity with expected at most 0.2", ex.Message);
        Assert.Contains("computed cosine similarity 1", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityWith_Between_Throws_WhenSimilarityFallsOutsideRange()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [1f, 0f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).Between(0.5f, 0.9f));

        Assert.Contains("Expected embedding to have cosine similarity with expected between 0.5 and 0.9 inclusive", ex.Message);
        Assert.Contains("computed cosine similarity 1", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityWith_AtMost_ThrowsForOutOfRangeThreshold()
    {
        float[] embedding = [1f];
        float[] expected = [1f];

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).AtMost(1.5f));

        Assert.Equal("threshold", ex.ParamName);
    }

    [Fact]
    public void HaveCosineSimilarityWith_Between_ThrowsForOutOfRangeMinimumThreshold()
    {
        float[] embedding = [1f];
        float[] expected = [1f];

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).Between(-1.5f, 0.5f));

        Assert.Equal("minimumThreshold", ex.ParamName);
    }

    [Fact]
    public void HaveCosineSimilarityWith_Between_ThrowsForOutOfRangeMaximumThreshold()
    {
        float[] embedding = [1f];
        float[] expected = [1f];

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).Between(0.5f, 1.5f));

        Assert.Equal("maximumThreshold", ex.ParamName);
    }

    [Fact]
    public void HaveCosineSimilarityWith_Between_Throws_WhenMinimumExceedsMaximum()
    {
        float[] embedding = [1f];
        float[] expected = [1f];

        var ex = Assert.Throws<ArgumentException>(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).Between(0.9f, 0.5f));

        Assert.Equal("minimumThreshold", ex.ParamName);
    }

    [Fact]
    public void HaveCosineSimilarityWith_ActualSimilarity_Throws_WhenUnavailable()
    {
        float[] embedding = [0f, 0f];
        float[] expected = [1f, 0f];

        var continuation = embedding.Should().HaveCosineSimilarityWith(expected);
        var ex = Assert.Throws<InvalidOperationException>(() => _ = continuation.ActualSimilarity);

        Assert.Contains("ActualSimilarity is unavailable because HaveCosineSimilarityWith failed", ex.Message);
        Assert.Contains("actual vector has zero magnitude", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityWith_AtLeast_Throws_ForZeroMagnitudeActualVector()
    {
        float[] embedding = [0f, 0f];
        float[] expected = [1f, 0f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.1f));

        Assert.Contains("Expected embedding to have cosine similarity with expected at least 0.1", ex.Message);
        Assert.Contains("actual vector has zero magnitude", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityWith_Between_Throws_ForBothZeroVectors()
    {
        float[] embedding = [0f, 0f];
        float[] expected = [0f, 0f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).Between(-0.1f, 0.1f));

        Assert.Contains("Expected embedding to have cosine similarity with expected between -0.1 and 0.1 inclusive", ex.Message);
        Assert.Contains("actual and expected vectors both have zero magnitude", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityWith_AtLeast_Throws_OnDimensionMismatch()
    {
        double[] embedding = [1d, 0d];
        double[] expected = [1d];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.5d));

        Assert.Contains("Expected embedding to have cosine similarity with expected at least 0.5", ex.Message);
        Assert.Contains("dimensions differed: expected 1, found 2", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityWith_AtLeast_Throws_WhenSimilarityIsNonFinite()
    {
        float[] embedding = [float.NaN, 1f];
        float[] expected = [1f, 1f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.5f));

        Assert.Contains("Expected embedding to have cosine similarity with expected at least 0.5", ex.Message);
        Assert.Contains("computed non-finite cosine similarity NaN", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityTo_RoutesToCompatibilityAlias()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [1f, 0f];

        var similarity = embedding.Should().HaveCosineSimilarityTo(expected).ActualSimilarity;

        Assert.Equal(1f, similarity);
    }
}
