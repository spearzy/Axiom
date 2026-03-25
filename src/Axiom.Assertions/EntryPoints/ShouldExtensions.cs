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

    public static AsyncFunctionAssertions<T> Should<T>(
        this Func<Task<T>> subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(() => new ValueTask<T>(subject()), subjectExpression);

    public static AsyncFunctionAssertions<T> Should<T>(
        this Func<ValueTask<T>> subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(subject, subjectExpression);

    public static AsyncEnumerableAssertions<T> Should<T>(
        this IAsyncEnumerable<T>? subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => CreateAsyncEnumerableAssertions(subject, subjectExpression);

    public static AsyncEnumerableAssertions<T> ShouldAsyncEnumerable<T>(
        this IAsyncEnumerable<T>? subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => CreateAsyncEnumerableAssertions(subject, subjectExpression);

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

    private static AsyncEnumerableAssertions<T> CreateAsyncEnumerableAssertions<T>(
        IAsyncEnumerable<T>? subject,
        string? subjectExpression)
        => new(subject, subjectExpression);
}
