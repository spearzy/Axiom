# Axiom Assertions for .NET

![Axiom](assets/images/axiom-logo.png)

[![CI](https://github.com/spearzy/Axiom/actions/workflows/ci.yml/badge.svg)](https://github.com/spearzy/Axiom/actions/workflows/ci.yml)
[![License: Apache-2.0](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

`Axiom.Assertions`  
[![Version](https://img.shields.io/nuget/v/Axiom.Assertions?label=version)](https://www.nuget.org/packages/Axiom.Assertions)
[![Downloads](https://img.shields.io/nuget/dt/Axiom.Assertions?label=downloads)](https://www.nuget.org/packages/Axiom.Assertions)

Axiom Assertions is a fluent assertion library for .NET tests. It focuses on deterministic failure output, explicit batch aggregation, configurable equivalency, analyzer-backed migration help, and optional vector and retrieval assertions.

Target frameworks: `net8.0`, `net9.0`, and `net10.0`.

Docs site: [spearzy.github.io/Axiom](https://spearzy.github.io/Axiom/)  

## Install

Most test projects should install `Axiom.Assertions`:

```bash
dotnet add package Axiom.Assertions
```

`Axiom.Assertions` gives you the main `Should()` API, pulls in `Axiom.Core`, and bundles the Axiom analyzers/code fixes automatically.

Install the other packages when you need something more specific:

```bash
dotnet add package Axiom.Core
dotnet add package Axiom.Analyzers
dotnet add package Axiom.Vectors
```

## Examples

Basic assertions and batch aggregation:

```csharp
using Axiom.Assertions;
using Axiom.Core;

user.Name.Should().NotBeNull();
user.Email.Should().Contain("@");

using var batch = Assert.Batch("profile");
user.Name.Should().StartWith("A");
user.Roles.Should().Contain("admin");
```

Structural equivalency:

```csharp
using Axiom.Assertions.Equivalency;

actual.Should().BeEquivalentTo(expected, options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
    options.IgnorePath("actual.UpdatedAt");
});
```

Vectors and retrieval/ranking:

```csharp
using Axiom.Assertions;
using Axiom.Vectors;

embedding.Should().HaveDotProductWith(expected, expectedDotProduct: 1f, tolerance: 0.001f);
results.Should().ContainInTopK("doc-7", 2);
queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 0.75);
```

## Packages

| Package | Use it when you want... |
| --- | --- |
| [`Axiom.Assertions`](https://www.nuget.org/packages/Axiom.Assertions) | the main fluent assertion library for most test projects |
| [`Axiom.Core`](https://www.nuget.org/packages/Axiom.Core) | low-level primitives such as `Batch`, formatting, and configuration without the full assertion surface |
| [`Axiom.Analyzers`](https://www.nuget.org/packages/Axiom.Analyzers) | the analyzers and code fixes without the runtime assertion library |
| [`Axiom.Vectors`](https://www.nuget.org/packages/Axiom.Vectors) | vector, embedding, and ranked retrieval assertions on top of the main Axiom assertion library |

## Why Axiom

- Deterministic messages that stay stable in CI and code review.
- Explicit multi-assertion aggregation with `Batch`.
- Strong equivalency support with configurable defaults.
- Analyzer support bundled into the normal `Axiom.Assertions` install path.
- Optional vector and retrieval assertions without complicating the core package.

## Docs

Use the docs site for the full guides and reference:

- [Getting Started](https://spearzy.github.io/Axiom/getting-started/)
- [Assertion Reference](https://spearzy.github.io/Axiom/assertion-reference/)
- [Migration Guide](https://spearzy.github.io/Axiom/migrating-to-axiom/)
- [Migrate from xUnit Assert to Axiom](https://spearzy.github.io/Axiom/migrate-from-xunit-assert/)
- [Axiom vs FluentAssertions](https://spearzy.github.io/Axiom/axiom-vs-fluentassertions/)
- [Axiom vs Shouldly](https://spearzy.github.io/Axiom/axiom-vs-shouldly/)
- [.NET assertion library](https://spearzy.github.io/Axiom/dotnet-assertion-library/)
- [Equivalency Guide](https://spearzy.github.io/Axiom/equivalency/)
- [Vectors Guide](https://spearzy.github.io/Axiom/vectors/)
- [Vector assertions for AI and retrieval tests in .NET](https://spearzy.github.io/Axiom/vector-assertions-for-ai-and-retrieval-tests-in-dotnet/)
- [Analyzers Guide](https://spearzy.github.io/Axiom/analyzers/)
- [Benchmarks](https://spearzy.github.io/Axiom/benchmarks/)

## Project Status

Axiom is versioned and released with semantic versioning and has a full docs site, test suite, analyzers, and package distribution. It is still early in adoption, so issues and small repros are especially useful when something feels unclear or missing.
