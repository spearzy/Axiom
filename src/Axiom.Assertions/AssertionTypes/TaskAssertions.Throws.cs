using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class TaskAssertions
{
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
}

public sealed partial class TaskAssertions<TResult>
{
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
}
