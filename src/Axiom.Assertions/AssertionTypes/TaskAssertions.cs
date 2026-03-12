using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed class TaskAssertions(Task subject, string? subjectExpression)
{
    private Task<TaskOutcome>? _capturedOutcomeTask;

    public Task Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;

    public async ValueTask<ThrownExceptionAssertions<TaskAssertions, TException>> ThrowAsync<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Exception is TException)
        {
            return new ThrownExceptionAssertions<TaskAssertions, TException>(
                outcome.Exception,
                wasThrowAssertionSatisfied: true,
                throwFailureMessage: null,
                this,
                SubjectLabel(),
                because,
                AssertionFailureDispatcher.Fail);
        }

        object actual = outcome.Exception is null
            ? TaskAssertionTokens.NoException
            : outcome.Exception.GetType();

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to throw", typeof(TException)),
            actual,
            because);
        var failureMessage = FailureMessageRenderer.Render(failure);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<TaskAssertions, TException>(
            outcome.Exception,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail);
    }

    public async ValueTask<ThrownExceptionAssertions<TaskAssertions, TException>> ThrowExactlyAsync<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Exception?.GetType() == typeof(TException))
        {
            return new ThrownExceptionAssertions<TaskAssertions, TException>(
                outcome.Exception,
                wasThrowAssertionSatisfied: true,
                throwFailureMessage: null,
                this,
                SubjectLabel(),
                because,
                AssertionFailureDispatcher.Fail);
        }

        object actual = outcome.Exception is null
            ? TaskAssertionTokens.NoException
            : outcome.Exception.GetType();

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to throw exactly", typeof(TException)),
            actual,
            because);
        var failureMessage = FailureMessageRenderer.Render(failure);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<TaskAssertions, TException>(
            outcome.Exception,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail);
    }

    public async ValueTask<AndContinuation<TaskAssertions>> NotThrowAsync(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Exception is null)
        {
            return new AndContinuation<TaskAssertions>(this);
        }

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to not throw", IncludeExpectedValue: false),
            outcome.Exception.GetType(),
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new AndContinuation<TaskAssertions>(this);
    }

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
    private Task<TaskOutcome> GetOutcomeAsync()
    {
        return _capturedOutcomeTask ??= TaskAssertionHelpers.CaptureOutcomeAsync(Subject);
    }

    private ThrownExceptionAssertions<TaskAssertions, TException> CreateFaultContinuation<TException>(
        TaskOutcome outcome,
        Expectation expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        where TException : Exception
    {
        if (outcome.Status == TaskStatus.Faulted && outcome.Exception is TException typedException)
        {
            return new ThrownExceptionAssertions<TaskAssertions, TException>(
                typedException,
                wasThrowAssertionSatisfied: true,
                throwFailureMessage: null,
                this,
                SubjectLabel(),
                because,
                AssertionFailureDispatcher.Fail,
                baseAssertionName: "BeFaultedWith");
        }

        var failureMessage = TaskAssertionHelpers.RenderFailure(
            SubjectLabel(),
            expectation,
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<TaskAssertions, TException>(
            outcome.Exception,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail,
            baseAssertionName: "BeFaultedWith");
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }
}

public sealed class TaskAssertions<TResult>(Task<TResult> subject, string? subjectExpression)
{
    private Task<TaskOutcome<TResult>>? _capturedOutcomeTask;

    public Task<TResult> Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;

    public async ValueTask<ThrownExceptionAssertions<TaskAssertions<TResult>, TException>> ThrowAsync<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Exception is TException)
        {
            return new ThrownExceptionAssertions<TaskAssertions<TResult>, TException>(
                outcome.Exception,
                wasThrowAssertionSatisfied: true,
                throwFailureMessage: null,
                this,
                SubjectLabel(),
                because,
                AssertionFailureDispatcher.Fail);
        }

        object actual = outcome.Exception is null
            ? TaskAssertionTokens.NoException
            : outcome.Exception.GetType();

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to throw", typeof(TException)),
            actual,
            because);
        var failureMessage = FailureMessageRenderer.Render(failure);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<TaskAssertions<TResult>, TException>(
            outcome.Exception,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail);
    }

    public async ValueTask<ThrownExceptionAssertions<TaskAssertions<TResult>, TException>> ThrowExactlyAsync<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Exception?.GetType() == typeof(TException))
        {
            return new ThrownExceptionAssertions<TaskAssertions<TResult>, TException>(
                outcome.Exception,
                wasThrowAssertionSatisfied: true,
                throwFailureMessage: null,
                this,
                SubjectLabel(),
                because,
                AssertionFailureDispatcher.Fail);
        }

        object actual = outcome.Exception is null
            ? TaskAssertionTokens.NoException
            : outcome.Exception.GetType();

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to throw exactly", typeof(TException)),
            actual,
            because);
        var failureMessage = FailureMessageRenderer.Render(failure);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<TaskAssertions<TResult>, TException>(
            outcome.Exception,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail);
    }

    public async ValueTask<AndContinuation<TaskAssertions<TResult>>> NotThrowAsync(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var outcome = await GetOutcomeAsync().ConfigureAwait(false);
        if (outcome.Exception is null)
        {
            return new AndContinuation<TaskAssertions<TResult>>(this);
        }

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to not throw", IncludeExpectedValue: false),
            outcome.Exception.GetType(),
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new AndContinuation<TaskAssertions<TResult>>(this);
    }

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
    private Task<TaskOutcome<TResult>> GetOutcomeAsync()
    {
        return _capturedOutcomeTask ??= TaskAssertionHelpers.CaptureOutcomeAsync(Subject);
    }

    private ThrownExceptionAssertions<TaskAssertions<TResult>, TException> CreateFaultContinuation<TException>(
        TaskOutcome<TResult> outcome,
        Expectation expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        where TException : Exception
    {
        if (outcome.Status == TaskStatus.Faulted && outcome.Exception is TException typedException)
        {
            return new ThrownExceptionAssertions<TaskAssertions<TResult>, TException>(
                typedException,
                wasThrowAssertionSatisfied: true,
                throwFailureMessage: null,
                this,
                SubjectLabel(),
                because,
                AssertionFailureDispatcher.Fail,
                baseAssertionName: "BeFaultedWith");
        }

        var failureMessage = TaskAssertionHelpers.RenderFailure(
            SubjectLabel(),
            expectation,
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<TaskAssertions<TResult>, TException>(
            outcome.Exception,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail,
            baseAssertionName: "BeFaultedWith");
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }
}

internal readonly record struct TaskOutcome(TaskStatus Status, Exception? Exception);

internal readonly record struct TaskOutcome<TResult>(TaskStatus Status, Exception? Exception, TResult? Result);

internal static class TaskAssertionHelpers
{
    public static void ValidateTimeout(TimeSpan timeout)
    {
        if (timeout < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "timeout must be zero or greater.");
        }
    }

    public static async ValueTask<bool> CompletesWithinAsync(Task subject, TimeSpan timeout)
    {
        var timeoutTask = Task.Delay(timeout);

        var completedTask = await Task.WhenAny(subject, timeoutTask).ConfigureAwait(false);
        if (!ReferenceEquals(completedTask, subject))
        {
            return false;
        }

        try
        {
            await subject.ConfigureAwait(false);
        }
        catch
        {
            // Completion checks only care whether the task finished inside the timeout window.
        }

        return true;
    }

    public static Task<TaskOutcome> CaptureOutcomeAsync(Task task)
    {
        return CaptureOutcomeCoreAsync(task);
    }

    public static Task<TaskOutcome<TResult>> CaptureOutcomeAsync<TResult>(Task<TResult> task)
    {
        return CaptureOutcomeCoreAsync(task);
    }

    public static string RenderFailure(string subject, Expectation expectation, object actual, string? because)
    {
        return FailureMessageRenderer.Render(new Failure(subject, expectation, actual, because));
    }

    public static void Fail(
        string subject,
        Expectation expectation,
        object actual,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        AssertionFailureDispatcher.Fail(
            RenderFailure(subject, expectation, actual, because),
            callerFilePath,
            callerLineNumber);
    }

    public static object DescribeOutcome(TaskOutcome outcome)
    {
        return outcome.Status switch
        {
            TaskStatus.RanToCompletion => TaskAssertionTokens.CompletedSuccessfully,
            TaskStatus.Canceled => TaskAssertionTokens.Canceled,
            _ when outcome.Exception is not null => outcome.Exception.GetType(),
            _ => TaskAssertionTokens.UnknownOutcome
        };
    }

    public static object DescribeOutcome<TResult>(TaskOutcome<TResult> outcome)
    {
        return outcome.Status switch
        {
            TaskStatus.RanToCompletion => TaskAssertionTokens.CompletedSuccessfully,
            TaskStatus.Canceled => TaskAssertionTokens.Canceled,
            _ when outcome.Exception is not null => outcome.Exception.GetType(),
            _ => TaskAssertionTokens.UnknownOutcome
        };
    }

    private static async Task<TaskOutcome> CaptureOutcomeCoreAsync(Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
            return new TaskOutcome(TaskStatus.RanToCompletion, null);
        }
        catch (Exception ex)
        {
            return new TaskOutcome(task.Status, ex);
        }
    }

    private static async Task<TaskOutcome<TResult>> CaptureOutcomeCoreAsync<TResult>(Task<TResult> task)
    {
        try
        {
            var result = await task.ConfigureAwait(false);
            return new TaskOutcome<TResult>(TaskStatus.RanToCompletion, null, result);
        }
        catch (Exception ex)
        {
            return new TaskOutcome<TResult>(task.Status, ex, default);
        }
    }
}

internal static class TaskAssertionTokens
{
    public static object NoException { get; } = new NoExceptionToken();
    public static object NotCompletedInTime { get; } = new NotCompletedInTimeToken();
    public static object CompletedInTime { get; } = new CompletedInTimeToken();
    public static object CompletedSuccessfully { get; } = new CompletedSuccessfullyToken();
    public static object Canceled { get; } = new CanceledToken();
    public static object UnknownOutcome { get; } = new UnknownOutcomeToken();

    private sealed class NoExceptionToken
    {
        public override string ToString()
        {
            return "<no exception>";
        }
    }

    private sealed class NotCompletedInTimeToken
    {
        public override string ToString()
        {
            return "<not completed within timeout>";
        }
    }

    private sealed class CompletedInTimeToken
    {
        public override string ToString()
        {
            return "<completed within timeout>";
        }
    }

    private sealed class CompletedSuccessfullyToken
    {
        public override string ToString()
        {
            return "<completed successfully>";
        }
    }

    private sealed class CanceledToken
    {
        public override string ToString()
        {
            return "<canceled>";
        }
    }

    private sealed class UnknownOutcomeToken
    {
        public override string ToString()
        {
            return "<unknown task outcome>";
        }
    }
}
