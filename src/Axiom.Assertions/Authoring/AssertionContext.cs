using Axiom.Assertions.AssertionTypes;

namespace Axiom.Assertions.Authoring;

public static class AssertionContext
{
    public static AssertionContext<ValueAssertions<T>, T> Create<T>(ValueAssertions<T> assertions)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        return new AssertionContext<ValueAssertions<T>, T>(
            assertions,
            assertions.Subject,
            ResolveSubjectLabel(assertions.SubjectExpression));
    }

    private static string ResolveSubjectLabel(string? subjectExpression)
    {
        return string.IsNullOrWhiteSpace(subjectExpression) ? "<subject>" : subjectExpression;
    }
}
