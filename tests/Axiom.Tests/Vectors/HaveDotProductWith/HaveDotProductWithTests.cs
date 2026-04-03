using Axiom.Vectors;

namespace Axiom.Tests.Vectors.HaveDotProductWith;

public sealed class HaveDotProductWithTests
{
    [Fact]
    public void HaveDotProductWith_Passes_WhenDotProductMatchesWithinTolerance()
    {
        double[] embedding = [1d, -2d];
        double[] expected = [3d, 4d];

        var continuation = embedding.Should().HaveDotProductWith(expected, -5d, 0.000001d);

        Assert.IsType<VectorAssertions<double>>(continuation.And);
    }

    [Fact]
    public void HaveDotProductWith_Passes_ForEmptyVectors_WhenExpectedDotProductIsZero()
    {
        float[] embedding = [];
        float[] expected = [];

        var continuation = embedding.Should().HaveDotProductWith(expected, 0f, 0f);

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void HaveDotProductWith_Throws_WhenDotProductDiffers()
    {
        float[] embedding = [1f, 2f];
        float[] expected = [3f, 4f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveDotProductWith(expected, 10f, 0.001f));

        Assert.Contains("Expected embedding to have dot product with expected equal to 10 within tolerance 0.001", ex.Message);
        Assert.Contains("dot product differed: expected 10, found 11, delta 1", ex.Message);
    }

    [Fact]
    public void HaveDotProductWith_Throws_WhenLateComponentsChangeDotProduct()
    {
        double[] embedding = [1d, 2d, 3d, 5d];
        double[] expected = [1d, 2d, 3d, 4d];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveDotProductWith(expected, 30d, 0.001d));

        Assert.Contains("dot product differed: expected 30, found 34, delta 4", ex.Message);
    }

    [Fact]
    public void HaveDotProductWith_Throws_OnDimensionMismatch()
    {
        double[] embedding = [1d, 2d];
        double[] expected = [3d];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveDotProductWith(expected, 3d, 0.0001d));

        Assert.Contains("Expected embedding to have dot product with expected equal to 3 within tolerance 0.0001", ex.Message);
        Assert.Contains("dimensions differed: expected 1, found 2", ex.Message);
    }

    [Fact]
    public void HaveDotProductWith_Throws_WhenSubjectIsNull()
    {
        float[]? embedding = null;
        float[] expected = [1f, 0f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            embedding.Should().HaveDotProductWith(expected, 0f, 0.001f));

        Assert.Contains("Expected embedding to have dot product with expected equal to 0 within tolerance 0.001", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }

    [Fact]
    public void HaveDotProductWith_ThrowsForNullExpectedArray()
    {
        float[] embedding = [1f, 0f];
        float[]? expected = null;

        Assert.Throws<ArgumentNullException>(() => embedding.Should().HaveDotProductWith(expected!, 0f, 0.001f));
    }

    [Fact]
    public void HaveDotProductWith_ThrowsForNonFiniteExpectedDotProduct()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [1f, 0f];

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            embedding.Should().HaveDotProductWith(expected, float.NaN, 0.001f));

        Assert.Equal("expectedDotProduct", ex.ParamName);
    }

    [Fact]
    public void HaveDotProductWith_ThrowsForNegativeTolerance()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [1f, 0f];

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            embedding.Should().HaveDotProductWith(expected, 1f, -0.1f));

        Assert.Equal("tolerance", ex.ParamName);
    }
}
