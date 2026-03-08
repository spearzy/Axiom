using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed class ActionAssertions(Action subject, string? subjectExpression)
{
    public Action Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;

    public ThrownExceptionAssertions<ActionAssertions, TException> Throw<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var capturedException = CaptureException();

        // Accept the requested type or any subtype (common assertion-library expectation).
        if (capturedException is TException)
        {
            return new ThrownExceptionAssertions<ActionAssertions, TException>(
                capturedException,
                wasThrowAssertionSatisfied: true,
                throwFailureMessage: null,
                this,
                SubjectLabel(),
                because,
                AssertionFailureDispatcher.Fail);
        }

        object actual = capturedException is null
            ? NoExceptionToken.Instance
            : capturedException.GetType();

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to throw", typeof(TException)),
            actual,
            because);
        var failureMessage = FailureMessageRenderer.Render(failure);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<ActionAssertions, TException>(
            capturedException,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail);
    }

    public ThrownExceptionAssertions<ActionAssertions, TException> ThrowExactly<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var capturedException = CaptureException();
        if (capturedException?.GetType() == typeof(TException))
        {
            return new ThrownExceptionAssertions<ActionAssertions, TException>(
                capturedException,
                wasThrowAssertionSatisfied: true,
                throwFailureMessage: null,
                this,
                SubjectLabel(),
                because,
                AssertionFailureDispatcher.Fail);
        }

        object actual = capturedException is null
            ? NoExceptionToken.Instance
            : capturedException.GetType();

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to throw exactly", typeof(TException)),
            actual,
            because);
        var failureMessage = FailureMessageRenderer.Render(failure);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<ActionAssertions, TException>(
            capturedException,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail);
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
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

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

    private sealed class NoExceptionToken
    {
        public static NoExceptionToken Instance { get; } = new();

        public override string ToString()
        {
            return "<no exception>";
        }
    }
}
