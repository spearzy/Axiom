using Axiom.Benchmarks;
using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(StringAssertionBenchmarks).Assembly).Run(args);
