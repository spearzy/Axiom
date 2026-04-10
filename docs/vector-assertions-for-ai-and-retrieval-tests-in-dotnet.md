---
title: Vector Assertions for AI and Retrieval Tests in .NET
description: Use Axiom Assertions and Axiom.Vectors to test embeddings, similarity, normalization, and ranked retrieval quality in .NET.
---

# Vector assertions for AI and retrieval tests in .NET

Most .NET assertion libraries stop at ordinary scalar, collection, and equivalency checks. Axiom also includes a focused vector and retrieval-testing layer for AI-oriented test suites.

That is a focused capability, not a claim that every .NET test suite needs vector-aware assertions. It is most useful when those checks already belong beside the rest of your application tests.

## What Axiom Covers Here

With `Axiom.Vectors`, you can assert:

- vector shape and dimension
- NaN and infinity validation
- approximate equality
- dot products, distances, and cosine similarity
- normalization and zero-vector checks
- ranked retrieval quality with top-k, rank, recall, precision, reciprocal rank, mean reciprocal rank, and hit rate assertions

```csharp
using Axiom.Assertions;
using Axiom.Vectors;

embedding.Should().HaveDimension(1536);
embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.995f);
results.Should().ContainInTopK("doc-7", 2);
queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 0.75);
```

## When This Is Useful

This is useful when you want the AI and retrieval assertions to live beside the rest of your .NET tests instead of building a separate assertion layer for those checks.

That is especially useful when one test suite includes both:

- ordinary application behavior
- embedding or retrieval behavior

## Where A Different Setup May Be Better

A different setup may be better when:

- you only need generic numeric assertions and do not need vector or ranking-aware checks
- your retrieval evaluation already lives in a dedicated benchmarking or experimentation stack outside your normal .NET test suite
- you do not want any AI-specific test surface in your assertion toolbox

## Where To Go Next

- Read the full [Vectors](vectors.md) guide
- Browse the [Assertion Reference](assertion-reference.md)
- Read [.NET assertion library](dotnet-assertion-library.md) if you are still choosing a base library for the rest of your tests
