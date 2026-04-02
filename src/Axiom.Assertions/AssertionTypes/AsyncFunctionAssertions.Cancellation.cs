using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncFunctionAssertions<TResult>
{
    public async ValueTask<AndContinuation<AsyncFunctionAssertions<TResult>>> BeCanceled(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var invocation = await GetInvocationAsync().ConfigureAwait(false);
        if (invocation.SynchronousException is not null)
        {
            TaskAssertionHelpers.Fail(
                SubjectLabel(),
                new Expectation("to be canceled", IncludeExpectedValue: false),
                invocation.SynchronousException.GetType(),
                because,
                callerFilePath,
                callerLineNumber);

            return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
        }

        var outcome = await GetOutcomeAsync(invocation.ReturnedTask!).ConfigureAwait(false);
        if (outcome.Status == TaskStatus.Canceled)
        {
            return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to be canceled", IncludeExpectedValue: false),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
    }

    public async ValueTask<AndContinuation<AsyncFunctionAssertions<TResult>>> BeCanceledWithin(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        var invocation = await GetInvocationAsync().ConfigureAwait(false);
        if (invocation.SynchronousException is not null)
        {
            TaskAssertionHelpers.Fail(
                SubjectLabel(),
                new Expectation("to be canceled within", timeout),
                invocation.SynchronousException.GetType(),
                because,
                callerFilePath,
                callerLineNumber);

            return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
        }

        if (!await TaskAssertionHelpers.CompletesWithinAsync(invocation.ReturnedTask!, timeout).ConfigureAwait(false))
        {
            TaskAssertionHelpers.Fail(
                SubjectLabel(),
                new Expectation("to be canceled within", timeout),
                TaskAssertionTokens.NotCompletedInTime,
                because,
                callerFilePath,
                callerLineNumber);

            return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
        }

        var outcome = await GetOutcomeAsync(invocation.ReturnedTask!).ConfigureAwait(false);
        if (outcome.Status == TaskStatus.Canceled)
        {
            return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to be canceled within", timeout),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
    }
}
