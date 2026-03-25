using Axiom.Vectors;

namespace Axiom.Tests.Vectors.BeNormalized;

public sealed class BeNormalizedTests
{
    [Fact]
    public void BeNormalized_Passes_WhenNormIsWithinTolerance()
    {
        var normalized = (float)(1 / Math.Sqrt(2));
        float[] embedding = [normalized, normalized];

        var continuation = embedding.Should().BeNormalized(0.0001f);

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void BeNormalized_UsesDefaultTolerance()
    {
        double[] embedding = [1d, 0d];

        var continuation = embedding.Should().BeNormalized();

        Assert.IsType<VectorAssertions<double>>(continuation.And);
    }

    [Fact]
    public void BeNormalized_Throws_WhenNormDiffers()
    {
        float[] embedding = [2f, 0f];

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().BeNormalized(0.001f));

        Assert.Contains("Expected embedding to be normalized within tolerance 0.001", ex.Message);
        Assert.Contains("computed L2 norm 2", ex.Message);
    }

    [Fact]
    public void BeNormalized_ThrowsForNegativeTolerance()
    {
        float[] embedding = [1f];

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => embedding.Should().BeNormalized(-0.1f));

        Assert.Equal("tolerance", ex.ParamName);
    }

    [Fact]
    public void BeNormalized_Throws_WhenSubjectIsNull()
    {
        float[]? embedding = null;

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().BeNormalized(0.001f));

        Assert.Contains("Expected embedding to be normalized within tolerance 0.001", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }

    [Fact]
    public void BeNormalized_IncludesBecauseReason()
    {
        float[] embedding = [2f, 0f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().BeNormalized(0.001f, because: "the embedding should be unit length"));

        Assert.Contains("because the embedding should be unit length", ex.Message);
    }
}
