# Assertion Reference

This page is the complete consumer-facing catalog of the shipped assertion surface so you can browse available APIs without opening the source or relying on IDE completion. If you want package guidance or first-step examples, start with [Getting Started](getting-started.md).

All fluent assertions either:

- return `.And` so you can continue chaining on the same assertion object
- return a specialized continuation that exposes an extracted value such as `Thrown`, `SingleItem`, `WhoseValue`, or `WhoseResult`

## Entry Points

| Subject | Entry point | Returns |
| --- | --- | --- |
| Any value `T` | `subject.Should()` | `ValueAssertions<T>` |
| `string?` | `subject.Should()` | `StringAssertions` |
| `Action` | `subject.Should()` | `ActionAssertions` |
| `Func<Task>` | `subject.Should()` | `AsyncActionAssertions` |
| `Func<ValueTask>` | `subject.Should()` | `AsyncActionAssertions` |
| `Func<Task<T>>` | `subject.Should()` | `AsyncFunctionAssertions<T>` |
| `Func<ValueTask<T>>` | `subject.Should()` | `AsyncFunctionAssertions<T>` |
| `IAsyncEnumerable<T>` | `subject.Should()` | `AsyncEnumerableAssertions<T>` |
| `IAsyncEnumerable<T>` | `subject.ShouldAsyncEnumerable()` | `AsyncEnumerableAssertions<T>` |
| `Task` | `subject.Should()` | `TaskAssertions` |
| `Task<T>` | `subject.Should()` | `TaskAssertions<T>` |
| `ValueTask` | `subject.Should()` | `TaskAssertions` |
| `ValueTask<T>` | `subject.Should()` | `TaskAssertions<T>` |
| `float[]?` with `using Axiom.Vectors;` | `subject.Should()` | `VectorAssertions<float>` |
| `double[]?` with `using Axiom.Vectors;` | `subject.Should()` | `VectorAssertions<double>` |
| `ReadOnlyMemory<float>` with `using Axiom.Vectors;` | `subject.Should()` | `VectorAssertions<float>` |
| `ReadOnlyMemory<double>` with `using Axiom.Vectors;` | `subject.Should()` | `VectorAssertions<double>` |

`Task<T>` and `ValueTask<T>` expose task-behaviour assertions plus success continuations through `WhoseResult`. They do not unwrap the result directly into `ValueAssertions<T>`.

## Core Primitives

Low-level primitives live in `Axiom.Core`.

```csharp
Assert.Batch(string? name = null)
```

Use `Batch` when you want to aggregate multiple failures and throw once at the end of the scope.

## Custom Assertion Authoring

Available in `Axiom.Assertions.Authoring`.

```csharp
AssertionContext.Create(assertions)
```

`AssertionContext.Create(...)` currently supports `ValueAssertions<T>` and returns `AssertionContext<ValueAssertions<T>, T>`.

`AssertionContext<TAssertions, TSubject>` exposes:

```csharp
Assertions
Subject
SubjectLabel
And()
Fail(expectation, actual, because = null, callerFilePath = null, callerLineNumber = 0)
```

Use it when you want to build domain-specific assertions that still respect `Batch`, the configured failure strategy, and Axiom's standard message rendering.

## Value Assertions

Available on `ValueAssertions<T>` from `subject.Should()`.

```csharp
Be(expected)
NotBe(unexpected)
BeOneOf(IEnumerable<T> expectedValues)
NotBeOneOf(IEnumerable<T> unexpectedValues)
Satisfy(Func<T, bool> predicate)
NotSatisfy(Func<T, bool> predicate)
BeSameAs(T? expectedReference)
NotBeSameAs(T? unexpectedReference)
BeEquivalentTo<TExpected>(expected)
BeEquivalentTo<TExpected>(expected, Action<EquivalencyOptions> configure)
NotBeEquivalentTo<TExpected>(expected)
NotBeEquivalentTo<TExpected>(expected, Action<EquivalencyOptions> configure)
BeNull()
NotBeNull()
BeOfType<TExpected>()
BeAssignableTo<TExpected>()
NotBeAssignableTo<TExpected>()
BeGreaterThan(value)
BeGreaterThanOrEqualTo(value)
BeLessThan(value)
BeLessThanOrEqualTo(value)
BeInRange(min, max)
```

Additional extension assertions:

```csharp
// ValueAssertions<bool>
BeTrue()
BeFalse()

// ValueAssertions<double>
BeApproximately(expected, tolerance)

// ValueAssertions<float>
BeApproximately(expected, tolerance)

// ValueAssertions<decimal>
BeApproximately(expected, tolerance)
```

## String Assertions

Available on `string?.Should()` and intentionally specialized beyond generic `ValueAssertions<string?>`.

```csharp
NotBeNull()
BeNull()
Be(expected)
NotBe(unexpected)
BeEquivalentTo(expected, StringComparison comparison)
StartWith(prefix)
StartWith(prefix, StringComparison comparison)
EndWith(suffix)
EndWith(suffix, StringComparison comparison)
Contain(substring)
Contain(substring, StringComparison comparison)
NotContain(substring)
NotContain(substring, StringComparison comparison)
HaveLength(expectedLength)
BeEmpty()
NotBeEmpty()
BeNullOrEmpty()
NotBeNullOrEmpty()
BeNullOrWhiteSpace()
NotBeNullOrWhiteSpace()
Match(pattern)
Match(pattern, TimeSpan timeout)
NotMatch(pattern)
NotMatch(pattern, TimeSpan timeout)
```

## Exception Assertions

Available on `ActionAssertions` from `Action.Should()`.

```csharp
Throw<TException>()
ThrowExactly<TException>()
NotThrow()
```

The throw assertions return `ThrownExceptionAssertions<TParent, TException>`, which exposes:

```csharp
And
Exception
Thrown
WithMessage(expectedMessage, StringComparison comparison = StringComparison.Ordinal)
WithParamName(expectedParamName)
WithInnerException<TInnerException>()
```

## Async Assertions

Available on `AsyncActionAssertions` from:

- `Func<Task>`
- `Func<ValueTask>`

```csharp
ThrowAsync<TException>()
ThrowExactlyAsync<TException>()
NotThrowAsync()
CompleteWithin(timeout)
NotCompleteWithin(timeout)
```

`ThrowAsync(...)` and `ThrowExactlyAsync(...)` return the same `ThrownExceptionAssertions<AsyncActionAssertions, TException>` continuation surface documented above.

## Async Function Result Assertions

Available on `AsyncFunctionAssertions<TResult>` from:

- `Func<Task<T>>`
- `Func<ValueTask<T>>`

```csharp
ThrowAsync<TException>()
ThrowExactlyAsync<TException>()
NotThrowAsync()
CompleteWithin(timeout)
NotCompleteWithin(timeout)
Succeed()
SucceedWithin(timeout)
BeCanceled()
BeCanceledWithin(timeout)
BeFaultedWith<TException>()
BeFaultedWithWithin<TException>(timeout)
```

Result continuations:

```csharp
// Succeed() / SucceedWithin(timeout)
And
WhoseResult
```

`ThrowAsync(...)`, `ThrowExactlyAsync(...)`, `BeFaultedWith(...)`, and `BeFaultedWithWithin(...)` return the same `ThrownExceptionAssertions<TParent, TException>` continuation surface documented in the exception section above.

## Async Stream Assertions

Available on `AsyncEnumerableAssertions<T>` from `IAsyncEnumerable<T>.Should()`.

If a concrete wrapper type implements `IAsyncEnumerable<T>` and `.Should()` resolves to the generic `ValueAssertions<T>` entry point, use `.ShouldAsyncEnumerable()` to force async-stream assertions.

```csharp
BeEmptyAsync()
NotBeEmptyAsync()
HaveCountAsync(expectedCount)
ContainAsync(expected)
ContainAsync(predicate)
ContainExactlyAsync(expectedSequence)
ContainAnyAsync(expectedItems)
NotContainAnyAsync(unexpectedItems)
OnlyContainAsync(predicate)
ContainSingleAsync()
ContainSingleAsync(predicate)
SatisfyRespectivelyAsync(assertionsForItems)
HaveUniqueItemsAsync()
HaveUniqueItemsByAsync(keySelector)
HaveUniqueItemsByAsync(keySelector, comparer)
ContainInOrderAsync(expectedSequence, allowGaps = true)
ContainInOrderAsync(expectedSequence, keySelector, allowGaps = true)
```

`ContainSingleAsync()` and `ContainSingleAsync(predicate)` return:

```csharp
And
SingleItem
```

Use them when you want to assert an async stream directly instead of materializing it into a list first.

`ContainExactlyAsync(...)` is the exact ordered async-stream assertion. The stream must match the expected sequence item-for-item in the same order, with no missing items and no extras.

`ContainAnyAsync(...)` passes when the async stream contains at least one of the expected items. `NotContainAnyAsync(...)` passes when the stream contains none of the unexpected items.

```csharp
await stepIds.Should().ContainExactlyAsync([1, 2, 3]);
await stepIds.Should().ContainAnyAsync([2, 9]);
await stepIds.Should().NotContainAnyAsync([8, 9]);
```

`SatisfyRespectivelyAsync(...)` is the ordered async-stream workflow assertion. It applies each assertion action to the corresponding item, fails when the stream is too short or too long, and reports the first failing item index when an item assertion fails.

```csharp
await orders.Should().SatisfyRespectivelyAsync(
    first => first.Total.Should().Be(10m),
    second => second.Total.Should().Be(20m));
```

`HaveUniqueItemsAsync()` and `HaveUniqueItemsByAsync(...)` let you assert direct async-stream uniqueness without materializing the sequence first.

```csharp
await users.Should().HaveUniqueItemsAsync();
await users.Should().HaveUniqueItemsByAsync(user => user.Id);
await users.Should().HaveUniqueItemsByAsync(user => user.Email, StringComparer.OrdinalIgnoreCase);
```

`ContainInOrderAsync(...)` mirrors the synchronous ordered-sequence assertions for async streams. With the default `allowGaps: true`, the expected sequence must appear in order as a subsequence. With `allowGaps: false`, the expected sequence must appear as an adjacent ordered run.

```csharp
await statuses.Should().ContainInOrderAsync([WorkflowStep.Started, WorkflowStep.Completed]);
await events.Should().ContainInOrderAsync(
    [WorkflowStep.Started, WorkflowStep.Completed],
    evt => evt.Step);
await stepIds.Should().ContainInOrderAsync([2, 3], allowGaps: false);
```

## Vector Assertions

Available from the optional `Axiom.Vectors` package.

```bash
dotnet add package Axiom.Vectors
```

Entry points live in `Axiom.Vectors.ShouldExtensions` and currently support:

- `float[]`
- `double[]`
- `ReadOnlyMemory<float>`
- `ReadOnlyMemory<double>`

`subject.Should()` returns `VectorAssertions<TNumeric>`, which exposes:

```csharp
HaveDimension(expectedDimension)
NotContainNaNOrInfinity()
BeApproximatelyEqualTo(expected, tolerance)
HaveCosineSimilarityWith(expected)
HaveCosineSimilarityTo(expected)
BeNormalized()
BeNormalized(tolerance)
```

`HaveCosineSimilarityWith(expected)` returns `CosineSimilarityAssertions<TNumeric>`, which exposes:

```csharp
ActualSimilarity
AtLeast(threshold)
AtMost(threshold)
Between(minimumThreshold, maximumThreshold)
```

`HaveCosineSimilarityTo(expected)` is retained as a compatibility alias, but `HaveCosineSimilarityWith(expected)` is the preferred form for new code.

Typical usage:

```csharp
using Axiom.Vectors;

embedding.Should().HaveDimension(1536);
embedding.Should().NotContainNaNOrInfinity();
embedding.Should().BeApproximatelyEqualTo(expected, tolerance: 1e-5f);
embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.995f).And.BeNormalized();
embedding.Should().HaveCosineSimilarityWith(unrelated).AtMost(0.2f);
embedding.Should().HaveCosineSimilarityWith(expected).Between(0.98f, 0.999f);
embedding.Should().BeNormalized(tolerance: 1e-5f);
```

## Direct Task Assertions

Available on `TaskAssertions` / `TaskAssertions<T>` from:

- `Task`
- `Task<T>`
- `ValueTask`
- `ValueTask<T>`

Shared task assertion surface:

```csharp
ThrowAsync<TException>()
ThrowExactlyAsync<TException>()
NotThrowAsync()
CompleteWithin(timeout)
NotCompleteWithin(timeout)
Succeed()
SucceedWithin(timeout)
BeCanceled()
BeCanceledWithin(timeout)
BeFaultedWith<TException>()
BeFaultedWithWithin<TException>(timeout)
```

Task result continuations:

```csharp
// Succeed() / SucceedWithin(timeout) on Task<T> / ValueTask<T>
And
WhoseResult
```

`BeFaultedWith(...)` and `BeFaultedWithWithin(...)` return the same `ThrownExceptionAssertions<TParent, TException>` continuation surface documented in the exception section above.

## Collection Assertions

Available as extension methods on `ValueAssertions<TCollection>` where `TCollection` is enumerable.

```csharp
Contain(item)
ContainAll(IEnumerable<TItem> expectedItems)
ContainAll(params TItem[] expectedItems)
ContainAny(IEnumerable<TItem> expectedItems)
ContainAny(params TItem[] expectedItems)
NotContainAny(IEnumerable<TItem> unexpectedItems)
NotContainAny(params TItem[] unexpectedItems)
HaveUniqueItems()
HaveUniqueItemsBy(keySelector)
HaveUniqueItemsBy(keySelector, comparer)
ContainExactly(IEnumerable<TItem> expectedSequence)
BeSubsetOf(IEnumerable<TItem> expectedSuperset)
BeSupersetOf(IEnumerable<TItem> expectedSubset)
HaveCount(expectedCount)
BeEmpty()
NotBeEmpty()
ContainSingle()
ContainSingle(predicate)
OnlyContain(predicate)
NotContain(predicate)
NotContain(item)
AllSatisfy(assertion)
SatisfyRespectively(params Action<TItem>[] assertionsForItems)
SatisfyRespectively(string? because, params Action<TItem>[] assertionsForItems)
ContainInOrder(IEnumerable<TItem> expectedSequence, bool allowGaps = true)
ContainInOrder(IEnumerable<TKey> expectedSequence, Func<TItem, TKey> keySelector, bool allowGaps = true)
BeInAscendingOrder()
BeInDescendingOrder()
BeInAscendingOrder(keySelector)
BeInAscendingOrder(keySelector, comparer)
BeInDescendingOrder(keySelector)
BeInDescendingOrder(keySelector, comparer)
```

Collection continuations:

```csharp
// ContainSingle()
And
SingleItem

// ContainSingle(predicate)
And
SingleItem
```

The predicate overload gives you a strongly typed `SingleItem`. The parameterless overload is also strongly typed for common generic collection subjects such as `List<T>`, arrays, and interface-typed generic enumerables. Nongeneric collections still expose `object? SingleItem`.

## Dictionary Assertions

Available on `ValueAssertions<TDictionary>` where the subject implements `IReadOnlyDictionary<TKey, TValue>`, with convenience overloads for `Dictionary<TKey, TValue>` and `IReadOnlyDictionary<TKey, TValue>`.

```csharp
ContainKey(key)
NotContainKey(key)
ContainValue(value)
NotContainValue(value)
ContainEntry(key, value)
NotContainEntry(key, value)
```

`ContainKey(...)` returns a continuation with:

```csharp
And
WhoseValue
```

## Temporal Assertions

Available on:

- `ValueAssertions<DateTime>`
- `ValueAssertions<DateTimeOffset>`
- `ValueAssertions<DateOnly>`
- `ValueAssertions<TimeOnly>`

```csharp
BeBefore(expected)
BeAfter(expected)
BeWithin(expected, tolerance)
```

## Equivalency Options

Available inside `BeEquivalentTo(..., options => { ... })` or via `EquivalencyDefaults.Configure(...)`.

For behavior, precedence, and common configuration patterns, see [equivalency.md](equivalency.md).

Configuration methods:

```csharp
IgnoreMember(memberName)
IgnorePath(path)
Ignore<TSubject>(x => x.Member)
Ignore<TSubject>(x => x.MemberA, x => x.MemberB)
OnlyCompareMember(memberPath)
OnlyCompareMembers(params string[] memberPaths)
OnlyCompare<TSubject>(x => x.Member)
OnlyCompare<TSubject>(x => x.MemberA, x => x.MemberB)
UseComparerForType<T>(comparer)
UseComparerForPath(path, comparer)
UseComparerForMember(memberPath, comparer)
UseComparer<TSubject>(x => x.Member, comparer)
UseCollectionItemComparerForPath(path, comparer)
UseCollectionItemComparer<TSubject>(x => x.Collection, comparer)
MatchMember<TActual, TExpected>(actualSelector, expectedSelector)
MatchMemberName(actualMember, expectedMember)
IgnoreExpectedNullMembers()
IgnoreActualNullMembers()
```

Key options:

- `CollectionOrder`
- `RequireStrictRuntimeTypes`
- `MaxDifferences`
- `StringComparison`
- `IncludePublicProperties`
- `IncludePublicFields`
- `FailOnMissingMembers`
- `FailOnExtraMembers`
- `FloatTolerance`
- `DoubleTolerance`
- `HalfTolerance`
- `DecimalTolerance`
- `DateOnlyTolerance`
- `DateTimeTolerance`
- `DateTimeOffsetTolerance`
- `TimeOnlyTolerance`
- `TimeSpanTolerance`

For cross-type renames:

- Prefer `MatchMember<TActual, TExpected>(...)` when you can point at members with expressions.
- Use `MatchMemberName(...)` when you only have string member names or are working from dynamic/string-based configuration.
- If both are configured for the same member, typed `MatchMember(...)` wins.

## Configuration And Extensibility

`Axiom.Assertions` works without startup code. By default, Axiom auto-detects xUnit, NUnit, and MSTest at runtime and uses the matching framework-native assertion exception type. If no supported framework is detected, it falls back to `InvalidOperationException`.

Add shared setup only when you want project-wide defaults:

```csharp
// AxiomSetup.cs
using System;
using Axiom.Assertions;
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

Call `AxiomSetup.Apply()` once from your framework startup hook when you want those shared defaults (xUnit fixture, NUnit one-time setup, or MSTest assembly initialise).

You only need setup/configuration when you want custom defaults such as:

- equivalency defaults
- custom comparer provider
- custom value formatter
- custom regex timeout
- explicit failure-strategy override
- shared reusable modules

Project-wide configuration:

```csharp
AxiomSettings.Configure(Action<AxiomSettingsOptions> configure)
AxiomSettings.Reset()
AxiomSettings.UseModule(IAxiomSettingsModule module)
AxiomSettings.UseModules(params IAxiomSettingsModule[] modules)
AxiomSettings.UseModule(IAxiomModule module)

EquivalencyDefaults.Configure(Action<EquivalencyOptions> configure)
EquivalencyDefaults.Reset()

AxiomServices.Configure(Action<AxiomConfiguration> configure)
AxiomServices.Reset()
AxiomServices.UseModule(IAxiomModule module)
```

For shared defaults, prefer `AxiomSettings.Configure(...)`. `AxiomServices.Configure(...)` and `EquivalencyDefaults.Configure(...)` remain available for lower-level or isolated configuration.

Low-level configuration surface:

```csharp
// AxiomConfiguration
ComparerProvider
ValueFormatter
RegexMatchTimeout

// Extensibility interfaces
IAxiomSettingsModule
IAxiomModule
IComparerProvider
IValueFormatter
```

`IAxiomSettingsModule` exposes:

```csharp
Configure(AxiomSettingsOptions options)
```

Use it when you want one reusable preset that can configure both `options.Core` and `options.Equivalency`.
