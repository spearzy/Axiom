using Axiom.Vectors;

namespace Axiom.Tests.Vectors.FluentWorkflows;

public sealed class VectorFluentWorkflowTests
{
    [Fact]
    public void VectorAssertions_Can_Be_Composed_In_A_RealisticEmbeddingWorkflow()
    {
        float[] embedding = [1f, 0f, 0f];
        float[] expected = [1f, 0f, 0f];
        float[] unrelated = [0f, 1f, 0f];

        var continuation = embedding.Should()
            .HaveDimension(3)
            .And.NotContainNaNOrInfinity()
            .And.BeApproximatelyEqualTo(expected, 0.001f)
            .And.HaveDotProductWith(expected, 1f, 0.001f)
            .And.HaveEuclideanDistanceTo(unrelated, 1.4142135f, 0.001f)
            .And.NotBeZeroVector()
            .And.HaveCosineSimilarityWith(expected).AtLeast(0.999f)
            .And.BeNormalized(0.001f);

        Assert.IsType<VectorAssertions<float>>(continuation.And);
    }
}
