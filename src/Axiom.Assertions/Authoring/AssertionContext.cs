using Axiom.Assertions.AssertionTypes;

namespace Axiom.Assertions.Authoring;

public static class AssertionContext
{
    public static AssertionContext<ValueAssertions<T>, T> Create<T>(ValueAssertions<T> assertions)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        return CreateCore(assertions, assertions.Subject, assertions.SubjectExpression);
    }

    public static AssertionContext<StringAssertions, string?> Create(StringAssertions assertions)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        return CreateCore(assertions, assertions.Subject, assertions.SubjectExpression);
    }

    private static AssertionContext<TAssertions, TSubject> CreateCore<TAssertions, TSubject>(
        TAssertions assertions,
        TSubject subject,
        string? subjectExpression)
    {
        return new AssertionContext<TAssertions, TSubject>(assertions, subject, ResolveSubjectLabel(subjectExpression));
    }

    private static string ResolveSubjectLabel(string? subjectExpression)
    {
        return string.IsNullOrWhiteSpace(subjectExpression) ? "<subject>" : subjectExpression;
    }
}
