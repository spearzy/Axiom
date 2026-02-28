namespace Axiom.Assertions;

public sealed class ValueAssertions<T>
{
    public ValueAssertions(T subject, string? subjectExpression)
    {
        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    public T Subject { get; }
    public string? SubjectExpression { get; }

    public AndContinuation<ValueAssertions<T>> Be(T expected)
    {
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> NotBe(T unexpected)
    {
        return new AndContinuation<ValueAssertions<T>>(this);
    }
}
