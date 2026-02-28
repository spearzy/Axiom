using System.Runtime.CompilerServices;

namespace Axiom.Assertions;

public static class GenericShouldExtensions
{
    public static ValueAssertions<T> Should<T>(
        this T subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(subject, subjectExpression);
}
