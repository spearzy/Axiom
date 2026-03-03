using System.Collections;
using System.Text;
using Axiom.Core;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;
using Axiom.Core.Output;

namespace Axiom.Assertions.AssertionTypes;

// Shared collection assertion logic invoked by extension methods to avoid per-call wrapper allocations.
internal static class CollectionAssertionEngine
{
    public static void AssertContain<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        T expected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain", expected),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var comparer = GetComparer<T>();
        foreach (var item in subject)
        {
            if (comparer.Equals(item, expected))
            {
                AssertionOutputWriter.ReportPass("Contain", subjectLabel, callerFilePath, callerLineNumber);
                return;
            }
        }

        var failure = new Failure(
            subjectLabel,
            new Expectation("to contain", expected),
            subject,
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertHaveCount(
        IEnumerable? subject,
        string? subjectExpression,
        int expectedCount,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to have count", expectedCount),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var actualCount = TryGetCount(subject, out var knownCount)
            ? knownCount
            : CountItems(subject);

        if (actualCount != expectedCount)
        {
            var failure = new Failure(
                subjectLabel,
                new Expectation("to have count", expectedCount),
                actualCount,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }

        AssertionOutputWriter.ReportPass("HaveCount", subjectLabel, callerFilePath, callerLineNumber);
    }

    public static void AssertBeEmpty(
        IEnumerable? subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to be empty", IncludeExpectedValue: false),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var actualCount = GetCount(subject);
        if (actualCount != 0)
        {
            var failure = new Failure(
                subjectLabel,
                new Expectation("to be empty", IncludeExpectedValue: false),
                actualCount,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }

        AssertionOutputWriter.ReportPass("BeEmpty", subjectLabel, callerFilePath, callerLineNumber);
    }

    public static void AssertNotBeEmpty(
        IEnumerable? subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to not be empty", IncludeExpectedValue: false),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var actualCount = GetCount(subject);
        if (actualCount == 0)
        {
            var failure = new Failure(
                subjectLabel,
                new Expectation("to not be empty", IncludeExpectedValue: false),
                actualCount,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }

        AssertionOutputWriter.ReportPass("NotBeEmpty", subjectLabel, callerFilePath, callerLineNumber);
    }

    public static void AssertContainSingle(
        IEnumerable? subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain a single item", IncludeExpectedValue: false),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var actualCount = GetCount(subject);
        if (actualCount != 1)
        {
            var failure = new Failure(
                subjectLabel,
                new Expectation("to contain a single item", IncludeExpectedValue: false),
                actualCount,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }

        AssertionOutputWriter.ReportPass("ContainSingle", subjectLabel, callerFilePath, callerLineNumber);
    }

    public static void AssertOnlyContain<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        Func<T, bool> predicate,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to only contain items matching predicate", IncludeExpectedValue: false),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var index = 0;
        foreach (var item in subject)
        {
            if (predicate(item))
            {
                index++;
                continue;
            }

            var failure = new Failure(
                subjectLabel,
                new Expectation($"to only contain items matching predicate (first non-matching index {index})", IncludeExpectedValue: false),
                item,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }

        AssertionOutputWriter.ReportPass("OnlyContain", subjectLabel, callerFilePath, callerLineNumber);
    }

    public static void AssertNotContain<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        Func<T, bool> predicate,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to not contain any item matching predicate", IncludeExpectedValue: false),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var index = 0;
        foreach (var item in subject)
        {
            if (!predicate(item))
            {
                index++;
                continue;
            }

            var failure = new Failure(
                subjectLabel,
                new Expectation($"to not contain any item matching predicate (first matching index {index})", IncludeExpectedValue: false),
                item,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }

        AssertionOutputWriter.ReportPass("NotContain", subjectLabel, callerFilePath, callerLineNumber);
    }

    public static void AssertContainInOrder<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<T> expectedSequence,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        var expectedItems = MaterialiseExpectedSequence(expectedSequence);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain items in order", new RenderedText(FormatSequence(expectedItems))),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (expectedItems.Length == 0)
        {
            AssertionOutputWriter.ReportPass("ContainInOrder", subjectLabel, callerFilePath, callerLineNumber);
            return;
        }

        var comparer = GetComparer<T>();
        var expectedIndex = 0;
        foreach (var item in subject)
        {
            if (!comparer.Equals(item, expectedItems[expectedIndex]))
            {
                continue;
            }

            expectedIndex++;
            if (expectedIndex == expectedItems.Length)
            {
                AssertionOutputWriter.ReportPass("ContainInOrder", subjectLabel, callerFilePath, callerLineNumber);
                return;
            }
        }

        var failure = new Failure(
            subjectLabel,
            new Expectation("to contain items in order", new RenderedText(FormatSequence(expectedItems))),
            new RenderedText(
                $"missing expected item at sequence index {expectedIndex}: {FormatSingleValue(expectedItems[expectedIndex])}"),
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    private static string SubjectLabel(string? subjectExpression)
    {
        return string.IsNullOrWhiteSpace(subjectExpression) ? "<subject>" : subjectExpression;
    }

    private static IEqualityComparer<T> GetComparer<T>()
    {
        if (AxiomServices.Configuration.ComparerProvider.TryGetEqualityComparer<T>(out var comparer) &&
            comparer is not null)
        {
            return comparer;
        }

        return EqualityComparer<T>.Default;
    }

    private static bool TryGetCount(IEnumerable subject, out int count)
    {
        // Prefer non-enumerating count paths when collection interfaces are available.
        if (subject is ICollection nonGenericCollection)
        {
            count = nonGenericCollection.Count;
            return true;
        }

        count = 0;
        return false;
    }

    private static int GetCount(IEnumerable subject)
    {
        return TryGetCount(subject, out var knownCount)
            ? knownCount
            : CountItems(subject);
    }

    private static T[] MaterialiseExpectedSequence<T>(IEnumerable<T> expectedSequence)
    {
        if (expectedSequence is T[] array)
        {
            return array;
        }

        var buffer = new List<T>();
        foreach (var item in expectedSequence)
        {
            buffer.Add(item);
        }

        return buffer.ToArray();
    }

    private static string FormatSingleValue<T>(T value)
    {
        return AxiomServices.Configuration.ValueFormatter.Format(value);
    }

    private static string FormatSequence<T>(IReadOnlyList<T> values)
    {
        var formatter = AxiomServices.Configuration.ValueFormatter;
        if (values.Count == 0)
        {
            return "[]";
        }

        var builder = new StringBuilder();
        builder.Append('[');
        for (var i = 0; i < values.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(", ");
            }

            builder.Append(formatter.Format(values[i]));
        }

        builder.Append(']');
        return builder.ToString();
    }

    private static int CountItems(IEnumerable subject)
    {
        var count = 0;
        var enumerator = subject.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                count++;
            }
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }

        return count;
    }

    private static void Fail(string message, string? callerFilePath, int callerLineNumber)
    {
        AssertionOutputWriter.ReportFailure(message, callerFilePath, callerLineNumber);

        var batch = Batch.Current;
        if (batch is not null)
        {
            // Collect failures during batch execution and throw once at root dispose.
            batch.AddFailure(message);
            return;
        }

        throw new InvalidOperationException(message);
    }

    private readonly record struct RenderedText(string Text)
    {
        public override string ToString()
        {
            return Text;
        }
    }
}
