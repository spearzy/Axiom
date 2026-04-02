using Axiom.Assertions.Chaining;
using Axiom.Core;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncEnumerableAssertions<T>
{
    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> SatisfyRespectivelyAsync(
        params Action<T>[] assertionsForItems)
    {
        return SatisfyRespectivelyAsync(because: null, assertionsForItems);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> SatisfyRespectivelyAsync(
        string? because,
        params Action<T>[] assertionsForItems)
    {
        ArgumentNullException.ThrowIfNull(assertionsForItems);

        for (var index = 0; index < assertionsForItems.Length; index++)
        {
            if (assertionsForItems[index] is null)
            {
                throw new ArgumentNullException(nameof(assertionsForItems), $"assertionsForItems[{index}] must not be null.");
            }
        }

        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to satisfy assertions respectively (same order and count)", IncludeExpectedValue: false),
                    subject,
                    because),
                callerFilePath: null,
                callerLineNumber: 0);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var expectedCount = assertionsForItems.Length;
        var captureInnerFailures = Batch.Current is not null;
        await using var enumerator = subject.GetAsyncEnumerator();

        for (var index = 0; index < expectedCount; index++)
        {
            if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                Fail(
                    new Failure(
                        SubjectLabel(),
                        new Expectation("to satisfy assertions respectively (same order and count)", IncludeExpectedValue: false),
                        new RenderedText($"async stream had fewer items than assertions (expected {expectedCount}, found {index})"),
                        because),
                    callerFilePath: null,
                    callerLineNumber: 0);
                return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
            }

            var item = enumerator.Current;
            var itemAssertion = assertionsForItems[index];
            var itemFailureMessage = captureInnerFailures
                ? ExecuteItemAssertionInBatch(itemAssertion, item)
                : ExecuteItemAssertionOutsideBatch(itemAssertion, item);

            if (itemFailureMessage is not null)
            {
                Fail(
                    new Failure(
                        SubjectLabel(),
                        new Expectation($"to satisfy assertions respectively (failing index {index})", IncludeExpectedValue: false),
                        new RenderedText(itemFailureMessage),
                        because),
                    callerFilePath: null,
                    callerLineNumber: 0);
                return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
            }
        }

        if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var actualCount = expectedCount + 1;
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            actualCount++;
        }

        Fail(
            new Failure(
                SubjectLabel(),
                new Expectation("to satisfy assertions respectively (same order and count)", IncludeExpectedValue: false),
                new RenderedText($"async stream had more items than assertions (expected {expectedCount}, found {actualCount})"),
                because),
            callerFilePath: null,
            callerLineNumber: 0);

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    private static string? ExecuteItemAssertionOutsideBatch(Action<T> itemAssertion, T item)
    {
        try
        {
            itemAssertion(item);
            return null;
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message;
        }
    }

    private static string? ExecuteItemAssertionInBatch(Action<T> itemAssertion, T item)
    {
        try
        {
            var capturedFailures = AssertionFailureCapture.Capture(
                () => itemAssertion(item));
            return capturedFailures.FirstFailureMessage;
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message;
        }
    }
}
