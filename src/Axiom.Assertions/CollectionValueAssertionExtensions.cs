using System.Collections;
using System.Runtime.CompilerServices;

namespace Axiom.Assertions;

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
}
