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
        // M3 behavior: fail immediately; M4 will route this through Batch when active.
        if (Subject is null)
        {
            throw new InvalidOperationException(
                $"Expected {SubjectLabel()} to not be null, but found <null>.");
        }

        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> StartWith(string expectedPrefix)
    {
        if (Subject is null)
        {
            throw new InvalidOperationException(
                $"Expected {SubjectLabel()} to start with \"{expectedPrefix}\", but found <null>.");
        }

        if (!Subject.StartsWith(expectedPrefix, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Expected {SubjectLabel()} to start with \"{expectedPrefix}\", but found \"{Subject}\".");
        }

        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> EndWith(string expectedSuffix)
    {
        if (Subject is null)
        {
            throw new InvalidOperationException(
                $"Expected {SubjectLabel()} to end with \"{expectedSuffix}\", but found <null>.");
        }

        if (!Subject.EndsWith(expectedSuffix, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Expected {SubjectLabel()} to end with \"{expectedSuffix}\", but found \"{Subject}\".");
        }

        return new AndContinuation<StringAssertions>(this);
    }

    private string SubjectLabel()
    {
        // Prefer captured expression text (e.g. "value") for clearer failure messages.
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }
}
