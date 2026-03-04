using Axiom.Assertions.EntryPoints;
using BenchmarkDotNet.Attributes;

namespace Axiom.Benchmarks;

[MemoryDiagnoser]
public class EquivalencyAssertionBenchmarks
{
    private readonly Person _actual = new() { Name = "Alice", Age = 42 };
    private readonly Person _expected = new() { Name = "Alice", Age = 42 };

    [Benchmark(Baseline = true)]
    public void BeEquivalentTo_Pass_NoPathComparer()
    {
        _actual.Should().BeEquivalentTo(_expected);
    }

    [Benchmark]
    public void BeEquivalentTo_Pass_WithPathComparer()
    {
        _actual.Should().BeEquivalentTo(
            _expected,
            options =>
            {
                options.StringComparison = StringComparison.Ordinal;
                options.UseComparerForPath("actual.Name", StringComparer.Ordinal);
            });
    }

    private sealed class Person
    {
        public string? Name { get; init; }
        public int Age { get; init; }
    }
}
