using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncFunctionAssertions<TResult>
{
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
