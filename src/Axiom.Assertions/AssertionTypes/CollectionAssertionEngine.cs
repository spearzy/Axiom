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
        bool allowGaps,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        var expectedItems = MaterialiseExpectedSequence(expectedSequence);
        var expectationText = BuildContainInOrderExpectationText(allowGaps, usesSelectedKey: false);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation(expectationText, new RenderedText(FormatSequence(expectedItems))),
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
        var matched = allowGaps
            ? ContainsInOrderAllowingGaps(subject, expectedItems, comparer, out expectedIndex)
            : ContainsInOrderWithoutGaps(subject, expectedItems, comparer);
        if (matched)
        {
            AssertionOutputWriter.ReportPass("ContainInOrder", subjectLabel, callerFilePath, callerLineNumber);
            return;
        }

        var failure = new Failure(
            subjectLabel,
            new Expectation(expectationText, new RenderedText(FormatSequence(expectedItems))),
            allowGaps
                ? new RenderedText(
                    $"missing expected item at sequence index {expectedIndex}: {FormatSingleValue(expectedItems[expectedIndex])}")
                : new RenderedText("missing adjacent ordered sequence"),
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertContainInOrderByKey<T, TKey>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<TKey> expectedSequence,
        Func<T, TKey> keySelector,
        string? because,
        bool allowGaps,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        var expectedItems = MaterialiseExpectedSequence(expectedSequence);
        var expectationText = BuildContainInOrderExpectationText(allowGaps, usesSelectedKey: true);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation(expectationText, new RenderedText(FormatSequence(expectedItems))),
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

        var comparer = GetComparer<TKey>();
        var expectedIndex = 0;
        var matched = allowGaps
            ? ContainsProjectedInOrderAllowingGaps(subject, expectedItems, keySelector, comparer, out expectedIndex)
            : ContainsProjectedInOrderWithoutGaps(subject, expectedItems, keySelector, comparer);
        if (matched)
        {
            AssertionOutputWriter.ReportPass("ContainInOrder", subjectLabel, callerFilePath, callerLineNumber);
            return;
        }

        var failure = new Failure(
            subjectLabel,
            new Expectation(expectationText, new RenderedText(FormatSequence(expectedItems))),
            allowGaps
                ? new RenderedText(
                    $"missing expected selected value at sequence index {expectedIndex}: {FormatSingleValue(expectedItems[expectedIndex])}")
                : new RenderedText("missing adjacent ordered sequence for selected values"),
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    private static string BuildContainInOrderExpectationText(bool allowGaps, bool usesSelectedKey)
    {
        var baseExpectation = usesSelectedKey
            ? "to contain selected values in order"
            : "to contain items in order";

        return allowGaps
            ? baseExpectation
            : $"{baseExpectation} with no gaps";
    }

    private static bool ContainsInOrderAllowingGaps<T>(
        IEnumerable<T> subject,
        IReadOnlyList<T> expectedItems,
        IEqualityComparer<T> comparer,
        out int expectedIndex)
    {
        expectedIndex = 0;
        foreach (var item in subject)
        {
            if (!comparer.Equals(item, expectedItems[expectedIndex]))
            {
                continue;
            }

            expectedIndex++;
            if (expectedIndex == expectedItems.Count)
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsProjectedInOrderAllowingGaps<T, TKey>(
        IEnumerable<T> subject,
        IReadOnlyList<TKey> expectedItems,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey> comparer,
        out int expectedIndex)
    {
        expectedIndex = 0;
        foreach (var item in subject)
        {
            var selectedValue = keySelector(item);
            if (!comparer.Equals(selectedValue, expectedItems[expectedIndex]))
            {
                continue;
            }

            expectedIndex++;
            if (expectedIndex == expectedItems.Count)
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsInOrderWithoutGaps<T>(
        IEnumerable<T> subject,
        IReadOnlyList<T> expectedItems,
        IEqualityComparer<T> comparer)
    {
        // Keep a running partial match and, on mismatch, jump to the best known fallback.
        // This avoids restarting the whole pattern check from scratch for each subject item.
        var fallbackTable = BuildFallbackTable(expectedItems, comparer);
        var matchedCount = 0;

        foreach (var item in subject)
        {
            while (matchedCount > 0 && !comparer.Equals(item, expectedItems[matchedCount]))
            {
                matchedCount = fallbackTable[matchedCount - 1];
            }

            if (!comparer.Equals(item, expectedItems[matchedCount]))
            {
                continue;
            }

            matchedCount++;
            if (matchedCount == expectedItems.Count)
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsProjectedInOrderWithoutGaps<T, TKey>(
        IEnumerable<T> subject,
        IReadOnlyList<TKey> expectedItems,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey> comparer)
    {
        // Same fallback matching approach as above, but against the selected key values.
        var fallbackTable = BuildFallbackTable(expectedItems, comparer);
        var matchedCount = 0;

        foreach (var item in subject)
        {
            var selectedValue = keySelector(item);
            while (matchedCount > 0 && !comparer.Equals(selectedValue, expectedItems[matchedCount]))
            {
                matchedCount = fallbackTable[matchedCount - 1];
            }

            if (!comparer.Equals(selectedValue, expectedItems[matchedCount]))
            {
                continue;
            }

            matchedCount++;
            if (matchedCount == expectedItems.Count)
            {
                return true;
            }
        }

        return false;
    }

    private static int[] BuildFallbackTable<T>(IReadOnlyList<T> pattern, IEqualityComparer<T> comparer)
    {
        // For each pattern index, store the length of the longest prefix that is also a suffix
        // for the pattern segment ending at that index.
        var fallbackTable = new int[pattern.Count];
        var candidateLength = 0;
        var i = 1;

        while (i < pattern.Count)
        {
            if (comparer.Equals(pattern[i], pattern[candidateLength]))
            {
                candidateLength++;
                fallbackTable[i] = candidateLength;
                i++;
                continue;
            }

            if (candidateLength == 0)
            {
                fallbackTable[i] = 0;
                i++;
                continue;
            }

            candidateLength = fallbackTable[candidateLength - 1];
        }

        return fallbackTable;
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
