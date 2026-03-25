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
}
