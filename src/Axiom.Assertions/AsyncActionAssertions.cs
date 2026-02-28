using System.Runtime.CompilerServices;
using Axiom.Core;
using Axiom.Core.Failures;
using Axiom.Core.Output;

namespace Axiom.Assertions;

public sealed class AsyncActionAssertions
{
    public AsyncActionAssertions(Func<ValueTask> subject, string? subjectExpression)
    {
        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    public Func<ValueTask> Subject { get; }
    public string? SubjectExpression { get; }

    public async ValueTask<AndContinuation<AsyncActionAssertions>> ThrowAsync<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        Exception? capturedException = null;
        try
        {
            await Subject().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            capturedException = ex;
        }

        if (capturedException is TException)
        {
            AssertionOutputWriter.ReportPass(nameof(ThrowAsync), SubjectLabel(), callerFilePath, callerLineNumber);
            return new AndContinuation<AsyncActionAssertions>(this);
        }

        object actual = capturedException is null
            ? NoExceptionToken.Instance
            : capturedException.GetType();

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to throw", typeof(TException)),
            actual,
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new AndContinuation<AsyncActionAssertions>(this);
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }

    private static void Fail(string message, string? callerFilePath, int callerLineNumber)
    {
        AssertionOutputWriter.ReportFailure(message, callerFilePath, callerLineNumber);

        var batch = Batch.Current;
        if (batch is not null)
        {
            // Keep going inside a batch; root dispose raises one combined exception.
            batch.AddFailure(message);
            return;
        }

        throw new InvalidOperationException(message);
    }

    private sealed class NoExceptionToken
    {
        public static NoExceptionToken Instance { get; } = new();

        public override string ToString()
        {
            return "<no exception>";
        }
    }
}
