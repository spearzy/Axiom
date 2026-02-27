namespace Axiom;

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
            Fail($"Expected {SubjectLabel()} to not be null, but found <null>.");

        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> StartWith(string expectedPrefix)
    {
        var subject = Subject;
        if (subject is null)
        {
            Fail($"Expected {SubjectLabel()} to start with \"{expectedPrefix}\", but found <null>.");
            return new AndContinuation<StringAssertions>(this);
        }

        if (!subject.StartsWith(expectedPrefix, StringComparison.Ordinal))
            Fail($"Expected {SubjectLabel()} to start with \"{expectedPrefix}\", but found \"{subject}\".");

        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> EndWith(string expectedSuffix)
    {
        var subject = Subject;
        if (subject is null) {
            Fail($"Expected {SubjectLabel()} to end with \"{expectedSuffix}\", but found <null>.");
            return new AndContinuation<StringAssertions>(this);
        }

        if (!subject.EndsWith(expectedSuffix, StringComparison.Ordinal))
            Fail($"Expected {SubjectLabel()} to end with \"{expectedSuffix}\", but found \"{subject}\".");

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
