using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class TaskAssertions
{
    public async ValueTask<AndContinuation<TaskAssertions>> BeCanceled(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Status == TaskStatus.Canceled)
        {
            return new AndContinuation<TaskAssertions>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to be canceled", IncludeExpectedValue: false),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<TaskAssertions>(this);
    }

    public async ValueTask<AndContinuation<TaskAssertions>> BeCanceledWithin(
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
                new Expectation("to be canceled within", timeout),
                TaskAssertionTokens.NotCompletedInTime,
                because,
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<TaskAssertions>(this);
        }

        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Status == TaskStatus.Canceled)
        {
            return new AndContinuation<TaskAssertions>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to be canceled within", timeout),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<TaskAssertions>(this);
    }
}

public sealed partial class TaskAssertions<TResult>
{
    public async ValueTask<AndContinuation<TaskAssertions<TResult>>> BeCanceled(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Status == TaskStatus.Canceled)
        {
            return new AndContinuation<TaskAssertions<TResult>>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to be canceled", IncludeExpectedValue: false),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<TaskAssertions<TResult>>(this);
    }

    public async ValueTask<AndContinuation<TaskAssertions<TResult>>> BeCanceledWithin(
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
                new Expectation("to be canceled within", timeout),
                TaskAssertionTokens.NotCompletedInTime,
                because,
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<TaskAssertions<TResult>>(this);
        }

        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Status == TaskStatus.Canceled)
        {
            return new AndContinuation<TaskAssertions<TResult>>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to be canceled within", timeout),
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<TaskAssertions<TResult>>(this);
    }
}
