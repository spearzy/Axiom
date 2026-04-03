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
        Assert.Contains("computed dot product 0", message);
        Assert.Contains("Expected embedding to have Euclidean distance to expected equal to 1 within tolerance 0.001", message);
        Assert.Contains("computed Euclidean distance 1.4142135", message);
        Assert.Contains("Expected embedding to be a zero vector", message);
        Assert.Contains("Expected zero to not be a zero vector", message);
    }
}
