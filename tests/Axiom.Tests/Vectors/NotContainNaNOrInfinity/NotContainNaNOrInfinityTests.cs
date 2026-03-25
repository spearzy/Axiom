using Axiom.Vectors;

namespace Axiom.Tests.Vectors.NotContainNaNOrInfinity;

public sealed class NotContainNaNOrInfinityTests
{
    [Fact]
    public void NotContainNaNOrInfinity_Passes_ForFiniteValues()
    {
        double[] embedding = [0.1d, -0.5d, 1.2d];

        var continuation = embedding.Should().NotContainNaNOrInfinity();

        Assert.IsType<VectorAssertions<double>>(continuation.And);
    }

    [Fact]
    public void NotContainNaNOrInfinity_Throws_OnNaN()
    {
        float[] embedding = [0.1f, float.NaN, 0.3f];

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().NotContainNaNOrInfinity());

        Assert.Contains("Expected embedding to not contain NaN or Infinity", ex.Message);
        Assert.Contains("found NaN at index 1", ex.Message);
    }

    [Fact]
    public void NotContainNaNOrInfinity_Throws_OnInfinity()
    {
        double[] embedding = [0.1d, double.PositiveInfinity];

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().NotContainNaNOrInfinity());

        Assert.Contains("Expected embedding to not contain NaN or Infinity", ex.Message);
        Assert.Contains("found Infinity at index 1", ex.Message);
    }

    [Fact]
    public void NotContainNaNOrInfinity_Throws_WhenSubjectIsNull()
    {
        double[]? embedding = null;

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().NotContainNaNOrInfinity());

        Assert.Contains("Expected embedding to not contain NaN or Infinity", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }
}
