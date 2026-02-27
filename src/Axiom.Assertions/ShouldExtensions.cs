using System.Runtime.CompilerServices;

namespace Axiom;

public static class ShouldExtensions
{
    public static StringAssertions Should( this string? subject,[CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(subject, subjectExpression);
}
