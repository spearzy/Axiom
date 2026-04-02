namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncFunctionAssertions<TResult>
{
    private Task<AsyncFunctionInvocation<TResult>>? _capturedInvocationTask;
    private Task<TaskOutcome<TResult>>? _capturedOutcomeTask;

    public AsyncFunctionAssertions(Func<ValueTask<TResult>> subject, string? subjectExpression)
    {
        ArgumentNullException.ThrowIfNull(subject);

        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    public Func<ValueTask<TResult>> Subject { get; }

    public string? SubjectExpression { get; }
}
