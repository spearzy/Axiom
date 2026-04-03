using Axiom.Vectors;

namespace Axiom.Tests.Vectors.SupportedShapes;

public sealed class VectorSupportedShapeParityTests
{
    [Fact]
    public void FloatArray_Supports_VectorMetrics_And_ZeroVectorAssertions()
    {
        float[] embedding = [1f, 0f, 0f];
        float[] expected = [1f, 0f, 0f];
        float[] unrelated = [0f, 1f, 0f];
        float[] zero = [0f, 0f, 0f];

        var continuation = embedding.Should()
            .HaveDotProductWith(expected, 1f, 0.001f)
            .And.HaveEuclideanDistanceTo(unrelated, 1.4142135f, 0.001f)
            .And.NotBeZeroVector();

        Assert.IsType<VectorAssertions<float>>(continuation.And);
        Assert.IsType<VectorAssertions<float>>(zero.Should().BeZeroVector().And);
    }

    [Fact]
    public void DoubleArray_Supports_VectorMetrics_And_ZeroVectorAssertions()
    {
        double[] embedding = [1d, 0d, 0d];
        double[] expected = [1d, 0d, 0d];
        double[] unrelated = [0d, 1d, 0d];
        double[] zero = [0d, 0d, 0d];

        var continuation = embedding.Should()
            .HaveDotProductWith(expected, 1d, 0.001d)
            .And.HaveEuclideanDistanceTo(unrelated, 1.4142135623730951d, 0.001d)
            .And.NotBeZeroVector();

        Assert.IsType<VectorAssertions<double>>(continuation.And);
        Assert.IsType<VectorAssertions<double>>(zero.Should().BeZeroVector().And);
    }

    [Fact]
    public void ReadOnlyMemoryFloat_Supports_VectorMetrics_And_ZeroVectorAssertions()
    {
        ReadOnlyMemory<float> embedding = new float[] { 1f, 0f, 0f };
        ReadOnlyMemory<float> expected = new float[] { 1f, 0f, 0f };
        ReadOnlyMemory<float> unrelated = new float[] { 0f, 1f, 0f };
        ReadOnlyMemory<float> zero = new float[] { 0f, 0f, 0f };

        var continuation = embedding.Should()
            .HaveDotProductWith(expected, 1f, 0.001f)
            .And.HaveEuclideanDistanceTo(unrelated, 1.4142135f, 0.001f)
            .And.NotBeZeroVector();

        Assert.IsType<VectorAssertions<float>>(continuation.And);
        Assert.IsType<VectorAssertions<float>>(zero.Should().BeZeroVector().And);
    }

    [Fact]
    public void ReadOnlyMemoryDouble_Supports_VectorMetrics_And_ZeroVectorAssertions()
    {
        ReadOnlyMemory<double> embedding = new double[] { 1d, 0d, 0d };
        ReadOnlyMemory<double> expected = new double[] { 1d, 0d, 0d };
        ReadOnlyMemory<double> unrelated = new double[] { 0d, 1d, 0d };
        ReadOnlyMemory<double> zero = new double[] { 0d, 0d, 0d };

        var continuation = embedding.Should()
            .HaveDotProductWith(expected, 1d, 0.001d)
            .And.HaveEuclideanDistanceTo(unrelated, 1.4142135623730951d, 0.001d)
            .And.NotBeZeroVector();

        Assert.IsType<VectorAssertions<double>>(continuation.And);
        Assert.IsType<VectorAssertions<double>>(zero.Should().BeZeroVector().And);
    }
}
