using System.Runtime.CompilerServices;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncFunctionAssertions<TResult>
{
    public async ValueTask<ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException>> BeFaultedWith<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var invocation = await GetInvocationAsync().ConfigureAwait(false);
        if (invocation.SynchronousException is not null)
        {
            return FailFaultAssertion<TException>(
                new Expectation("to be faulted with", typeof(TException)),
                invocation.SynchronousException,
                because,
                callerFilePath,
                callerLineNumber);
        }

        var outcome = await GetOutcomeAsync(invocation.ReturnedTask!).ConfigureAwait(false);
        return CreateFaultContinuation<TException>(
            outcome,
            new Expectation("to be faulted with", typeof(TException)),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public async ValueTask<ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException>> BeFaultedWithWithin<TException>(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        var invocation = await GetInvocationAsync().ConfigureAwait(false);
        if (invocation.SynchronousException is not null)
        {
            return FailFaultAssertion<TException>(
                new Expectation("to be faulted with", $"{typeof(TException)} within {timeout}"),
                invocation.SynchronousException,
                because,
                callerFilePath,
                callerLineNumber);
        }

        if (!await TaskAssertionHelpers.CompletesWithinAsync(invocation.ReturnedTask!, timeout).ConfigureAwait(false))
        {
            var timeoutFailureMessage = TaskAssertionHelpers.RenderFailure(
                SubjectLabel(),
                new Expectation("to be faulted with", $"{typeof(TException)} within {timeout}"),
                TaskAssertionTokens.NotCompletedInTime,
                because);
            AssertionFailureDispatcher.Fail(timeoutFailureMessage, callerFilePath, callerLineNumber);

            return new ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException>(
                capturedException: null,
                wasThrowAssertionSatisfied: false,
                throwFailureMessage: timeoutFailureMessage,
                parentAssertions: this,
                subjectLabel: SubjectLabel(),
                inheritedReason: because,
                fail: AssertionFailureDispatcher.Fail,
                baseAssertionName: "BeFaultedWith");
        }

        var outcome = await GetOutcomeAsync(invocation.ReturnedTask!).ConfigureAwait(false);
        return CreateFaultContinuation<TException>(
            outcome,
            new Expectation("to be faulted with", $"{typeof(TException)} within {timeout}"),
            because,
            callerFilePath,
            callerLineNumber);
    }
}
