using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using Axiom.Benchmarks.Infrastructure;
using Axiom.Benchmarks.Models;
using Axiom.Core;
using BenchmarkDotNet.Attributes;

namespace Axiom.Benchmarks.Scenarios;

public class BatchAggregationBenchmarks : AssertionBenchmarkBase
{
    private BatchUserSnapshot _user = null!;

    public override void GlobalSetup()
    {
        base.GlobalSetup();
        _user = BenchmarkDataFactory.CreateBatchUser();
    }

    [Benchmark(Description = "Batch aggregation (2 failures)")]
    public void AggregateSmallBatch()
    {
        AssertionFailureConsumer.ConsumeExpectedFailure(() =>
        {
            using var batch = Assert.Batch("profile");
            _user.Name.Should().Be("Ada");
            _user.Email.Should().Contain("@");
            _user.CountryCode.Should().Be("GB");
            _user.Age.Should().BeGreaterThan(40);
            _user.IsActive.Should().BeFalse();
        });
    }
}
