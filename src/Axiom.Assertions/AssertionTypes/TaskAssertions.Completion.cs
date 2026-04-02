using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class TaskAssertions
{
    public async ValueTask<AndContinuation<TaskAssertions>> CompleteWithin(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if (await TaskAssertionHelpers.CompletesWithinAsync(Subject, timeout).ConfigureAwait(false))
        {
            return new AndContinuation<TaskAssertions>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to complete within", timeout),
            TaskAssertionTokens.NotCompletedInTime,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<TaskAssertions>(this);
    }

    public async ValueTask<AndContinuation<TaskAssertions>> NotCompleteWithin(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if (!await TaskAssertionHelpers.CompletesWithinAsync(Subject, timeout).ConfigureAwait(false))
        {
            return new AndContinuation<TaskAssertions>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to not complete within", timeout),
            TaskAssertionTokens.CompletedInTime,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<TaskAssertions>(this);
    }
}

public sealed partial class TaskAssertions<TResult>
{
    public async ValueTask<AndContinuation<TaskAssertions<TResult>>> CompleteWithin(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if (await TaskAssertionHelpers.CompletesWithinAsync(Subject, timeout).ConfigureAwait(false))
        {
            return new AndContinuation<TaskAssertions<TResult>>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to complete within", timeout),
            TaskAssertionTokens.NotCompletedInTime,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<TaskAssertions<TResult>>(this);
    }

    public async ValueTask<AndContinuation<TaskAssertions<TResult>>> NotCompleteWithin(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if (!await TaskAssertionHelpers.CompletesWithinAsync(Subject, timeout).ConfigureAwait(false))
        {
            return new AndContinuation<TaskAssertions<TResult>>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to not complete within", timeout),
            TaskAssertionTokens.CompletedInTime,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<TaskAssertions<TResult>>(this);
    }
}
