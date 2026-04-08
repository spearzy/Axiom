# Benchmarks

Axiom tracks a small set of representative benchmarks. Simple assertions are expected to stay cheap, while more capable features cost more only when you use them. These snapshots exist to catch regressions and show the shape of the library.

Snapshot last refreshed: 2026-04-08.

## Representative Benchmarks

| Scenario | Mean | Allocated | What it shows |
| --- | ---: | ---: | --- |
| Simple `Be(...)` pass | `7.23 ns` | `32 B` | The baseline pass path stays cheap. |
| Collection `Contain(...)` pass | `7.20 ns` | `0 B` | Common collection membership checks stay cheap on small inputs. |
| `BeEquivalentTo(...)` pass | `5.3 μs` | `20.0 KB` | Structural comparison does more work, but only when you use it. |
| Async `ContainAllAsync(...)` pass | `133.7 ns` | `344 B` | Async stream assertions pay to enumerate the stream, not before. |
| Vector `BeNormalized(...)` pass | `5.92 ns` | `48 B` | Vector-specific checks stay focused on the metric they compute. |
| Ranking `HaveRecallAt(...)` pass | `197.6 ns` | `600 B` | Retrieval metrics add top-k and set work only when you ask for them. |

## Methodology

These numbers come from the repo's BenchmarkDotNet suite using the shared `ShortRun` configuration and a Release build. The published values are rounded so tiny machine noise does not churn the docs. Use them to spot regressions and understand Axiom's pay-to-play shape, not to predict full test-suite runtime or compare different machines.
