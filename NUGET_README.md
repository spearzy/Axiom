# Axiom

Axiom is a fluent assertion library for modern .NET tests. It is designed around deterministic failure output, explicit batch aggregation, low pass-path overhead, and configurable equivalency.

Target frameworks: `net8.0`, `net9.0`, and `net10.0`.

## Install

Most test projects should install `Axiom.Assertions`:

```bash
dotnet add package Axiom.Assertions
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
```

### Exceptions And Async

```csharp
Action act = () => throw new ArgumentNullException("userId");

act.Should()
    .Throw<ArgumentNullException>()
    .WithParamName("userId");

Task completed = Task.CompletedTask;
await completed.Should().CompleteWithin(TimeSpan.FromMilliseconds(50));
```

## Assertion Coverage

Axiom currently includes:

- value assertions: equality, nullability, type/reference checks, numeric comparisons, ranges, predicates, approximate numeric checks, equivalency
- string assertions: exact equality, null/empty/whitespace checks, prefix/suffix/contain, regex, comparison-aware matching
- exceptions and async: throw, exact throw, message/parameter/inner-exception checks, completion assertions, direct task entrypoints
- collections and dictionaries: containment, exact sequence, count/empty checks, ordering, uniqueness, single-item extraction, key/value extraction
- temporal assertions: before, after, and within-tolerance checks

## Documentation

- [GitHub repository](https://github.com/spearzy/Axiom)
- [Main README](https://github.com/spearzy/Axiom/blob/main/README.md)
- [Assertion reference](https://github.com/spearzy/Axiom/blob/main/docs/assertion-reference.md)
- [Equivalency guide](https://github.com/spearzy/Axiom/blob/main/docs/equivalency.md)
