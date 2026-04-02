using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncFunctionAssertions<TResult>
{
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
}
