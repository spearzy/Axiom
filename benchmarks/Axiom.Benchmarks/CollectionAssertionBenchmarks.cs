using Axiom.Assertions;
using BenchmarkDotNet.Attributes;

namespace Axiom.Benchmarks;

[MemoryDiagnoser]
public class CollectionAssertionBenchmarks
{
    private readonly int[] _values = Enumerable.Range(0, 128).ToArray();

    [Benchmark(Baseline = true)]
    public void Contain_Pass_OutsideBatch()
    {
        _values.Should().Contain(127);
    }

    [Benchmark]
    public void Contain_Fail_OutsideBatch()
    {
        try
        {
            _values.Should().Contain(256);
        }
        catch (InvalidOperationException)
        {
            // Keep the benchmark alive so we can compare failure-path cost.
        }
    }

    [Benchmark]
    public void HaveCount_Pass_OutsideBatch()
    {
        _values.Should().HaveCount(128);
    }

    [Benchmark]
    public void HaveCount_Fail_OutsideBatch()
    {
        try
        {
            _values.Should().HaveCount(64);
        }
        catch (InvalidOperationException)
        {
            // Keep the benchmark alive so we can compare failure-path cost.
        }
    }

    [Benchmark]
    public void ContainAndHaveCount_Pass_OutsideBatch()
    {
        _values.Should().Contain(64).And.HaveCount(128);
    }
}
