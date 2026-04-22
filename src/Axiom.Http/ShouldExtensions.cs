using System.Runtime.CompilerServices;

namespace Axiom.Http;

public static class ShouldExtensions
{
    public static HttpResponseAssertions Should(
        this HttpResponseMessage? subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(subject, subjectExpression);
}
