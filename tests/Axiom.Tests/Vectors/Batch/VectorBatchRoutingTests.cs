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
    public void CosineSimilarityLowerBoundAssertion_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        float[] embedding = [1f, 0f];
        float[] expected = [0f, 1f];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
            embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.5f));

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected embedding to have cosine similarity with expected at least 0.5", disposeEx.Message);
        Assert.Contains("computed cosine similarity 0", disposeEx.Message);
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
    public void AdditionalVectorAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        float[] embedding = [2f, 0f];
        float[] expected = [0f, 1f];
        float[] zero = [0f, 0f];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
        {
            embedding.Should().HaveDotProductWith(expected, 1f, 0.001f);
            embedding.Should().HaveEuclideanDistanceTo(expected, 1f, 0.001f);
            embedding.Should().BeNormalized(0.001f);
            embedding.Should().BeZeroVector();
            zero.Should().NotBeZeroVector();
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        var message = disposeEx.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Expected embedding to have dot product with expected equal to 1 within tolerance 0.001", message);
        Assert.Contains("dot product differed: expected 1, found 0, delta 1", message);
        Assert.Contains("Expected embedding to have Euclidean distance to expected equal to 1 within tolerance 0.001", message);
        Assert.Contains("Euclidean distance differed: expected 1, found", message);
        Assert.Contains("Expected embedding to be normalized within tolerance 0.001", message);
        Assert.Contains("computed L2 norm 2", message);
        Assert.Contains("Expected embedding to be a zero vector", message);
        Assert.Contains("index 0 differed: expected 0, found 2", message);
        Assert.Contains("Expected zero to not be a zero vector", message);
        Assert.Contains("all 2 component(s) were zero", message);
    }

    [Fact]
    public void VectorBatchFailures_Appear_InAssertionCallOrder()
    {
        float[] embedding = [2f, 0f];
        float[] expected = [0f, 1f];
        float[] zero = [0f, 0f];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("vectors");
            embedding.Should().HaveDimension(3);
            embedding.Should().BeNormalized(0.001f);
            embedding.Should().HaveDotProductWith(expected, 1f, 0.001f);
            embedding.Should().BeZeroVector();
            zero.Should().NotBeZeroVector();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        var dimensionIndex = message.IndexOf("Expected embedding to have dimension 3", StringComparison.Ordinal);
        var normalizedIndex = message.IndexOf("Expected embedding to be normalized within tolerance 0.001", StringComparison.Ordinal);
        var dotProductIndex = message.IndexOf(
            "Expected embedding to have dot product with expected equal to 1 within tolerance 0.001",
            StringComparison.Ordinal);
        var zeroVectorIndex = message.IndexOf("Expected embedding to be a zero vector", StringComparison.Ordinal);
        var notZeroVectorIndex = message.IndexOf("Expected zero to not be a zero vector", StringComparison.Ordinal);

        Assert.True(dimensionIndex >= 0, message);
        Assert.True(normalizedIndex >= 0, message);
        Assert.True(dotProductIndex >= 0, message);
        Assert.True(zeroVectorIndex >= 0, message);
        Assert.True(notZeroVectorIndex >= 0, message);

        Assert.True(dimensionIndex < normalizedIndex, message);
        Assert.True(normalizedIndex < dotProductIndex, message);
        Assert.True(dotProductIndex < zeroVectorIndex, message);
        Assert.True(zeroVectorIndex < notZeroVectorIndex, message);
    }

    [Fact]
    public void RankingAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        var results = new[] { "doc-1", "doc-2", "doc-7" };
        var relevantItems = new[] { "doc-2", "doc-5" };
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2", "doc-7"], ["doc-2"]),
            new RankingQuery<string>(["doc-8", "doc-3"], ["doc-5"]),
        };

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
        {
            results.Should().ContainInTopK("doc-7", 2);
            results.Should().HaveRank("doc-7", 2);
            results.Should().HaveRecallAt(2, relevantItems, expectedRecall: 1d);
            results.Should().HavePrecisionAt(2, relevantItems, expectedPrecision: 1d);
            results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 1d);
            queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 1d);
            queries.Should().HaveHitRateAt(k: 1, expectedHitRate: 1d);
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        var message = disposeEx.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Expected results to contain item \"doc-7\" in the top 2 result(s)", message);
        Assert.Contains("item \"doc-7\" was found at rank 3", message);
        Assert.Contains("Expected results to have item \"doc-7\" at rank 2", message);
        Assert.Contains("Expected results to have recall@2 equal to 1", message);
        Assert.Contains("actual recall@2 was 0.5", message);
        Assert.Contains("Expected results to have precision@2 equal to 1", message);
        Assert.Contains("actual precision@2 was 0.5", message);
        Assert.Contains("Expected results to have reciprocal rank for item \"doc-7\" equal to 1", message);
        Assert.Contains("actual reciprocal rank for item \"doc-7\" was 0.333333333333333", message);
        Assert.Contains("Expected queries to have mean reciprocal rank equal to 1", message);
        Assert.Contains("actual mean reciprocal rank was 0.5 across 2 queries", message);
        Assert.Contains("Expected queries to have hit rate@1 equal to 1", message);
        Assert.Contains("actual hit rate@1 was 0.5", message);
    }

    [Fact]
    public void RankingBatchFailures_Appear_InAssertionCallOrder()
    {
        var results = new[] { "doc-1", "doc-2", "doc-7" };
        var relevantItems = new[] { "doc-2", "doc-5" };
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2", "doc-7"], ["doc-2"]),
            new RankingQuery<string>(["doc-8", "doc-3"], ["doc-5"]),
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("ranking");
            results.Should().ContainInTopK("doc-7", 2);
            results.Should().HaveRank("doc-7", 2);
            results.Should().HavePrecisionAt(2, relevantItems, expectedPrecision: 1d);
            results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 1d);
            queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 1d);
            queries.Should().HaveHitRateAt(k: 1, expectedHitRate: 1d);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        var topKIndex = message.IndexOf("Expected results to contain item \"doc-7\" in the top 2 result(s)", StringComparison.Ordinal);
        var rankIndex = message.IndexOf("Expected results to have item \"doc-7\" at rank 2", StringComparison.Ordinal);
        var precisionIndex = message.IndexOf("Expected results to have precision@2 equal to 1", StringComparison.Ordinal);
        var reciprocalRankIndex = message.IndexOf(
            "Expected results to have reciprocal rank for item \"doc-7\" equal to 1",
            StringComparison.Ordinal);
        var mrrIndex = message.IndexOf("Expected queries to have mean reciprocal rank equal to 1", StringComparison.Ordinal);
        var hitRateIndex = message.IndexOf("Expected queries to have hit rate@1 equal to 1", StringComparison.Ordinal);

        Assert.True(topKIndex >= 0, message);
        Assert.True(rankIndex >= 0, message);
        Assert.True(precisionIndex >= 0, message);
        Assert.True(reciprocalRankIndex >= 0, message);
        Assert.True(mrrIndex >= 0, message);
        Assert.True(hitRateIndex >= 0, message);

        Assert.True(topKIndex < rankIndex, message);
        Assert.True(rankIndex < precisionIndex, message);
        Assert.True(precisionIndex < reciprocalRankIndex, message);
        Assert.True(reciprocalRankIndex < mrrIndex, message);
        Assert.True(mrrIndex < hitRateIndex, message);
    }
}
