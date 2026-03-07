using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed class AsyncActionAssertions(Func<ValueTask> subject, string? subjectExpression)
{
    public Func<ValueTask> Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;

    public async ValueTask<ThrownExceptionAssertions<AsyncActionAssertions>> ThrowAsync<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var capturedException = await CaptureExceptionAsync().ConfigureAwait(false);

        if (capturedException is TException)
        {
            return new ThrownExceptionAssertions<AsyncActionAssertions>(
                capturedException,
                wasThrowAssertionSatisfied: true,
                this,
                SubjectLabel(),
                because,
                Fail);
        }

        object actual = capturedException is null
            ? NoExceptionToken.Instance
            : capturedException.GetType();

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to throw", typeof(TException)),
            actual,
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<AsyncActionAssertions>(
            capturedException,
            wasThrowAssertionSatisfied: false,
            this,
            SubjectLabel(),
            because,
            Fail);
    }

    public async ValueTask<ThrownExceptionAssertions<AsyncActionAssertions>> ThrowExactlyAsync<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var capturedException = await CaptureExceptionAsync().ConfigureAwait(false);
        if (capturedException?.GetType() == typeof(TException))
        {
            return new ThrownExceptionAssertions<AsyncActionAssertions>(
                capturedException,
                wasThrowAssertionSatisfied: true,
                this,
                SubjectLabel(),
                because,
                Fail);
        }

        object actual = capturedException is null
            ? NoExceptionToken.Instance
            : capturedException.GetType();

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to throw exactly", typeof(TException)),
            actual,
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<AsyncActionAssertions>(
            capturedException,
            wasThrowAssertionSatisfied: false,
            this,
            SubjectLabel(),
            because,
            Fail);
    }

    public async ValueTask<AndContinuation<AsyncActionAssertions>> NotThrowAsync(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var capturedException = await CaptureExceptionAsync().ConfigureAwait(false);
        if (capturedException is null)
        {
            return new AndContinuation<AsyncActionAssertions>(this);
        }

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to not throw", IncludeExpectedValue: false),
            capturedException.GetType(),
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new AndContinuation<AsyncActionAssertions>(this);
    }

    public async ValueTask<AndContinuation<AsyncActionAssertions>> CompleteWithin(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ValidateTimeout(timeout);

        if (await CompletesWithinAsync(timeout).ConfigureAwait(false))
        {
            return new AndContinuation<AsyncActionAssertions>(this);
        }

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to complete within", timeout),
            NotCompletedInTimeToken.Instance,
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new AndContinuation<AsyncActionAssertions>(this);
    }

    public async ValueTask<AndContinuation<AsyncActionAssertions>> NotCompleteWithin(
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ValidateTimeout(timeout);

        if (!await CompletesWithinAsync(timeout).ConfigureAwait(false))
        {
            return new AndContinuation<AsyncActionAssertions>(this);
        }

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to not complete within", timeout),
            CompletedInTimeToken.Instance,
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new AndContinuation<AsyncActionAssertions>(this);
    }

    private async ValueTask<Exception?> CaptureExceptionAsync()
    {
        try
        {
            await Subject().ConfigureAwait(false);
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private async ValueTask<bool> CompletesWithinAsync(TimeSpan timeout)
    {
        var executionTask = Subject().AsTask();
        var timeoutTask = Task.Delay(timeout);

        var completedTask = await Task.WhenAny(executionTask, timeoutTask).ConfigureAwait(false);
        if (!ReferenceEquals(completedTask, executionTask))
        {
            return false;
        }

        // Observe the task result to avoid unobserved faulted task exceptions.
        try
        {
            await executionTask.ConfigureAwait(false);
        }
        catch
        {
            // Completion assertions only care whether the action completed in time.
        }

        return true;
    }

    private static void ValidateTimeout(TimeSpan timeout)
    {
        if (timeout < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "timeout must be zero or greater.");
        }
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }

    private static void Fail(string message, string? callerFilePath, int callerLineNumber)
    {

        var batch = Batch.Current;
        if (batch is not null)
        {
            // Keep going inside a batch; root dispose raises one combined exception.
            batch.AddFailure(message);
            return;
        }

        throw new InvalidOperationException(message);
    }

    private sealed class NoExceptionToken
    {
        public static NoExceptionToken Instance { get; } = new();

        public override string ToString()
        {
            return "<no exception>";
        }
    }

    private sealed class NotCompletedInTimeToken
    {
        public static NotCompletedInTimeToken Instance { get; } = new();

        public override string ToString()
        {
            return "<not completed within timeout>";
        }
    }

    private sealed class CompletedInTimeToken
    {
        public static CompletedInTimeToken Instance { get; } = new();

        public override string ToString()
        {
            return "<completed within timeout>";
        }
    }
}
