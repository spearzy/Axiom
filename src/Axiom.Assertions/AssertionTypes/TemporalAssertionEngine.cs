using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

internal static class TemporalAssertionEngine
{
    public static void AssertBeBefore(
        DateTime subject, string? subjectExpression, DateTime expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be before",
            static (left, right) => left < right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeOnOrBefore(
        DateTime subject, string? subjectExpression, DateTime expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be on or before",
            static (left, right) => left <= right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeAfter(
        DateTime subject, string? subjectExpression, DateTime expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be after",
            static (left, right) => left > right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeOnOrAfter(
        DateTime subject, string? subjectExpression, DateTime expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be on or after",
            static (left, right) => left >= right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeWithin(
        DateTime subject, string? subjectExpression, DateTime expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalTolerance(
            subject,
            subjectExpression,
            expected,
            tolerance,
            "to be within",
            static (difference, normalisedTolerance) => difference <= normalisedTolerance,
            static (left, right) => (left - right).Duration(),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertNotBeWithin(
        DateTime subject, string? subjectExpression, DateTime expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalTolerance(
            subject,
            subjectExpression,
            expected,
            tolerance,
            "not to be within",
            static (difference, normalisedTolerance) => difference > normalisedTolerance,
            static (left, right) => (left - right).Duration(),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeBetween(
        DateTime subject, string? subjectExpression, DateTime lowerBound, DateTime upperBound,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalRange(
            subject,
            subjectExpression,
            lowerBound,
            upperBound,
            "to be between",
            static (value, lower, upper) => value >= lower && value <= upper,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeBefore(
        DateTimeOffset subject, string? subjectExpression, DateTimeOffset expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be before",
            static (left, right) => left < right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeOnOrBefore(
        DateTimeOffset subject, string? subjectExpression, DateTimeOffset expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be on or before",
            static (left, right) => left <= right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeAfter(
        DateTimeOffset subject, string? subjectExpression, DateTimeOffset expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be after",
            static (left, right) => left > right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeOnOrAfter(
        DateTimeOffset subject, string? subjectExpression, DateTimeOffset expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be on or after",
            static (left, right) => left >= right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeWithin(
        DateTimeOffset subject, string? subjectExpression, DateTimeOffset expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalTolerance(
            subject,
            subjectExpression,
            expected,
            tolerance,
            "to be within",
            static (difference, normalisedTolerance) => difference <= normalisedTolerance,
            static (left, right) => (left - right).Duration(),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertNotBeWithin(
        DateTimeOffset subject, string? subjectExpression, DateTimeOffset expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalTolerance(
            subject,
            subjectExpression,
            expected,
            tolerance,
            "not to be within",
            static (difference, normalisedTolerance) => difference > normalisedTolerance,
            static (left, right) => (left - right).Duration(),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeBetween(
        DateTimeOffset subject, string? subjectExpression, DateTimeOffset lowerBound, DateTimeOffset upperBound,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalRange(
            subject,
            subjectExpression,
            lowerBound,
            upperBound,
            "to be between",
            static (value, lower, upper) => value >= lower && value <= upper,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeBefore(
        DateOnly subject, string? subjectExpression, DateOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be before",
            static (left, right) => left < right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeOnOrBefore(
        DateOnly subject, string? subjectExpression, DateOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be on or before",
            static (left, right) => left <= right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeAfter(
        DateOnly subject, string? subjectExpression, DateOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be after",
            static (left, right) => left > right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeOnOrAfter(
        DateOnly subject, string? subjectExpression, DateOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be on or after",
            static (left, right) => left >= right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeWithin(
        DateOnly subject, string? subjectExpression, DateOnly expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalTolerance(
            subject,
            subjectExpression,
            expected,
            tolerance,
            "to be within",
            static (difference, normalisedTolerance) => difference <= normalisedTolerance,
            static (left, right) => TimeSpan.FromDays(Math.Abs(left.DayNumber - right.DayNumber)),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertNotBeWithin(
        DateOnly subject, string? subjectExpression, DateOnly expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalTolerance(
            subject,
            subjectExpression,
            expected,
            tolerance,
            "not to be within",
            static (difference, normalisedTolerance) => difference > normalisedTolerance,
            static (left, right) => TimeSpan.FromDays(Math.Abs(left.DayNumber - right.DayNumber)),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeBetween(
        DateOnly subject, string? subjectExpression, DateOnly lowerBound, DateOnly upperBound,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalRange(
            subject,
            subjectExpression,
            lowerBound,
            upperBound,
            "to be between",
            static (value, lower, upper) => value >= lower && value <= upper,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeBefore(
        TimeOnly subject, string? subjectExpression, TimeOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be before",
            static (left, right) => left < right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeOnOrBefore(
        TimeOnly subject, string? subjectExpression, TimeOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be on or before",
            static (left, right) => left <= right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeAfter(
        TimeOnly subject, string? subjectExpression, TimeOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be after",
            static (left, right) => left > right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeOnOrAfter(
        TimeOnly subject, string? subjectExpression, TimeOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalComparison(
            subject,
            subjectExpression,
            expected,
            "to be on or after",
            static (left, right) => left >= right,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeWithin(
        TimeOnly subject, string? subjectExpression, TimeOnly expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalTolerance(
            subject,
            subjectExpression,
            expected,
            tolerance,
            "to be within",
            static (difference, normalisedTolerance) => difference <= normalisedTolerance,
            AbsoluteTimeOnlyDifference,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertNotBeWithin(
        TimeOnly subject, string? subjectExpression, TimeOnly expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalTolerance(
            subject,
            subjectExpression,
            expected,
            tolerance,
            "not to be within",
            static (difference, normalisedTolerance) => difference > normalisedTolerance,
            AbsoluteTimeOnlyDifference,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeBetween(
        TimeOnly subject, string? subjectExpression, TimeOnly lowerBound, TimeOnly upperBound,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertTemporalRange(
            subject,
            subjectExpression,
            lowerBound,
            upperBound,
            "to be between",
            static (value, lower, upper) => value >= lower && value <= upper,
            because,
            callerFilePath,
            callerLineNumber);
    }

    private static void AssertTemporalComparison<T>(
        T subject, string? subjectExpression, T expected, string expectationText,
        Func<T, T, bool> passes, string? because, string? callerFilePath, int callerLineNumber)
    {
        if (passes(subject, expected))
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation(expectationText, expected),
            subject,
            because);

        AssertionFailureDispatcher.Fail(
            FailureMessageRenderer.Render(failure),
            callerFilePath,
            callerLineNumber);
    }

    private static void AssertTemporalTolerance<T>(
        T subject, string? subjectExpression, T expected, TimeSpan tolerance, string expectationText,
        Func<TimeSpan, TimeSpan, bool> passes, Func<T, T, TimeSpan> difference,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        var normalisedTolerance = NormaliseTolerance(tolerance);
        var actualDifference = difference(subject, expected);
        if (passes(actualDifference, normalisedTolerance))
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation($"{expectationText} {normalisedTolerance} of", expected),
            subject,
            because);

        AssertionFailureDispatcher.Fail(
            FailureMessageRenderer.Render(failure),
            callerFilePath,
            callerLineNumber);
    }

    private static void AssertTemporalRange<T>(
        T subject, string? subjectExpression, T lowerBound, T upperBound, string expectationText,
        Func<T, T, T, bool> passes, string? because, string? callerFilePath, int callerLineNumber)
    {
        if (passes(subject, lowerBound, upperBound))
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation(expectationText, new InclusiveRange<T>(lowerBound, upperBound)),
            subject,
            because);

        AssertionFailureDispatcher.Fail(
            FailureMessageRenderer.Render(failure),
            callerFilePath,
            callerLineNumber);
    }

    private static string SubjectLabel(string? subjectExpression)
    {
        return string.IsNullOrWhiteSpace(subjectExpression) ? "<subject>" : subjectExpression;
    }

    private static TimeSpan NormaliseTolerance(TimeSpan tolerance)
    {
        if (tolerance == TimeSpan.MinValue)
        {
            throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must not be TimeSpan.MinValue.");
        }

        return tolerance.Duration();
    }

    private static TimeSpan AbsoluteTimeOnlyDifference(TimeOnly left, TimeOnly right)
    {
        var directTicks = Math.Abs(left.Ticks - right.Ticks);
        var wrappedTicks = TimeSpan.TicksPerDay - directTicks;
        return TimeSpan.FromTicks(Math.Min(directTicks, wrappedTicks));
    }

    private readonly record struct InclusiveRange<T>(T Minimum, T Maximum)
    {
        public override string ToString()
        {
            return $"[{Minimum}, {Maximum}]";
        }
    }
}
