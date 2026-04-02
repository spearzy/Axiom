using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;

namespace Axiom.Assertions.Extensions;

public static partial class CollectionValueAssertionExtensions
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
}
