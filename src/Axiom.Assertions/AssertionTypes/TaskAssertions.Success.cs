using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class TaskAssertions
{
    public async ValueTask<AndContinuation<TaskAssertions>> Succeed(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Status == TaskStatus.RanToCompletion)
        {
            return new AndContinuation<TaskAssertions>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to succeed", IncludeExpectedValue: false),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<TaskAssertions>(this);
    }

    public async ValueTask<AndContinuation<TaskAssertions>> SucceedWithin(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if (!await TaskAssertionHelpers.CompletesWithinAsync(Subject, timeout).ConfigureAwait(false))
        {
            TaskAssertionHelpers.Fail(
                SubjectLabel(),
                new Expectation("to succeed within", timeout),
                TaskAssertionTokens.NotCompletedInTime,
                because,
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<TaskAssertions>(this);
        }

        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Status == TaskStatus.RanToCompletion)
        {
            return new AndContinuation<TaskAssertions>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to succeed within", timeout),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<TaskAssertions>(this);
    }
}

public sealed partial class TaskAssertions<TResult>
{
    public async ValueTask<SuccessfulTaskContinuation<TaskAssertions<TResult>, TResult>> Succeed(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Status == TaskStatus.RanToCompletion)
        {
            return new SuccessfulTaskContinuation<TaskAssertions<TResult>, TResult>(
                this,
                hasResult: true,
                outcome.Result,
                successFailureMessage: null);
        }

        var failureMessage = TaskAssertionHelpers.RenderFailure(
            SubjectLabel(),
            new Expectation("to succeed", IncludeExpectedValue: false),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new SuccessfulTaskContinuation<TaskAssertions<TResult>, TResult>(
            this,
            hasResult: false,
            result: default!,
            successFailureMessage: failureMessage);
    }

    public async ValueTask<SuccessfulTaskContinuation<TaskAssertions<TResult>, TResult>> SucceedWithin(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if (!await TaskAssertionHelpers.CompletesWithinAsync(Subject, timeout).ConfigureAwait(false))
        {
            var timeoutFailureMessage = TaskAssertionHelpers.RenderFailure(
                SubjectLabel(),
                new Expectation("to succeed within", timeout),
                TaskAssertionTokens.NotCompletedInTime,
                because);
            AssertionFailureDispatcher.Fail(timeoutFailureMessage, callerFilePath, callerLineNumber);

            return new SuccessfulTaskContinuation<TaskAssertions<TResult>, TResult>(
                this,
                hasResult: false,
                result: default!,
                successFailureMessage: timeoutFailureMessage);
        }

        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Status == TaskStatus.RanToCompletion)
        {
            return new SuccessfulTaskContinuation<TaskAssertions<TResult>, TResult>(
                this,
                hasResult: true,
                outcome.Result,
                successFailureMessage: null);
        }

        var failureMessage = TaskAssertionHelpers.RenderFailure(
            SubjectLabel(),
            new Expectation("to succeed within", timeout),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new SuccessfulTaskContinuation<TaskAssertions<TResult>, TResult>(
            this,
            hasResult: false,
            result: default!,
            successFailureMessage: failureMessage);
    }
}
