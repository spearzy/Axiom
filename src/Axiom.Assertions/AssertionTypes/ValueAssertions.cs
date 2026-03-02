using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Assertions.Equivalency;
using Axiom.Core;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;
using Axiom.Core.Output;

namespace Axiom.Assertions.AssertionTypes;

public sealed class ValueAssertions<T>
{
    public ValueAssertions(T subject, string? subjectExpression)
    {
        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    public T Subject { get; }
    public string? SubjectExpression { get; }

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
}
