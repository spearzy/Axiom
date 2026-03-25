using Axiom.Vectors;

namespace Axiom.Tests.Vectors.EntryPoints;

public sealed class VectorShouldEntryPointTests
{
    [Fact]
    public void FloatArray_Should_ReturnsVectorAssertions()
    {
        float[] embedding = [0.1f, 0.2f];

        var assertions = embedding.Should();

        Assert.IsType<VectorAssertions<float>>(assertions);
    }

    [Fact]
    public void ReadOnlyMemory_Should_ReturnsVectorAssertions()
    {
        ReadOnlyMemory<double> embedding = new double[] { 0.1d, 0.2d };

        var assertions = embedding.Should();

        Assert.IsType<VectorAssertions<double>>(assertions);
    }

    [Fact]
    public void NullFloatArray_Should_ReturnsVectorAssertions()
    {
        float[]? embedding = null;

        var assertions = embedding.Should();

        Assert.IsType<VectorAssertions<float>>(assertions);
    }
}
