using Axiom.Assertions;
using Axiom.Benchmarks.Infrastructure;
using Axiom.Benchmarks.Models;
using BenchmarkDotNet.Attributes;

namespace Axiom.Benchmarks.Scenarios;

public class EquivalencyBenchmarks : AssertionBenchmarkBase
{
    private OrderSnapshot _expected = null!;
    private OrderSnapshot _matchingActual = null!;
    private OrderSnapshot _mismatchedActual = null!;

    public override void GlobalSetup()
    {
        base.GlobalSetup();
        _expected = BenchmarkDataFactory.CreateEquivalentOrder();
        _matchingActual = BenchmarkDataFactory.CreateEquivalentOrder();
        _mismatchedActual = BenchmarkDataFactory.CreateOrderWithNestedMismatch();
    }

    [Benchmark(Description = "BeEquivalentTo pass (medium graph)")]
    public void EquivalentGraphPass()
    {
        _matchingActual.Should().BeEquivalentTo(_expected);
    }

    [Benchmark(Description = "BeEquivalentTo fail (nested mismatch)")]
    public void EquivalentGraphFail()
    {
        AssertionFailureConsumer.ConsumeExpectedFailure(() => _mismatchedActual.Should().BeEquivalentTo(_expected));
    }
}
