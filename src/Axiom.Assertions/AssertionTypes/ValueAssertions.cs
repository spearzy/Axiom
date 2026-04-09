using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Assertions.Equivalency;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;

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
        return BeInternal(expected, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> Be(
        T expected,
        IEqualityComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        return BeInternal(expected, comparer, because, callerFilePath, callerLineNumber);
    }

    private AndContinuation<ValueAssertions<T>> BeInternal(
        T expected,
        IEqualityComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var equalityComparer = GetComparer(comparer);
        if (!equalityComparer.Equals(Subject, expected))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be", expected),
                Subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> NotBe(
        T unexpected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return NotBeInternal(unexpected, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> NotBe(
        T unexpected,
        IEqualityComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        return NotBeInternal(unexpected, comparer, because, callerFilePath, callerLineNumber);
    }

    private AndContinuation<ValueAssertions<T>> NotBeInternal(
        T unexpected,
        IEqualityComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var equalityComparer = GetComparer(comparer);
        if (equalityComparer.Equals(Subject, unexpected))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be", unexpected),
                Subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeOneOf(
        IEnumerable<T> expectedValues,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return BeOneOfInternal(expectedValues, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> BeOneOf(
        IEnumerable<T> expectedValues,
        IEqualityComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        return BeOneOfInternal(expectedValues, comparer, because, callerFilePath, callerLineNumber);
    }

    private AndContinuation<ValueAssertions<T>> BeOneOfInternal(
        IEnumerable<T> expectedValues,
        IEqualityComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        ArgumentNullException.ThrowIfNull(expectedValues);

        var replayableExpectedValues = EnsureReplayableExpectedValues(expectedValues);
        var equalityComparer = GetComparer(comparer);
        var hasMatch = TryFindMatch(replayableExpectedValues, Subject, equalityComparer, out var hasExpectedValues);
        if (!hasExpectedValues)
        {
            throw new ArgumentException("expectedValues must contain at least one value.", nameof(expectedValues));
        }

        if (!hasMatch)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be one of", RenderExpectedSet(replayableExpectedValues)),
                Subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> NotBeOneOf(
        IEnumerable<T> unexpectedValues,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return NotBeOneOfInternal(unexpectedValues, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> NotBeOneOf(
        IEnumerable<T> unexpectedValues,
        IEqualityComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        return NotBeOneOfInternal(unexpectedValues, comparer, because, callerFilePath, callerLineNumber);
    }

    private AndContinuation<ValueAssertions<T>> NotBeOneOfInternal(
        IEnumerable<T> unexpectedValues,
        IEqualityComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        ArgumentNullException.ThrowIfNull(unexpectedValues);

        var replayableUnexpectedValues = EnsureReplayableExpectedValues(unexpectedValues);
        var equalityComparer = GetComparer(comparer);
        var hasMatch = TryFindMatch(replayableUnexpectedValues, Subject, equalityComparer, out var hasUnexpectedValues);
        if (!hasUnexpectedValues)
        {
            throw new ArgumentException("unexpectedValues must contain at least one value.", nameof(unexpectedValues));
        }

        if (hasMatch)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be one of", RenderExpectedSet(replayableUnexpectedValues)),
                Subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> Satisfy(
        Func<T, bool> predicate,
        string? because = null,
        [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        if (!predicate(Subject))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation(AssertionMessageText.BuildPredicateExpectationText("to satisfy predicate", predicateExpression), IncludeExpectedValue: false),
                Subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> NotSatisfy(
        Func<T, bool> predicate,
        string? because = null,
        [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        if (predicate(Subject))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation(AssertionMessageText.BuildPredicateExpectationText("to not satisfy predicate", predicateExpression), IncludeExpectedValue: false),
                Subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
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

    public AndContinuation<ValueAssertions<T>> NotBeEquivalentTo<TExpected>(
        TExpected expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var options = EquivalencyDefaults.Snapshot();
        return NotBeEquivalentToInternal(expected, options, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> NotBeEquivalentTo<TExpected>(
        TExpected expected,
        Action<EquivalencyOptions> configure,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = EquivalencyDefaults.Snapshot();
        configure(options);
        return NotBeEquivalentToInternal(expected, options, because, callerFilePath, callerLineNumber);
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

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
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

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
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

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
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

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
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

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeGreaterThan(
        T threshold,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return BeGreaterThanInternal(threshold, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> BeGreaterThan(
        T threshold,
        IComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        return BeGreaterThanInternal(threshold, comparer, because, callerFilePath, callerLineNumber);
    }

    private AndContinuation<ValueAssertions<T>> BeGreaterThanInternal(
        T threshold,
        IComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (!TryCompareValues(Subject, threshold, comparer, out var comparison) || comparison <= 0)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be greater than", threshold),
                Subject,
                because);

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeGreaterThanOrEqualTo(
        T threshold,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return BeGreaterThanOrEqualToInternal(threshold, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> BeGreaterThanOrEqualTo(
        T threshold,
        IComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        return BeGreaterThanOrEqualToInternal(threshold, comparer, because, callerFilePath, callerLineNumber);
    }

    private AndContinuation<ValueAssertions<T>> BeGreaterThanOrEqualToInternal(
        T threshold,
        IComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (!TryCompareValues(Subject, threshold, comparer, out var comparison) || comparison < 0)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be greater than or equal to", threshold),
                Subject,
                because);

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeLessThan(
        T threshold,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return BeLessThanInternal(threshold, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> BeLessThan(
        T threshold,
        IComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        return BeLessThanInternal(threshold, comparer, because, callerFilePath, callerLineNumber);
    }

    private AndContinuation<ValueAssertions<T>> BeLessThanInternal(
        T threshold,
        IComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (!TryCompareValues(Subject, threshold, comparer, out var comparison) || comparison >= 0)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be less than", threshold),
                Subject,
                because);

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeLessThanOrEqualTo(
        T threshold,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return BeLessThanOrEqualToInternal(threshold, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> BeLessThanOrEqualTo(
        T threshold,
        IComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        return BeLessThanOrEqualToInternal(threshold, comparer, because, callerFilePath, callerLineNumber);
    }

    private AndContinuation<ValueAssertions<T>> BeLessThanOrEqualToInternal(
        T threshold,
        IComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (!TryCompareValues(Subject, threshold, comparer, out var comparison) || comparison > 0)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be less than or equal to", threshold),
                Subject,
                because);

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> BeInRange(
        T minimum,
        T maximum,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return BeInRangeInternal(minimum, maximum, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<ValueAssertions<T>> BeInRange(
        T minimum,
        T maximum,
        IComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        return BeInRangeInternal(minimum, maximum, comparer, because, callerFilePath, callerLineNumber);
    }

    private AndContinuation<ValueAssertions<T>> BeInRangeInternal(
        T minimum,
        T maximum,
        IComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (!TryCompareValues(minimum, maximum, comparer, out var boundsComparison))
        {
            throw new ArgumentException("Range bounds must support ordering comparisons.", nameof(minimum));
        }

        if (boundsComparison > 0)
        {
            throw new ArgumentException("minimum must be less than or equal to maximum.", nameof(minimum));
        }

        var isLowerBoundComparable = TryCompareValues(Subject, minimum, comparer, out var lowerComparison);
        var isUpperBoundComparable = TryCompareValues(Subject, maximum, comparer, out var upperComparison);
        if (!isLowerBoundComparable || !isUpperBoundComparable || lowerComparison < 0 || upperComparison > 0)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be in range", new InclusiveRange(minimum, maximum)),
                Subject,
                because);

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

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

    private static IEqualityComparer<T> GetComparer(IEqualityComparer<T>? comparer)
    {
        return comparer ?? GetComparer();
    }

    private static bool TryCompareValues(T candidate, T other, out int comparison)
    {
        if (candidate is IComparable<T> genericComparable)
        {
            try
            {
                comparison = genericComparable.CompareTo(other);
                return true;
            }
            catch (ArgumentException)
            {
                // Incompatible runtime type for this comparer; fall through to alternate strategies.
            }
        }

        if (candidate is IComparable comparable)
        {
            try
            {
                comparison = comparable.CompareTo(other);
                return true;
            }
            catch (ArgumentException)
            {
                // Incompatible runtime type for this comparer; treat as not comparable.
            }
        }

        comparison = 0;
        return false;
    }

    private static bool TryCompareValues(
        T candidate,
        T other,
        IComparer<T>? comparer,
        out int comparison)
    {
        if (comparer is not null)
        {
            comparison = comparer.Compare(candidate, other);
            return true;
        }

        return TryCompareValues(candidate, other, out comparison);
    }

    private static IEnumerable<T> EnsureReplayableExpectedValues(IEnumerable<T> expectedValues)
    {
        if (expectedValues is T[] or List<T> or ICollection<T> or IReadOnlyCollection<T>)
        {
            return expectedValues;
        }

        return expectedValues.ToArray();
    }

    private static bool TryFindMatch(
        IEnumerable<T> candidates,
        T subject,
        IEqualityComparer<T> comparer,
        out bool hasCandidates)
    {
        hasCandidates = false;
        foreach (var candidate in candidates)
        {
            hasCandidates = true;
            if (comparer.Equals(subject, candidate))
            {
                return true;
            }
        }

        return false;
    }

    private static RenderedText RenderExpectedSet(IEnumerable<T> expectedValues)
    {
        var formatter = AxiomServices.Configuration.ValueFormatter;
        var formattedValues = new List<string>();
        foreach (var expected in expectedValues)
        {
            formattedValues.Add(formatter.Format(expected));
        }

        // Keep output deterministic across collection types with unstable enumeration order.
        formattedValues.Sort(StringComparer.Ordinal);
        return new RenderedText($"[{string.Join(", ", formattedValues)}]");
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

            AssertionFailureDispatcher.Fail(message, callerFilePath, callerLineNumber);
        }
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    private AndContinuation<ValueAssertions<T>> NotBeEquivalentToInternal<TExpected>(
        TExpected expected,
        EquivalencyOptions options,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel();
        var differences = EquivalencyEngine.Compare(Subject, expected, subjectLabel, options);
        // NotBeEquivalentTo should fail only when there are no differences.
        if (differences.Count == 0)
        {
            var failure = new Failure(
                subjectLabel,
                new Expectation("to not be equivalent to", expected),
                Subject,
                because);

            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return new AndContinuation<ValueAssertions<T>>(this);
        }
        return new AndContinuation<ValueAssertions<T>>(this);
    }

    private readonly record struct InclusiveRange(T Minimum, T Maximum)
    {
        public override string ToString()
        {
            return $"[{Minimum}, {Maximum}]";
        }
    }

    private readonly record struct RenderedText(string Text)
    {
        public override string ToString()
        {
            return Text;
        }
    }
}
