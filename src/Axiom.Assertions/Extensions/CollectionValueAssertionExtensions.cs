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

    public static AndContinuation<ValueAssertions<TCollection>> ContainSingle<TCollection>(
        this ValueAssertions<TCollection> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertContainSingle(
            assertions.Subject,
            assertions.SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> OnlyContain<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, bool> predicate,
        string? because = null,
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
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> NotContain<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, bool> predicate,
        string? because = null,
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
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
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
}
