using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed class ActionAssertions(Action subject, string? subjectExpression)
{
    public Action Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;

    public ThrownExceptionAssertions<ActionAssertions> Throw<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var capturedException = CaptureException();

        // Accept the requested type or any subtype (common assertion-library expectation).
        if (capturedException is TException)
        {
            return new ThrownExceptionAssertions<ActionAssertions>(
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

        return new ThrownExceptionAssertions<ActionAssertions>(
            capturedException,
            wasThrowAssertionSatisfied: false,
            this,
            SubjectLabel(),
            because,
            Fail);
    }

    public ThrownExceptionAssertions<ActionAssertions> ThrowExactly<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var capturedException = CaptureException();
        if (capturedException?.GetType() == typeof(TException))
        {
            return new ThrownExceptionAssertions<ActionAssertions>(
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

        return new ThrownExceptionAssertions<ActionAssertions>(
            capturedException,
            wasThrowAssertionSatisfied: false,
            this,
            SubjectLabel(),
            because,
            Fail);
    }

    public AndContinuation<ActionAssertions> NotThrow(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var capturedException = CaptureException();
        if (capturedException is null)
        {
            return new AndContinuation<ActionAssertions>(this);
        }

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to not throw", IncludeExpectedValue: false),
            capturedException.GetType(),
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new AndContinuation<ActionAssertions>(this);
    }

    private Exception? CaptureException()
    {
        try
        {
            // Execute once and capture what happened so we can evaluate it deterministically.
            Subject();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
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
            // In a batch we aggregate; root batch dispose will throw one combined exception.
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
}
