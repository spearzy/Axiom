using System.Collections;
using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;

namespace Axiom.Assertions.Extensions;

public static partial class CollectionValueAssertionExtensions
{
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
            comparer: null,
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
            comparer: null,
            because,
            allowGaps,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainInOrder<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSequence,
        IEqualityComparer<TItem> comparer,
        string? because = null,
        bool allowGaps = true,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSequence);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertContainInOrder(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSequence,
            comparer,
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
        IEqualityComparer<TKey> comparer,
        string? because = null,
        bool allowGaps = true,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSequence);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertContainInOrderByKey(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSequence,
            keySelector,
            comparer,
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

    public static AndContinuation<ValueAssertions<TCollection>> BeInAscendingOrder<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IComparer<TItem> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertBeInAscendingOrder(
            assertions.Subject,
            assertions.SubjectExpression,
            comparer,
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

    public static AndContinuation<ValueAssertions<TCollection>> BeInDescendingOrder<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IComparer<TItem> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertBeInDescendingOrder(
            assertions.Subject,
            assertions.SubjectExpression,
            comparer,
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
