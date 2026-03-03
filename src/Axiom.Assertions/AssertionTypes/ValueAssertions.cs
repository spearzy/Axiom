using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Assertions.Equivalency;
using Axiom.Core;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;
using Axiom.Core.Output;

namespace Axiom.Assertions.AssertionTypes;

public sealed class ValueAssertions<T>(T subject, string? subjectExpression)
{
    public T Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;

    public AndContinuation<ValueAssertions<T>> Be(
        T expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var comparer = GetComparer();
        if (!comparer.Equals(Subject, expected))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be", expected),
                Subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(Be), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> NotBe(
        T unexpected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var comparer = GetComparer();
        if (comparer.Equals(Subject, unexpected))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be", unexpected),
                Subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(NotBe), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeSameAs(
        T? expectedReference,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!ReferenceEquals(Subject, expectedReference))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be same reference as", expectedReference),
                Subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(BeSameAs), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> NotBeSameAs(
        T? unexpectedReference,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (ReferenceEquals(Subject, unexpectedReference))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be same reference as", unexpectedReference),
                Subject,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(NotBeSameAs), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeEquivalentTo<TExpected>(
        TExpected expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var options = EquivalencyDefaults.Snapshot();
        return BeEquivalentToInternal(expected, options, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> BeEquivalentTo<TExpected>(
        TExpected expected,
        Action<EquivalencyOptions> configure,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = EquivalencyDefaults.Snapshot();
        configure(options);
        return BeEquivalentToInternal(expected, options, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> BeNull(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (Subject is not null)
        {
            var failure = new Failure(SubjectLabel(),
                new Expectation("to be null", IncludeExpectedValue: false), Subject, because);

            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(BeNull), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> NotBeNull(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (Subject is null)
        {
            var failure = new Failure(SubjectLabel(),
                new Expectation("to not be null", IncludeExpectedValue: false), Subject, because);

            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(NotBeNull), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeOfType<TExpected>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (Subject is null || Subject.GetType() != typeof(TExpected))
        {
            var failure = new Failure(SubjectLabel(),
                new Expectation("to be of type", typeof(TExpected)), Subject, because);

            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(BeOfType), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeAssignableTo<TExpected>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        // Runtime assignability check (includes inheritance/interface implementations).
        if (Subject is not TExpected)
        {
            var failure = new Failure(SubjectLabel(),
                new Expectation("to be assignable to", typeof(TExpected)), Subject, because);

            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(BeAssignableTo), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> NotBeAssignableTo<TExpected>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (Subject is TExpected)
        {
            var failure = new Failure(SubjectLabel(),
                new Expectation("to not be assignable to", typeof(TExpected)), Subject, because);

            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(NotBeAssignableTo), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeGreaterThan(
        T threshold,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!TryCompareValues(Subject, threshold, out var comparison) || comparison <= 0)
        {
            var failure = new Failure(SubjectLabel(),
                new Expectation("to be greater than", threshold), Subject, because);

            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(BeGreaterThan), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeGreaterThanOrEqualTo(
        T threshold,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!TryCompareValues(Subject, threshold, out var comparison) || comparison < 0)
        {
            var failure = new Failure(SubjectLabel(),
                new Expectation("to be greater than or equal to", threshold), Subject, because);

            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(BeGreaterThanOrEqualTo), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeLessThan(
        T threshold,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!TryCompareValues(Subject, threshold, out var comparison) || comparison >= 0)
        {
            var failure = new Failure(SubjectLabel(),
                new Expectation("to be less than", threshold), Subject, because);

            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(BeLessThan), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeLessThanOrEqualTo(
        T threshold,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!TryCompareValues(Subject, threshold, out var comparison) || comparison > 0)
        {
            var failure = new Failure(SubjectLabel(),
                new Expectation("to be less than or equal to", threshold), Subject, because);

            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(BeLessThanOrEqualTo), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeInRange(
        T minimum,
        T maximum,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!TryCompareValues(minimum, maximum, out var boundsComparison))
        {
            throw new ArgumentException("Range bounds must support ordering comparisons.", nameof(minimum));
        }

        if (boundsComparison > 0)
        {
            throw new ArgumentException("minimum must be less than or equal to maximum.", nameof(minimum));
        }

        var isLowerBoundComparable = TryCompareValues(Subject, minimum, out var lowerComparison);
        var isUpperBoundComparable = TryCompareValues(Subject, maximum, out var upperComparison);
        if (!isLowerBoundComparable || !isUpperBoundComparable || lowerComparison < 0 || upperComparison > 0)
        {
            var failure = new Failure(SubjectLabel(),
                new Expectation("to be in range", new InclusiveRange(minimum, maximum)), Subject, because);

            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        AssertionOutputWriter.ReportPass(nameof(BeInRange), SubjectLabel(), callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }

    private static IEqualityComparer<T> GetComparer()
    {
        // Prefer configured provider; fall back to runtime default if provider declines.
        if (AxiomServices.Configuration.ComparerProvider.TryGetEqualityComparer<T>(out var comparer) &&
            comparer is not null)
        {
            return comparer;
        }

        return EqualityComparer<T>.Default;
    }

    private static bool TryCompareValues(T left, T right, out int comparison)
    {
        // Prefer the strongly typed comparer when available.
        if (left is IComparable<T> genericComparable)
        {
            try
            {
                comparison = genericComparable.CompareTo(right);
                return true;
            }
            catch (ArgumentException)
            {
            }
        }

        if (left is IComparable comparable)
        {
            try
            {
                comparison = comparable.CompareTo(right);
                return true;
            }
            catch (ArgumentException)
            {
            }
        }

        comparison = 0;
        return false;
    }

    private AndContinuation<ValueAssertions<T>> BeEquivalentToInternal<TExpected>(
        TExpected expected,
        EquivalencyOptions options,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel();
        var differences = EquivalencyEngine.Compare(Subject, expected, subjectLabel, options);
        if (differences.Count > 0)
        {
            var message = EquivalencyReportRenderer.Render(
                subjectLabel,
                expected,
                differences,
                options.MaxDifferences,
                because);

            Fail(message, callerFilePath, callerLineNumber);
            return new AndContinuation<ValueAssertions<T>>(this);
        }

        AssertionOutputWriter.ReportPass(nameof(BeEquivalentTo), subjectLabel, callerFilePath, callerLineNumber);
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    private static void Fail(string message, string? callerFilePath, int callerLineNumber)
    {
        AssertionOutputWriter.ReportFailure(message, callerFilePath, callerLineNumber);

        var batch = Batch.Current;
        if (batch is not null)
        {
            batch.AddFailure(message);
            return;
        }

        throw new InvalidOperationException(message);
    }

    private readonly record struct InclusiveRange(T Minimum, T Maximum)
    {
        public override string ToString()
        {
            return $"[{Minimum}, {Maximum}]";
        }
    }
}
