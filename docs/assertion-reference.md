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
Be(expected, comparer)
NotBe(unexpected)
NotBe(unexpected, comparer)
BeOneOf(IEnumerable<T> expectedValues)
BeOneOf(IEnumerable<T> expectedValues, comparer)
NotBeOneOf(IEnumerable<T> unexpectedValues)
NotBeOneOf(IEnumerable<T> unexpectedValues, comparer)
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
BeGreaterThan(value, comparer)
BeGreaterThanOrEqualTo(value)
BeGreaterThanOrEqualTo(value, comparer)
BeLessThan(value)
BeLessThan(value, comparer)
BeLessThanOrEqualTo(value)
BeLessThanOrEqualTo(value, comparer)
BeInRange(min, max)
BeInRange(min, max, comparer)
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

The comparer overloads on `Be(...)`, `NotBe(...)`, `BeOneOf(...)`, and `NotBeOneOf(...)` let you apply local equality rules to a single value assertion without changing the shared comparer-provider configuration.

```csharp
var errorCodeComparer = EqualityComparer<ApiResponse>.Create(
    static (left, right) => string.Equals(left?.ErrorCode, right?.ErrorCode, StringComparison.OrdinalIgnoreCase),
    static response => StringComparer.OrdinalIgnoreCase.GetHashCode(response.ErrorCode ?? string.Empty));

failedResponse.Should().Be(new ApiResponse { ErrorCode = "order_not_found" }, errorCodeComparer);
failedResponse.Should().NotBe(new ApiResponse { ErrorCode = "rate_limited" }, errorCodeComparer);
failedResponse.Should().BeOneOf(
    [new ApiResponse { ErrorCode = "timeout" }, new ApiResponse { ErrorCode = "order_not_found" }],
    errorCodeComparer);
failedResponse.Should().NotBeOneOf(
    [new ApiResponse { ErrorCode = "duplicate" }, new ApiResponse { ErrorCode = "unauthorized" }],
    errorCodeComparer);
```

The ordered value comparer overloads let you apply local ordering rules to one assertion when the default runtime ordering is not the rule you want to use.

```csharp
var descendingPriority = Comparer<int>.Create(static (left, right) => right.CompareTo(left));

var priority = 3;
priority.Should().BeGreaterThan(5, descendingPriority);
priority.Should().BeGreaterThanOrEqualTo(3, descendingPriority);
priority.Should().BeLessThan(1, descendingPriority);
priority.Should().BeLessThanOrEqualTo(1, descendingPriority);
priority.Should().BeInRange(5, 1, descendingPriority);
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
ContainAsync(expected, comparer)
ContainAsync(predicate)
ContainExactlyAsync(expectedSequence)
ContainExactlyAsync(expectedSequence, comparer)
ContainExactlyInAnyOrderAsync(expectedSequence)
ContainExactlyInAnyOrderAsync(expectedSequence, comparer)
ContainAllAsync(expectedItems)
ContainAllAsync(expectedItems, comparer)
ContainAnyAsync(expectedItems)
ContainAnyAsync(expectedItems, comparer)
NotContainAsync(unexpected)
NotContainAsync(unexpected, comparer)
NotContainAnyAsync(unexpectedItems)
NotContainAnyAsync(unexpectedItems, comparer)
BeSubsetOfAsync(expectedSuperset)
BeSubsetOfAsync(expectedSuperset, comparer)
BeSupersetOfAsync(expectedSubset)
BeSupersetOfAsync(expectedSubset, comparer)
OnlyContainAsync(predicate)
ContainSingleAsync()
ContainSingleAsync(predicate)
SatisfyRespectivelyAsync(assertionsForItems)
HaveUniqueItemsAsync()
HaveUniqueItemsAsync(comparer)
HaveUniqueItemsByAsync(keySelector)
HaveUniqueItemsByAsync(keySelector, comparer)
BeInAscendingOrderAsync()
BeInAscendingOrderAsync(comparer)
BeInDescendingOrderAsync()
BeInDescendingOrderAsync(comparer)
ContainInOrderAsync(expectedSequence, allowGaps = true)
ContainInOrderAsync(expectedSequence, comparer, allowGaps = true)
ContainInOrderAsync(expectedSequence, keySelector, allowGaps = true)
ContainInOrderAsync(expectedSequence, keySelector, comparer, allowGaps = true)
```

`ContainSingleAsync()` and `ContainSingleAsync(predicate)` return:

```csharp
And
SingleItem
```

Use them when you want to assert an async stream directly instead of materializing it into a list first.

`ContainExactlyAsync(...)` is the exact ordered async-stream assertion. The stream must match the expected sequence item-for-item in the same order, with no missing items and no extras. The comparer overload lets you keep that same exactness while opting into local equality for just this assertion.

`ContainExactlyInAnyOrderAsync(...)` is the exact unordered async-stream assertion. The stream must contain the same items with the same counts, but order does not matter. The comparer overload lets you opt into custom equality without adding any extra cost to the default path.

`ContainAllAsync(...)` checks that every expected item is present somewhere in the stream. `ContainAnyAsync(...)` passes when the async stream contains at least one of the expected items. `NotContainAsync(...)` passes when a specific item never appears. `NotContainAnyAsync(...)` passes when the stream contains none of the unexpected items. `ContainAsync(...)`, `ContainExactlyAsync(...)`, `ContainAllAsync(...)`, `ContainAnyAsync(...)`, `NotContainAsync(...)`, `NotContainAnyAsync(...)`, `BeSubsetOfAsync(...)`, and `BeSupersetOfAsync(...)` all have comparer overloads when you need local non-default equality.

`BeSubsetOfAsync(...)` and `BeSupersetOfAsync(...)` mirror the synchronous collection subset/superset assertions for async streams.

```csharp
await stepIds.Should().ContainExactlyAsync([1, 2, 3]);
await eventNames.Should().ContainExactlyAsync(["created", "queued"], StringComparer.OrdinalIgnoreCase);
await stepIds.Should().ContainExactlyInAnyOrderAsync([3, 1, 2]);
await eventNames.Should().ContainExactlyInAnyOrderAsync(["created", "queued"], StringComparer.OrdinalIgnoreCase);
await stepIds.Should().ContainAllAsync([1, 3]);
await stepIds.Should().ContainAnyAsync([2, 9]);
await eventNames.Should().ContainAnyAsync(["FAILED", "QUEUED"], StringComparer.OrdinalIgnoreCase);
await eventNames.Should().ContainAsync("QUEUED", StringComparer.OrdinalIgnoreCase);
await stepIds.Should().NotContainAsync(9);
await eventNames.Should().NotContainAsync("failed", StringComparer.OrdinalIgnoreCase);
await stepIds.Should().NotContainAnyAsync([8, 9]);
await eventNames.Should().NotContainAnyAsync(["FAILED", "CANCELED"], StringComparer.OrdinalIgnoreCase);
await stepIds.Should().BeSubsetOfAsync([1, 2, 3, 4]);
await stepIds.Should().BeSupersetOfAsync([1, 3]);
```

`SatisfyRespectivelyAsync(...)` is the ordered async-stream workflow assertion. It applies each assertion action to the corresponding item, fails when the stream is too short or too long, and reports the first failing item index when an item assertion fails.

```csharp
await orders.Should().SatisfyRespectivelyAsync(
    first => first.Total.Should().Be(10m),
    second => second.Total.Should().Be(20m));
```

`HaveUniqueItemsAsync()` and `HaveUniqueItemsByAsync(...)` let you assert direct async-stream uniqueness without materializing the sequence first. `HaveUniqueItemsAsync(comparer)` is useful when uniqueness should use local equality rather than the configured comparer provider or the default comparer.

```csharp
await users.Should().HaveUniqueItemsAsync();
await labels.Should().HaveUniqueItemsAsync(StringComparer.OrdinalIgnoreCase);
await users.Should().HaveUniqueItemsByAsync(user => user.Id);
await users.Should().HaveUniqueItemsByAsync(user => user.Email, StringComparer.OrdinalIgnoreCase);
```

`ContainInOrderAsync(...)` mirrors the synchronous ordered-sequence assertions for async streams. With the default `allowGaps: true`, the expected sequence must appear in order as a subsequence. With `allowGaps: false`, the expected sequence must appear as an adjacent ordered run. Both the direct and key-selector forms also have comparer overloads when ordered matching should use local equality.

`BeInAscendingOrderAsync(...)` and `BeInDescendingOrderAsync(...)` check the stream order directly and report the first out-of-order pair. They also have comparer overloads when the stream's natural ordering is not the one you want to assert.

```csharp
await stepIds.Should().BeInAscendingOrderAsync();
await descendingStepIds.Should().BeInDescendingOrderAsync();
await labels.Should().BeInAscendingOrderAsync(StringComparer.OrdinalIgnoreCase);
await statuses.Should().ContainInOrderAsync([WorkflowStep.Started, WorkflowStep.Completed]);
await eventNames.Should().ContainInOrderAsync(["created", "queued"], StringComparer.OrdinalIgnoreCase);
await events.Should().ContainInOrderAsync(
    [WorkflowStep.Started, WorkflowStep.Completed],
    evt => evt.Step);
await events.Should().ContainInOrderAsync(
    ["started", "completed"],
    evt => evt.Step.ToString(),
    StringComparer.OrdinalIgnoreCase);
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
- ranking assertions on ordered result collections via `subject.Should()` with `using Axiom.Assertions;` and `using Axiom.Vectors;`

`subject.Should()` returns `VectorAssertions<TNumeric>`, which exposes:

```csharp
HaveDimension(expectedDimension)
NotContainNaNOrInfinity()
BeApproximatelyEqualTo(expected, tolerance)
HaveDotProductWith(expected, expectedDotProduct, tolerance)
HaveEuclideanDistanceTo(expected, expectedDistance, tolerance)
HaveCosineSimilarityWith(expected)
HaveCosineSimilarityTo(expected)
BeNormalized()
BeNormalized(tolerance)
BeZeroVector()
NotBeZeroVector()
```

When you reference `Axiom.Vectors`, the usual `subject.Should()` entry point from `Axiom.Assertions` also gains ranking extensions for ordered result collections:

```csharp
ContainInTopK(target, k)
HaveRank(target, expectedRank)
HaveRecallAt(k, relevantItems, expectedRecall, tolerance)
HavePrecisionAt(k, relevantItems, expectedPrecision, tolerance)
HaveReciprocalRank(target, expectedReciprocalRank, tolerance)
```

For query-set aggregates on `IEnumerable<RankingQuery<T>>`, the same `subject.Should()` entry point exposes:

```csharp
HaveMeanReciprocalRank(expectedMeanReciprocalRank, tolerance)
HaveHitRateAt(k, expectedHitRate, tolerance)
```

These ranking metric assertions default to exact comparison with `tolerance: 0`.
Omitting `tolerance` means the computed metric must match exactly.
Each `RankingQuery<T>` must contain at least one relevant item.

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
using Axiom.Assertions;
using Axiom.Vectors;

embedding.Should().HaveDimension(1536);
embedding.Should().NotContainNaNOrInfinity();
embedding.Should().BeApproximatelyEqualTo(expected, tolerance: 0.001f);
embedding.Should().HaveDotProductWith(expected, expectedDotProduct: 1f, tolerance: 0.001f);
embedding.Should().HaveEuclideanDistanceTo(unrelated, expectedDistance: 1.4142f, tolerance: 0.001f);
embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.995f).And.BeNormalized();
embedding.Should().HaveCosineSimilarityWith(unrelated).AtMost(0.2f);
embedding.Should().HaveCosineSimilarityWith(expected).Between(0.98f, 0.999f);
embedding.Should().BeNormalized(tolerance: 0.001f);
new float[] { 0f, 0f }.Should().BeZeroVector();
embedding.Should().NotBeZeroVector();

results.Should().ContainInTopK("doc-7", 2);
results.Should().HaveRank("doc-7", 2);
results.Should().HaveRecallAt(2, relevantItems, expectedRecall: 0.5);
results.Should().HavePrecisionAt(2, relevantItems, expectedPrecision: 0.5);
results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 0.5);

queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 0.75);
queries.Should().HaveHitRateAt(k: 1, expectedHitRate: 0.5);
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
Contain(item, comparer)
ContainAll(IEnumerable<TItem> expectedItems)
ContainAll(params TItem[] expectedItems)
ContainAll(IEnumerable<TItem> expectedItems, comparer)
ContainAny(IEnumerable<TItem> expectedItems)
ContainAny(params TItem[] expectedItems)
ContainAny(IEnumerable<TItem> expectedItems, comparer)
NotContainAny(IEnumerable<TItem> unexpectedItems)
NotContainAny(params TItem[] unexpectedItems)
NotContainAny(IEnumerable<TItem> unexpectedItems, comparer)
HaveUniqueItems()
HaveUniqueItems(comparer)
HaveUniqueItemsBy(keySelector)
HaveUniqueItemsBy(keySelector, comparer)
ContainExactly(IEnumerable<TItem> expectedSequence)
ContainExactly(IEnumerable<TItem> expectedSequence, comparer)
ContainExactlyInAnyOrder(IEnumerable<TItem> expectedSequence)
ContainExactlyInAnyOrder(IEnumerable<TItem> expectedSequence, comparer)
BeSubsetOf(IEnumerable<TItem> expectedSuperset)
BeSubsetOf(IEnumerable<TItem> expectedSuperset, comparer)
BeSupersetOf(IEnumerable<TItem> expectedSubset)
BeSupersetOf(IEnumerable<TItem> expectedSubset, comparer)
HaveCount(expectedCount)
BeEmpty()
NotBeEmpty()
ContainSingle()
ContainSingle(predicate)
OnlyContain(predicate)
NotContain(predicate)
NotContain(item)
NotContain(item, comparer)
AllSatisfy(assertion)
SatisfyRespectively(params Action<TItem>[] assertionsForItems)
SatisfyRespectively(string? because, params Action<TItem>[] assertionsForItems)
ContainInOrder(IEnumerable<TItem> expectedSequence, bool allowGaps = true)
ContainInOrder(IEnumerable<TItem> expectedSequence, comparer, bool allowGaps = true)
ContainInOrder(IEnumerable<TKey> expectedSequence, Func<TItem, TKey> keySelector, bool allowGaps = true)
ContainInOrder(IEnumerable<TKey> expectedSequence, Func<TItem, TKey> keySelector, comparer, bool allowGaps = true)
BeInAscendingOrder()
BeInAscendingOrder(comparer)
BeInDescendingOrder()
BeInDescendingOrder(comparer)
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

Local comparer overloads are available on the collection assertions whose semantics depend on item equality or ordering. Use them when you want case-insensitive or domain-specific matching for a single assertion without changing the shared comparer-provider configuration.

`ContainExactly(...)` is the ordered exact-sequence assertion. `ContainExactlyInAnyOrder(...)` is the unordered exact-membership assertion: same items, same counts, no extras, and no omissions, with an opt-in comparer overload when you need custom equality.

```csharp
int[] ids = [3, 1, 2];
ids.Should().ContainExactlyInAnyOrder([1, 2, 3]);
ids.Should().Contain(2);

string[] labels = ["Alpha", "beta", "BETA"];
labels.Should().Contain("alpha", StringComparer.OrdinalIgnoreCase);
labels.Should().ContainExactlyInAnyOrder(
    ["alpha", "beta", "beta"],
    StringComparer.OrdinalIgnoreCase);

labels.Should().ContainAll(["alpha", "beta"], StringComparer.OrdinalIgnoreCase);
labels.Should().ContainAny(["queued", "beta"], StringComparer.OrdinalIgnoreCase);
labels.Should().NotContain("archived", StringComparer.OrdinalIgnoreCase);
labels.Should().NotContainAny(["archived", "deleted"], StringComparer.OrdinalIgnoreCase);
labels.Should().HaveUniqueItems(StringComparer.OrdinalIgnoreCase);
labels.Should().BeSubsetOf(["alpha", "beta", "gamma"], StringComparer.OrdinalIgnoreCase);
labels.Should().BeSupersetOf(["alpha", "beta"], StringComparer.OrdinalIgnoreCase);
labels.Should().BeInAscendingOrder(StringComparer.OrdinalIgnoreCase);
```

## Dictionary Assertions

Available on `ValueAssertions<TDictionary>` where the subject implements `IReadOnlyDictionary<TKey, TValue>`, with convenience overloads for `Dictionary<TKey, TValue>` and `IReadOnlyDictionary<TKey, TValue>`.

```csharp
ContainKey(key)
NotContainKey(key)
ContainValue(value)
ContainValue(value, comparer)
NotContainValue(value)
NotContainValue(value, comparer)
ContainEntry(key, value)
ContainEntry(key, value, comparer)
NotContainEntry(key, value)
NotContainEntry(key, value, comparer)
```

`ContainKey(...)` returns a continuation with:

```csharp
And
WhoseValue
```

```csharp
lookup.Should().ContainValue("queued", StringComparer.OrdinalIgnoreCase);
lookup.Should().NotContainValue("failed", StringComparer.OrdinalIgnoreCase);
lookup.Should().ContainEntry(2, "queued", StringComparer.OrdinalIgnoreCase);
lookup.Should().NotContainEntry(2, "failed", StringComparer.OrdinalIgnoreCase);
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
BeOnOrBefore(expected)
BeOnOrAfter(expected)
BeWithin(expected, tolerance)
NotBeWithin(expected, tolerance)
BeBetween(min, max)
```

`BeOnOrBefore(...)` and `BeOnOrAfter(...)` are inclusive. `BeWithin(...)` and `NotBeWithin(...)` use the temporal type's native distance semantics, including wrap-midnight behavior for `TimeOnly`.

```csharp
invoice.DueDate.Should().BeOnOrBefore(today);
invoice.DueDate.Should().BeBetween(today.AddDays(-40), today);

var publishedAt = new DateTimeOffset(2026, 4, 2, 9, 30, 0, TimeSpan.Zero);
publishedAt.Should().BeOnOrAfter(new DateTimeOffset(2026, 4, 2, 9, 0, 0, TimeSpan.Zero));

var reminderAt = new TimeOnly(23, 59);
reminderAt.Should().NotBeWithin(new TimeOnly(12, 0), TimeSpan.FromHours(1));
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
- Both mapping styles are one-to-one. Repeating the same mapping is fine, but conflicting remaps fail fast.
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
