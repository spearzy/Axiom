---
title: Getting Started with Axiom Assertions for .NET
description: Install Axiom Assertions for .NET, write your first assertions, and find the right guides for migration, equivalency, JSON, HTTP/API, analyzer, and vector testing.
---

# Getting Started

## Install Axiom.Assertions

Most test projects should install `Axiom.Assertions`.

```bash
dotnet add package Axiom.Assertions
```

That gives you:

- the fluent `Should()` API
- `Axiom.Core` transitively
- bundled Axiom analyzers/code fixes
- framework-native failure behavior by default for xUnit, NUnit, and MSTest

You do not need `AxiomSetup.cs` just to start using Axiom Assertions.

## Start Using It

```csharp
using Axiom.Assertions;
using Axiom.Core;

user.Name.Should().NotBeNull();
user.Email.Should().Contain("@");
user.Age.Should().BeGreaterThan(18);

using var batch = Assert.Batch("profile");
user.Name.Should().StartWith("A");
user.Roles.Should().Contain("admin");
```

That default install path also bundles the Axiom analyzers/code fixes automatically, so common diagnostics light up without a second package.

Starter consumer projects are available in the repository if you want a minimal xUnit, NUnit, or MSTest example wired to the normal `Axiom.Assertions` install path:

- [xUnit starter](https://github.com/spearzy/Axiom/tree/main/starters/Axiom.Assertions.Starter.Xunit)
- [NUnit starter](https://github.com/spearzy/Axiom/tree/main/starters/Axiom.Assertions.Starter.NUnit)
- [MSTest starter](https://github.com/spearzy/Axiom/tree/main/starters/Axiom.Assertions.Starter.MSTest)

## Optional Packages

These are add-ons on top of `Axiom.Assertions`.

Install `Axiom.Json` when you want structural JSON equivalency and simple JSON path assertions:

```bash
dotnet add package Axiom.Json
```

Install `Axiom.Http` when you want deterministic `HttpResponseMessage` assertions for exact status codes, headers, content types, JSON bodies, and ProblemDetails-style API responses:

```bash
dotnet add package Axiom.Http
```

Install `Axiom.Vectors` only when you want vector and embedding-focused assertions:

```bash
dotnet add package Axiom.Vectors
```

Install `Axiom.Analyzers` separately only if you want the diagnostics without the runtime assertion library:

```bash
dotnet add package Axiom.Analyzers
```

Install `Axiom.Core` directly only when you want low-level primitives such as `Batch`, formatting, or configuration without the full fluent assertion surface.

## Optional Configuration

For most teams, there is no required setup step. Install `Axiom.Assertions`, write assertions, and Axiom will use native xUnit, NUnit, or MSTest assertion exceptions automatically when it detects those frameworks.

Add shared setup only when you want custom defaults across a test project:

```csharp
using System;
using Axiom.Assertions.Configuration;
public static class AxiomSetup
{
    public static void Apply()
    {
        AxiomSettings.Configure(options =>
        {
            options.Core.RegexMatchTimeout = TimeSpan.FromMilliseconds(500);

            options.Equivalency.RequireStrictRuntimeTypes = false;
            options.Equivalency.FailOnMissingMembers = false;
            options.Equivalency.FailOnExtraMembers = false;
        });
    }
}
```

Call `AxiomSetup.Apply()` once from your test framework startup when you want those shared defaults:

- xUnit: fixture constructor
- NUnit: `[SetUpFixture]` + `[OneTimeSetUp]`
- MSTest: `[AssemblyInitialize]`

You only need setup/configuration when you want custom defaults such as:

- equivalency defaults
- custom comparer provider
- custom value formatter
- custom regex timeout
- explicit failure-strategy override
- shared reusable modules

For shared defaults, prefer `AxiomSettings.Configure(...)`. `AxiomServices.Configure(...)` and `EquivalencyDefaults.Configure(...)` remain available for lower-level or isolated configuration.

## Async And Equivalency Examples

```csharp
Task<string> rollout = Task.FromResult("pricing-api");
var continuation = await rollout.Should().SucceedWithin(TimeSpan.FromMilliseconds(50));
continuation.WhoseResult.Should().Be("pricing-api");

actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
});
```

## Vector Example

```csharp
using Axiom.Vectors;

embedding.Should().HaveDimension(1536);
embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.995f);
```

## JSON Example

```csharp
using Axiom.Json;

var actualJson = """{ "id": 1, "name": "Ada", "roles": ["admin", "author"] }""";
var expectedJson = """{ "roles": ["admin", "author"], "name": "Ada", "id": 1.0 }""";

actualJson.Should().BeJsonEquivalentTo(expectedJson);
actualJson.Should().HaveJsonStringAtPath("$.name", "Ada");
actualJson.Should().HaveJsonPath("$.roles[1]");
```

## HTTP Example

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
{
    Content = new StringContent(
        """{ "title": "Validation failed", "status": 400 }""",
        Encoding.UTF8,
        "application/problem+json")
};

response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
response.Should().HaveProblemDetailsTitle("Validation failed");
```

## Next Steps

- Read the [Compatibility Matrix](compatibility-matrix.md) if you want the current package, target framework, and test framework support in one place
- Browse the full [Assertion Reference](assertion-reference.md)
- Read [Migrate from xUnit Assert to Axiom](migrate-from-xunit-assert.md) if you are coming from classic xUnit assertions
- Read [Migrating to Axiom](migrating-to-axiom.md) for the broader migration guide
- Compare [Axiom vs FluentAssertions](axiom-vs-fluentassertions.md) or [Axiom vs Shouldly](axiom-vs-shouldly.md) if you are choosing a library
- Read [.NET assertion library](dotnet-assertion-library.md) for the broader category overview
- Read the [Equivalency](equivalency.md) guide for object-graph configuration
- Read [Custom Assertions](custom-assertions.md) when you want domain-specific extensions
- Read [JSON](json.md) when you want JSON equivalency or path assertions
- Read [HTTP and API assertions](http.md) when you want `HttpResponseMessage` checks
- Read [Analyzers](analyzers.md) for the shipped diagnostics
- Read [Vectors](vectors.md) or [Vector assertions for AI and retrieval tests in .NET](vector-assertions-for-ai-and-retrieval-tests-in-dotnet.md) for embedding and retrieval-style assertions
