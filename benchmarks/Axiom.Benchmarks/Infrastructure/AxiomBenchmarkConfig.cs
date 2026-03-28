using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace Axiom.Benchmarks.Infrastructure;

internal sealed class AxiomBenchmarkConfig : ManualConfig
{
    public AxiomBenchmarkConfig()
    {
        AddJob(Job.ShortRun.WithId("ShortRun"));
        AddDiagnoser(MemoryDiagnoser.Default);
        Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest);
    }
}
