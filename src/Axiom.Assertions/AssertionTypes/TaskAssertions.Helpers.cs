using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class TaskAssertions
{
    private Task<TaskOutcome> GetOutcomeAsync()
    {
        return _capturedOutcomeTask ??= TaskAssertionHelpers.CaptureOutcomeAsync(Subject);
    }

    private ThrownExceptionAssertions<TaskAssertions, TException> CreateFaultContinuation<TException>(
        TaskOutcome outcome,
        Expectation expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        where TException : Exception
    {
        if (outcome.Status == TaskStatus.Faulted && outcome.Exception is TException typedException)
        {
            return new ThrownExceptionAssertions<TaskAssertions, TException>(
                typedException,
                wasThrowAssertionSatisfied: true,
                throwFailureMessage: null,
                this,
                SubjectLabel(),
                because,
                AssertionFailureDispatcher.Fail,
                baseAssertionName: "BeFaultedWith");
        }

        var failureMessage = TaskAssertionHelpers.RenderFailure(
            SubjectLabel(),
            expectation,
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<TaskAssertions, TException>(
            outcome.Exception,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail,
            baseAssertionName: "BeFaultedWith");
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }
}

public sealed partial class TaskAssertions<TResult>
{
    private Task<TaskOutcome<TResult>> GetOutcomeAsync()
    {
        return _capturedOutcomeTask ??= TaskAssertionHelpers.CaptureOutcomeAsync(Subject);
    }

    private ThrownExceptionAssertions<TaskAssertions<TResult>, TException> CreateFaultContinuation<TException>(
        TaskOutcome<TResult> outcome,
        Expectation expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        where TException : Exception
    {
        if (outcome.Status == TaskStatus.Faulted && outcome.Exception is TException typedException)
        {
            return new ThrownExceptionAssertions<TaskAssertions<TResult>, TException>(
                typedException,
                wasThrowAssertionSatisfied: true,
                throwFailureMessage: null,
                this,
                SubjectLabel(),
                because,
                AssertionFailureDispatcher.Fail,
                baseAssertionName: "BeFaultedWith");
        }

        var failureMessage = TaskAssertionHelpers.RenderFailure(
            SubjectLabel(),
            expectation,
            TaskAssertionHelpers.DescribeOutcome(outcome),
            because);
        AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);

        return new ThrownExceptionAssertions<TaskAssertions<TResult>, TException>(
            outcome.Exception,
            wasThrowAssertionSatisfied: false,
            throwFailureMessage: failureMessage,
            this,
            SubjectLabel(),
            because,
            AssertionFailureDispatcher.Fail,
            baseAssertionName: "BeFaultedWith");
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }
}

internal readonly record struct TaskOutcome(TaskStatus Status, Exception? Exception);

internal readonly record struct TaskOutcome<TResult>(TaskStatus Status, Exception? Exception, TResult? Result);

internal static class TaskAssertionHelpers
{
    public static void ValidateTimeout(TimeSpan timeout)
    {
        if (timeout < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "timeout must be zero or greater.");
        }
    }

    public static async ValueTask<bool> CompletesWithinAsync(Task subject, TimeSpan timeout)
    {
        var timeoutTask = Task.Delay(timeout);

        var completedTask = await Task.WhenAny(subject, timeoutTask).ConfigureAwait(false);
        if (!ReferenceEquals(completedTask, subject))
        {
            return false;
        }

        try
        {
            await subject.ConfigureAwait(false);
        }
        catch
        {
            // Completion checks only care whether the task finished inside the timeout window.
        }

        return true;
    }

    public static Task<TaskOutcome> CaptureOutcomeAsync(Task task)
    {
        return CaptureOutcomeCoreAsync(task);
    }

    public static Task<TaskOutcome<TResult>> CaptureOutcomeAsync<TResult>(Task<TResult> task)
    {
        return CaptureOutcomeCoreAsync(task);
    }

    public static string RenderFailure(string subject, Expectation expectation, object actual, string? because)
    {
        return FailureMessageRenderer.Render(new Failure(subject, expectation, actual, because));
    }

    public static void Fail(
        string subject,
        Expectation expectation,
        object actual,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        AssertionFailureDispatcher.Fail(
            RenderFailure(subject, expectation, actual, because),
            callerFilePath,
            callerLineNumber);
    }

    public static object DescribeOutcome(TaskOutcome outcome)
    {
        return outcome.Status switch
        {
            TaskStatus.RanToCompletion => TaskAssertionTokens.CompletedSuccessfully,
            TaskStatus.Canceled => TaskAssertionTokens.Canceled,
            _ when outcome.Exception is not null => outcome.Exception.GetType(),
            _ => TaskAssertionTokens.UnknownOutcome
        };
    }

    public static object DescribeOutcome<TResult>(TaskOutcome<TResult> outcome)
    {
        return outcome.Status switch
        {
            TaskStatus.RanToCompletion => TaskAssertionTokens.CompletedSuccessfully,
            TaskStatus.Canceled => TaskAssertionTokens.Canceled,
            _ when outcome.Exception is not null => outcome.Exception.GetType(),
            _ => TaskAssertionTokens.UnknownOutcome
        };
    }

    private static async Task<TaskOutcome> CaptureOutcomeCoreAsync(Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
            return new TaskOutcome(TaskStatus.RanToCompletion, null);
        }
        catch (Exception ex)
        {
            return new TaskOutcome(task.Status, ex);
        }
    }

    private static async Task<TaskOutcome<TResult>> CaptureOutcomeCoreAsync<TResult>(Task<TResult> task)
    {
        try
        {
            var result = await task.ConfigureAwait(false);
            return new TaskOutcome<TResult>(TaskStatus.RanToCompletion, null, result);
        }
        catch (Exception ex)
        {
            return new TaskOutcome<TResult>(task.Status, ex, default);
        }
    }
}

internal static class TaskAssertionTokens
{
    public static object NoException { get; } = new NoExceptionToken();
    public static object NotCompletedInTime { get; } = new NotCompletedInTimeToken();
    public static object CompletedInTime { get; } = new CompletedInTimeToken();
    public static object CompletedSuccessfully { get; } = new CompletedSuccessfullyToken();
    public static object Canceled { get; } = new CanceledToken();
    public static object UnknownOutcome { get; } = new UnknownOutcomeToken();

    private sealed class NoExceptionToken
    {
        public override string ToString()
        {
            return "<no exception>";
        }
    }

    private sealed class NotCompletedInTimeToken
    {
        public override string ToString()
        {
            return "<not completed within timeout>";
        }
    }

    private sealed class CompletedInTimeToken
    {
        public override string ToString()
        {
            return "<completed within timeout>";
        }
    }

    private sealed class CompletedSuccessfullyToken
    {
        public override string ToString()
        {
            return "<completed successfully>";
        }
    }

    private sealed class CanceledToken
    {
        public override string ToString()
        {
            return "<canceled>";
        }
    }

    private sealed class UnknownOutcomeToken
    {
        public override string ToString()
        {
            return "<unknown task outcome>";
        }
    }
}
