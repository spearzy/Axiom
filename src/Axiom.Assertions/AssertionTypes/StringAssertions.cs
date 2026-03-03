using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core;
using Axiom.Core.Failures;
using Axiom.Core.Output;

namespace Axiom.Assertions.AssertionTypes;

public sealed class StringAssertions(string? subject, string? subjectExpression)
{
    public string? Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;

    public AndContinuation<StringAssertions> NotBeNull(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (Subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be null", IncludeExpectedValue: false),
                Subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(NotBeNull), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> StartWith(
        string expectedPrefix,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to start with", expectedPrefix),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (!subject.StartsWith(expectedPrefix, StringComparison.Ordinal))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to start with", expectedPrefix),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(StartWith), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> EndWith(
        string expectedSuffix,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to end with", expectedSuffix),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (!subject.EndsWith(expectedSuffix, StringComparison.Ordinal))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to end with", expectedSuffix),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(EndWith), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> Contain(
        string expectedSubstring,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to contain", expectedSubstring),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (!subject.Contains(expectedSubstring, StringComparison.Ordinal))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to contain", expectedSubstring),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(Contain), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> NotContain(
        string unexpectedSubstring,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not contain", unexpectedSubstring),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (subject.Contains(unexpectedSubstring, StringComparison.Ordinal))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not contain", unexpectedSubstring),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(NotContain), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> HaveLength(
        int expectedLength,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (expectedLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedLength), "expectedLength must be non-negative.");
        }

        var subject = Subject;
        if (subject is null || subject.Length != expectedLength)
        {
            object? actual = subject is null ? null : subject.Length;
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to have length", expectedLength),
                actual,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(HaveLength), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> BeEmpty(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null || subject.Length != 0)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be empty", IncludeExpectedValue: false),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(BeEmpty), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> NotBeEmpty(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (string.IsNullOrEmpty(subject))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be empty", IncludeExpectedValue: false),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(NotBeEmpty), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<StringAssertions>(this);
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }

    private void Fail(string message, string? callerFilePath, int callerLineNumber)
    {
        AssertionOutputWriter.ReportFailure(message, callerFilePath, callerLineNumber);

        var batch = Batch.Current;
        if (batch is not null)
        {
            batch.AddFailure(message);
            return;
        }

        throw new InvalidOperationException(message);
    }
}
