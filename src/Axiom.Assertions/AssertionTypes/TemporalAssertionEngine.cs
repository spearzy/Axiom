using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

internal static class TemporalAssertionEngine
{
    public static void AssertBeBefore(
        DateTime subject,
        string? subjectExpression,
        DateTime expected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (subject < expected)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation("to be before", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertBeAfter(
        DateTime subject,
        string? subjectExpression,
        DateTime expected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (subject > expected)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation("to be after", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertBeWithin(
        DateTime subject,
        string? subjectExpression,
        DateTime expected,
        TimeSpan tolerance,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var normalisedTolerance = NormaliseTolerance(tolerance);
        if ((subject - expected).Duration() <= normalisedTolerance)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation($"to be within {normalisedTolerance} of", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertBeBefore(
        DateTimeOffset subject,
        string? subjectExpression,
        DateTimeOffset expected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (subject < expected)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation("to be before", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertBeAfter(
        DateTimeOffset subject,
        string? subjectExpression,
        DateTimeOffset expected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (subject > expected)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation("to be after", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertBeWithin(
        DateTimeOffset subject,
        string? subjectExpression,
        DateTimeOffset expected,
        TimeSpan tolerance,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var normalisedTolerance = NormaliseTolerance(tolerance);
        if ((subject - expected).Duration() <= normalisedTolerance)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation($"to be within {normalisedTolerance} of", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertBeBefore(
        DateOnly subject,
        string? subjectExpression,
        DateOnly expected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (subject < expected)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation("to be before", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertBeAfter(
        DateOnly subject,
        string? subjectExpression,
        DateOnly expected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (subject > expected)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation("to be after", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertBeWithin(
        DateOnly subject,
        string? subjectExpression,
        DateOnly expected,
        TimeSpan tolerance,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var normalisedTolerance = NormaliseTolerance(tolerance);
        var difference = TimeSpan.FromDays(Math.Abs(subject.DayNumber - expected.DayNumber));
        if (difference <= normalisedTolerance)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation($"to be within {normalisedTolerance} of", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertBeBefore(
        TimeOnly subject,
        string? subjectExpression,
        TimeOnly expected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (subject < expected)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation("to be before", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertBeAfter(
        TimeOnly subject,
        string? subjectExpression,
        TimeOnly expected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (subject > expected)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation("to be after", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertBeWithin(
        TimeOnly subject,
        string? subjectExpression,
        TimeOnly expected,
        TimeSpan tolerance,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var normalisedTolerance = NormaliseTolerance(tolerance);
        if (AbsoluteTimeOnlyDifference(subject, expected) <= normalisedTolerance)
        {
            return;
        }

        var failure = new Failure(
            SubjectLabel(subjectExpression),
            new Expectation($"to be within {normalisedTolerance} of", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
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
}
