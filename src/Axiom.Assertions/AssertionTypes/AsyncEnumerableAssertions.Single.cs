using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncEnumerableAssertions<T>
{
    public async ValueTask<ContainSingleContinuation<AsyncEnumerableAssertions<T>, T>> ContainSingleAsync(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var result = await EvaluateContainSingleAsync(because).ConfigureAwait(false);
        if (result.FailureMessage is not null)
        {
            AssertionFailureDispatcher.Fail(result.FailureMessage, callerFilePath, callerLineNumber);
        }

        return new ContainSingleContinuation<AsyncEnumerableAssertions<T>, T>(
            this,
            result.HasSingleItem,
            result.SingleItem,
            result.FailureMessage,
            "ContainSingleAsync");
    }

    public async ValueTask<ContainSingleContinuation<AsyncEnumerableAssertions<T>, T>> ContainSingleAsync(
        Func<T, bool> predicate,
        string? because = null,
        [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var result = await EvaluateContainSingleAsync(predicate, predicateExpression, because).ConfigureAwait(false);
        if (result.FailureMessage is not null)
        {
            AssertionFailureDispatcher.Fail(result.FailureMessage, callerFilePath, callerLineNumber);
        }

        return new ContainSingleContinuation<AsyncEnumerableAssertions<T>, T>(
            this,
            result.HasSingleItem,
            result.SingleItem,
            result.FailureMessage,
            "ContainSingleAsync");
    }

    private async ValueTask<ContainSingleResult> EvaluateContainSingleAsync(string? because)
    {
        var subject = Subject;
        if (subject is null)
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure("to contain a single item", subject, because));
        }

        await using var enumerator = subject.GetAsyncEnumerator();
        if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure("to contain a single item", 0, because));
        }

        var singleItem = enumerator.Current;
        if (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(
                    "to contain a single item",
                    AtLeastTwoItemsToken.Instance,
                    because));
        }

        return new ContainSingleResult(
            HasSingleItem: true,
            SingleItem: singleItem,
            FailureMessage: null);
    }

    private async ValueTask<ContainSingleResult> EvaluateContainSingleAsync(
        Func<T, bool> predicate,
        string? predicateExpression,
        string? because)
    {
        var expectationText = AssertionMessageText.BuildPredicateExpectationText(
            "to contain a single item matching predicate",
            predicateExpression);
        var subject = Subject;
        if (subject is null)
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(expectationText, subject, because));
        }

        var index = 0;
        var hasSingleItem = false;
        T singleItem = default!;

        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            if (!predicate(enumerator.Current))
            {
                index++;
                continue;
            }

            if (!hasSingleItem)
            {
                hasSingleItem = true;
                singleItem = enumerator.Current;
                index++;
                continue;
            }

            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(
                    expectationText,
                    new AtLeastTwoMatchingItemsToken(index),
                    because));
        }

        if (!hasSingleItem)
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(expectationText, 0, because));
        }

        return new ContainSingleResult(
            HasSingleItem: true,
            SingleItem: singleItem,
            FailureMessage: null);
    }
}
