using System.Collections;
using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;

namespace Axiom.Assertions.Extensions;

public static partial class CollectionValueAssertionExtensions
{
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
}
