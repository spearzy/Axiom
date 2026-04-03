using Axiom.Vectors;

namespace Axiom.Tests.Vectors.Batch;

public sealed class VectorBatchRoutingTests
{
    [Fact]
    public void VectorAssertion_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        float[] embedding = [0.1f, float.NaN];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => embedding.Should().NotContainNaNOrInfinity());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_ForVectorAssertions()
    {
        float[] actual = [0.1f, 0.31f];
        float[] expected = [0.1f, 0.3f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("vectors");
            actual.Should().BeApproximatelyEqualTo(expected, 0.001f);
            actual.Should().HaveDimension(3);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'vectors' failed with 2 assertion failure(s):", message);
        Assert.Contains("Expected actual to be approximately equal to expected within tolerance 0.001", message);
        Assert.Contains("Expected actual to have dimension 3", message);
    }

    [Fact]
    public void CosineSimilarityThresholdAssertion_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [1f, 0f];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).AtMost(0.2f));

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected embedding to have cosine similarity with expected at most 0.2", disposeEx.Message);
        Assert.Contains("computed cosine similarity 1", disposeEx.Message);
    }

    [Fact]
    public void CosineSimilarityUnavailableAssertion_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        float[] embedding = [0f, 0f];
        float[] expected = [0f, 0f];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).Between(-0.1f, 0.1f));

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains(
            "Expected embedding to have cosine similarity with expected between -0.1 and 0.1 inclusive",
            disposeEx.Message);
        Assert.Contains("actual and expected vectors both have zero magnitude", disposeEx.Message);
    }

    [Fact]
    public void NewVectorAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [0f, 1f];
        float[] zero = [0f, 0f];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
        {
            embedding.Should().HaveDotProductWith(expected, 1f, 0.001f);
            embedding.Should().HaveEuclideanDistanceTo(expected, 1f, 0.001f);
            embedding.Should().BeZeroVector();
            zero.Should().NotBeZeroVector();
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        var message = disposeEx.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Expected embedding to have dot product with expected equal to 1 within tolerance 0.001", message);
        Assert.Contains("dot product differed: expected 1, found 0, delta 1", message);
        Assert.Contains("Expected embedding to have Euclidean distance to expected equal to 1 within tolerance 0.001", message);
        Assert.Contains("Euclidean distance differed: expected 1, found 1.4142135", message);
        Assert.Contains("Expected embedding to be a zero vector", message);
        Assert.Contains("index 0 differed: expected 0, found 1", message);
        Assert.Contains("Expected zero to not be a zero vector", message);
        Assert.Contains("all 2 component(s) were zero", message);
    }

    [Fact]
    public void VectorBatchFailures_Appear_InAssertionCallOrder()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [0f, 1f];
        float[] zero = [0f, 0f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("vectors");
            embedding.Should().HaveDimension(3);
            embedding.Should().HaveDotProductWith(expected, 1f, 0.001f);
            embedding.Should().BeZeroVector();
            zero.Should().NotBeZeroVector();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        var dimensionIndex = message.IndexOf("Expected embedding to have dimension 3", StringComparison.Ordinal);
        var dotProductIndex = message.IndexOf(
            "Expected embedding to have dot product with expected equal to 1 within tolerance 0.001",
            StringComparison.Ordinal);
        var zeroVectorIndex = message.IndexOf("Expected embedding to be a zero vector", StringComparison.Ordinal);
        var notZeroVectorIndex = message.IndexOf("Expected zero to not be a zero vector", StringComparison.Ordinal);

        Assert.True(dimensionIndex >= 0, message);
        Assert.True(dotProductIndex >= 0, message);
        Assert.True(zeroVectorIndex >= 0, message);
        Assert.True(notZeroVectorIndex >= 0, message);

        Assert.True(dimensionIndex < dotProductIndex, message);
        Assert.True(dotProductIndex < zeroVectorIndex, message);
        Assert.True(zeroVectorIndex < notZeroVectorIndex, message);
    }
}
