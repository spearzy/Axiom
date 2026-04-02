using System.Collections;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

internal static partial class CollectionAssertionEngine
{
    public static void AssertHaveUniqueItems(
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
                new Expectation("to have unique items", IncludeExpectedValue: false),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var seen = TryGetCount(subject, out var knownCount)
            ? new HashSet<object?>(knownCount)
            : new HashSet<object?>();
        var index = 0;
        foreach (var item in subject)
        {
            if (seen.Add(item))
            {
                index++;
                continue;
            }

            var duplicateItemFailure = new Failure(
                subjectLabel,
                new Expectation("to have unique items", IncludeExpectedValue: false),
                new RenderedText($"first duplicate item at index {index}: {FormatSingleValue(item)}"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(duplicateItemFailure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertHaveUniqueItems<T>(
        IEnumerable<T>? subject,
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
                new Expectation("to have unique items", IncludeExpectedValue: false),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var comparer = GetComparer<T>();
        _ = subject.TryGetNonEnumeratedCount(out var initialCapacity);
        var seen = initialCapacity > 0
            ? new HashSet<T>(initialCapacity, comparer)
            : new HashSet<T>(comparer);
        var index = 0;
        foreach (var item in subject)
        {
            if (seen.Add(item))
            {
                index++;
                continue;
            }

            var duplicateItemFailure = new Failure(
                subjectLabel,
                new Expectation("to have unique items", IncludeExpectedValue: false),
                new RenderedText($"first duplicate item at index {index}: {FormatSingleValue(item)}"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(duplicateItemFailure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertHaveUniqueItemsBy<TItem, TKey>(
        IEnumerable<TItem>? subject,
        string? subjectExpression,
        Func<TItem, TKey> keySelector,
        IEqualityComparer<TKey>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to have unique items by selected key", IncludeExpectedValue: false),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var keyComparer = comparer ?? GetComparer<TKey>();
        _ = subject.TryGetNonEnumeratedCount(out var initialCapacity);
        var seen = initialCapacity > 0
            ? new HashSet<TKey>(initialCapacity, keyComparer)
            : new HashSet<TKey>(keyComparer);
        var index = 0;
        foreach (var item in subject)
        {
            var key = keySelector(item);
            if (seen.Add(key))
            {
                index++;
                continue;
            }

            var duplicateKeyFailure = new Failure(
                subjectLabel,
                new Expectation("to have unique items by selected key", IncludeExpectedValue: false),
                new RenderedText($"first duplicate selected key at index {index}: {FormatSingleValue(key)}"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(duplicateKeyFailure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertContainExactly<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<T> expectedSequence,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        var expectedItems = MaterialiseExpectedSequence(expectedSequence);
        RenderedText? expectedSequenceText = null;

        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain exactly", expectedSequenceText ??= new RenderedText(FormatSequence(expectedItems))),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var comparer = GetComparer<T>();
        var index = 0;
        // Compare position-by-position and stop at the first deterministic difference.
        foreach (var item in subject)
        {
            if (index >= expectedItems.Length)
            {
                var extraItemFailure = new Failure(
                    subjectLabel,
                    new Expectation("to contain exactly", expectedSequenceText ??= new RenderedText(FormatSequence(expectedItems))),
                    new RenderedText($"extra item at index {index}: {FormatSingleValue(item)}"),
                    because);
                AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(extraItemFailure), callerFilePath, callerLineNumber);
                return;
            }

            if (!comparer.Equals(item, expectedItems[index]))
            {
                var mismatchFailure = new Failure(
                    subjectLabel,
                    new Expectation("to contain exactly", expectedSequenceText ??= new RenderedText(FormatSequence(expectedItems))),
                    new RenderedText(
                        $"item mismatch at index {index}: expected {FormatSingleValue(expectedItems[index])} but found {FormatSingleValue(item)}"),
                    because);
                AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(mismatchFailure), callerFilePath, callerLineNumber);
                return;
            }

            index++;
        }

        // Subject ended before the expected sequence finished.
        if (index < expectedItems.Length)
        {
            var missingItemFailure = new Failure(
                subjectLabel,
                new Expectation("to contain exactly", expectedSequenceText ??= new RenderedText(FormatSequence(expectedItems))),
                new RenderedText($"missing item at index {index}: {FormatSingleValue(expectedItems[index])}"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(missingItemFailure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertBeSubsetOf<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<T> expectedSuperset,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        var supersetItems = MaterialiseExpectedSequence(expectedSuperset);
        RenderedText? supersetText = null;

        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to be a subset of", supersetText ??= new RenderedText(FormatSequence(supersetItems))),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var comparer = GetComparer<T>();
        var index = 0;
        // Report the first missing item so subset failures are deterministic and easy to scan.
        foreach (var item in subject)
        {
            if (ContainsItem(supersetItems, item, comparer))
            {
                index++;
                continue;
            }

            var missingItemFailure = new Failure(
                subjectLabel,
                new Expectation("to be a subset of", supersetText ??= new RenderedText(FormatSequence(supersetItems))),
                new RenderedText($"missing item at index {index}: {FormatSingleValue(item)}"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(missingItemFailure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertBeSupersetOf<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<T> expectedSubset,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        var subsetItems = MaterialiseExpectedSequence(expectedSubset);
        RenderedText? subsetText = null;

        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to be a superset of", subsetText ??= new RenderedText(FormatSequence(subsetItems))),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var subjectItems = MaterialiseExpectedSequence(subject);
        var comparer = GetComparer<T>();
        for (var index = 0; index < subsetItems.Length; index++)
        {
            if (ContainsItem(subjectItems, subsetItems[index], comparer))
            {
                continue;
            }

            var missingItemFailure = new Failure(
                subjectLabel,
                new Expectation("to be a superset of", subsetText ??= new RenderedText(FormatSequence(subsetItems))),
                new RenderedText($"missing expected item at index {index}: {FormatSingleValue(subsetItems[index])}"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(missingItemFailure), callerFilePath, callerLineNumber);
            return;
        }
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static ContainSingleResult AssertContainSingleAndCaptureResult(
        IEnumerable? subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var result = EvaluateContainSingle(subject, subjectExpression, because);
        if (result.FailureMessage is not null)
        {
            AssertionFailureDispatcher.Fail(result.FailureMessage, callerFilePath, callerLineNumber);
        }

        return result;
    }

    public static ContainSingleResult<T> AssertContainSingleAndCaptureResult<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var result = EvaluateContainSingle(subject, subjectExpression, because);
        if (result.FailureMessage is not null)
        {
            AssertionFailureDispatcher.Fail(result.FailureMessage, callerFilePath, callerLineNumber);
        }

        return result;
    }

    public static ContainSingleResult<T> AssertContainSingleAndCaptureResult<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        Func<T, bool> predicate,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var result = EvaluateContainSingle(subject, subjectExpression, predicate, because);
        if (result.FailureMessage is not null)
        {
            AssertionFailureDispatcher.Fail(result.FailureMessage, callerFilePath, callerLineNumber);
        }

        return result;
    }

    private static ContainSingleResult EvaluateContainSingle(
        IEnumerable? subject,
        string? subjectExpression,
        string? because)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: null,
                FailureMessage: RenderContainSingleFailure(subjectLabel, subject, because));
        }

        var count = 0;
        object? singleItem = null;

        var enumerator = subject.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                if (count == 0)
                {
                    singleItem = enumerator.Current;
                }

                count++;
            }
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }

        if (count != 1)
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: null,
                FailureMessage: RenderContainSingleFailure(subjectLabel, count, because));
        }

        return new ContainSingleResult(
            HasSingleItem: true,
            SingleItem: singleItem,
            FailureMessage: null);
    }

    private static ContainSingleResult<T> EvaluateContainSingle<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        string? because)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            return new ContainSingleResult<T>(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(subjectLabel, subject, because));
        }

        var count = 0;
        T singleItem = default!;

        foreach (var item in subject)
        {
            if (count == 0)
            {
                singleItem = item;
            }

            count++;
        }

        if (count != 1)
        {
            return new ContainSingleResult<T>(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(subjectLabel, count, because));
        }

        return new ContainSingleResult<T>(
            HasSingleItem: true,
            SingleItem: singleItem,
            FailureMessage: null);
    }

    private static ContainSingleResult<T> EvaluateContainSingle<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        Func<T, bool> predicate,
        string? because)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        const string expectationText = "to contain a single item matching predicate";
        if (subject is null)
        {
            return new ContainSingleResult<T>(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(subjectLabel, expectationText, subject, because));
        }

        var count = 0;
        T singleItem = default!;

        foreach (var item in subject)
        {
            if (!predicate(item))
            {
                continue;
            }

            if (count == 0)
            {
                singleItem = item;
            }

            count++;
        }

        if (count != 1)
        {
            return new ContainSingleResult<T>(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(subjectLabel, expectationText, count, because));
        }

        return new ContainSingleResult<T>(
            HasSingleItem: true,
            SingleItem: singleItem,
            FailureMessage: null);
    }

    private static string RenderContainSingleFailure(string subjectLabel, object? actual, string? because)
    {
        return RenderContainSingleFailure(subjectLabel, "to contain a single item", actual, because);
    }

    private static string RenderContainSingleFailure(string subjectLabel, string expectationText, object? actual, string? because)
    {
        var failure = new Failure(
            subjectLabel,
            new Expectation(expectationText, IncludeExpectedValue: false),
            actual,
            because);
        return FailureMessageRenderer.Render(failure);
    }

    private static int CountRemainingIncludingCurrent<T>(IEnumerator<T> enumerator)
    {
        var count = 1;
        while (enumerator.MoveNext())
        {
            count++;
        }

        return count;
    }
}
