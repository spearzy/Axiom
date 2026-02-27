namespace Axiom;

public sealed class StringAssertions
{
    public StringAssertions(string? subject, string? subjectExpression)
    {
        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    public string? Subject { get; }
    public string? SubjectExpression { get; }
}
