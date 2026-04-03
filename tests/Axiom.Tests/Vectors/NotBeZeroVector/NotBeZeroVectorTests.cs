using Axiom.Vectors;

namespace Axiom.Tests.Vectors.NotBeZeroVector;

public sealed class NotBeZeroVectorTests
{
    [Fact]
    public void NotBeZeroVector_Passes_WhenAnyComponentIsNonZero()
    {
        float[] embedding = [0f, -1f, 0f];

        var continuation = embedding.Should().NotBeZeroVector();

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void NotBeZeroVector_Throws_WhenAllComponentsAreZero()
    {
        double[] embedding = [0d, 0d, 0d];

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().NotBeZeroVector());

        Assert.Contains("Expected embedding to not be a zero vector", ex.Message);
        Assert.Contains("all 3 component(s) were zero", ex.Message);
    }

    [Fact]
    public void NotBeZeroVector_Throws_ForEmptyVector()
    {
        float[] embedding = [];

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().NotBeZeroVector());

        Assert.Contains("Expected embedding to not be a zero vector", ex.Message);
        Assert.Contains("vector had no components and was zero by definition", ex.Message);
    }

    [Fact]
    public void NotBeZeroVector_Throws_WhenSubjectIsNull()
    {
        float[]? embedding = null;

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().NotBeZeroVector());

        Assert.Contains("Expected embedding to not be a zero vector", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }
}
