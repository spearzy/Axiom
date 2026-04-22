---
title: .NET Assertion Library
description: Understand where Axiom Assertions fits as a .NET assertion library, what it emphasizes, and when another testing style or library may be the better choice.
---

# .NET Assertion Library

If you are searching for a .NET assertion library, the real decision is usually about trade-offs rather than raw feature count.

Axiom Assertions is a newer option in that category. It is aimed at teams that want deterministic messages, explicit batching, analyzers, equivalency support, and optional JSON, HTTP/API, plus vector and retrieval testing support.

## What Category Axiom Fits

Axiom fits into the fluent assertion library category for .NET tests.

It is not trying to replace your test runner. It sits on top of xUnit, NUnit, or MSTest and gives you a more expressive assertion layer.

## What Axiom Emphasizes

- deterministic failure output
- explicit multi-assertion aggregation with `Batch`
- configurable structural equivalency
- analyzers and code fixes in the normal install path
- optional JSON assertions for structural JSON equivalency and simple path checks
- optional HTTP assertions for exact `HttpResponseMessage` status, header, content-type, JSON body, and ProblemDetails checks
- optional vector and retrieval assertions for AI-oriented tests

## When Axiom May Be Worth Evaluating

Axiom may be worth evaluating when your team wants:

- predictable messages in CI and code review
- a clear path from framework assertions to fluent assertions
- one library that can cover normal app tests, API-response checks, and AI or retrieval testing without mixing those APIs into every project

## When Another Library May Still Be The Safer Choice

Another assertion library may still be the safer choice when you want:

- the broadest and oldest fluent assertion ecosystem
- a more conversational assertion style
- no interest in batching, analyzers, HTTP/API assertions, or vector and retrieval testing

If your current assertion approach is already clear, stable, and well understood by the team, there may be no reason to switch.

Those trade-offs are easiest to see in the comparison pages:

- [Axiom vs FluentAssertions](axiom-vs-fluentassertions.md)
- [Axiom vs Shouldly](axiom-vs-shouldly.md)

## Where To Go Next

- Start with [Getting Started](getting-started.md)
- Browse the [Assertion Reference](assertion-reference.md)
- Read [Migrate from xUnit Assert to Axiom](migrate-from-xunit-assert.md) if you are moving an existing suite
- Read [HTTP and API assertions](http.md) if you need practical `HttpResponseMessage` checks
- Read [Vector assertions for AI and retrieval tests in .NET](vector-assertions-for-ai-and-retrieval-tests-in-dotnet.md) if you need embedding or ranking assertions
