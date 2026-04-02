namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncEnumerableAssertions<T>(IAsyncEnumerable<T>? subject, string? subjectExpression)
{
    public IAsyncEnumerable<T>? Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;
}
