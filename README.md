# Axiom

![Axiom](assets/images/axiom-logo.png)

[![CI](https://github.com/spearzy/Axiom/actions/workflows/ci.yml/badge.svg)](https://github.com/spearzy/Axiom/actions/workflows/ci.yml)
[![License: Apache-2.0](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

`Axiom.Assertions`  
[![Version](https://img.shields.io/nuget/v/Axiom.Assertions?label=version)](https://www.nuget.org/packages/Axiom.Assertions)
[![Downloads](https://img.shields.io/nuget/dt/Axiom.Assertions?label=downloads)](https://www.nuget.org/packages/Axiom.Assertions)

`Axiom.Core`  
[![Version](https://img.shields.io/nuget/v/Axiom.Core?label=version)](https://www.nuget.org/packages/Axiom.Core)
[![Downloads](https://img.shields.io/nuget/dt/Axiom.Core?label=downloads)](https://www.nuget.org/packages/Axiom.Core)

`Axiom.Analyzers`  
[![Version](https://img.shields.io/nuget/v/Axiom.Analyzers?label=version)](https://www.nuget.org/packages/Axiom.Analyzers)
[![Downloads](https://img.shields.io/nuget/dt/Axiom.Analyzers?label=downloads)](https://www.nuget.org/packages/Axiom.Analyzers)

`Axiom.Vectors`  
[![Version](https://img.shields.io/nuget/v/Axiom.Vectors?label=version)](https://www.nuget.org/packages/Axiom.Vectors)
[![Downloads](https://img.shields.io/nuget/dt/Axiom.Vectors?label=downloads)](https://www.nuget.org/packages/Axiom.Vectors)

Axiom is a fluent assertion library for modern .NET tests. It is designed around deterministic failure output, explicit batch aggregation, low pass-path overhead, and configurable equivalency.

Target frameworks: `net8.0`, `net9.0`, and `net10.0`.

Documentation: [spearzy.github.io/Axiom](https://spearzy.github.io/Axiom/)

## Packages

Most test projects should reference `Axiom.Assertions`. It contains the fluent `Should()` API, pulls in `Axiom.Core` automatically, and bundles the Axiom analyzers/code fixes so editor and build diagnostics light up without extra package setup. The bundled rules currently cover ignored async Axiom assertions and `Batch` instances that are created without being disposed.

Use `Axiom.Core` directly only when you need low-level primitives such as `Batch`, formatting, or configuration without the full assertion surface.

```bash
dotnet add package Axiom.Assertions
```

If you only need the core primitives:

```bash
dotnet add package Axiom.Core
```

If you want vector and embedding-focused assertions:

```bash
dotnet add package Axiom.Vectors
```

Install `Axiom.Analyzers` separately only if you want the diagnostics without the runtime assertion library:

```bash
dotnet add package Axiom.Analyzers
```

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

Example failure output is deterministic and stable:

```text
Expected user.Email to contain "@", but found "invalid-email".
```

## Global Setup

For most teams, the clearest approach is one setup file in the test project.

Create `AxiomSetup.cs`:

```csharp
using Axiom.Assertions;
using Axiom.Assertions.Configuration;
using Axiom.Assertions.Equivalency;
using Axiom.Core.Failures;

public static class AxiomSetup
{
    public static void Apply()
    {
        AxiomSettings.Configure(options =>
        {
            options.Core.RegexMatchTimeout = TimeSpan.FromMilliseconds(500);
            options.Core.FailureStrategy = XunitFailureStrategy.Instance;

            options.Equivalency.CollectionOrder = EquivalencyCollectionOrder.Any;
            options.Equivalency.RequireStrictRuntimeTypes = false;
            options.Equivalency.FailOnMissingMembers = false;
            options.Equivalency.FailOnExtraMembers = false;
        });
    }
}
```

Set `options.Core.FailureStrategy` to the strategy that matches your test framework. Call `AxiomSetup.Apply()` once in your test framework's startup hook:

- xUnit: call it in a collection fixture constructor.
- NUnit: call it from `[SetUpFixture]` + `[OneTimeSetUp]`.
- MSTest: call it from `[AssemblyInitialize]`.

`AxiomServices` and `EquivalencyDefaults` still work and remain fully supported for lower-level or separate configuration flows.

If your team reuses the same setup across many test projects, package it as a module:

```csharp
AxiomSettings.UseModule(new ApiTestModule());

public sealed class ApiTestModule : IAxiomSettingsModule
{
    public void Configure(AxiomSettingsOptions options)
    {
        options.Core.RegexMatchTimeout = TimeSpan.FromMilliseconds(500);
        options.Equivalency.RequireStrictRuntimeTypes = false;
        options.Equivalency.FailOnMissingMembers = false;
        options.Equivalency.FailOnExtraMembers = false;
    }
}
```

## Why Axiom

- Deterministic messages you can rely on in CI, code review, and snapshot-like tests.
- Explicit multi-assertion aggregation with `Batch` instead of hidden ambient scope behavior.
- Fluent assertions with straightforward chaining via `.And`.
- Configurable object equivalency with clear per-call and global defaults.
- Extensibility hooks for custom comparers, value formatters, and modules.

## Core Workflows

### Batch Related Assertions

Use `Batch` when you want to collect multiple failures and throw once at the end of a scope.

```csharp
using var batch = Assert.Batch("user profile");

user.Name.Should().NotBeNull();
user.Email.Should().Contain("@");
user.Roles.Should().Contain("admin");
```

If more than one assertion fails, the root batch throws one combined report:

```text
Batch 'user profile' failed with 2 assertion failure(s):
1) ...
2) ...
```

### Value And String Assertions

```csharp
42.Should()
    .BeGreaterThan(0).And
    .BeInRange(1, 100).And
    .Satisfy(x => x % 2 == 0);

"Admin".Should().Be("Admin");
"Admin".Should().Be("admin", StringComparison.OrdinalIgnoreCase);
"Admin".Should().NotBe("ops", StringComparison.OrdinalIgnoreCase);
"ops@example.com".Should().Contain("@");
"ABC".Should().StartWith("ab", StringComparison.OrdinalIgnoreCase);
```

String assertions cover exact equality, null checks, empty/whitespace checks, substring/prefix/suffix assertions, and regex matching.

### Object Equivalency

Use `BeEquivalentTo(...)` when you want structural comparison instead of direct equality.

```csharp
using Axiom.Assertions.Equivalency;

var actual = new
{
    Name = "Ada",
    Scores = new[] { 3, 1, 2 },
    UpdatedAt = new DateTime(2026, 3, 8, 12, 0, 0, DateTimeKind.Utc)
};

var expected = new
{
    Name = "Ada",
    Scores = new[] { 1, 2, 3 },
    UpdatedAt = new DateTime(2026, 3, 8, 12, 0, 1, DateTimeKind.Utc)
};

actual.Should().BeEquivalentTo(expected, options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
    options.IgnorePath("actual.UpdatedAt");
});
```

Current equivalency configuration supports:

- strict or any-order collection comparison
- member and path exclusion, including expression-based selectors on named types
- typed or name-based member mapping between different object shapes
- per-path, per-member, and per-type comparers
- missing and extra member controls
- numeric and temporal tolerances
- diagnostics that distinguish missing vs extra members and show mapped expected-side paths on failures
- global defaults via `EquivalencyDefaults.Configure(...)`

For cross-type renames, prefer the typed mapping API:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
    options.MatchMember<ActualUser, ExpectedUser>(x => x.Address.Postcode, x => x.Location.ZipCode);
});
```

Equality precedence inside `BeEquivalentTo(...)` is:

1. configured tolerance for the leaf type
2. per-path comparer
3. `StringComparison` for strings
4. per-call type comparer
5. global comparer provider
6. default equality

For the full configuration guide, precedence rules, and practical recipes, see the [Equivalency guide](https://spearzy.github.io/Axiom/equivalency/).

### Exception And Async Assertions

```csharp
Action act = () => throw new ArgumentNullException("userId");

act.Should()
    .Throw<ArgumentNullException>()
    .WithParamName("userId");

var thrown = act.Should()
    .ThrowExactly<ArgumentNullException>()
    .Thrown;

thrown.ParamName.Should().Be("userId");

Func<Task> failingTask = () => Task.FromException(new InvalidOperationException("boom"));
await (await failingTask.Should().ThrowAsync<InvalidOperationException>())
    .WithMessage("boom");

Task<string> rollout = Task.FromResult("pricing-api");

var continuation = await rollout.Should().SucceedWithin(TimeSpan.FromMilliseconds(50));
continuation.WhoseResult.Should().Be("pricing-api");

Func<Task<User>> loadUser = () => userClient.LoadAsync("ada");
var loadedUser = await loadUser.Should().SucceedWithin(TimeSpan.FromMilliseconds(250));
loadedUser.WhoseResult.Email.Should().Contain("@");

Func<ValueTask<int>> loadCount = () => ValueTask.FromResult(3);
await loadCount.Should().NotThrowAsync();

IAsyncEnumerable<Order> orders = orderRepository.StreamRecentAsync();
await orders.Should().NotBeEmptyAsync();

var priorityOrder = await orders.Should().ContainSingleAsync(order => order.IsPriority);
priorityOrder.SingleItem.Total.Should().BeGreaterThan(0m);
```

Async exception and completion assertions are supported on:

- `Func<Task>`
- `Func<ValueTask>`
- `Func<Task<T>>`
- `Func<ValueTask<T>>`
- `Task`
- `Task<T>`
- `ValueTask`
- `ValueTask<T>`

Direct task subjects also support outcome assertions such as `Succeed()`, `SucceedWithin(...)`, `BeCanceled()`, and `BeFaultedWith<TException>()`.

Axiom also supports `IAsyncEnumerable<T>` directly, so you can assert async streams without materializing them into a list first.

If you have a concrete wrapper type that implements `IAsyncEnumerable<T>` and `.Should()` binds to the generic value assertion entry point, use `.ShouldAsyncEnumerable()` to force async-stream assertions:

```csharp
MyWrappedStream<User> stream = GetUsers();

await stream.ShouldAsyncEnumerable()
    .ContainSingleAsync(user => user.Id == 42);
```

### Vector And Embedding Assertions

`Axiom.Vectors` is an optional package for vector and embedding-style test scenarios.

```csharp
using Axiom.Vectors;

embedding.Should().HaveDimension(1536);
embedding.Should().NotContainNaNOrInfinity();
embedding.Should().BeApproximatelyEqualTo(expected, tolerance: 1e-5f);
embedding.Should().HaveCosineSimilarityTo(expected).AtLeast(0.995f);
embedding.Should().BeNormalized(tolerance: 1e-5f);
```

For the full vectors guide, see the [Vectors guide](https://spearzy.github.io/Axiom/vectors/).

### Collections And Dictionaries

```csharp
Order order = orders.Should()
    .ContainSingle()
    .SingleItem;

order.Total.Should().Be(19.99m);

Order matchingOrder = orders.Should()
    .ContainSingle((Order x) => x.Id == 42)
    .SingleItem;

scores.Should()
    .ContainKey("a")
    .WhoseValue
    .Should()
    .Be(1);

users.Should().HaveUniqueItemsBy(
    (User x) => x.Email,
    StringComparer.OrdinalIgnoreCase);

steps.Should().SatisfyRespectively(
    first => first.Name.Should().Be("Queued"),
    second => second.Name.Should().Be("Running"),
    third => third.Name.Should().Be("Completed"));

timestamps.Should().BeInAscendingOrder();
```

For common generic collection subjects, parameterless `ContainSingle()` keeps `SingleItem` strongly typed so follow-up assertions do not need a cast.

Representative collection coverage includes:

- containment and negative containment
- count and emptiness checks
- subset, superset, and exact-sequence assertions
- single-item assertions and value extraction
- key and value assertions for dictionaries
- uniqueness assertions, including `HaveUniqueItemsBy(...)`
- ordered assertions, including key-selector overloads
- workflow-style assertions such as `SatisfyRespectively(...)`

Parameterless `ContainSingle()` returns a typed `SingleItem` for common generic collection subjects such as `List<T>`, arrays, and interface-typed generic enumerables. Nongeneric collections still expose `object? SingleItem`.

### Custom Assertions

Use `AssertionContext.Create(...)` when you want to write your own domain assertions on top of `ValueAssertions<T>` without reimplementing failure rendering or batch routing.

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

That custom assertion automatically respects `Batch`, your configured failure strategy, and the same subject-expression labelling as built-in Axiom assertions.

### Temporal Assertions

```csharp
var now = DateTime.UtcNow;
var later = now.AddMinutes(2);

later.Should()
    .BeAfter(now).And
    .BeWithin(now.AddMinutes(2), TimeSpan.FromSeconds(1));
```

Temporal assertions are available for `DateTime`, `DateTimeOffset`, `DateOnly`, and `TimeOnly`.

## Assertion Coverage At A Glance

This README focuses on common workflows rather than listing every method in the library. Current coverage includes:

- Values: equality, nullability, type/reference checks, numeric comparisons, ranges, predicates, approximate numeric checks, equivalency
- Strings: exact equality, null/empty/whitespace checks, prefix/suffix/contain, regex, case-aware comparisons
- Exceptions and async: throw, exact throw, message/parameter/inner-exception checks, delegate-based async assertions, async function result assertions, direct task completion and outcome assertions
- Collections and dictionaries: containment, sequence checks, ordering, uniqueness, count/empty checks, single-item extraction, key/value extraction, direct `IAsyncEnumerable<T>` assertions
- Temporal values: before, after, and within-tolerance checks
- Custom assertions: supported authoring on top of `ValueAssertions<T>` via `AssertionContext.Create(...)`

## Assertion Reference

For the complete current method catalog, see the [Assertion Reference](https://spearzy.github.io/Axiom/assertion-reference/). That page is the API-discovery layer for consumers who want to evaluate Axiom without opening the source or relying on IDE completion.

For practical guidance on building domain-specific assertions, see the [Custom Assertions guide](https://spearzy.github.io/Axiom/custom-assertions/).

For deeper guidance on structural comparison, configuration precedence, and common equivalency recipes, see the [Equivalency guide](https://spearzy.github.io/Axiom/equivalency/).

For the current analyzer rules and examples, see the [Analyzers guide](https://spearzy.github.io/Axiom/analyzers/).

High-level categories:

- Values: `Be`, `NotBe`, `BeOneOf`, nullability, type/reference checks, ranges, predicates, numeric approximation, structural equivalency
- Strings: exact equality, null/empty/whitespace checks, `StartWith`, `EndWith`, `Contain`, regex, comparison-aware matching
- Exceptions and async: `Throw`, `ThrowExactly`, `WithMessage`, `WithParamName`, `WithInnerException`, `CompleteWithin`, `NotCompleteWithin`
- Collections: containment, exact sequence, count and emptiness, subset and superset checks, uniqueness, `ContainSingle`, `OnlyContain`, `AllSatisfy`, `SatisfyRespectively`, ordering, direct async-stream assertions such as `ContainAsync` and `ContainSingleAsync`
- Dictionaries: `ContainKey`, `ContainValue`, `ContainEntry`, plus value extraction through `WhoseValue`
- Temporal: `BeBefore`, `BeAfter`, and `BeWithin`

The reference page also documents the specialized continuation members exposed by Axiom, including `Thrown`, `SingleItem`, and `WhoseValue`.

## Configuration And Extensibility

### Project-Wide Equivalency Defaults

```csharp
using Axiom.Assertions.Equivalency;

EquivalencyDefaults.Configure(options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
    options.FailOnExtraMembers = false;
});
```

Per-call configuration still overrides global defaults.

### Core Services

Use `AxiomServices.Configure(...)` to change low-level behaviour such as comparer selection, value formatting, regex timeout, or failure dispatch.

```csharp
using Axiom.Core.Configuration;

AxiomServices.Configure(config =>
{
    config.RegexMatchTimeout = TimeSpan.FromMilliseconds(500);
    config.ComparerProvider = new DomainComparerProvider();
});
```

### Failure Strategy

Failure dispatch is configurable through `IFailureStrategy`.

- Outside a `Batch`, each failed assertion goes through the configured strategy immediately.
- Inside a `Batch`, assertion failures are aggregated first; when the root batch is disposed, the combined report goes through the configured strategy.

Default behaviour throws `InvalidOperationException`.

Use this when you want Axiom failures to integrate with your test framework's native assertion exception type (for example xUnit/NUnit/MSTest), or when your team wants a custom exception policy.

Failure flow is:

1. An assertion fails and calls Axiom's dispatcher.
2. If a batch is active, the failure message is recorded.
3. If no batch is active, the configured `IFailureStrategy` is called immediately.
4. When a root batch is disposed, one combined failure message is sent through the same strategy.

```csharp
using Axiom.Core.Configuration;
using Axiom.Core.Failures;

AxiomServices.Configure(config =>
{
    config.FailureStrategy = new CustomFailureStrategy();
});

public sealed class CustomFailureStrategy : IFailureStrategy
{
    public void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0)
    {
        throw new InvalidOperationException(
            $"{message} (at {callerFilePath}:{callerLineNumber})");
    }
}
```

`IFailureStrategy.Fail(...)` should always throw. If a strategy returns instead of throwing, Axiom raises an `InvalidOperationException` guard failure.

Built-in framework strategies are also available:

```csharp
using Axiom.Core.Configuration;
using Axiom.Core.Failures;

AxiomServices.Configure(c => c.FailureStrategy = XunitFailureStrategy.Instance);
AxiomServices.Configure(c => c.FailureStrategy = NUnitFailureStrategy.Instance);
AxiomServices.Configure(c => c.FailureStrategy = MSTestFailureStrategy.Instance);
```

Each built-in strategy requires the corresponding test framework package to be referenced by the test project.

Quick selection guide:

- Keep default `InvalidOperationException`: simplest option, no framework coupling.
- Use `XunitFailureStrategy` / `NUnitFailureStrategy` / `MSTestFailureStrategy`: framework-native assertion exception types.
- Implement custom `IFailureStrategy`: project-specific exception type or reporting policy.

Use a custom comparer provider when your domain equality rules differ from default `.Equals(...)`. For one-off equivalency rules, prefer per-assertion options such as `UseComparerForType(...)`, `UseComparerForPath(...)`, or `UseComparerForMember(...)`.

If you need to package recurring configuration for test projects, implement `IAxiomSettingsModule` and apply it with `AxiomSettings.UseModule(...)`. Core-only `IAxiomModule` modules remain available through `AxiomServices` and can also be bridged through `AxiomSettings.UseModule(...)`.

## Notes

- `Axiom.Assertions` is the recommended package for test projects.
- `Axiom.Core` is intentionally small and can be used independently.
- Axiom is still in the `0.x` phase, so the public API is growing, but the project already targets real-world use rather than being a demo-only package.

## Security

Please report security issues privately first. See [SECURITY.md](SECURITY.md) for the current policy and reporting path.

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

Licensed under Apache-2.0. See [LICENSE](LICENSE).
