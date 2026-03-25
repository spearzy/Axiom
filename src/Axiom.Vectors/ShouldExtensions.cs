using System.Runtime.CompilerServices;

namespace Axiom.Vectors;

public static class ShouldExtensions
{
    public static VectorAssertions<float> Should(
        this float[]? subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => CreateAssertions(subject, subjectExpression);

    public static VectorAssertions<double> Should(
        this double[]? subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => CreateAssertions(subject, subjectExpression);

    public static VectorAssertions<float> Should(
        this ReadOnlyMemory<float> subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(subject, subjectExpression, hasSubject: true);

    public static VectorAssertions<double> Should(
        this ReadOnlyMemory<double> subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(subject, subjectExpression, hasSubject: true);

    private static VectorAssertions<float> CreateAssertions(float[]? subject, string? subjectExpression)
        => new(subject ?? Array.Empty<float>(), subjectExpression, hasSubject: subject is not null);

    private static VectorAssertions<double> CreateAssertions(double[]? subject, string? subjectExpression)
        => new(subject ?? Array.Empty<double>(), subjectExpression, hasSubject: subject is not null);
}
