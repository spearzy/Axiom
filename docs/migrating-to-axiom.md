---
title: Migrating to Axiom Assertions for .NET
description: Move a .NET test suite onto Axiom Assertions in stages, with focused guidance for analyzer-driven rewrites, structural assertions, and manual migration decisions.
---

# Migrating to Axiom

This page is for teams moving an existing test suite onto Axiom Assertions in stages.

If your starting point is mostly xUnit `Assert.*`, the quickest companion guide is [Migrate from xUnit Assert to Axiom](migrate-from-xunit-assert.md).

The built-in migration analyzers help with a growing set of safe, mechanical rewrites. Structural assertions still need engineering judgment. That is normal. This guide tries to make that judgment simpler.

## What Can Be Migrated Automatically Today

The current analyzer and code-fix wave focuses on high-confidence xUnit `Assert.*` shapes plus conservative first NUnit and MSTest migration waves.

| Source style | Axiom style |
| --- | --- |
| `Assert.Equal(expected, actual)` | `actual.Should().Be(expected)` |
| `Assert.NotEqual(expected, actual)` | `actual.Should().NotBe(expected)` |
| `Assert.Equal(expected, actual, comparer)` for non-string value assertions | `actual.Should().Be(expected, comparer)` |
| `Assert.NotEqual(expected, actual, comparer)` for non-string value assertions | `actual.Should().NotBe(expected, comparer)` |
| `Assert.Null(value)` | `value.Should().BeNull()` |
| `Assert.NotNull(value)` | `value.Should().NotBeNull()` |
| `Assert.True(condition)` | `condition.Should().BeTrue()` |
| `Assert.False(condition)` | `condition.Should().BeFalse()` |
| `Assert.Empty(subject)` | `subject.Should().BeEmpty()` |
| `Assert.NotEmpty(subject)` | `subject.Should().NotBeEmpty()` |
| `Assert.Contains(item, collection)` | `collection.Should().Contain(item)` |
| `Assert.DoesNotContain(item, collection)` | `collection.Should().NotContain(item)` |
| `Assert.Contains(expectedSubstring, actualString)` | `actualString.Should().Contain(expectedSubstring)` |
| `Assert.DoesNotContain(expectedSubstring, actualString)` | `actualString.Should().NotContain(expectedSubstring)` |
| `Assert.Contains(expectedSubstring, actualString, comparison)` | `actualString.Should().Contain(expectedSubstring, comparison)` |
| `Assert.DoesNotContain(expectedSubstring, actualString, comparison)` | `actualString.Should().NotContain(expectedSubstring, comparison)` |
| `Assert.StartsWith(expectedPrefix, actualString)` | `actualString.Should().StartWith(expectedPrefix)` |
| `Assert.EndsWith(expectedSuffix, actualString)` | `actualString.Should().EndWith(expectedSuffix)` |
| `Assert.StartsWith(expectedPrefix, actualString, comparison)` when `expectedPrefix` is an obvious non-null constant string | `actualString.Should().StartWith(expectedPrefix, comparison)` |
| `Assert.EndsWith(expectedSuffix, actualString, comparison)` when `expectedSuffix` is an obvious non-null constant string | `actualString.Should().EndWith(expectedSuffix, comparison)` |
| `Assert.Contains(key, dictionary)` | `dictionary.Should().ContainKey(key)` or `.WhoseValue` when you use the associated value |
| `Assert.DoesNotContain(key, dictionary)` | `dictionary.Should().NotContainKey(key)` |
| `Assert.Single(subject)` | `subject.Should().ContainSingle()` or `.SingleItem` when you use the item |
| `Assert.Single(subject, predicate)` | `subject.Should().ContainSingle(predicate)` or `.SingleItem` when you use the matched item |
| `Assert.Same(expected, actual)` | `actual.Should().BeSameAs(expected)` |
| `Assert.NotSame(expected, actual)` | `actual.Should().NotBeSameAs(expected)` |
| `Assert.IsType<T>(actual)` | `actual.Should().BeOfType<T>()` |
| `Assert.IsAssignableFrom<T>(actual)` | `actual.Should().BeAssignableTo<T>()` |
| `Assert.Throws<TException>(() => work())` | `new Action(() => work()).Should().Throw<TException>()` |
| `Assert.Throws<TException>("name", () => work())` with a non-null constant param name | `new Action(() => work()).Should().Throw<TException>().WithParamName("name")` or append `.Thrown` when you use the exception |
| `Assert.That(actual, Is.EqualTo(expected))` | `actual.Should().Be(expected)` |
| `Assert.That(actual, Is.Not.EqualTo(expected))` | `actual.Should().NotBe(expected)` |
| `Assert.That(value, Is.Null)` | `value.Should().BeNull()` |
| `Assert.That(value, Is.Not.Null)` | `value.Should().NotBeNull()` |
| `Assert.That(condition, Is.True)` | `condition.Should().BeTrue()` |
| `Assert.That(condition, Is.False)` | `condition.Should().BeFalse()` |
| `Assert.That(collection, Is.Empty)` | `collection.Should().BeEmpty()` |
| `Assert.That(collection, Is.Not.Empty)` | `collection.Should().NotBeEmpty()` |
| `Assert.AreEqual(expected, actual)` | `actual.Should().Be(expected)` |
| `Assert.AreNotEqual(expected, actual)` | `actual.Should().NotBe(expected)` |
| `Assert.IsNull(value)` | `value.Should().BeNull()` |
| `Assert.IsNotNull(value)` | `value.Should().NotBeNull()` |
| `Assert.IsTrue(condition)` | `condition.Should().BeTrue()` |
| `Assert.IsFalse(condition)` | `condition.Should().BeFalse()` |
| `Assert.AreSame(expected, actual)` | `actual.Should().BeSameAs(expected)` |
| `Assert.AreNotSame(expected, actual)` | `actual.Should().NotBeSameAs(expected)` |

For dictionary-key migration, the receiver has to fit Axiom's `ContainKey` / `NotContainKey` surface. The current support covers `IDictionary<TKey, TValue>`, `IReadOnlyDictionary<TKey, TValue>`, `Dictionary<TKey, TValue>`, `ReadOnlyDictionary<TKey, TValue>`, `ConcurrentDictionary<TKey, TValue>`, and `ImmutableDictionary<TKey, TValue>`.

These suggestions ship in:

- `Axiom.Assertions` by default
- `Axiom.Analyzers` if you only want the diagnostics

The NUnit and MSTest support is intentionally narrower than the xUnit support today. These first waves only handle the simple shapes that map directly to Axiom without carrying extra framework-specific semantics across.

## What Still Needs Manual Migration

The migration tooling is conservative on purpose.

It skips cases where the rewrite is not obviously semantics-preserving yet, including:

- precision, inspectors, and message-bearing overloads
- string-equality overloads such as `Assert.Equal(..., ignoreCase: ...)`
- comparer-bearing equality overloads that would land on Axiom's specialized string assertion surface rather than a direct local-comparer value assertion
- `Assert.StartsWith(...)` and `Assert.EndsWith(...)` overloads that use `Memory<char>` or `Span<char>`
- `Assert.StartsWith(...)` and `Assert.EndsWith(...)` when the expected prefix or suffix is not an obvious non-null constant string
- nongeneric `Assert.Single(subject)` when you use the returned value
- `Assert.Throws<TException>(...)` when you use the returned exception outside the `string? paramName, Action testCode` overload
- `Assert.Throws<TException>(paramName, ...)` when `paramName` is not an obvious non-null constant string
- richer NUnit constraint chains such as `Does.*`, `Has.*`, comparer/tolerance variants, and message-bearing `Assert.That(...)` overloads
- MSTest message-bearing, comparer, precision, and other assertion-family overloads
- most structural-comparison assertions

If a code fix appears, it is meant to be a safe one. If it does not appear, treat the migration as a normal test rewrite rather than a missed trick.

## Choosing Between Scalar And Structural Migration

A simple rule helps here.

If the original test is checking one fact at a time, migrate it to scalar Axiom assertions.

```csharp
actual.Name.Should().Be("Bob");
actual.Email.Should().Contain("@");
```

If the original test is really comparing one object graph with another, move it to `BeEquivalentTo(...)` instead of rewriting it into many scalar assertions.

```csharp
actual.Should().BeEquivalentTo(expected);
```

That keeps the test aligned with the shape of the behavior you actually care about.

## Migrating Structural Assertions

### Simple Object-Graph Comparison

If the original assertion is conceptually "these two objects should match member-for-member", start here:

```csharp
actual.Should().BeEquivalentTo(expected);
```

Do this before adding options. The defaults are strict and easy to reason about.

### Cross-Type Comparison

If the types differ but they represent the same concept and the member names already line up, disable strict runtime type matching:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.RequireStrictRuntimeTypes = false);
```

That is the normal Axiom target for DTO-to-domain or snapshot-to-response comparisons when the shapes are mostly the same.

### Renamed Members

If the shapes are the same but some member names changed, add only the mappings you need.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
    options.MatchMember<ActualUser, ExpectedUser>(x => x.Address.Postcode, x => x.Location.ZipCode);
});
```

Use typed mappings when you can. They are easier to read and safer to refactor.

If the mapping has to come from strings, use `MatchMemberName(...)` instead. As with typed mappings, repeated identical mappings are fine, but conflicting remaps fail fast.

### Custom Comparers, Ordering, And Tolerances

If the original test used domain-specific comparison rules, carry those rules over directly instead of flattening the test.

Examples:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.CollectionOrder = EquivalencyCollectionOrder.Any);

actual.Should().BeEquivalentTo(expected, options =>
    options.DateTimeTolerance = TimeSpan.FromSeconds(1));

actual.Should().BeEquivalentTo(expected, options =>
    options.UseComparer<OrderSnapshot>(x => x.Code, StringComparer.OrdinalIgnoreCase));
```

This is usually the right migration shape when the structural comparison is still the core idea and only a few comparison rules need to change.

## When Manual Migration Is Better

Some structural assertions should stay manual even if they look close to a possible automation target.

Keep the migration manual when:

- the original assertion mixes many domain-specific comparison rules
- the original test intentionally compares only a small stable subset of the graph
- the original assertion relies on custom comparers or tolerances that need review
- the original test is easier to understand as a few scalar assertions than as a broad graph comparison
- the structural comparison has unclear semantics and needs a test-design decision, not a syntax rewrite

That last point matters. A manual migration is often the right move when the old test itself was not very explicit.

## Example: Rewriting A Structural Test

Before:

```csharp
Assert.Equal(expected.Name, actual.Name);
Assert.Equal(expected.Email, actual.Email);
Assert.Equal(expected.Address.Postcode, actual.Address.Postcode);
Assert.Equal(expected.Address.CountryCode, actual.Address.CountryCode);
```

After:

```csharp
actual.Should().BeEquivalentTo(expected);
```

If one member was renamed:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
});
```

If order does not matter for one collection:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.CollectionOrder = EquivalencyCollectionOrder.Any);
```

## Recommended Migration Flow

For most teams, the smoothest sequence is:

1. Install `Axiom.Assertions`.
2. Apply the obvious analyzer-driven rewrites first.
3. Move structural comparisons to `BeEquivalentTo(...)` manually.
4. Add only the minimum equivalency options needed for that test.
5. Keep unclear or overloaded cases manual until the intended semantics are explicit.

That keeps the migration trustworthy.

## Where To Go Next

- Read [Migrate from xUnit Assert to Axiom](migrate-from-xunit-assert.md) for the xUnit-focused path
- Read [Axiom vs FluentAssertions](axiom-vs-fluentassertions.md) or [Axiom vs Shouldly](axiom-vs-shouldly.md) if you are still choosing an assertion library
- For the structural-comparison rules, defaults, and failure-output examples, read [Equivalency](equivalency.md)
