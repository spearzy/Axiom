using System.Globalization;
using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.Extensions;

public static class NumericValueAssertionExtensions
{
    public static AndContinuation<ValueAssertions<double>> BeApproximately(
        this ValueAssertions<double> assertions,
        double expected,
        double tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ValidateTolerance(tolerance);

        if (!AreApproximatelyEqual(assertions.Subject, expected, tolerance))
        {
            var failure = new Failure(
                SubjectLabel(assertions.SubjectExpression),
                new Expectation($"to be within {tolerance.ToString(CultureInfo.InvariantCulture)} of", expected),
                assertions.Subject,
                because);

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<double>>(assertions);
    }

    public static AndContinuation<ValueAssertions<float>> BeApproximately(
        this ValueAssertions<float> assertions,
        float expected,
        float tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ValidateTolerance(tolerance);

        if (!AreApproximatelyEqual(assertions.Subject, expected, tolerance))
        {
            var failure = new Failure(
                SubjectLabel(assertions.SubjectExpression),
                new Expectation($"to be within {tolerance.ToString(CultureInfo.InvariantCulture)} of", expected),
                assertions.Subject,
                because);

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<float>>(assertions);
    }

    public static AndContinuation<ValueAssertions<decimal>> BeApproximately(
        this ValueAssertions<decimal> assertions,
        decimal expected,
        decimal tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ValidateTolerance(tolerance);

        if (!AreApproximatelyEqual(assertions.Subject, expected, tolerance))
        {
            var failure = new Failure(
                SubjectLabel(assertions.SubjectExpression),
                new Expectation($"to be within {tolerance.ToString(CultureInfo.InvariantCulture)} of", expected),
                assertions.Subject,
                because);

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<decimal>>(assertions);
    }

    private static string SubjectLabel(string? subjectExpression)
    {
        return string.IsNullOrWhiteSpace(subjectExpression) ? "<subject>" : subjectExpression;
    }

    private static bool AreApproximatelyEqual(double actual, double expected, double tolerance)
    {
        // Fast path covers exact equality, including NaN==NaN and matching infinities.
        if (actual.Equals(expected))
        {
            return true;
        }

        if (!double.IsFinite(actual) || !double.IsFinite(expected))
        {
            return false;
        }

        return Math.Abs(actual - expected) <= tolerance;
    }

    private static bool AreApproximatelyEqual(float actual, float expected, float tolerance)
    {
        // Fast path covers exact equality, including NaN==NaN and matching infinities.
        if (actual.Equals(expected))
        {
            return true;
        }

        if (!float.IsFinite(actual) || !float.IsFinite(expected))
        {
            return false;
        }

        return MathF.Abs(actual - expected) <= tolerance;
    }

    private static bool AreApproximatelyEqual(decimal actual, decimal expected, decimal tolerance)
    {
        if (actual == expected)
        {
            return true;
        }

        try
        {
            return decimal.Abs(actual - expected) <= tolerance;
        }
        catch (OverflowException)
        {
            return false;
        }
    }

    private static void ValidateTolerance(double tolerance)
    {
        if (double.IsNaN(tolerance) || double.IsInfinity(tolerance) || tolerance < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tolerance),
                "Tolerance must be a finite value greater than or equal to zero.");
        }
    }

    private static void ValidateTolerance(float tolerance)
    {
        if (float.IsNaN(tolerance) || float.IsInfinity(tolerance) || tolerance < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tolerance),
                "Tolerance must be a finite value greater than or equal to zero.");
        }
    }

    private static void ValidateTolerance(decimal tolerance)
    {
        if (tolerance < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tolerance),
                "Tolerance must be greater than or equal to zero.");
        }
    }
}
