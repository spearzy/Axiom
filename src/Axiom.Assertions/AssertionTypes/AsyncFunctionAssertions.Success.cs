using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncFunctionAssertions<TResult>
{
    public async ValueTask<SuccessfulTaskContinuation<AsyncFunctionAssertions<TResult>, TResult>> Succeed(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var invocation = await GetInvocationAsync().ConfigureAwait(false);
        if (invocation.SynchronousException is not null)
        {
            return FailSucceedAssertion(
                new Expectation("to succeed", IncludeExpectedValue: false),
                invocation.SynchronousException.GetType(),
                because,
                callerFilePath,
                callerLineNumber);
        }

        var outcome = await GetOutcomeAsync(invocation.ReturnedTask!).ConfigureAwait(false);
        if (outcome.Status == TaskStatus.RanToCompletion)
        {
            return new SuccessfulTaskContinuation<AsyncFunctionAssertions<TResult>, TResult>(
                this,
                hasResult: true,
                outcome.Result,
                successFailureMessage: null);
        }

        return FailSucceedAssertion(
            new Expectation("to succeed", IncludeExpectedValue: false),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public async ValueTask<SuccessfulTaskContinuation<AsyncFunctionAssertions<TResult>, TResult>> SucceedWithin(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        var invocation = await GetInvocationAsync().ConfigureAwait(false);
        if (invocation.SynchronousException is not null)
        {
            return FailSucceedAssertion(
                new Expectation("to succeed within", timeout),
                invocation.SynchronousException.GetType(),
                because,
                callerFilePath,
                callerLineNumber);
        }

        if (!await TaskAssertionHelpers.CompletesWithinAsync(invocation.ReturnedTask!, timeout).ConfigureAwait(false))
        {
            return FailSucceedAssertion(
                new Expectation("to succeed within", timeout),
                TaskAssertionTokens.NotCompletedInTime,
                because,
                callerFilePath,
                callerLineNumber);
        }

        var outcome = await GetOutcomeAsync(invocation.ReturnedTask!).ConfigureAwait(false);
        if (outcome.Status == TaskStatus.RanToCompletion)
        {
            return new SuccessfulTaskContinuation<AsyncFunctionAssertions<TResult>, TResult>(
                this,
                hasResult: true,
                outcome.Result,
                successFailureMessage: null);
        }

        return FailSucceedAssertion(
            new Expectation("to succeed within", timeout),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because,
            callerFilePath,
            callerLineNumber);
    }
}
