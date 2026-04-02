namespace Axiom.Assertions.AssertionTypes;

public sealed partial class TaskAssertions(Task subject, string? subjectExpression)
{
    private Task<TaskOutcome>? _capturedOutcomeTask;

    public Task Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;
}

public sealed partial class TaskAssertions<TResult>(Task<TResult> subject, string? subjectExpression)
{
    private Task<TaskOutcome<TResult>>? _capturedOutcomeTask;

    public Task<TResult> Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;
}
