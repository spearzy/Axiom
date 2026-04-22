# Axiom Design Notes

This document captures the design direction for Axiom.

## Core Goals

- Pay-for-play assertions with low pass-path overhead.
- Deterministic failure output suitable for strict tests.
- Batch-based failure aggregation (`Batch`) for grouped assertions.
- First-class extensibility for custom assertions and formatting/comparison behaviour.

## Architecture

- `Axiom.Core`: batch, failure model, formatting pipeline, entrypoints.
- `Axiom.Assertions`: built-in assertion sets and chaining APIs.
- `Axiom.Analyzers`: Roslyn analyzers and code fixes for common Axiom usage mistakes.
- `Axiom.Json`: optional JSON-focused assertions layered on top of Axiom's core assertion pipeline.
- `Axiom.Http`: optional `HttpResponseMessage` assertions layered on top of Axiom's core assertion pipeline and `Axiom.Json`.
- `Axiom.Vectors`: optional vector and embedding-focused assertions layered on top of Axiom's core assertion pipeline.

## Open Questions

- Final shape of formatter/comparer registration.
- API boundaries between `Axiom.Core` and `Axiom.Assertions`.
- Error message layout and stable sorting/formatting rules.
