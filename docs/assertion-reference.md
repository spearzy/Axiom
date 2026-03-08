# Assertion Reference

Axiom's [README](../README.md) focuses on adoption and common workflows. This page is the complete consumer-facing catalog of the current `Axiom.Assertions` surface so you can browse available assertions without opening the source or relying on IDE completion.

All fluent assertions either:

- return `.And` so you can continue chaining on the same assertion object
- return a specialized continuation that exposes an extracted value such as `Thrown`, `SingleItem`, or `WhoseValue`

## Entry Points

| Subject | Entry point | Returns |
| --- | --- | --- |
| Any value `T` | `subject.Should()` | `ValueAssertions<T>` |
| `string?` | `subject.Should()` | `StringAssertions` |
| `Action` | `subject.Should()` | `ActionAssertions` |
| `Func<Task>` | `subject.Should()` | `AsyncActionAssertions` |
| `Func<ValueTask>` | `subject.Should()` | `AsyncActionAssertions` |
| `Task` | `subject.Should()` | `AsyncActionAssertions` |
| `Task<T>` | `subject.Should()` | `AsyncActionAssertions` |
| `ValueTask` | `subject.Should()` | `AsyncActionAssertions` |
| `ValueTask<T>` | `subject.Should()` | `AsyncActionAssertions` |

`Task<T>` and `ValueTask<T>` currently expose task-behavior assertions such as throws and completion timing. They do not yet unwrap the result into `ValueAssertions<T>`.

## Core Primitives

Low-level primitives live in `Axiom.Core`.

```csharp
Assert.Batch(string? name = null)
```

Use `Batch` when you want to aggregate multiple failures and throw once at the end of the scope.

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
- `Task`
- `Task<T>`
- `ValueTask`
- `ValueTask<T>`

```csharp
ThrowAsync<TException>()
ThrowExactlyAsync<TException>()
NotThrowAsync()
CompleteWithin(timeout)
NotCompleteWithin(timeout)
```

`ThrowAsync(...)` and `ThrowExactlyAsync(...)` return the same `ThrownExceptionAssertions<AsyncActionAssertions, TException>` continuation surface documented above.

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

The predicate overload gives you a strongly typed `SingleItem`. The parameterless overload currently exposes `object? SingleItem`.

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
OnlyCompareMember(memberPath)
OnlyCompareMembers(params string[] memberPaths)
UseComparerForType<T>(comparer)
UseComparerForPath(path, comparer)
UseComparerForMember(memberPath, comparer)
UseCollectionItemComparerForPath(path, comparer)
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

## Configuration And Extensibility

Project-wide configuration:

```csharp
EquivalencyDefaults.Configure(Action<EquivalencyOptions> configure)
EquivalencyDefaults.Reset()

AxiomServices.Configure(Action<AxiomConfiguration> configure)
AxiomServices.Reset()
AxiomServices.UseModule(IAxiomModule module)
```

Low-level configuration surface:

```csharp
// AxiomConfiguration
ComparerProvider
ValueFormatter
RegexMatchTimeout

// Extensibility interfaces
IAxiomModule
IComparerProvider
IValueFormatter
```
