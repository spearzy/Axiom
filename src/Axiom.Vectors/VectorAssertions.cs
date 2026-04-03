using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;
using Axiom.Core.Formatting;

namespace Axiom.Vectors;

public sealed class VectorAssertions<TNumeric>
    where TNumeric : struct, IFloatingPointIeee754<TNumeric>
{
    private static readonly IValueFormatter Formatter = DefaultValueFormatter.Instance;
    private readonly bool _hasSubject;

    internal VectorAssertions(ReadOnlyMemory<TNumeric> subject, string? subjectExpression, bool hasSubject)
    {
        Subject = subject;
        SubjectExpression = subjectExpression;
        _hasSubject = hasSubject;
    }

    public ReadOnlyMemory<TNumeric> Subject { get; }

    public string? SubjectExpression { get; }

    internal string SubjectLabel => string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;

    public AndContinuation<VectorAssertions<TNumeric>> HaveDimension(
        int expectedDimension,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (expectedDimension < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedDimension), "expectedDimension must be non-negative.");
        }

        if (!_hasSubject)
        {
            Fail(
                BuildFailureMessage(SubjectLabel, $"to have dimension {expectedDimension}", "found <null>", because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<VectorAssertions<TNumeric>>(this);
        }

        if (Subject.Length != expectedDimension)
        {
            Fail(
                BuildFailureMessage(SubjectLabel, $"to have dimension {expectedDimension}", $"found dimension {Subject.Length}", because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<VectorAssertions<TNumeric>>(this);
    }

    public AndContinuation<VectorAssertions<TNumeric>> NotContainNaNOrInfinity(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!_hasSubject)
        {
            Fail(
                BuildFailureMessage(SubjectLabel, "to not contain NaN or Infinity", "found <null>", because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<VectorAssertions<TNumeric>>(this);
        }

        var span = Subject.Span;
        for (var index = 0; index < span.Length; index++)
        {
            var value = span[index];
            if (TNumeric.IsNaN(value))
            {
                Fail(
                    BuildFailureMessage(SubjectLabel, "to not contain NaN or Infinity", $"found NaN at index {index}", because),
                    callerFilePath,
                    callerLineNumber);
                break;
            }

            if (TNumeric.IsInfinity(value))
            {
                Fail(
                    BuildFailureMessage(SubjectLabel, "to not contain NaN or Infinity", $"found {FormatValue(value)} at index {index}", because),
                    callerFilePath,
                    callerLineNumber);
                break;
            }
        }

        return new AndContinuation<VectorAssertions<TNumeric>>(this);
    }

    public AndContinuation<VectorAssertions<TNumeric>> BeApproximatelyEqualTo(
        TNumeric[] expected,
        TNumeric tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expected);
        return BeApproximatelyEqualTo((ReadOnlyMemory<TNumeric>)expected, tolerance, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<VectorAssertions<TNumeric>> BeApproximatelyEqualTo(
        ReadOnlyMemory<TNumeric> expected,
        TNumeric tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ValidateTolerance(tolerance);

        if (!_hasSubject)
        {
            Fail(
                BuildFailureMessage(
                    SubjectLabel,
                    $"to be approximately equal to expected within tolerance {FormatNumeric(tolerance)}",
                    "found <null>",
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<VectorAssertions<TNumeric>>(this);
        }

        if (Subject.Length != expected.Length)
        {
            Fail(
                BuildFailureMessage(
                    SubjectLabel,
                    $"to be approximately equal to expected within tolerance {FormatNumeric(tolerance)}",
                    BuildDimensionMismatchDetail(expected.Length, Subject.Length),
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<VectorAssertions<TNumeric>>(this);
        }

        var actualSpan = Subject.Span;
        var expectedSpan = expected.Span;
        for (var index = 0; index < actualSpan.Length; index++)
        {
            var actual = actualSpan[index];
            var expectedValue = expectedSpan[index];
            if (actual == expectedValue)
            {
                continue;
            }

            var delta = TNumeric.Abs(actual - expectedValue);
            if (TNumeric.IsNaN(delta) || delta > tolerance)
            {
                Fail(
                    BuildFailureMessage(
                        SubjectLabel,
                        $"to be approximately equal to expected within tolerance {FormatNumeric(tolerance)}",
                        $"index {index} differed: expected {FormatValue(expectedValue)}, found {FormatValue(actual)}, delta {FormatValue(delta)}",
                        because),
                    callerFilePath,
                    callerLineNumber);
                break;
            }
        }

        return new AndContinuation<VectorAssertions<TNumeric>>(this);
    }

    public CosineSimilarityAssertions<TNumeric> HaveCosineSimilarityWith(TNumeric[] expected)
    {
        ArgumentNullException.ThrowIfNull(expected);
        return HaveCosineSimilarityWith((ReadOnlyMemory<TNumeric>)expected);
    }

    public CosineSimilarityAssertions<TNumeric> HaveCosineSimilarityWith(ReadOnlyMemory<TNumeric> expected)
        => CreateCosineSimilarityAssertions(expected);

    public CosineSimilarityAssertions<TNumeric> HaveCosineSimilarityTo(TNumeric[] expected)
        => HaveCosineSimilarityWith(expected);

    public CosineSimilarityAssertions<TNumeric> HaveCosineSimilarityTo(ReadOnlyMemory<TNumeric> expected)
        => HaveCosineSimilarityWith(expected);

    public AndContinuation<VectorAssertions<TNumeric>> HaveDotProductWith(
        TNumeric[] expected,
        TNumeric expectedDotProduct,
        TNumeric tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expected);
        return HaveDotProductWith((ReadOnlyMemory<TNumeric>)expected, expectedDotProduct, tolerance, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<VectorAssertions<TNumeric>> HaveDotProductWith(
        ReadOnlyMemory<TNumeric> expected,
        TNumeric expectedDotProduct,
        TNumeric tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ValidateFiniteMetric(expectedDotProduct, nameof(expectedDotProduct));
        ValidateTolerance(tolerance);

        var expectation =
            $"to have dot product with expected equal to {FormatNumeric(expectedDotProduct)} within tolerance {FormatNumeric(tolerance)}";

        if (!_hasSubject)
        {
            Fail(
                BuildFailureMessage(SubjectLabel, expectation, "found <null>", because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<VectorAssertions<TNumeric>>(this);
        }

        if (Subject.Length != expected.Length)
        {
            Fail(
                BuildFailureMessage(
                    SubjectLabel,
                    expectation,
                    BuildDimensionMismatchDetail(expected.Length, Subject.Length),
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<VectorAssertions<TNumeric>>(this);
        }

        var dotProduct = ComputeDotProduct(Subject.Span, expected.Span);
        var delta = TNumeric.Abs(dotProduct - expectedDotProduct);
        if (TNumeric.IsNaN(delta) || delta > tolerance)
        {
            Fail(
                BuildFailureMessage(
                    SubjectLabel,
                    expectation,
                    BuildMetricMismatchDetail("dot product", expectedDotProduct, dotProduct, delta),
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<VectorAssertions<TNumeric>>(this);
    }

    public AndContinuation<VectorAssertions<TNumeric>> HaveEuclideanDistanceTo(
        TNumeric[] expected,
        TNumeric expectedDistance,
        TNumeric tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expected);
        return HaveEuclideanDistanceTo((ReadOnlyMemory<TNumeric>)expected, expectedDistance, tolerance, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<VectorAssertions<TNumeric>> HaveEuclideanDistanceTo(
        ReadOnlyMemory<TNumeric> expected,
        TNumeric expectedDistance,
        TNumeric tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ValidateNonNegativeFiniteMetric(expectedDistance, nameof(expectedDistance));
        ValidateTolerance(tolerance);

        var expectation =
            $"to have Euclidean distance to expected equal to {FormatNumeric(expectedDistance)} within tolerance {FormatNumeric(tolerance)}";

        if (!_hasSubject)
        {
            Fail(
                BuildFailureMessage(SubjectLabel, expectation, "found <null>", because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<VectorAssertions<TNumeric>>(this);
        }

        if (Subject.Length != expected.Length)
        {
            Fail(
                BuildFailureMessage(
                    SubjectLabel,
                    expectation,
                    BuildDimensionMismatchDetail(expected.Length, Subject.Length),
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<VectorAssertions<TNumeric>>(this);
        }

        var distance = ComputeEuclideanDistance(Subject.Span, expected.Span);
        var delta = TNumeric.Abs(distance - expectedDistance);
        if (TNumeric.IsNaN(delta) || delta > tolerance)
        {
            Fail(
                BuildFailureMessage(
                    SubjectLabel,
                    expectation,
                    BuildMetricMismatchDetail("Euclidean distance", expectedDistance, distance, delta),
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<VectorAssertions<TNumeric>>(this);
    }

    private CosineSimilarityAssertions<TNumeric> CreateCosineSimilarityAssertions(ReadOnlyMemory<TNumeric> expected)
    {
        if (!_hasSubject)
        {
            return new CosineSimilarityAssertions<TNumeric>(this, hasSimilarity: false, similarity: TNumeric.Zero, unavailableActualDetail: "found <null>");
        }

        if (Subject.Length != expected.Length)
        {
            return new CosineSimilarityAssertions<TNumeric>(
                this,
                hasSimilarity: false,
                similarity: TNumeric.Zero,
                unavailableActualDetail: BuildDimensionMismatchDetail(expected.Length, Subject.Length));
        }

        var actualSpan = Subject.Span;
        var expectedSpan = expected.Span;

        var dot = TNumeric.Zero;
        var actualMagnitudeSquared = TNumeric.Zero;
        var expectedMagnitudeSquared = TNumeric.Zero;

        for (var index = 0; index < actualSpan.Length; index++)
        {
            var actual = actualSpan[index];
            var expectedValue = expectedSpan[index];

            dot += actual * expectedValue;
            actualMagnitudeSquared += actual * actual;
            expectedMagnitudeSquared += expectedValue * expectedValue;
        }

        if (actualMagnitudeSquared == TNumeric.Zero && expectedMagnitudeSquared == TNumeric.Zero)
        {
            return new CosineSimilarityAssertions<TNumeric>(
                this,
                hasSimilarity: false,
                similarity: TNumeric.Zero,
                unavailableActualDetail: "actual and expected vectors both have zero magnitude");
        }

        if (actualMagnitudeSquared == TNumeric.Zero)
        {
            return new CosineSimilarityAssertions<TNumeric>(
                this,
                hasSimilarity: false,
                similarity: TNumeric.Zero,
                unavailableActualDetail: "actual vector has zero magnitude");
        }

        if (expectedMagnitudeSquared == TNumeric.Zero)
        {
            return new CosineSimilarityAssertions<TNumeric>(
                this,
                hasSimilarity: false,
                similarity: TNumeric.Zero,
                unavailableActualDetail: "expected vector has zero magnitude");
        }

        var similarity = dot / (TNumeric.Sqrt(actualMagnitudeSquared) * TNumeric.Sqrt(expectedMagnitudeSquared));
        if (!TNumeric.IsFinite(similarity))
        {
            return new CosineSimilarityAssertions<TNumeric>(
                this,
                hasSimilarity: false,
                similarity: TNumeric.Zero,
                unavailableActualDetail: $"computed non-finite cosine similarity {FormatValue(similarity)}");
        }

        return new CosineSimilarityAssertions<TNumeric>(this, hasSimilarity: true, similarity, unavailableActualDetail: null);
    }

    public AndContinuation<VectorAssertions<TNumeric>> BeNormalized(
        TNumeric tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ValidateTolerance(tolerance);

        if (!_hasSubject)
        {
            Fail(
                BuildFailureMessage(
                    SubjectLabel,
                    $"to be normalized within tolerance {FormatNumeric(tolerance)}",
                    "found <null>",
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<VectorAssertions<TNumeric>>(this);
        }

        var norm = ComputeNorm(Subject.Span);
        if (TNumeric.IsNaN(norm) || TAbs(norm - TNumeric.One) > tolerance)
        {
            Fail(
                BuildFailureMessage(
                    SubjectLabel,
                    $"to be normalized within tolerance {FormatNumeric(tolerance)}",
                    $"computed L2 norm {FormatValue(norm)}",
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<VectorAssertions<TNumeric>>(this);
    }

    public AndContinuation<VectorAssertions<TNumeric>> BeZeroVector(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!_hasSubject)
        {
            Fail(
                BuildFailureMessage(SubjectLabel, "to be a zero vector", "found <null>", because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<VectorAssertions<TNumeric>>(this);
        }

        var span = Subject.Span;
        for (var index = 0; index < span.Length; index++)
        {
            var value = span[index];
            if (value != TNumeric.Zero)
            {
                Fail(
                    BuildFailureMessage(
                        SubjectLabel,
                        "to be a zero vector",
                        $"index {index} differed: expected 0, found {FormatValue(value)}",
                        because),
                    callerFilePath,
                    callerLineNumber);
                break;
            }
        }

        return new AndContinuation<VectorAssertions<TNumeric>>(this);
    }

    public AndContinuation<VectorAssertions<TNumeric>> NotBeZeroVector(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!_hasSubject)
        {
            Fail(
                BuildFailureMessage(SubjectLabel, "to not be a zero vector", "found <null>", because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<VectorAssertions<TNumeric>>(this);
        }

        var span = Subject.Span;
        for (var index = 0; index < span.Length; index++)
        {
            if (span[index] != TNumeric.Zero)
            {
                return new AndContinuation<VectorAssertions<TNumeric>>(this);
            }
        }

        Fail(
            BuildFailureMessage(
                SubjectLabel,
                "to not be a zero vector",
                BuildAllZeroVectorDetail(span.Length),
                because),
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<VectorAssertions<TNumeric>>(this);
    }

    public AndContinuation<VectorAssertions<TNumeric>> BeNormalized(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var tolerance = TNumeric.CreateChecked(1e-5);
        return BeNormalized(tolerance, because, callerFilePath, callerLineNumber);
    }

    internal void Fail(string message, string? callerFilePath, int callerLineNumber)
    {
        AssertionFailureDispatcher.Fail(message, callerFilePath, callerLineNumber);
    }

    internal string BuildCosineSimilarityUnavailableMessage(string? unavailableActualDetail)
        => BuildFailureMessage(
            SubjectLabel,
            "to have cosine similarity with expected",
            unavailableActualDetail ?? "cosine similarity could not be computed",
            because: null);

    internal string FormatNumeric(TNumeric value)
        => value.ToString(null, CultureInfo.InvariantCulture);

    internal static void ValidateSimilarityThreshold(TNumeric threshold)
    {
        if (!TNumeric.IsFinite(threshold) || threshold < -TNumeric.One || threshold > TNumeric.One)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold), "threshold must be a finite value between -1 and 1.");
        }
    }

    internal static void ValidateSimilarityRange(TNumeric minimumThreshold, TNumeric maximumThreshold)
    {
        if (!TNumeric.IsFinite(minimumThreshold) || minimumThreshold < -TNumeric.One || minimumThreshold > TNumeric.One)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumThreshold), "minimumThreshold must be a finite value between -1 and 1.");
        }

        if (!TNumeric.IsFinite(maximumThreshold) || maximumThreshold < -TNumeric.One || maximumThreshold > TNumeric.One)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumThreshold), "maximumThreshold must be a finite value between -1 and 1.");
        }

        if (minimumThreshold > maximumThreshold)
        {
            throw new ArgumentException("minimumThreshold must be less than or equal to maximumThreshold.", nameof(minimumThreshold));
        }
    }

    internal static string BuildFailureMessage(string subjectLabel, string expectation, string actualDetail, string? because)
        => $"Expected {subjectLabel} {expectation}{BuildReasonClause(because)}, but {actualDetail}.";

    private static string BuildDimensionMismatchDetail(int expectedLength, int actualLength)
        => $"dimensions differed: expected {expectedLength}, found {actualLength}";

    private static string BuildMetricMismatchDetail(string metricName, TNumeric expectedValue, TNumeric actualValue, TNumeric delta)
        => $"{metricName} differed: expected {FormatValue(expectedValue)}, found {FormatValue(actualValue)}, delta {FormatValue(delta)}";

    private static string BuildAllZeroVectorDetail(int length)
        => length == 0
            ? "vector had no components and was zero by definition"
            : $"all {length} component(s) were zero";

    private static string BuildReasonClause(string? because)
        => string.IsNullOrWhiteSpace(because) ? string.Empty : $" because {because.Trim()}";

    private static void ValidateTolerance(TNumeric tolerance)
    {
        if (!TNumeric.IsFinite(tolerance) || tolerance < TNumeric.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance must be a finite value greater than or equal to zero.");
        }
    }

    private static void ValidateFiniteMetric(TNumeric value, string paramName)
    {
        if (!TNumeric.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be a finite value.");
        }
    }

    private static void ValidateNonNegativeFiniteMetric(TNumeric value, string paramName)
    {
        if (!TNumeric.IsFinite(value) || value < TNumeric.Zero)
        {
            throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be a finite value greater than or equal to zero.");
        }
    }

    private static TNumeric ComputeDotProduct(ReadOnlySpan<TNumeric> left, ReadOnlySpan<TNumeric> right)
    {
        var dotProduct = TNumeric.Zero;
        for (var index = 0; index < left.Length; index++)
        {
            dotProduct += left[index] * right[index];
        }

        return dotProduct;
    }

    private static TNumeric ComputeEuclideanDistance(ReadOnlySpan<TNumeric> left, ReadOnlySpan<TNumeric> right)
    {
        var sumOfSquares = TNumeric.Zero;
        for (var index = 0; index < left.Length; index++)
        {
            var delta = left[index] - right[index];
            sumOfSquares += delta * delta;
        }

        return TNumeric.Sqrt(sumOfSquares);
    }

    private static TNumeric ComputeNorm(ReadOnlySpan<TNumeric> values)
    {
        var sumOfSquares = TNumeric.Zero;
        for (var index = 0; index < values.Length; index++)
        {
            var value = values[index];
            sumOfSquares += value * value;
        }

        return TNumeric.Sqrt(sumOfSquares);
    }

    private static TNumeric TAbs(TNumeric value) => TNumeric.Abs(value);

    private static string FormatValue(object? value) => Formatter.Format(value);
}
