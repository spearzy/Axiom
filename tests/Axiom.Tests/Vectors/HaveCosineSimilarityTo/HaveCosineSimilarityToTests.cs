using Axiom.Vectors;

namespace Axiom.Tests.Vectors.HaveCosineSimilarityTo;

public sealed class HaveCosineSimilarityToTests
{
    [Fact]
    public void HaveCosineSimilarityTo_AtLeast_Passes_WhenThresholdIsMet()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [0.9999f, 0.01f];

        var continuation = embedding.Should().HaveCosineSimilarityTo(expected).AtLeast(0.999f);

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void HaveCosineSimilarityTo_ExposesComputedSimilarity()
    {
        double[] embedding = [1d, 0d];
        double[] expected = [1d, 0d];

        var similarity = embedding.Should().HaveCosineSimilarityTo(expected).ActualSimilarity;

        Assert.Equal(1d, similarity);
    }

    [Fact]
    public void HaveCosineSimilarityTo_AtLeast_Throws_WhenThresholdIsNotMet()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [0f, 1f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityTo(expected).AtLeast(0.5f));

        Assert.Contains("Expected embedding to have cosine similarity to expected at least 0.5", ex.Message);
        Assert.Contains("computed cosine similarity 0", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityTo_AtLeast_Throws_ForZeroMagnitudeActualVector()
    {
        float[] embedding = [0f, 0f];
        float[] expected = [1f, 0f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityTo(expected).AtLeast(0.1f));

        Assert.Contains("Expected embedding to have cosine similarity to expected at least 0.1", ex.Message);
        Assert.Contains("actual vector has zero magnitude", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityTo_AtLeast_Throws_ForBothZeroVectors()
    {
        float[] embedding = [0f, 0f];
        float[] expected = [0f, 0f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityTo(expected).AtLeast(0.1f));

        Assert.Contains("Expected embedding to have cosine similarity to expected at least 0.1", ex.Message);
        Assert.Contains("actual and expected vectors both have zero magnitude", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityTo_AtLeast_Throws_OnDimensionMismatch()
    {
        double[] embedding = [1d, 0d];
        double[] expected = [1d];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityTo(expected).AtLeast(0.5d));

        Assert.Contains("Expected embedding to have cosine similarity to expected at least 0.5", ex.Message);
        Assert.Contains("dimensions differed: expected 1, found 2", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityTo_AtLeast_ThrowsForOutOfRangeThreshold()
    {
        float[] embedding = [1f];
        float[] expected = [1f];

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            embedding.Should().HaveCosineSimilarityTo(expected).AtLeast(1.5f));

        Assert.Equal("threshold", ex.ParamName);
    }

    [Fact]
    public void HaveCosineSimilarityTo_ActualSimilarity_Throws_WhenUnavailable()
    {
        float[] embedding = [0f, 0f];
        float[] expected = [1f, 0f];

        var continuation = embedding.Should().HaveCosineSimilarityTo(expected);
        var ex = Assert.Throws<InvalidOperationException>(() => _ = continuation.ActualSimilarity);

        Assert.Contains("ActualSimilarity is unavailable because HaveCosineSimilarityTo failed", ex.Message);
        Assert.Contains("actual vector has zero magnitude", ex.Message);
    }

    [Fact]
    public void HaveCosineSimilarityTo_AtLeast_Throws_WhenSimilarityIsNonFinite()
    {
        float[] embedding = [float.NaN, 1f];
        float[] expected = [1f, 1f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveCosineSimilarityTo(expected).AtLeast(0.5f));

        Assert.Contains("Expected embedding to have cosine similarity to expected at least 0.5", ex.Message);
        Assert.Contains("computed non-finite cosine similarity NaN", ex.Message);
    }
}
