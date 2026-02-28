using Axiom.Assertions;
using Axiom.Core;
using BenchmarkDotNet.Attributes;

namespace Axiom.Benchmarks;

[MemoryDiagnoser]
public class StringAssertionBenchmarks
{
    private const string Value = "test";

    [Benchmark(Baseline = true)]
    public void StartWith_Pass_OutsideBatch()
    {
        Value.Should().StartWith("te");
    }

    [Benchmark]
    public void StartWith_Fail_OutsideBatch()
    {
        try
        {
            Value.Should().StartWith("zz");
        }
        catch (InvalidOperationException)
        {
            // Benchmark should measure assertion failure cost, not fail the run.
        }
    }

    [Benchmark]
    public void StartWith_Fail_InsideBatch()
    {
        try
        {
            using var batch = Assert.Batch("bench");
            Value.Should().StartWith("zz");
        }
        catch (InvalidOperationException)
        {
            // Root batch dispose throws aggregated failure.
        }
    }
}
