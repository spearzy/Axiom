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
}
