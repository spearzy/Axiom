using Axiom.Vectors;

namespace Axiom.Tests.Vectors.HaveDimension;

public sealed class HaveDimensionTests
{
    [Fact]
    public void HaveDimension_Passes_WhenLengthMatches()
    {
        float[] embedding = [0.1f, 0.2f, 0.3f];

        var continuation = embedding.Should().HaveDimension(3);

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void HaveDimension_Throws_WhenLengthDiffers()
    {
        float[] embedding = [0.1f, 0.2f];

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().HaveDimension(3));

        Assert.Contains("Expected embedding to have dimension 3", ex.Message);
        Assert.Contains("found dimension 2", ex.Message);
    }

    [Fact]
    public void HaveDimension_ThrowsForNegativeExpectedDimension()
    {
        float[] embedding = [0.1f];

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => embedding.Should().HaveDimension(-1));

        Assert.Equal("expectedDimension", ex.ParamName);
    }

    [Fact]
    public void HaveDimension_Throws_WhenSubjectIsNull()
    {
        float[]? embedding = null;

        var ex = Assert.Throws<InvalidOperationException>(() => embedding.Should().HaveDimension(3));

        Assert.Contains("Expected embedding to have dimension 3", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }
}
