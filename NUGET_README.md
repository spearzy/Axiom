# Axiom

Axiom is a fluent assertion library for modern .NET tests. It is designed around deterministic failure output, explicit batch aggregation, low pass-path overhead, and configurable equivalency.

Target frameworks: `net8.0`, `net9.0`, and `net10.0`.

Documentation: [spearzy.github.io/Axiom](https://spearzy.github.io/Axiom/)

## Install

Most test projects should install `Axiom.Assertions`:

```bash
dotnet add package Axiom.Assertions
```

`Axiom.Assertions` bundles the Axiom analyzers/code fixes automatically, including checks for ignored async Axiom assertions, undisposed `Batch` usage, and high-confidence xUnit, NUnit, and MSTest migration suggestions.

Install it and start writing assertions. You do not need `AxiomSetup.cs` just to get started.

Install `Axiom.Analyzers` separately only if you want the diagnostics without the runtime assertion library:

```bash
dotnet add package Axiom.Analyzers
```

Install `Axiom.Core` directly only when you want low-level primitives such as `Batch`, formatting, or configuration without the full fluent assertion surface:

```bash
dotnet add package Axiom.Core
```

Install `Axiom.Vectors` when you want vector and embedding-focused assertions on top of the main Axiom assertion library:

```bash
dotnet add package Axiom.Vectors
```

`Axiom.Vectors` adds dimension, approximate equality, dot product, Euclidean distance, cosine similarity, zero-vector, NaN/infinity, normalization, and ranked retrieval evaluation assertions for vector, embedding, and search-style test scenarios.

```csharp
embedding.Should().HaveDotProductWith(expected, expectedDotProduct: 1f, tolerance: 0.001f);
embedding.Should().HaveEuclideanDistanceTo(unrelated, expectedDistance: 1.4142f, tolerance: 0.001f);
embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.995f).And.BeNormalized();
new float[] { 0f, 0f }.Should().BeZeroVector();
```

## Why Axiom

- Deterministic failure messages that stay stable in CI and code review.
- Explicit multi-assertion aggregation with `Batch`.
- Fluent assertion chaining with straightforward `.And` continuations.
- Configurable structural equivalency for object graphs.
- Extensibility hooks for comparer providers, value formatters, and modules.

## Quick Start

```csharp
using Axiom.Assertions;
using Axiom.Core;

user.Name.Should().NotBeNull();
user.Age.Should().BeGreaterThan(18);
user.Email.Should().Contain("@");

using var batch = Assert.Batch("profile");
user.Name.Should().StartWith("A");
user.Roles.Should().Contain("admin");
```

Example deterministic failure output:

```text
Expected user.Email to contain "@", but found "invalid-email".
```

## Optional Configuration

Axiom automatically uses framework-native assertion exception types for xUnit, NUnit, and MSTest when it detects those frameworks at runtime.

You only need configuration when you want custom defaults such as:

- equivalency defaults
- custom comparer provider
- custom value formatter
- custom regex timeout
- explicit failure-strategy override
- shared reusable modules

For shared defaults, prefer `AxiomSettings.Configure(...)`. `AxiomServices.Configure(...)` and `EquivalencyDefaults.Configure(...)` remain available for lower-level or isolated configuration.

See the docs site for full setup and customization guidance:

- [Getting Started](https://spearzy.github.io/Axiom/getting-started/)
- [Assertion Reference](https://spearzy.github.io/Axiom/assertion-reference/)

## Core Workflows

### Batch Aggregation

Use `Batch` when you want to collect multiple failures and throw one combined report at the end of a scope.

```csharp
using var batch = Assert.Batch("user profile");

user.Name.Should().NotBeNull();
user.Email.Should().Contain("@");
user.Roles.Should().Contain("admin");
```

### Object Equivalency

Use `BeEquivalentTo(...)` when you want structural comparison rather than direct equality.

```csharp
using Axiom.Assertions.Equivalency;

actual.Should().BeEquivalentTo(expected, options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
    options.IgnorePath("actual.UpdatedAt");
});

actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
});
```

### Exceptions And Async

```csharp
Action act = () => throw new ArgumentNullException("userId");

act.Should()
    .Throw<ArgumentNullException>()
    .WithParamName("userId");

Task<string> rollout = Task.FromResult("pricing-api");

var continuation = await rollout.Should().SucceedWithin(TimeSpan.FromMilliseconds(50));
continuation.WhoseResult.Should().Be("pricing-api");

Func<Task<User>> loadUser = () => userClient.LoadAsync("bob");
var loadedUser = await loadUser.Should().SucceedWithin(TimeSpan.FromMilliseconds(250));
loadedUser.WhoseResult.Email.Should().Contain("@");

IAsyncEnumerable<Order> orders = orderRepository.StreamRecentAsync();
var priorityOrder = await orders.Should().ContainSingleAsync(order => order.IsPriority);
priorityOrder.SingleItem.Total.Should().BeGreaterThan(0m);

await orders.Should().SatisfyRespectivelyAsync(
    first => first.Total.Should().Be(10m),
    second => second.Total.Should().Be(20m));

await orders.Should().HaveUniqueItemsByAsync(order => order.Id);
await stepIds.Should().ContainInOrderAsync([WorkflowStep.Started, WorkflowStep.Completed]);
```

If a concrete wrapper type implements `IAsyncEnumerable<T>` and `.Should()` binds to generic value assertions, use `.ShouldAsyncEnumerable()` to force the async-stream assertion surface.

### Custom Assertions

```csharp
using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Authoring;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

public static class InvoiceAssertionExtensions
{
    public static AndContinuation<ValueAssertions<Invoice>> HaveCurrency(
        this ValueAssertions<Invoice> assertions,
        string expectedCurrency,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var context = AssertionContext.Create(assertions);

        if (!string.Equals(context.Subject.Currency, expectedCurrency, StringComparison.Ordinal))
        {
            context.Fail(
                new Expectation("to have currency", expectedCurrency),
                context.Subject.Currency,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return context.And();
    }
}
```

For common generic collection subjects, parameterless `ContainSingle()` keeps `SingleItem` strongly typed so follow-up assertions do not need a cast.

## Assertion Coverage

Axiom currently includes:

- value assertions: equality, nullability, type/reference checks, numeric comparisons, ranges, predicates, approximate numeric checks, equivalency
- string assertions: exact equality, null/empty/whitespace checks, prefix/suffix/contain, regex, comparison-aware matching
- exceptions and async: throw, exact throw, message/parameter/inner-exception checks, delegate-based async assertions, async function result assertions, direct task completion and outcome assertions
- collections and dictionaries: containment, exact sequence, count/empty checks, ordering, uniqueness, single-item extraction, key/value extraction, direct `IAsyncEnumerable<T>` assertions
- temporal assertions: before, after, and within-tolerance checks
- vector assertions: dimension checks, NaN/infinity validation, approximate equality, dot product, Euclidean distance, cosine similarity thresholds, normalization, ranked retrieval evaluation
- custom assertion authoring: `AssertionContext.Create(...)` for domain assertions on `ValueAssertions<T>`

## Documentation

- [Docs site](https://spearzy.github.io/Axiom/)
- [GitHub repository](https://github.com/spearzy/Axiom)
- [Getting Started](https://spearzy.github.io/Axiom/getting-started/)
- [Assertion reference](https://spearzy.github.io/Axiom/assertion-reference/)
- [Custom assertions guide](https://spearzy.github.io/Axiom/custom-assertions/)
- [Equivalency guide](https://spearzy.github.io/Axiom/equivalency/)
- [Analyzer guide](https://spearzy.github.io/Axiom/analyzers/)
- [Vectors guide](https://spearzy.github.io/Axiom/vectors/)
