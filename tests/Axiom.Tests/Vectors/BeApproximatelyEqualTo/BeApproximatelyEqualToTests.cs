using Axiom.Vectors;

namespace Axiom.Tests.Vectors.BeApproximatelyEqualTo;

public sealed class BeApproximatelyEqualToTests
{
    [Fact]
    public void BeApproximatelyEqualTo_Passes_ForFloatArrayWithinTolerance()
    {
        float[] actual = [0.1f, 0.2f, 0.30001f];
        float[] expected = [0.1f, 0.2f, 0.3f];

        var continuation = actual.Should().BeApproximatelyEqualTo(expected, 0.0001f);

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void BeApproximatelyEqualTo_Passes_ForDoubleReadOnlyMemoryWithinTolerance()
    {
        ReadOnlyMemory<double> actual = new double[] { 0.1d, 0.2d, 0.3000001d };
        ReadOnlyMemory<double> expected = new double[] { 0.1d, 0.2d, 0.3d };

        var continuation = actual.Should().BeApproximatelyEqualTo(expected, 0.000001d);

        Assert.IsType<VectorAssertions<double>>(continuation.And);
    }

    [Fact]
    public void BeApproximatelyEqualTo_Passes_ForEmptyVectors()
    {
        float[] actual = [];
        float[] expected = [];

        var continuation = actual.Should().BeApproximatelyEqualTo(expected, 0f);

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }

    [Fact]
    public void BeApproximatelyEqualTo_Throws_OnFirstFloatMismatch()
    {
        float[] actual = [0.1f, 0.2f, 0.31f];
        float[] expected = [0.1f, 0.2f, 0.3f];

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeApproximatelyEqualTo(expected, 0.001f));

        Assert.Contains("Expected actual to be approximately equal to expected within tolerance 0.001", ex.Message);
        Assert.Contains("index 2 differed", ex.Message);
        Assert.Contains("expected 0.3", ex.Message);
        Assert.Contains("found 0.31", ex.Message);
    }

    [Fact]
    public void BeApproximatelyEqualTo_Throws_OnLateMismatch()
    {
        float[] actual = [0.1f, 0.2f, 0.3f, 0.45f];
        float[] expected = [0.1f, 0.2f, 0.3f, 0.4f];

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeApproximatelyEqualTo(expected, 0.001f));

        Assert.Contains("index 3 differed", ex.Message);
        Assert.DoesNotContain("index 0 differed", ex.Message);
    }

    [Fact]
    public void BeApproximatelyEqualTo_Throws_OnFirstDoubleMismatch()
    {
        double[] actual = [0.1d, 0.25d];
        double[] expected = [0.1d, 0.2d];

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeApproximatelyEqualTo(expected, 0.001d));

        Assert.Contains("Expected actual to be approximately equal to expected within tolerance 0.001", ex.Message);
        Assert.Contains("index 1 differed", ex.Message);
        Assert.Contains("expected 0.2", ex.Message);
        Assert.Contains("found 0.25", ex.Message);
    }

    [Fact]
    public void BeApproximatelyEqualTo_Throws_OnDimensionMismatch()
    {
        float[] actual = [0.1f, 0.2f];
        float[] expected = [0.1f];

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeApproximatelyEqualTo(expected, 0.001f));

        Assert.Contains("Expected actual to be approximately equal to expected within tolerance 0.001", ex.Message);
        Assert.Contains("dimensions differed: expected 1, found 2", ex.Message);
    }

    [Fact]
    public void BeApproximatelyEqualTo_ThrowsForNegativeTolerance()
    {
        float[] actual = [0.1f];
        float[] expected = [0.1f];

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => actual.Should().BeApproximatelyEqualTo(expected, -0.1f));

        Assert.Equal("tolerance", ex.ParamName);
    }

    [Fact]
    public void BeApproximatelyEqualTo_Throws_WhenSubjectIsNull()
    {
        float[]? actual = null;
        float[] expected = [0.1f];

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeApproximatelyEqualTo(expected, 0.01f));

        Assert.Contains("Expected actual to be approximately equal to expected within tolerance 0.01", ex.Message);
        Assert.Contains("found <null>", ex.Message);
    }

    [Fact]
    public void BeApproximatelyEqualTo_ThrowsForNullExpectedArray()
    {
        float[] actual = [0.1f];
        float[]? expected = null;

        Assert.Throws<ArgumentNullException>(() => actual.Should().BeApproximatelyEqualTo(expected!, 0.01f));
    }
}
