using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed class AsyncFunctionAssertions<TResult>
{
    private Task<AsyncFunctionInvocation<TResult>>? _capturedInvocationTask;
    private Task<TaskOutcome<TResult>>? _capturedOutcomeTask;

    public AsyncFunctionAssertions(Func<ValueTask<TResult>> subject, string? subjectExpression)
    {
        ArgumentNullException.ThrowIfNull(subject);

        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    public Func<ValueTask<TResult>> Subject { get; }

    public string? SubjectExpression { get; }

    public async ValueTask<ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException>> ThrowAsync<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var invocation = await GetInvocationAsync().ConfigureAwait(false);
        if (invocation.SynchronousException is TException synchronousException)
        {
            return CreateThrowContinuation<TException>(synchronousException, because);
        }

        if (!invocation.HasReturnedTask)
        {
            return FailThrowAssertion<TException>(
                invocation.SynchronousException,
                new Expectation("to throw", typeof(TException)),
                because,
                callerFilePath,
                callerLineNumber);
        }

        var outcome = await GetOutcomeAsync(invocation.ReturnedTask!).ConfigureAwait(false);
        if (outcome.Exception is TException exception)
        {
            return CreateThrowContinuation<TException>(exception, because);
        }

        return FailThrowAssertion<TException>(
            outcome.Exception,
            new Expectation("to throw", typeof(TException)),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public async ValueTask<ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException>> ThrowExactlyAsync<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var invocation = await GetInvocationAsync().ConfigureAwait(false);
        if (invocation.SynchronousException?.GetType() == typeof(TException))
        {
            return CreateThrowContinuation<TException>(invocation.SynchronousException, because);
        }

        if (!invocation.HasReturnedTask)
        {
            return FailThrowAssertion<TException>(
                invocation.SynchronousException,
                new Expectation("to throw exactly", typeof(TException)),
                because,
                callerFilePath,
                callerLineNumber);
        }

        var outcome = await GetOutcomeAsync(invocation.ReturnedTask!).ConfigureAwait(false);
        if (outcome.Exception?.GetType() == typeof(TException))
        {
            return CreateThrowContinuation<TException>(outcome.Exception, because);
        }

        return FailThrowAssertion<TException>(
            outcome.Exception,
            new Expectation("to throw exactly", typeof(TException)),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public async ValueTask<AndContinuation<AsyncFunctionAssertions<TResult>>> NotThrowAsync(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var invocation = await GetInvocationAsync().ConfigureAwait(false);
        if (invocation.SynchronousException is not null)
        {
            TaskAssertionHelpers.Fail(
                SubjectLabel(),
                new Expectation("to not throw", IncludeExpectedValue: false),
                invocation.SynchronousException.GetType(),
                because,
                callerFilePath,
                callerLineNumber);

            return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
        }

        var outcome = await GetOutcomeAsync(invocation.ReturnedTask!).ConfigureAwait(false);
        if (outcome.Exception is null)
        {
            return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to not throw", IncludeExpectedValue: false),
            outcome.Exception.GetType(),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
    }

    public async ValueTask<AndContinuation<AsyncFunctionAssertions<TResult>>> CompleteWithin(
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
                new Expectation("to complete within", timeout),
                invocation.SynchronousException.GetType(),
                because,
                callerFilePath,
                callerLineNumber);

            return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
        }

        if (await TaskAssertionHelpers.CompletesWithinAsync(invocation.ReturnedTask!, timeout).ConfigureAwait(false))
        {
            return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to complete within", timeout),
            TaskAssertionTokens.NotCompletedInTime,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
    }

    public async ValueTask<AndContinuation<AsyncFunctionAssertions<TResult>>> NotCompleteWithin(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        var invocation = await GetInvocationAsync().ConfigureAwait(false);
        if (invocation.SynchronousException is not null)
        {
            return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
        }

        if (!await TaskAssertionHelpers.CompletesWithinAsync(invocation.ReturnedTask!, timeout).ConfigureAwait(false))
        {
            return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
        }

        TaskAssertionHelpers.Fail(
            SubjectLabel(),
            new Expectation("to not complete within", timeout),
            TaskAssertionTokens.CompletedInTime,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<AsyncFunctionAssertions<TResult>>(this);
    }

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

    private Task<AsyncFunctionInvocation<TResult>> GetInvocationAsync()
    {
        // Normalise the delegate once so later assertions reuse the same Task<TResult>.
        return _capturedInvocationTask ??= CaptureInvocationAsync(Subject);
    }

    private Task<TaskOutcome<TResult>> GetOutcomeAsync(Task<TResult> task)
    {
        return _capturedOutcomeTask ??= TaskAssertionHelpers.CaptureOutcomeAsync(task);
    }

    private ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException> CreateThrowContinuation<TException>(
        Exception capturedException,
        string? because)
        where TException : Exception
    {
        return new ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException>(
            capturedException,
            wasThrowAssertionSatisfied: true,
            throwFailureMessage: null,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail);
    }

    private ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException> FailThrowAssertion<TException>(
        Exception? capturedException,
        Expectation expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        where TException : Exception
    {
        object actual = capturedException is null
            ? TaskAssertionTokens.NoException
            : capturedException.GetType();

        var failureMessage = TaskAssertionHelpers.RenderFailure(SubjectLabel(), expectation, actual, because);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException>(
            capturedException,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail);
    }

    private SuccessfulTaskContinuation<AsyncFunctionAssertions<TResult>, TResult> FailSucceedAssertion(
        Expectation expectation,
        object actual,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var failureMessage = TaskAssertionHelpers.RenderFailure(SubjectLabel(), expectation, actual, because);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new SuccessfulTaskContinuation<AsyncFunctionAssertions<TResult>, TResult>(
            this,
            hasResult: false,
            result: default,
            successFailureMessage: failureMessage);
    }

    private ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException> CreateFaultContinuation<TException>(
        TaskOutcome<TResult> outcome,
        Expectation expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        where TException : Exception
    {
        if (outcome.Status == TaskStatus.Faulted && outcome.Exception is TException typedException)
        {
            return new ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException>(
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

        return new ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException>(
            outcome.Exception,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail,
            baseAssertionName: "BeFaultedWith");
    }

    private ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException> FailFaultAssertion<TException>(
        Expectation expectation,
        Exception capturedException,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        where TException : Exception
    {
        var failureMessage = TaskAssertionHelpers.RenderFailure(
            SubjectLabel(),
            expectation,
            capturedException.GetType(),
            because);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<AsyncFunctionAssertions<TResult>, TException>(
            capturedException,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail,
            baseAssertionName: "BeFaultedWith");
    }

    private static Task<AsyncFunctionInvocation<TResult>> CaptureInvocationAsync(Func<ValueTask<TResult>> subject)
    {
        try
        {
            return Task.FromResult(new AsyncFunctionInvocation<TResult>(subject().AsTask(), null));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new AsyncFunctionInvocation<TResult>(null, ex));
        }
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }
}

internal readonly record struct AsyncFunctionInvocation<TResult>(Task<TResult>? ReturnedTask, Exception? SynchronousException)
{
    public bool HasReturnedTask => ReturnedTask is not null;
}
