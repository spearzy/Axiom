using Axiom.Core;
using Axiom.Core.Failures;

namespace Axiom.Assertions;

public sealed class StringAssertions
{
    public string? Subject { get; }
    public string? SubjectExpression { get; }

    public StringAssertions(string? subject, string? subjectExpression)
    {
        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    public AndContinuation<StringAssertions> NotBeNull()
    {
        if (Subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be null", IncludeExpectedValue: false),
                Subject);
            Fail(FailureMessageRenderer.Render(failure));
        }

        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> StartWith(string expectedPrefix)
    {
        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to start with", expectedPrefix),
                subject);
            Fail(FailureMessageRenderer.Render(failure));
            return new AndContinuation<StringAssertions>(this);
        }

        if (!subject.StartsWith(expectedPrefix, StringComparison.Ordinal))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to start with", expectedPrefix),
                subject);
            Fail(FailureMessageRenderer.Render(failure));
        }

        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> EndWith(string expectedSuffix)
    {
        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to end with", expectedSuffix),
                subject);
            Fail(FailureMessageRenderer.Render(failure));
            return new AndContinuation<StringAssertions>(this);
        }

        if (!subject.EndsWith(expectedSuffix, StringComparison.Ordinal))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to end with", expectedSuffix),
                subject);
            Fail(FailureMessageRenderer.Render(failure));
        }

        return new AndContinuation<StringAssertions>(this);
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }

    private void Fail(string message)
    {
        var batch = Batch.Current;
        if (batch is not null)
        {
            batch.AddFailure(message);
            return;
        }

        throw new InvalidOperationException(message);
    }
}
