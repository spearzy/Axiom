using System.Diagnostics;
using System.Runtime.CompilerServices;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed class ThrownExceptionAssertions<TParent, TException>(
    Exception? capturedException,
    bool wasThrowAssertionSatisfied,
    string? throwFailureMessage,
    TParent parentAssertions,
    string subjectLabel,
    string? inheritedReason,
    Action<string, string?, int> fail,
    string baseAssertionName = "Throw")
    where TException : Exception
{
    public TParent And { get; } = parentAssertions;

    public Exception? Exception { get; } = capturedException;

    public TException Thrown
    {
        get
        {
            if (wasThrowAssertionSatisfied && Exception is TException typedException)
            {
                return typedException;
            }

            var message = throwFailureMessage is null
                ? $"Thrown is unavailable because {baseAssertionName} assertion failed."
                : $"Thrown is unavailable because {baseAssertionName} assertion failed with error: {throwFailureMessage}";
            AssertionFailureDispatcher.Throw(message);
            throw new UnreachableException();
        }
    }

    public ThrownExceptionAssertions<TParent, TException> WithMessage(
        string expectedMessage,
        StringComparison comparison = StringComparison.Ordinal,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedMessage);

        if (!wasThrowAssertionSatisfied || Exception is null)
        {
            // Base Throw*/ThrowExactly* assertion has already failed, so skip detail checks.
            return this;
        }

        if (!string.Equals(Exception.Message, expectedMessage, comparison))
        {
            var failure = new Failure(
                subjectLabel,
                new Expectation("to have exception message", expectedMessage),
                Exception.Message,
                ResolveReason(because));
            fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        return this;
    }

    public ThrownExceptionAssertions<TParent, TException> WithParamName(
        string expectedParamName,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedParamName);

        if (!wasThrowAssertionSatisfied || Exception is null)
        {
            return this;
        }

        if (Exception is not ArgumentException argumentException)
        {
            var wrongTypeFailure = new Failure(
                subjectLabel,
                new Expectation("to have parameter name", expectedParamName),
                Exception.GetType(),
                ResolveReason(because));
            fail(FailureMessageRenderer.Render(wrongTypeFailure), callerFilePath, callerLineNumber);
            return this;
        }

        if (!string.Equals(argumentException.ParamName, expectedParamName, StringComparison.Ordinal))
        {
            var failure = new Failure(
                subjectLabel,
                new Expectation("to have parameter name", expectedParamName),
                argumentException.ParamName,
                ResolveReason(because));
            fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        return this;
    }

    public ThrownExceptionAssertions<TParent, TException> WithInnerException<TInnerException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TInnerException : Exception
    {
        if (!wasThrowAssertionSatisfied || Exception is null)
        {
            return this;
        }

        if (Exception.InnerException is TInnerException)
        {
            return this;
        }

        object actual = Exception.InnerException is null
            ? NoInnerExceptionToken.Instance
            : Exception.InnerException.GetType();

        var failure = new Failure(
            subjectLabel,
            new Expectation("to have inner exception", typeof(TInnerException)),
            actual,
            ResolveReason(because));
        fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        return this;
    }

    private string? ResolveReason(string? because)
    {
        return string.IsNullOrWhiteSpace(because) ? inheritedReason : because;
    }

    private sealed class NoInnerExceptionToken
    {
        public static NoInnerExceptionToken Instance { get; } = new();

        public override string ToString()
        {
            return "<no inner exception>";
        }
    }
}
