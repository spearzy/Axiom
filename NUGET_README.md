# Axiom

Axiom is a fluent assertion library for modern .NET tests. It is designed around deterministic failure output, explicit batch aggregation, low pass-path overhead, and configurable equivalency.

Target frameworks: `net8.0`, `net9.0`, and `net10.0`.

## Install

Most test projects should install `Axiom.Assertions`:

```bash
dotnet add package Axiom.Assertions
```

`Axiom.Assertions` bundles the Axiom analyzers/code fixes automatically, including checks for ignored async Axiom assertions and undisposed `Batch` usage.

Install `Axiom.Analyzers` separately only if you want the diagnostics without the runtime assertion library:

```bash
dotnet add package Axiom.Analyzers
```

Install `Axiom.Core` directly only when you want low-level primitives such as `Batch`, formatting, or configuration without the full fluent assertion surface:

```bash
dotnet add package Axiom.Core
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

## Global Setup

Put shared configuration in one `AxiomSetup.cs` file in your test project:

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

Set `options.Core.FailureStrategy` to the strategy that matches your test framework, then call `AxiomSetup.Apply()` once from your framework startup hook (xUnit fixture, NUnit one-time setup, or MSTest assembly initialise).

If you package shared defaults for several test projects, use `AxiomSettings.UseModule(...)` with an `IAxiomSettingsModule`:

```csharp
AxiomSettings.UseModule(new ApiTestModule());
```

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

Func<Task<User>> loadUser = () => userClient.LoadAsync("ada");
var loadedUser = await loadUser.Should().SucceedWithin(TimeSpan.FromMilliseconds(250));
loadedUser.WhoseResult.Email.Should().Contain("@");
```

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
- collections and dictionaries: containment, exact sequence, count/empty checks, ordering, uniqueness, single-item extraction, key/value extraction
- temporal assertions: before, after, and within-tolerance checks
- custom assertion authoring: `AssertionContext.Create(...)` for domain assertions on `ValueAssertions<T>`

## Documentation

- [GitHub repository](https://github.com/spearzy/Axiom)
- [Main README](https://github.com/spearzy/Axiom/blob/main/README.md)
- [Assertion reference](https://github.com/spearzy/Axiom/blob/main/docs/assertion-reference.md)
- [Custom assertions guide](https://github.com/spearzy/Axiom/blob/main/docs/custom-assertions.md)
- [Equivalency guide](https://github.com/spearzy/Axiom/blob/main/docs/equivalency.md)
- [Analyzer guide](https://github.com/spearzy/Axiom/blob/main/docs/analyzers.md)
