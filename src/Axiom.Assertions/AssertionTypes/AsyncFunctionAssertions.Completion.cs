using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncFunctionAssertions<TResult>
{
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
}
