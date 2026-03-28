using Axiom.Assertions;
using Axiom.Benchmarks.Infrastructure;
using BenchmarkDotNet.Attributes;

namespace Axiom.Benchmarks.Scenarios;

public class ScalarAssertionBenchmarks : AssertionBenchmarkBase
{
    private readonly int _expected = 42;
    private readonly int _actual = 42;

    [Benchmark(Description = "Be pass (int)")]
    public void SimpleBePass()
    {
        _actual.Should().Be(_expected);
    }
}
