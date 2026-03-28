using Axiom.Core.Configuration;
using Axiom.Core.Failures;
using BenchmarkDotNet.Attributes;

namespace Axiom.Benchmarks.Infrastructure;

[Config(typeof(AxiomBenchmarkConfig))]
public abstract class AssertionBenchmarkBase
{
    [GlobalSetup]
    public virtual void GlobalSetup()
    {
        AxiomServices.Configure(configuration => configuration.FailureStrategy = InvalidOperationFailureStrategy.Instance);
    }

    [GlobalCleanup]
    public virtual void GlobalCleanup()
    {
        AxiomServices.Reset();
    }
}
