using Axiom.Vectors;

namespace Axiom.Tests.Vectors.BeZeroVector;

public sealed class BeZeroVectorTests
{
    [Fact]
    public void BeZeroVector_Passes_WhenAllComponentsAreZero()
    {
        double[] embedding = [0d, -0d, 0d];

        var continuation = embedding.Should().BeZeroVector();

        Assert.IsType<VectorAssertions<double>>(continuation.And);
    }

    [Fact]
    public void BeZeroVector_Passes_ForEmptyVector()
    {
        float[] embedding = [];

        var ex = Record.Exception(() => embedding.Should().BeZeroVector());

        Assert.Null(ex);
    }

    [Fact]
    public void BeZeroVector_Throws_WhenComponentIsNonZero()
    {
        float[] embedding = [0f, 0f, 0f, 0.5f];

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().BeZeroVector());

        Assert.Contains("Expected embedding to be a zero vector", ex.Message);
        Assert.Contains("index 3 differed: expected 0, found 0.5", ex.Message);
    }

    [Fact]
    public void BeZeroVector_Throws_WhenSubjectIsNull()
    {
        float[]? embedding = null;

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().BeZeroVector());

        Assert.Contains("Expected embedding to be a zero vector", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }

    [Fact]
    public void BeZeroVector_IncludesBecauseReason()
    {
        double[] embedding = [0d, 1d];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().BeZeroVector(because: "padding vectors should stay zero"));

        Assert.Contains("because padding vectors should stay zero", ex.Message);
    }
}
