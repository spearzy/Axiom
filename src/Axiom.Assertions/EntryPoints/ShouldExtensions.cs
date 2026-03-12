using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;

namespace Axiom.Assertions.EntryPoints;

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

    public static TaskAssertions Should(
        this Task subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(subject, subjectExpression);

    public static TaskAssertions<T> Should<T>(
        this Task<T> subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(subject, subjectExpression);

    public static TaskAssertions Should(
        this ValueTask subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
    {
        // Normalize once at the boundary so every later assertion observes the same Task instance.
        var task = subject.AsTask();
        return new(task, subjectExpression);
    }

    public static TaskAssertions<T> Should<T>(
        this ValueTask<T> subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
    {
        // ValueTask<T> may be single-consumption, so do not carry it deeper into the assertion graph.
        var task = subject.AsTask();
        return new(task, subjectExpression);
    }
}
