using Axiom.Vectors;

namespace Axiom.Tests.Vectors.HaveEuclideanDistanceTo;

public sealed class HaveEuclideanDistanceToTests
{
    [Fact]
    public void HaveEuclideanDistanceTo_Passes_WhenDistanceMatchesWithinTolerance()
    {
        float[] embedding = [1f, 2f];
        float[] expected = [4f, 6f];

        var continuation = embedding.Should().HaveEuclideanDistanceTo(expected, 5f, 0.0001f);

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void HaveEuclideanDistanceTo_Throws_WhenDistanceDiffers()
    {
        double[] embedding = [0d, 0d];
        double[] expected = [3d, 4d];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveEuclideanDistanceTo(expected, 4d, 0.001d));

        Assert.Contains("Expected embedding to have Euclidean distance to expected equal to 4 within tolerance 0.001", ex.Message);
        Assert.Contains("computed Euclidean distance 5", ex.Message);
    }

    [Fact]
    public void HaveEuclideanDistanceTo_Throws_OnDimensionMismatch()
    {
        float[] embedding = [1f, 2f];
        float[] expected = [1f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveEuclideanDistanceTo(expected, 0f, 0.0001f));

        Assert.Contains("Expected embedding to have Euclidean distance to expected equal to 0 within tolerance 0.0001", ex.Message);
        Assert.Contains("dimensions differed: expected 1, found 2", ex.Message);
    }

    [Fact]
    public void HaveEuclideanDistanceTo_Throws_WhenSubjectIsNull()
    {
        double[]? embedding = null;
        double[] expected = [1d, 0d];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveEuclideanDistanceTo(expected, 1d, 0.001d));

        Assert.Contains("Expected embedding to have Euclidean distance to expected equal to 1 within tolerance 0.001", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }

    [Fact]
    public void HaveEuclideanDistanceTo_ThrowsForNullExpectedArray()
    {
        double[] embedding = [1d, 0d];
        double[]? expected = null;

        Assert.Throws<ArgumentNullException>(() => embedding.Should().HaveEuclideanDistanceTo(expected!, 1d, 0.001d));
    }

    [Fact]
    public void HaveEuclideanDistanceTo_ThrowsForNegativeExpectedDistance()
    {
        double[] embedding = [1d, 0d];
        double[] expected = [1d, 0d];

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            embedding.Should().HaveEuclideanDistanceTo(expected, -1d, 0.001d));

        Assert.Equal("expectedDistance", ex.ParamName);
    }

    [Fact]
    public void HaveEuclideanDistanceTo_ThrowsForNegativeTolerance()
    {
        double[] embedding = [1d, 0d];
        double[] expected = [1d, 0d];

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            embedding.Should().HaveEuclideanDistanceTo(expected, 0d, -0.1d));

        Assert.Equal("tolerance", ex.ParamName);
    }
}
