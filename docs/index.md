---
title: Axiom Assertions for .NET
description: Deterministic fluent assertions for .NET tests, with explicit batching, analyzers, equivalency, and optional JSON, HTTP/API, vector, and retrieval assertions.
---

# Axiom Assertions for .NET

<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "WebSite",
  "name": "Axiom Assertions",
  "alternateName": "Axiom",
  "url": "https://spearzy.github.io/Axiom/",
  "description": "Deterministic fluent assertions for .NET tests, with explicit batching, analyzers, equivalency, and optional JSON, HTTP/API, vector, and retrieval assertions."
}
</script>

Axiom Assertions is a deterministic fluent assertion library for .NET tests. It focuses on stable failure output, explicit grouped assertions with `Batch`, configurable equivalency, and a package layout that stays clear about what is core and what is optional.

Axiom is still early in adoption, so these docs focus on what is implemented today and the test scenarios it currently covers well.

## What Axiom Focuses On

- deterministic failure messages that stay stable in CI and code review
- explicit multi-assertion aggregation with `Batch`
- built-in equivalency support with configurable defaults
- analyzers and code fixes shipped with the normal `Axiom.Assertions` install path
- optional JSON assertions without bloating the main package
- optional HTTP and API-response assertions without pushing that surface into every test project
- optional vector and retrieval assertions without forcing AI-specific APIs into the base package

## Install

Most test projects should start with `Axiom.Assertions`:

```bash
dotnet add package Axiom.Assertions
```

Optional add-ons:

```bash
dotnet add package Axiom.Json
dotnet add package Axiom.Http
dotnet add package Axiom.Vectors
```

Advanced or special-case installs:

```bash
dotnet add package Axiom.Core
dotnet add package Axiom.Analyzers
```

## Package Lineup

### Axiom.Assertions

The default package for most test projects.

- fluent `Should()` assertions
- built-in equivalency, async, collection, string, and exception assertions
- bundled Axiom analyzers and code fixes

### Axiom.Core

Install this directly when you want low-level primitives such as `Batch`, formatting, or configuration without the full fluent assertion surface.

### Axiom.Analyzers

Install this separately when you want the diagnostics without the runtime assertion library.

### Axiom.Json

Install this when you want structural JSON equivalency and simple JSON path assertions on top of the main Axiom assertion library.

### Axiom.Http

Install this when you want `HttpResponseMessage` assertions for exact status codes, headers, content types, JSON bodies, and ProblemDetails-style API responses.

### Axiom.Vectors

Install this when you want vector, embedding, and retrieval-focused assertions for AI and ranking tests in .NET.

## Where To Start

- New to the library? Start with [Getting Started](getting-started.md)
- Need current package, framework, or prerequisite support details? Read the [Compatibility Matrix](compatibility-matrix.md)
- Need a minimal test-project example? Use the repository starter projects for [xUnit](https://github.com/spearzy/Axiom/tree/main/starters/Axiom.Assertions.Starter.Xunit), [NUnit](https://github.com/spearzy/Axiom/tree/main/starters/Axiom.Assertions.Starter.NUnit), or [MSTest](https://github.com/spearzy/Axiom/tree/main/starters/Axiom.Assertions.Starter.MSTest)
- For the API catalog: go to [Assertion Reference](assertion-reference.md)
- Migrating a test suite: read [Migrating to Axiom](migrating-to-axiom.md) or the focused [Migrate from xUnit Assert to Axiom](migrate-from-xunit-assert.md) guide
- Evaluating trade-offs: read [Axiom vs FluentAssertions](axiom-vs-fluentassertions.md), [Axiom vs Shouldly](axiom-vs-shouldly.md), or the broader [.NET assertion library](dotnet-assertion-library.md) page
- Working with structural comparison: go to [Equivalency](equivalency.md)
- Working with JSON payloads or documents: go to [JSON](json.md)
- Testing HTTP or API responses: go to [HTTP and API assertions](http.md)
- Testing embeddings or ranked retrieval: go to [Vectors](vectors.md) or the focused [Vector assertions for AI and retrieval tests in .NET](vector-assertions-for-ai-and-retrieval-tests-in-dotnet.md)
- Using diagnostics only: go to [Analyzers](analyzers.md)

## A Quick Example

```csharp
using Axiom.Assertions;
using Axiom.Core;

user.Name.Should().NotBeNull();
user.Email.Should().Contain("@");

using var batch = Assert.Batch("profile");
user.Name.Should().StartWith("A");
user.Roles.Should().Contain("admin");
```
