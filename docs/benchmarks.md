# Performance and Benchmarks

This page exists for transparency and regression tracking.

Axiom publishes a small BenchmarkDotNet harness so readers and maintainers can see the cost of a few representative assertion scenarios, rerun them locally, and notice when those costs move over time. The benchmark set is intentionally small: the goal is to show the shape of Axiom's own work, not to build a synthetic leaderboard.

## What This Page Is For

Use this page for:

- understanding representative per-scenario costs inside Axiom
- tracking performance regressions over time
- making implementation tradeoffs more visible

Do not use this page for:

- predicting whole test-suite runtime
- ranking Axiom against other assertion libraries
- overinterpreting tiny deltas from one machine or one benchmark run

## Benchmark Project

The harness lives in `benchmarks/Axiom.Benchmarks`.

Current scenarios:

- `Be pass (int)`: successful scalar `Be(...)`
- `BeEquivalentTo pass (medium graph)`: successful structural comparison on a nested object graph
- `BeEquivalentTo fail (nested mismatch)`: structural comparison that fails on a nested member and generates failure output
- `Batch aggregation (2 failures)`: a small explicit batch that mixes passing and failing assertions and throws one aggregated failure

The benchmark code is split into:

- `Infrastructure/`: BenchmarkDotNet config, shared setup, and failure-consumption helpers
- `Models/`: stable benchmark data models
- `Scenarios/`: one focused class per scenario group

That keeps the first wave small, repeatable, and easy to grow without turning the suite into maintenance overhead.

## Methodology

These results were collected with:

- BenchmarkDotNet `0.14.0`
- `.NET 10.0.3`
- `ShortRun`
- memory diagnostics enabled

The benchmark harness configures Axiom to use `InvalidOperationFailureStrategy` explicitly so the benchmark process does not vary by detected test framework.

Failure-path benchmarks catch the expected assertion exception inside the benchmark body so the measured work is the assertion path itself, not a broken harness.

## Current Results

Recorded on March 28, 2026.

Each row below represents a different kind of work:

- `Be(...)` pass: the baseline cost of a successful basic assertion
- `BeEquivalentTo(...)` pass: structural comparison on a representative nested graph
- `BeEquivalentTo(...)` fail: structural comparison plus deterministic diagnostics and exception creation
- `Batch` aggregation: explicit aggregation work across several assertions

That means the rows are not interchangeable "speed scores." They represent different behaviors and different amounts of work.

| Scenario | Mean | Allocated | What it represents |
| --- | ---: | ---: | --- |
| `Be pass (int)` | `7.689 ns` | `32 B` | Baseline cost of a successful basic assertion |
| `BeEquivalentTo pass (medium graph)` | `10.56 us` | `42.58 KB` | Structural comparison on a representative nested graph |
| `BeEquivalentTo fail (nested mismatch)` | `16.61 us` | `60.37 KB` | Structural comparison plus deterministic failure generation |
| `Batch aggregation (2 failures)` | `3.725 us` | `2.57 KB` | Explicit aggregation of multiple assertions into one failure |

## Interpreting The Numbers

A few guardrails matter:

- These are per-scenario microbenchmarks, not end-to-end test-suite timings.
- They are useful for spotting Axiom regressions and understanding where Axiom spends work.
- They were collected on one machine, one runtime, and one BenchmarkDotNet job shape.
- The failure-path numbers include exception creation and deterministic message generation, which is intentional.

More specifically:

- `Be(...)` pass is the closest thing here to a pass-path baseline.
- `BeEquivalentTo(...)` failure is intentionally measuring richer work, including exception construction and deterministic failure rendering.
- `Batch` aggregation has its own measurable overhead because it is doing extra coordination and reporting work on purpose.

The useful signal is not that one row is universally "better" than another. The useful signal is that these measurements make Axiom's own tradeoffs visible and give us a stable baseline for future regression checks.

## Running Locally

Build the benchmark project:

```bash
dotnet build benchmarks/Axiom.Benchmarks/Axiom.Benchmarks.csproj -c Release
```

Run the full benchmark set:

```bash
dotnet run -c Release --project benchmarks/Axiom.Benchmarks/Axiom.Benchmarks.csproj -- --filter '*'
```

BenchmarkDotNet writes reports to `BenchmarkDotNet.Artifacts/results/` from the repo root when you run that command there.

## Caveats

- The published numbers above come from `ShortRun`, which is a pragmatic regression-tracking job, not a lab-grade long-run benchmark configuration.
- macOS could not elevate the benchmark process priority in this run, so the results should be read as practical local numbers rather than tightly controlled lab measurements.
- Future benchmark waves may add more scenarios, but this page will stay intentionally small and representative.
