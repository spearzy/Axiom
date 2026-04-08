using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;

namespace Axiom.Assertions.Extensions;

public static class TemporalValueAssertionExtensions
{
    #region DateTime
    public static AndContinuation<ValueAssertions<DateTime>> BeBefore(
        this ValueAssertions<DateTime> assertions, DateTime expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeBefore(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTime>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTime>> BeOnOrBefore(
        this ValueAssertions<DateTime> assertions, DateTime expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeOnOrBefore(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTime>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTime>> BeAfter(
        this ValueAssertions<DateTime> assertions, DateTime expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeAfter(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTime>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTime>> BeOnOrAfter(
        this ValueAssertions<DateTime> assertions, DateTime expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeOnOrAfter(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTime>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTime>> BeWithin(
        this ValueAssertions<DateTime> assertions, DateTime expected, TimeSpan tolerance,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeWithin(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            tolerance,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTime>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTime>> NotBeWithin(
        this ValueAssertions<DateTime> assertions, DateTime expected, TimeSpan tolerance,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertNotBeWithin(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            tolerance,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTime>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTime>> BeBetween(
        this ValueAssertions<DateTime> assertions, DateTime lowerBound, DateTime upperBound,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeBetween(
            assertions.Subject,
            assertions.SubjectExpression,
            lowerBound,
            upperBound,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTime>>(assertions);
    }
    #endregion

    #region DateTimeOffset
    public static AndContinuation<ValueAssertions<DateTimeOffset>> BeBefore(
        this ValueAssertions<DateTimeOffset> assertions, DateTimeOffset expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeBefore(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTimeOffset>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTimeOffset>> BeOnOrBefore(
        this ValueAssertions<DateTimeOffset> assertions, DateTimeOffset expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeOnOrBefore(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTimeOffset>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTimeOffset>> BeAfter(
        this ValueAssertions<DateTimeOffset> assertions, DateTimeOffset expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeAfter(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTimeOffset>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTimeOffset>> BeOnOrAfter(
        this ValueAssertions<DateTimeOffset> assertions, DateTimeOffset expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeOnOrAfter(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTimeOffset>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTimeOffset>> BeWithin(
        this ValueAssertions<DateTimeOffset> assertions, DateTimeOffset expected, TimeSpan tolerance,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeWithin(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            tolerance,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTimeOffset>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTimeOffset>> NotBeWithin(
        this ValueAssertions<DateTimeOffset> assertions, DateTimeOffset expected, TimeSpan tolerance,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertNotBeWithin(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            tolerance,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTimeOffset>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTimeOffset>> BeBetween(
        this ValueAssertions<DateTimeOffset> assertions, DateTimeOffset lowerBound, DateTimeOffset upperBound,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeBetween(
            assertions.Subject,
            assertions.SubjectExpression,
            lowerBound,
            upperBound,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTimeOffset>>(assertions);
    }
    #endregion

    #region DateOnly
    public static AndContinuation<ValueAssertions<DateOnly>> BeBefore(
        this ValueAssertions<DateOnly> assertions, DateOnly expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeBefore(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateOnly>> BeOnOrBefore(
        this ValueAssertions<DateOnly> assertions, DateOnly expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeOnOrBefore(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateOnly>> BeAfter(
        this ValueAssertions<DateOnly> assertions, DateOnly expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeAfter(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateOnly>> BeOnOrAfter(
        this ValueAssertions<DateOnly> assertions, DateOnly expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeOnOrAfter(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateOnly>> BeWithin(
        this ValueAssertions<DateOnly> assertions, DateOnly expected, TimeSpan tolerance,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeWithin(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            tolerance,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateOnly>> NotBeWithin(
        this ValueAssertions<DateOnly> assertions, DateOnly expected, TimeSpan tolerance,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertNotBeWithin(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            tolerance,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateOnly>> BeBetween(
        this ValueAssertions<DateOnly> assertions, DateOnly lowerBound, DateOnly upperBound,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeBetween(
            assertions.Subject,
            assertions.SubjectExpression,
            lowerBound,
            upperBound,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateOnly>>(assertions);
    }
    #endregion

    #region TimeOnly
    public static AndContinuation<ValueAssertions<TimeOnly>> BeBefore(
        this ValueAssertions<TimeOnly> assertions, TimeOnly expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeBefore(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TimeOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TimeOnly>> BeOnOrBefore(
        this ValueAssertions<TimeOnly> assertions, TimeOnly expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeOnOrBefore(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TimeOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TimeOnly>> BeAfter(
        this ValueAssertions<TimeOnly> assertions, TimeOnly expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeAfter(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TimeOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TimeOnly>> BeOnOrAfter(
        this ValueAssertions<TimeOnly> assertions, TimeOnly expected, string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeOnOrAfter(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TimeOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TimeOnly>> BeWithin(
        this ValueAssertions<TimeOnly> assertions, TimeOnly expected, TimeSpan tolerance,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeWithin(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            tolerance,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TimeOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TimeOnly>> NotBeWithin(
        this ValueAssertions<TimeOnly> assertions, TimeOnly expected, TimeSpan tolerance,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertNotBeWithin(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            tolerance,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TimeOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TimeOnly>> BeBetween(
        this ValueAssertions<TimeOnly> assertions, TimeOnly lowerBound, TimeOnly upperBound,
        string? because = null, [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeBetween(
            assertions.Subject,
            assertions.SubjectExpression,
            lowerBound,
            upperBound,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TimeOnly>>(assertions);
    }
    #endregion
}
