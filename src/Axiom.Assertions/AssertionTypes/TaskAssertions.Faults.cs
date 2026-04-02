using System.Runtime.CompilerServices;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class TaskAssertions
{
    public async ValueTask<ThrownExceptionAssertions<TaskAssertions, TException>> BeFaultedWith<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        return CreateFaultContinuation<TException>(
            outcome,
            new Expectation("to be faulted with", typeof(TException)),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public async ValueTask<ThrownExceptionAssertions<TaskAssertions, TException>> BeFaultedWithWithin<TException>(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if (!await TaskAssertionHelpers.CompletesWithinAsync(Subject, timeout).ConfigureAwait(false))
        {
            var timeoutFailureMessage = TaskAssertionHelpers.RenderFailure(
                SubjectLabel(),
                new Expectation("to be faulted with", $"{typeof(TException)} within {timeout}"),
                TaskAssertionTokens.NotCompletedInTime,
                because);
            AssertionFailureDispatcher.Fail(timeoutFailureMessage, callerFilePath, callerLineNumber);

            return new ThrownExceptionAssertions<TaskAssertions, TException>(
                capturedException: null,
                wasThrowAssertionSatisfied: false,
                throwFailureMessage: timeoutFailureMessage,
                parentAssertions: this,
                subjectLabel: SubjectLabel(),
                inheritedReason: because,
                fail: AssertionFailureDispatcher.Fail,
                baseAssertionName: "BeFaultedWith");
        }

        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        return CreateFaultContinuation<TException>(
            outcome,
            new Expectation("to be faulted with", $"{typeof(TException)} within {timeout}"),
            because,
            callerFilePath,
            callerLineNumber);
    }

    // The cached task outcome is the key safety property: every follow-up assertion observes one terminal state.
}

public sealed partial class TaskAssertions<TResult>
{
    public async ValueTask<ThrownExceptionAssertions<TaskAssertions<TResult>, TException>> BeFaultedWith<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        return CreateFaultContinuation<TException>(
            outcome,
            new Expectation("to be faulted with", typeof(TException)),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public async ValueTask<ThrownExceptionAssertions<TaskAssertions<TResult>, TException>> BeFaultedWithWithin<TException>(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if (!await TaskAssertionHelpers.CompletesWithinAsync(Subject, timeout).ConfigureAwait(false))
        {
            var timeoutFailureMessage = TaskAssertionHelpers.RenderFailure(
                SubjectLabel(),
                new Expectation("to be faulted with", $"{typeof(TException)} within {timeout}"),
                TaskAssertionTokens.NotCompletedInTime,
                because);
            AssertionFailureDispatcher.Fail(timeoutFailureMessage, callerFilePath, callerLineNumber);

            return new ThrownExceptionAssertions<TaskAssertions<TResult>, TException>(
                capturedException: null,
                wasThrowAssertionSatisfied: false,
                throwFailureMessage: timeoutFailureMessage,
                parentAssertions: this,
                subjectLabel: SubjectLabel(),
                inheritedReason: because,
                fail: AssertionFailureDispatcher.Fail,
                baseAssertionName: "BeFaultedWith");
        }

        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        return CreateFaultContinuation<TException>(
            outcome,
            new Expectation("to be faulted with", $"{typeof(TException)} within {timeout}"),
            because,
            callerFilePath,
            callerLineNumber);
    }

    // Cache the observed result so success/result chains never need to consume the task again.
}
