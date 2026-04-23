---
title: Migrate from xUnit Assert to Axiom
description: Move from xUnit Assert to Axiom Assertions with a practical guide to direct rewrites, analyzer-assisted migration, and where manual review still matters.
---

# Migrate from xUnit Assert to Axiom

If your tests are mostly built on `Assert.*`, Axiom is designed to give you a direct migration path without forcing a broad rewrite of how the suite is structured.

That does not mean every xUnit suite should migrate. If the current tests are clear and the team is not feeling friction, leaving them alone may be the better engineering choice.

## What The Straight Rewrite Looks Like

Many common xUnit assertions map directly to `Should()` assertions:

```csharp
var actualCount = 3;
string? name = "Alice";
var isReady = true;
var items = new[] { "admin", "user" };

actualCount.Should().Be(3);
name.Should().NotBeNull();
isReady.Should().BeTrue();
items.Should().Contain("admin");
```

That directness is the main reason teams can migrate in stages instead of pausing all test work for a large assertion rewrite.

## Where The Built-In Analyzers Help

The Axiom analyzers cover a growing set of safe, mechanical xUnit rewrites. They are useful when you want to move a codebase gradually and reserve manual review time for the tests that really need judgment.

That now includes awaited async exception assertions such as `Assert.ThrowsAsync<T>(...)`, `Assert.ThrowsAsync<T>(paramName, ...)`, and `Assert.ThrowsAnyAsync<T>(...)` when they map directly onto Axiom's async throw assertions without changing value flow.

Install path:

```bash
dotnet add package Axiom.Assertions
```

That default package already brings the analyzers with it.

## Where You Still Need Judgment

Some tests should stay manual during migration:

- assertions with precision rules or xUnit comparer overloads that do not have a direct, semantics-preserving Axiom equivalent yet
- structural comparisons that are really object-graph assertions
- exception assertions where the returned exception is used in detail
- non-awaited async exception assertions that would need a broader rewrite than a direct awaited `Should()` conversion
- tests that currently hide several behaviors inside a long chain of `Assert.*` calls

For those cases, use the broader [Migrating to Axiom](migrating-to-axiom.md) guide.

## When Axiom Can Be A Reasonable Migration Target

Axiom can be a reasonable xUnit Assert migration target when you want:

- direct scalar rewrites for common assertions
- deterministic failure output
- explicit grouped failures with `Batch`
- analyzer support that helps with the obvious rewrites first

The case is strongest when those benefits line up with the assertion shapes your suite already uses today.

## When A Different Move May Be Better

If your team is not trying to leave framework assertions yet, or if you mainly want the most familiar assertion ecosystem possible, it may be reasonable to stay with xUnit Assert for now or assess another fluent library first.

If you are making that choice, these pages can help:

- [Axiom vs FluentAssertions](axiom-vs-fluentassertions.md)
- [Axiom vs Shouldly](axiom-vs-shouldly.md)
- [.NET assertion library](dotnet-assertion-library.md)
