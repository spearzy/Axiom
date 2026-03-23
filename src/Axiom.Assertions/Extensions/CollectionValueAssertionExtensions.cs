using System.Collections;
using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;

namespace Axiom.Assertions.Extensions;

public static class CollectionValueAssertionExtensions
{
    public static AndContinuation<ValueAssertions<TCollection>> Contain<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        TItem expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        // Delegate to shared engine so the fluent API stays on ValueAssertions without extra object creation.
        CollectionAssertionEngine.AssertContain(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainAll<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedItems,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedItems);

        CollectionAssertionEngine.AssertContainAll(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedItems,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainAll<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        params TItem[] expectedItems)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedItems);

        // Keep the convenience overload allocation-free by forwarding the array directly.
        CollectionAssertionEngine.AssertContainAll(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedItems,
            because: null,
            callerFilePath: null,
            callerLineNumber: 0);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainAny<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedItems,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedItems);

        CollectionAssertionEngine.AssertContainAny(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedItems,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainAny<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        params TItem[] expectedItems)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedItems);

        CollectionAssertionEngine.AssertContainAny(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedItems,
            because: null,
            callerFilePath: null,
            callerLineNumber: 0);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> NotContainAny<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> unexpectedItems,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(unexpectedItems);

        CollectionAssertionEngine.AssertNotContainAny(
            assertions.Subject,
            assertions.SubjectExpression,
            unexpectedItems,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> NotContainAny<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        params TItem[] unexpectedItems)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(unexpectedItems);

        CollectionAssertionEngine.AssertNotContainAny(
            assertions.Subject,
            assertions.SubjectExpression,
            unexpectedItems,
            because: null,
            callerFilePath: null,
            callerLineNumber: 0);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveUniqueItems<TCollection>(
        this ValueAssertions<TCollection> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertHaveUniqueItems(
            assertions.Subject,
            assertions.SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveUniqueItemsBy<TCollection, TItem, TKey>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, TKey> keySelector,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(keySelector);

        CollectionAssertionEngine.AssertHaveUniqueItemsBy(
            assertions.Subject,
            assertions.SubjectExpression,
            keySelector,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveUniqueItemsBy<TCollection, TItem, TKey>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, TKey> keySelector,
        IEqualityComparer<TKey> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertHaveUniqueItemsBy(
            assertions.Subject,
            assertions.SubjectExpression,
            keySelector,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainExactly<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSequence,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSequence);

        CollectionAssertionEngine.AssertContainExactly(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSequence,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeSubsetOf<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSuperset,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSuperset);

        CollectionAssertionEngine.AssertBeSubsetOf(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSuperset,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeSupersetOf<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSubset,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSubset);

        CollectionAssertionEngine.AssertBeSupersetOf(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSubset,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveCount<TCollection>(
        this ValueAssertions<TCollection> assertions,
        int expectedCount,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertHaveCount(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedCount,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeEmpty<TCollection>(
        this ValueAssertions<TCollection> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertBeEmpty(
            assertions.Subject,
            assertions.SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> NotBeEmpty<TCollection>(
        this ValueAssertions<TCollection> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertNotBeEmpty(
            assertions.Subject,
            assertions.SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static ContainSingleContinuation<ValueAssertions<TCollection>, TItem> ContainSingle<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        return ContainSingleTyped<TCollection, TItem>(assertions, because, callerFilePath, callerLineNumber);
    }

    public static ContainSingleContinuation<ValueAssertions<TItem[]>, TItem> ContainSingle<TItem>(
        this ValueAssertions<TItem[]> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return ContainSingleTyped<TItem[], TItem>(assertions, because, callerFilePath, callerLineNumber);
    }

    public static ContainSingleContinuation<ValueAssertions<List<TItem>>, TItem> ContainSingle<TItem>(
        this ValueAssertions<List<TItem>> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return ContainSingleTyped<List<TItem>, TItem>(assertions, because, callerFilePath, callerLineNumber);
    }

    public static ContainSingleContinuation<ValueAssertions<IEnumerable<TItem>>, TItem> ContainSingle<TItem>(
        this ValueAssertions<IEnumerable<TItem>> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return ContainSingleTyped<IEnumerable<TItem>, TItem>(assertions, because, callerFilePath, callerLineNumber);
    }

    public static ContainSingleContinuation<ValueAssertions<ICollection<TItem>>, TItem> ContainSingle<TItem>(
        this ValueAssertions<ICollection<TItem>> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return ContainSingleTyped<ICollection<TItem>, TItem>(assertions, because, callerFilePath, callerLineNumber);
    }

    public static ContainSingleContinuation<ValueAssertions<IList<TItem>>, TItem> ContainSingle<TItem>(
        this ValueAssertions<IList<TItem>> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return ContainSingleTyped<IList<TItem>, TItem>(assertions, because, callerFilePath, callerLineNumber);
    }

    public static ContainSingleContinuation<ValueAssertions<IReadOnlyCollection<TItem>>, TItem> ContainSingle<TItem>(
        this ValueAssertions<IReadOnlyCollection<TItem>> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return ContainSingleTyped<IReadOnlyCollection<TItem>, TItem>(assertions, because, callerFilePath, callerLineNumber);
    }

    public static ContainSingleContinuation<ValueAssertions<IReadOnlyList<TItem>>, TItem> ContainSingle<TItem>(
        this ValueAssertions<IReadOnlyList<TItem>> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return ContainSingleTyped<IReadOnlyList<TItem>, TItem>(assertions, because, callerFilePath, callerLineNumber);
    }

    public static ContainSingleContinuation<ValueAssertions<TCollection>> ContainSingle<TCollection>(
        this ValueAssertions<TCollection> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        var result = CollectionAssertionEngine.AssertContainSingleAndCaptureResult(
            assertions.Subject,
            assertions.SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new ContainSingleContinuation<ValueAssertions<TCollection>>(
            assertions,
            result.HasSingleItem,
            result.SingleItem,
            result.FailureMessage);
    }

    public static ContainSingleContinuation<ValueAssertions<TCollection>, TItem> ContainSingle<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, bool> predicate,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(predicate);

        var result = CollectionAssertionEngine.AssertContainSingleAndCaptureResult(
            assertions.Subject,
            assertions.SubjectExpression,
            predicate,
            because,
            callerFilePath,
            callerLineNumber);

        return new ContainSingleContinuation<ValueAssertions<TCollection>, TItem>(
            assertions,
            result.HasSingleItem,
            result.SingleItem,
            result.FailureMessage);
    }

    public static AndContinuation<ValueAssertions<TCollection>> OnlyContain<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, bool> predicate,
        string? because = null,
        [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(predicate);

        CollectionAssertionEngine.AssertOnlyContain(
            assertions.Subject,
            assertions.SubjectExpression,
            predicate,
            predicateExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    private static ContainSingleContinuation<ValueAssertions<TCollection>, TItem> ContainSingleTyped<TCollection, TItem>(
        ValueAssertions<TCollection> assertions,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        var result = CollectionAssertionEngine.AssertContainSingleAndCaptureResult(
            assertions.Subject,
            assertions.SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new ContainSingleContinuation<ValueAssertions<TCollection>, TItem>(
            assertions,
            result.HasSingleItem,
            result.SingleItem,
            result.FailureMessage);
    }

    public static AndContinuation<ValueAssertions<TCollection>> NotContain<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, bool> predicate,
        string? because = null,
        [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(predicate);

        CollectionAssertionEngine.AssertNotContain(
            assertions.Subject,
            assertions.SubjectExpression,
            predicate,
            predicateExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> NotContain<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        TItem unexpected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertNotContainItem(
            assertions.Subject,
            assertions.SubjectExpression,
            unexpected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> AllSatisfy<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        Action<TItem> assertion,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(assertion);

        CollectionAssertionEngine.AssertAllSatisfy(
            assertions.Subject,
            assertions.SubjectExpression,
            assertion,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> SatisfyRespectively<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        params Action<TItem>[] assertionsForItems)
        where TCollection : IEnumerable<TItem>
    {
        return SatisfyRespectively(assertions, because: null, assertionsForItems);
    }

    public static AndContinuation<ValueAssertions<TCollection>> SatisfyRespectively<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        string? because,
        params Action<TItem>[] assertionsForItems)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(assertionsForItems);

        for (var index = 0; index < assertionsForItems.Length; index++)
        {
            if (assertionsForItems[index] is null)
            {
                throw new ArgumentNullException(nameof(assertionsForItems), $"assertionsForItems[{index}] must not be null.");
            }
        }

        CollectionAssertionEngine.AssertSatisfyRespectively(
            assertions.Subject,
            assertions.SubjectExpression,
            assertionsForItems,
            because,
            callerFilePath: null,
            callerLineNumber: 0);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static ContainKeyContinuation<ValueAssertions<TDictionary>, TValue> ContainKey<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TKey expectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        var result = CollectionAssertionEngine.AssertContainKeyAndCaptureResult(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedKey,
            because,
            callerFilePath,
            callerLineNumber);

        return new ContainKeyContinuation<ValueAssertions<TDictionary>, TValue>(
            assertions,
            result.HasValue,
            result.Value,
            result.FailureMessage);
    }

    public static ContainKeyContinuation<ValueAssertions<Dictionary<TKey, TValue>>, TValue> ContainKey<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TKey expectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainKey<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedKey,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static ContainKeyContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>, TValue> ContainKey<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TKey expectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainKey<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedKey,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> NotContainKey<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TKey unexpectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertNotContainKey(
            assertions.Subject,
            assertions.SubjectExpression,
            unexpectedKey,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> NotContainKey<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TKey unexpectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainKey<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedKey,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> NotContainKey<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TKey unexpectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainKey<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedKey,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> ContainValue<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertContainValue(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> ContainValue<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainValue<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> ContainValue<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainValue<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> NotContainValue<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertNotContainValue(
            assertions.Subject,
            assertions.SubjectExpression,
            unexpectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> NotContainValue<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainValue<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> NotContainValue<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainValue<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> ContainEntry<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TKey expectedKey,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertContainEntry(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedKey,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> ContainEntry<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TKey expectedKey,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainEntry<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedKey,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> ContainEntry<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TKey expectedKey,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainEntry<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedKey,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> NotContainEntry<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TKey unexpectedKey,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertNotContainEntry(
            assertions.Subject,
            assertions.SubjectExpression,
            unexpectedKey,
            unexpectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> NotContainEntry<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TKey unexpectedKey,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainEntry<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedKey,
            unexpectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> NotContainEntry<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TKey unexpectedKey,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainEntry<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedKey,
            unexpectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainInOrder<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSequence,
        string? because = null,
        bool allowGaps = true,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSequence);

        CollectionAssertionEngine.AssertContainInOrder(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSequence,
            because,
            allowGaps,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainInOrder<TCollection, TItem, TKey>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TKey> expectedSequence,
        Func<TItem, TKey> keySelector,
        string? because = null,
        bool allowGaps = true,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSequence);
        ArgumentNullException.ThrowIfNull(keySelector);

        CollectionAssertionEngine.AssertContainInOrderByKey(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSequence,
            keySelector,
            because,
            allowGaps,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeInAscendingOrder<TCollection>(
        this ValueAssertions<TCollection> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertBeInAscendingOrder(
            assertions.Subject,
            assertions.SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeInDescendingOrder<TCollection>(
        this ValueAssertions<TCollection> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertBeInDescendingOrder(
            assertions.Subject,
            assertions.SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeInAscendingOrder<TCollection, TItem, TKey>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, TKey> keySelector,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(keySelector);

        CollectionAssertionEngine.AssertBeInAscendingOrderByKey(
            assertions.Subject,
            assertions.SubjectExpression,
            keySelector,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeInAscendingOrder<TCollection, TItem, TKey>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, TKey> keySelector,
        IComparer<TKey> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertBeInAscendingOrderByKey(
            assertions.Subject,
            assertions.SubjectExpression,
            keySelector,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeInDescendingOrder<TCollection, TItem, TKey>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, TKey> keySelector,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(keySelector);

        CollectionAssertionEngine.AssertBeInDescendingOrderByKey(
            assertions.Subject,
            assertions.SubjectExpression,
            keySelector,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeInDescendingOrder<TCollection, TItem, TKey>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, TKey> keySelector,
        IComparer<TKey> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertBeInDescendingOrderByKey(
            assertions.Subject,
            assertions.SubjectExpression,
            keySelector,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }
}
