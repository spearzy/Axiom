using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using Axiom.Benchmarks.Infrastructure;
using Axiom.Vectors;
using BenchmarkDotNet.Attributes;

namespace Axiom.Benchmarks.Scenarios;

public class ScalarAssertionBenchmarks : AssertionBenchmarkBase
{
    private readonly int _expected = 42;
    private readonly int _actual = 42;
    private readonly int[] _collectionValues = [1, 2, 3, 4, 5];
    private readonly int[] _asyncExpected = [1, 4];
    private readonly float[] _normalizedEmbedding = [0.6f, 0.8f, 0f];
    private readonly string[] _rankingResults = ["doc-3", "doc-7", "doc-1", "doc-9"];
    private readonly string[] _relevantItems = ["doc-7", "doc-2"];

    [Benchmark(Description = "Be pass (int)")]
    public void SimpleBePass()
    {
        _actual.Should().Be(_expected);
    }

    [Benchmark(Description = "Contain pass (small int array)")]
    public void CollectionContainPass()
    {
        _collectionValues.Should().Contain(4);
    }

    [Benchmark(Description = "ContainAllAsync pass (small stream)")]
    public async Task AsyncContainAllPass()
    {
        await CreateAsyncSequence(_collectionValues).Should().ContainAllAsync(_asyncExpected);
    }

    [Benchmark(Description = "BeNormalized pass (float[3])")]
    public void VectorBeNormalizedPass()
    {
        _normalizedEmbedding.Should().BeNormalized(0.001f);
    }

    [Benchmark(Description = "HaveRecallAt pass (top-3)")]
    public void RankingRecallAtPass()
    {
        _rankingResults.Should().HaveRecallAt(3, _relevantItems, expectedRecall: 0.5d);
    }

    private static async IAsyncEnumerable<int> CreateAsyncSequence(IEnumerable<int> values)
    {
        foreach (var value in values)
        {
            yield return value;
            await Task.CompletedTask;
        }
    }
}
