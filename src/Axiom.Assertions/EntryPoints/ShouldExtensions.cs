using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;

namespace Axiom.Assertions;

public static class ShouldExtensions
{
    public static StringAssertions Should(
        this string? subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(subject, subjectExpression);

    public static ActionAssertions Should(
        this Action subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(subject, subjectExpression);

    public static AsyncActionAssertions Should(
        this Func<Task> subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(() => new ValueTask(subject()), subjectExpression);

    public static AsyncActionAssertions Should(
        this Func<ValueTask> subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(subject, subjectExpression);

    public static AsyncActionAssertions Should(
        this Task subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(() => new ValueTask(subject), subjectExpression);

    public static AsyncActionAssertions Should<T>(
        this Task<T> subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(() => new ValueTask(subject), subjectExpression);

    public static AsyncActionAssertions Should(
        this ValueTask subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
    {
        // A raw ValueTask can be unsafe to await repeatedly; Task is safe for repeated checks/chaining.
        var task = subject.AsTask();
        return new(() => new ValueTask(task), subjectExpression);
    }

    public static AsyncActionAssertions Should<T>(
        this ValueTask<T> subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
    {
        var task = subject.AsTask();
        return new(() => new ValueTask(task), subjectExpression);
    }
}
