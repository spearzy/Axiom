# Axiom

![Axiom](assets/images/axiom-logo.png)

[![CI](https://github.com/spearzy/Axiom/actions/workflows/ci.yml/badge.svg)](https://github.com/spearzy/Axiom/actions/workflows/ci.yml)
[![License: Apache-2.0](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

**Packages**

`Axiom.Assertions`  
[![Version](https://img.shields.io/nuget/v/Axiom.Assertions?label=version)](https://www.nuget.org/packages/Axiom.Assertions)
[![Downloads](https://img.shields.io/nuget/dt/Axiom.Assertions?label=downloads)](https://www.nuget.org/packages/Axiom.Assertions)

`Axiom.Core`  
[![Version](https://img.shields.io/nuget/v/Axiom.Core?label=version)](https://www.nuget.org/packages/Axiom.Core)
[![Downloads](https://img.shields.io/nuget/dt/Axiom.Core?label=downloads)](https://www.nuget.org/packages/Axiom.Core)

Axiom is an open-source assertion library for .NET tests. It helps you write fluent, readable assertions, get clear deterministic failure messages, and optionally collect multiple failures in one run with `Batch`.

## Table of Contents

- [Why Axiom?](#why-axiom)
- [Key Features](#key-features)
- [Implemented Assertion Methods (Current)](#implemented-assertion-methods-current)
- [Usage](#usage)
  - [Batch Assertions](#batch-assertions)
  - [Fluent String Assertions](#fluent-string-assertions)
  - [Value Assertions](#value-assertions)
  - [Equivalency Assertions](#equivalency-assertions)
  - [Tolerance Rules For Non-Finite Numbers](#tolerance-rules-for-non-finite-numbers)
  - [Optional Global Equivalency Defaults](#optional-global-equivalency-defaults)
  - [Custom Comparer Providers](#custom-comparer-providers)
  - [Per-Assertion Type Comparers (Equivalency)](#per-assertion-type-comparers-equivalency)
  - [Exception Assertions](#exception-assertions-1)
  - [Collection Assertions](#collection-assertions)
  - [Temporal Assertions](#temporal-assertions)
- [Installation](#installation)
- [Security](#security)
- [Contributing](#contributing)
- [License](#license)

## Why Axiom?

Axiom started as a learning project to understand how a modern .NET assertion library is built end to end.
It is open source so other developers can use it, inspect the design decisions, and extend it where useful.

In practice, that means:
- deterministic failure output you can rely on in CI and code reviews,
- low pass-path overhead (defer work until failure where possible),
- explicit batch aggregation with `Batch` for multi-check validation,
- configurable equivalency behaviour with clear precedence,
- extension points for custom comparers and value formatters.

Example failure output includes subject context and expected/actual values:

```text
Expected value to start with "ab", but found "test".
```

## Key Features

- Fluent `Should()` API with `.And` chaining
- `Batch` assertions for one combined failure from multiple checks
- `BeEquivalentTo(...)` for object graph comparison with configurable options
- Optional global equivalency defaults with per-call overrides
- Deterministic, testable failure messages and batch reports
- Extensible value comparison/formatting via comparer and formatter hooks
- Target frameworks: `net10.0` (primary), `net9.0`, and `net8.0`

## Implemented Assertion Methods (Current)

### Value assertions
- `Be(expected)`
- `NotBe(unexpected)`
- `BeOneOf(expectedValues)`
- `NotBeOneOf(unexpectedValues)`
- `Satisfy(predicate)`
- `NotSatisfy(predicate)`
- `BeNull()`
- `NotBeNull()`
- `BeSameAs(expectedReference)`
- `NotBeSameAs(unexpectedReference)`
- `BeOfType<TExpected>()`
- `BeAssignableTo<TExpected>()`
- `NotBeAssignableTo<TExpected>()`
- `BeGreaterThan(value)`
- `BeGreaterThanOrEqualTo(value)`
- `BeLessThan(value)`
- `BeLessThanOrEqualTo(value)`
- `BeInRange(min, max)`
- `BeApproximately(expected, tolerance)` for `double`, `float`, and `decimal`
- `BeEquivalentTo(expected)`
- `BeEquivalentTo(expected, configureOptions)`
- `NotBeEquivalentTo(expected)`
- `NotBeEquivalentTo(expected, configureOptions)`
- `BeTrue()` / `BeFalse()` (extension methods on `ValueAssertions<bool>`)

### Equivalency configuration (`Action<EquivalencyOptions>`)
- `IgnoreMember(memberName)`
- `IgnorePath(path)`
- `OnlyCompareMember(memberPath)`
- `OnlyCompareMembers(params memberPaths)`
- `UseComparerForType<T>(comparer)`
- `UseComparerForPath(path, comparer)`
- `UseComparerForMember(memberPath, comparer)`
- `UseCollectionItemComparerForPath(path, comparer)`
- `MatchMemberName(actualMember, expectedMember)`
- `IgnoreExpectedNullMembers()`
- `IgnoreActualNullMembers()`
- `CollectionOrder`, `RequireStrictRuntimeTypes`, `StringComparison`
- `FailOnMissingMembers`, `FailOnExtraMembers`, `MaxDifferences`
- `IncludePublicProperties`, `IncludePublicFields`
- `FloatTolerance`, `DoubleTolerance`, `HalfTolerance`, `DecimalTolerance`
- `DateOnlyTolerance`, `DateTimeTolerance`, `DateTimeOffsetTolerance`, `TimeOnlyTolerance`, `TimeSpanTolerance`

### String assertions
- `NotBeNull()`
- `StartWith(expectedPrefix[, comparison])`
- `EndWith(expectedSuffix[, comparison])`
- `Contain(expectedSubstring[, comparison])`
- `NotContain(unexpectedSubstring[, comparison])`
- `HaveLength(expectedLength)`
- `BeEmpty()`
- `NotBeEmpty()`
- `BeNullOrEmpty()`
- `NotBeNullOrEmpty()`
- `BeNullOrWhiteSpace()`
- `NotBeNullOrWhiteSpace()`
- `BeEquivalentTo(expected, comparison)`
- `Match(pattern)` / `Match(pattern, timeout)`
- `NotMatch(pattern)` / `NotMatch(pattern, timeout)`

### Exception assertions
- `Throw<TException>()`
- `Throw<TException>().Thrown`
- `ThrowExactly<TException>()`
- `ThrowExactly<TException>().Thrown`
- `WithMessage(expectedMessage[, comparison])`
- `WithParamName(expectedParamName)`
- `WithInnerException<TInnerException>()`
- `NotThrow()`
- `ThrowAsync<TException>()`
- `ThrowAsync<TException>().Thrown`
- `ThrowExactlyAsync<TException>()`
- `ThrowExactlyAsync<TException>().Thrown`
- `NotThrowAsync()`
- `CompleteWithin(timeout)`
- `NotCompleteWithin(timeout)`
- Async exception/completion assertions are supported on `Func<Task>`, `Func<ValueTask>`, `Task`, `Task<T>`, `ValueTask`, and `ValueTask<T>`

### Collection assertions
- `Contain(item)`
- `ContainAll(expectedItems)`
- `ContainAny(expectedItems)`
- `NotContainAny(unexpectedItems)`
- `HaveUniqueItems()`
- `HaveUniqueItemsBy(selector)`
- `HaveUniqueItemsBy(selector, comparer)`
- `ContainExactly(expectedSequence)`
- `BeSubsetOf(expectedSuperset)`
- `BeSupersetOf(expectedSubset)`
- `HaveCount(expectedCount)`
- `BeEmpty()`
- `NotBeEmpty()`
- `ContainSingle()`
- `ContainSingle(predicate)`
- `ContainSingle().SingleItem`
- `ContainSingle(predicate).SingleItem` (typed for generic collections)
- `OnlyContain(predicate)`
- `NotContain(item)` / `NotContain(predicate)`
- `AllSatisfy(assertion)`
- `SatisfyRespectively(assertions...)`
- `ContainInOrder(expectedSequence, allowGaps = true)`
- `ContainInOrder(expectedSequence, keySelector, allowGaps = true)`
- `BeInAscendingOrder()`
- `BeInDescendingOrder()`
- `BeInAscendingOrder(keySelector[, comparer])`
- `BeInDescendingOrder(keySelector[, comparer])`

### Dictionary assertions
- `ContainKey(key)`
- `ContainKey(key).WhoseValue`
- `NotContainKey(key)`
- `ContainValue(value)`
- `NotContainValue(value)`
- `ContainEntry(key, value)`
- `NotContainEntry(key, value)`

### Temporal assertions
- `BeBefore(expected)`
- `BeAfter(expected)`
- `BeWithin(expected, tolerance)`
for `DateTime`, `DateTimeOffset`, `DateOnly`, and `TimeOnly`.

## Usage

### Batch Assertions

Use `Batch` when you want to run several related assertions and see all failures together.

Without `Batch`, the first failing assertion throws immediately and stops execution.
With `Batch`, failures are collected and one combined exception is thrown when the root batch is disposed.

`Batch` is useful for validating multiple fields in one object or multiple expectations in one scenario.

```csharp
using var batch = Assert.Batch("user profile");

user.Name.Should().StartWith("A");
user.Email.Should().EndWith("@example.com");
user.Roles.Should().Contain("admin");
```

Disposing the root batch throws one combined deterministic message:

```text
Batch 'user profile' failed with 3 assertion failure(s):
1) ...
2) ...
3) ...
```

### Fluent String Assertions

```csharp
"test".Should()
    .StartWith("te").And
    .Contain("es").And
    .HaveLength(4).And
    .NotContain("ab").And
    .EndWith("st").And
    .NotBeEmpty().And
    .NotBeNull();

"ABC".Should().StartWith("ab", StringComparison.OrdinalIgnoreCase);
```

### Value Assertions

```csharp
42.Should()
    .Be(42).And
    .BeOneOf([40, 41, 42]).And
    .NotBe(0).And
    .NotBeOneOf([100, 200]).And
    .Satisfy(x => x % 2 == 0).And
    .NotSatisfy(x => x < 0).And
    .BeGreaterThan(10).And
    .BeInRange(40, 50);

42.1d.Should().BeApproximately(42d, 0.2d);

object value = "hello";
value.Should().BeOfType<string>().And.BeAssignableTo<object>();
```

### Equivalency Assertions

```csharp
var actual = new { Name = "Bob", Scores = new[] { 3, 1, 2 } };
var expected = new { Name = "Bob", Scores = new[] { 1, 2, 3 } };

actual.Should().BeEquivalentTo(
    expected,
    options => options.CollectionOrder = EquivalencyCollectionOrder.Any);
```

### Tolerance Rules For Non-Finite Numbers

When a tolerance is configured for `float`, `double`, or `Half`, Axiom applies explicit rules for non-finite values:

- `NaN` only matches `NaN`.
- `+Infinity` only matches `+Infinity`.
- `-Infinity` only matches `-Infinity`.

This keeps equivalency behaviour predictable for edge-case numeric inputs.

For time-based tolerances (`DateOnly`, `DateTime`, `DateTimeOffset`, `TimeOnly`, `TimeSpan`), negative values are normalised to absolute duration. `TimeSpan.MinValue` is rejected and throws `ArgumentOutOfRangeException`.

### Optional Global Equivalency Defaults

You only need this if you want project-wide defaults.  
If you do nothing, Axiom uses built-in defaults.

```csharp
using Axiom.Assertions.Equivalency;

EquivalencyDefaults.Configure(options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
    options.FailOnExtraMembers = false;
});
```

Per-call options always override global defaults:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Strict;
});
```

The `options` parameter above is an `Action<EquivalencyOptions>`.

### Custom Comparer Providers

Use a custom comparer provider when your domain needs equality rules that differ from default `.Equals(...)`.

Typical cases:
- value objects that should compare case-insensitively,
- tolerance-based equality for domain numbers,
- types from external/generated code where you cannot (or should not) change the type itself.

How Axiom uses comparer providers:
- `Be(...)` and `NotBe(...)`: uses the provider for that value type.
- `BeEquivalentTo(...)`: uses the provider for non-string leaf values during graph comparison.
- Strings in `BeEquivalentTo(...)` use `EquivalencyOptions.StringComparison`.

Implementation pattern:
- Implement `IComparerProvider`.
- Return an `IEqualityComparer<T>` for handled types.
- Return `false` for unhandled types so Axiom falls back to default equality.

```csharp
using Axiom.Core.Comparison;
using Axiom.Core.Configuration;

public sealed class OrderCodeComparerProvider : IComparerProvider
{
    public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
    {
        if (typeof(T) == typeof(OrderCode))
        {
            comparer = (IEqualityComparer<T>)(object)new OrderCodeComparer();
            return true;
        }

        comparer = null;
        return false;
    }
}

public sealed class OrderCodeComparer : IEqualityComparer<OrderCode>
{
    // Domain rule: order codes are equal ignoring case.
    public bool Equals(OrderCode? x, OrderCode? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(OrderCode obj)
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
    }
}

public sealed record OrderCode(string Value);

AxiomServices.Configure(c =>
{
    c.ComparerProvider = new OrderCodeComparerProvider();
});
```

### Per-Assertion Type Comparers (Equivalency)

Use `UseComparerForType<T>(...)` when you want a comparer for one assertion call only.

This is useful when:
- one test needs a different rule than your project default,
- you want to avoid changing global comparer behaviour for other tests.

```csharp
var actual = new { Score = 3 };
var expected = new { Score = 5 };

actual.Should().BeEquivalentTo(expected, options =>
    options.UseComparerForType<int>(new OddEvenMatchIntComparer()));
```

`NotBeEquivalentTo(...)` respects the same per-call comparer:

```csharp
var actual = 3;

actual.Should().NotBeEquivalentTo(8, options =>
    options.UseComparerForType<int>(new OddEvenMatchIntComparer()));
```

For path-specific rules, use `UseComparerForPath(...)`:

```csharp
var actual = new { Name = "ABC", Age = 30 };
var expected = new { Name = "abc", Age = 30 };

actual.Should().BeEquivalentTo(expected, options =>
{
    options.StringComparison = StringComparison.Ordinal;
    options.UseComparerForPath("actual.Name", StringComparer.OrdinalIgnoreCase);
});
```

Use `UseComparerForMember(...)` when you want to target a specific member name/path and keep the intent obvious.
`UseComparerForPath(nameof(Address), comparer)` targets the whole `Address` branch.
For a specific nested member, use a string path such as `"Address.Name"`.
`UseComparerForMember(...)` behaves the same as `UseComparerForPath(...)`, but reads more clearly when the rule is about one member.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.UseComparerForMember("Name", StringComparer.OrdinalIgnoreCase));
```

For collection item rules on a specific path, use `UseCollectionItemComparerForPath(...)`:

```csharp
var actual = new Order
{
    Items =
    [
        new LineItem("A-1", 1),
        new LineItem("B-2", 2)
    ]
};

var expected = new Order
{
    Items =
    [
        new LineItem("A-1", 100),
        new LineItem("B-2", 200)
    ]
};

actual.Should().BeEquivalentTo(expected, options =>
    options.UseCollectionItemComparerForPath("actual.Items", new LineItemSkuComparer()));
```

For member name mapping between different object shapes, use `MatchMemberName(...)`:

```csharp
var actual = new { GivenName = "Ada", Age = 36 };
var expected = new { FirstName = "Ada", Age = 36 };

actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.MatchMemberName("GivenName", "FirstName");
});
```

For strings in `BeEquivalentTo(...)`, `UseComparerForType<string>(...)` is not used; configure `EquivalencyOptions.StringComparison` instead.

```csharp
object actual = "ABC";

actual.Should().BeEquivalentTo("abc", options =>
    options.StringComparison = StringComparison.OrdinalIgnoreCase);
```

Precedence for leaf value equality in `BeEquivalentTo(...)`/`NotBeEquivalentTo(...)` is:
1. Tolerance option for that leaf type (if configured).
2. Per-path comparer (`UseComparerForPath(...)`).
3. `StringComparison` (string leaves only).
4. Per-call type comparer (`UseComparerForType<T>`, non-string leaves).
5. Global comparer provider (`AxiomServices.Configure(...)`, non-string leaves).
6. Default equality.

For collection items, `UseCollectionItemComparerForPath(...)` takes precedence for the configured collection path.

### Exception Assertions

```csharp
Action strictThrow = () => throw new InvalidOperationException("boom");
var thrownSync = strictThrow.Should().ThrowExactly<InvalidOperationException>().Thrown;
thrownSync.Message.Should().Be("boom");

Action withDetails = () => throw new ArgumentNullException("value", "Value is required.");
withDetails.Should()
    .Throw<ArgumentException>()
    .WithMessage("Value is required. (Parameter 'value')")
    .WithParamName("value");

Action wrapped = () => throw new InvalidOperationException("outer", new ArgumentException("inner"));
wrapped.Should()
    .ThrowExactly<InvalidOperationException>()
    .WithInnerException<ArgumentException>();

Action noThrow = () => { };
noThrow.Should().NotThrow();

Func<Task> asyncAction = () => Task.FromException(new InvalidOperationException("boom"));
await asyncAction.Should().ThrowAsync<Exception>();
await asyncAction.Should().ThrowExactlyAsync<InvalidOperationException>();
await (await asyncAction.Should().ThrowExactlyAsync<InvalidOperationException>())
    .WithMessage("boom");
var thrownAsync = (await asyncAction.Should().ThrowExactlyAsync<InvalidOperationException>()).Thrown;
thrownAsync.Message.Should().Be("boom");

Func<Task> asyncArgumentFailure =
    () => Task.FromException(new ArgumentNullException("userId", "User id is required."));
await (await asyncArgumentFailure.Should().ThrowAsync<ArgumentException>())
    .WithParamName("userId")
    .WithMessage("User id is required. (Parameter 'userId')");

Func<Task> asyncNoThrow = () => Task.CompletedTask;
await asyncNoThrow.Should().NotThrowAsync();

var completion = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
Func<Task> slowAction = () => completion.Task;
await slowAction.Should().NotCompleteWithin(TimeSpan.FromMilliseconds(50));
completion.TrySetResult(null);

Task finishedTask = Task.CompletedTask;
await finishedTask.Should().NotThrowAsync();

Task<int> finishedResultTask = Task.FromResult(42);
await finishedResultTask.Should().CompleteWithin(TimeSpan.FromMilliseconds(50));

ValueTask failedValueTask = ValueTask.FromException(new InvalidOperationException("boom"));
await failedValueTask.Should().ThrowAsync<InvalidOperationException>();
```

### Collection Assertions

```csharp
int[] values = [1, 2, 3];
values.Should()
    .BeInAscendingOrder().And
    .Contain(2).And
    .SatisfyRespectively(
        (int item) => item.Should().BeGreaterThan(0),
        (int item) => item.Should().BeGreaterThan(1),
        (int item) => item.Should().BeGreaterThan(2)).And
    .ContainInOrder([1, 3], allowGaps: true).And
    .HaveCount(3).And
    .NotBeEmpty();

Order[] orders = [new(42, 19.99m)];
var onlyOrder = (Order)orders.Should().ContainSingle().SingleItem!;
onlyOrder.Total.Should().Be(19.99m);
Order matchedOrder = orders.Should().ContainSingle((Order order) => order.Id == 42).SingleItem;
matchedOrder.Total.Should().Be(19.99m);

Dictionary<string, int> scores = new()
{
    ["a"] = 1,
    ["b"] = 2
};

scores.Should()
    .ContainKey("a").And
    .ContainValue(2).And
    .ContainEntry("b", 2);

var scoreA = scores.Should().ContainKey("a").WhoseValue;
scoreA.Should().Be(1);

User[] users =
[
    new(1, "A@example.com"),
    new(2, "b@example.com")
];
users.Should().HaveUniqueItemsBy((User user) => user.Email, StringComparer.OrdinalIgnoreCase);
users.Should().BeInAscendingOrder((User user) => user.Email, StringComparer.OrdinalIgnoreCase);

public sealed record Order(int Id, decimal Total);
public sealed record User(int Id, string Email);
```

### Extractor Behaviour (`Thrown` / `SingleItem` / `WhoseValue`)

- If the base assertion succeeds, the extractor returns the matched value.
- If the base assertion fails outside `Batch`, it throws immediately and the extractor is not reached.
- If the base assertion fails inside `Batch`, the failure is aggregated and the extractor throws an explicit unavailable message that includes the original assertion failure text.
- `ContainSingle(predicate).SingleItem` is strongly typed for generic collections.
- Parameterless `ContainSingle().SingleItem` remains `object?` for non-generic compatibility paths.

### Temporal Assertions

```csharp
var now = DateTime.UtcNow;
var later = now.AddMinutes(2);

later.Should().BeAfter(now).And.BeWithin(now.AddMinutes(2), TimeSpan.FromSeconds(1));
```

## Installation

### Which package do I need?

- `Axiom.Assertions`: use this in test projects. It contains the fluent `Should()` API and references `Axiom.Core` automatically.
- `Axiom.Core`: use this only when you need core primitives directly (for example `Batch`), or when building custom assertion layers.

Install:

```bash
dotnet add package Axiom.Assertions
```

If you only need the core primitives:

```bash
dotnet add package Axiom.Core
```

Common namespaces when writing tests:

```csharp
using Axiom.Assertions; // Should()
using Axiom.Core; // Assert.Batch(...)
```

## Security

Security reports should be submitted privately first via GitHub vulnerability reporting.

See [SECURITY.md](SECURITY.md) for details.

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

Licensed under Apache-2.0. See [LICENSE](LICENSE).
