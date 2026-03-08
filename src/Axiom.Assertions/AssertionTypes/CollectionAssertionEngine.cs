using System.Collections;
using System.Text;
using System.Linq;
using Axiom.Core;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

// Shared collection assertion logic invoked by extension methods to avoid per-call wrapper allocations.
internal static class CollectionAssertionEngine
{
    public readonly record struct ContainSingleResult(
        bool HasSingleItem,
        object? SingleItem,
        string? FailureMessage);

    public readonly record struct ContainKeyResult<TValue>(
        bool HasValue,
        TValue Value,
        string? FailureMessage);

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

    public static void AssertContainAll<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<T> expectedItems,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        var expected = MaterialiseExpectedSequence(expectedItems);
        RenderedText? expectedText = null;

        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain all", expectedText ??= new RenderedText(FormatSequence(expected))),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (expected.Length == 0)
        {
            return;
        }

        var subjectItems = MaterialiseExpectedSequence(subject);
        var comparer = GetComparer<T>();
        for (var index = 0; index < expected.Length; index++)
        {
            if (ContainsItem(subjectItems, expected[index], comparer))
            {
                continue;
            }

            var missingItemFailure = new Failure(
                subjectLabel,
                new Expectation("to contain all", expectedText ??= new RenderedText(FormatSequence(expected))),
                new RenderedText($"missing expected item at index {index}: {FormatSingleValue(expected[index])}"),
                because);
            Fail(FailureMessageRenderer.Render(missingItemFailure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertContainAny<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<T> expectedItems,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        var expected = MaterialiseExpectedSequence(expectedItems);
        RenderedText? expectedText = null;

        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain any of", expectedText ??= new RenderedText(FormatSequence(expected))),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (expected.Length == 0)
        {
            var noExpectedItemsFailure = new Failure(
                subjectLabel,
                new Expectation("to contain any of", expectedText ??= new RenderedText(FormatSequence(expected))),
                new RenderedText("no expected items were provided"),
                because);
            Fail(FailureMessageRenderer.Render(noExpectedItemsFailure), callerFilePath, callerLineNumber);
            return;
        }

        var comparer = GetComparer<T>();
        foreach (var subjectItem in subject)
        {
            if (ContainsItem(expected, subjectItem, comparer))
            {
                return;
            }
        }

        var missingAnyFailure = new Failure(
            subjectLabel,
            new Expectation("to contain any of", expectedText ??= new RenderedText(FormatSequence(expected))),
            new RenderedText("none of the expected items were found"),
            because);
        Fail(FailureMessageRenderer.Render(missingAnyFailure), callerFilePath, callerLineNumber);
    }

    public static void AssertNotContainAny<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<T> unexpectedItems,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        var unexpected = MaterialiseExpectedSequence(unexpectedItems);
        RenderedText? unexpectedText = null;

        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to not contain any of", unexpectedText ??= new RenderedText(FormatSequence(unexpected))),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (unexpected.Length == 0)
        {
            return;
        }

        var comparer = GetComparer<T>();
        var subjectIndex = 0;
        foreach (var subjectItem in subject)
        {
            if (ContainsItem(unexpected, subjectItem, comparer))
            {
                var matchingUnexpectedFailure = new Failure(
                    subjectLabel,
                    new Expectation("to not contain any of", unexpectedText ??= new RenderedText(FormatSequence(unexpected))),
                    new RenderedText($"first matching item at subject index {subjectIndex}: {FormatSingleValue(subjectItem)}"),
                    because);
                Fail(FailureMessageRenderer.Render(matchingUnexpectedFailure), callerFilePath, callerLineNumber);
                return;
            }

            subjectIndex++;
        }
    }

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
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
            Fail(FailureMessageRenderer.Render(duplicateItemFailure), callerFilePath, callerLineNumber);
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
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
            Fail(FailureMessageRenderer.Render(duplicateKeyFailure), callerFilePath, callerLineNumber);
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
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
                Fail(FailureMessageRenderer.Render(extraItemFailure), callerFilePath, callerLineNumber);
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
                Fail(FailureMessageRenderer.Render(mismatchFailure), callerFilePath, callerLineNumber);
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
            Fail(FailureMessageRenderer.Render(missingItemFailure), callerFilePath, callerLineNumber);
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
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
            Fail(FailureMessageRenderer.Render(missingItemFailure), callerFilePath, callerLineNumber);
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
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
            Fail(FailureMessageRenderer.Render(missingItemFailure), callerFilePath, callerLineNumber);
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
            Fail(result.FailureMessage, callerFilePath, callerLineNumber);
        }

        return result;
    }

    public static ContainSingleResult AssertContainSingleAndCaptureResult<T>(
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
            Fail(result.FailureMessage, callerFilePath, callerLineNumber);
        }

        return result;
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
    }

    public static void AssertNotContainItem<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        T unexpected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to not contain", unexpected),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var comparer = GetComparer<T>();
        foreach (var item in subject)
        {
            if (!comparer.Equals(item, unexpected))
            {
                continue;
            }

            var failure = new Failure(
                subjectLabel,
                new Expectation("to not contain", unexpected),
                item,
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertAllSatisfy<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        Action<T> assertion,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to satisfy all assertions for each item", IncludeExpectedValue: false),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var index = 0;
        foreach (var item in subject)
        {
            try
            {
                assertion(item);
            }
            catch (InvalidOperationException ex)
            {
                var failure = new Failure(
                    subjectLabel,
                    new Expectation($"to satisfy all assertions for each item (first failing index {index})", IncludeExpectedValue: false),
                    new RenderedText(ex.Message),
                    because);
                Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
                return;
            }

            index++;
        }
    }

    public static void AssertSatisfyRespectively<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IReadOnlyList<Action<T>> assertionsForItems,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to satisfy assertions respectively (same order and count)", IncludeExpectedValue: false),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        using var enumerator = subject.GetEnumerator();
        var expectedCount = assertionsForItems.Count;

        for (var index = 0; index < expectedCount; index++)
        {
            if (!enumerator.MoveNext())
            {
                var fewerItemsFailure = new Failure(
                    subjectLabel,
                    new Expectation("to satisfy assertions respectively (same order and count)", IncludeExpectedValue: false),
                    new RenderedText($"collection had fewer items than assertions (expected {expectedCount}, found {index})"),
                    because);
                Fail(FailureMessageRenderer.Render(fewerItemsFailure), callerFilePath, callerLineNumber);
                return;
            }

            try
            {
                //For each assertion, compare against item list sequentially
                assertionsForItems[index](enumerator.Current);
            }
            catch (InvalidOperationException ex)
            {
                var failure = new Failure(
                    subjectLabel,
                    new Expectation($"to satisfy assertions respectively (failing index {index})", IncludeExpectedValue: false),
                    new RenderedText(ex.Message),
                    because);
                Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
                return;
            }
        }

        if (!enumerator.MoveNext())
        {
            return;
        }

        var actualCount = expectedCount + CountRemainingIncludingCurrent(enumerator);
        var moreItemsFailure = new Failure(
            subjectLabel,
            new Expectation("to satisfy assertions respectively (same order and count)", IncludeExpectedValue: false),
            new RenderedText($"collection had more items than assertions (expected {expectedCount}, found {actualCount})"),
            because);
        Fail(FailureMessageRenderer.Render(moreItemsFailure), callerFilePath, callerLineNumber);
    }

    public static ContainKeyResult<TValue> AssertContainKeyAndCaptureResult<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue>? subject,
        string? subjectExpression,
        TKey expectedKey,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var failureMessage = RenderContainKeyFailure(subjectLabel, expectedKey, subject, because);
            Fail(failureMessage, callerFilePath, callerLineNumber);
            return new ContainKeyResult<TValue>(HasValue: false, Value: default!, FailureMessage: failureMessage);
        }

        if (subject.TryGetValue(expectedKey, out var value))
        {
            return new ContainKeyResult<TValue>(HasValue: true, Value: value, FailureMessage: null);
        }

        var failureText = new RenderedText($"keys were {FormatSortedSequence(subject.Keys)}");
        var failure = RenderContainKeyFailure(subjectLabel, expectedKey, failureText, because);
        Fail(failure, callerFilePath, callerLineNumber);
        return new ContainKeyResult<TValue>(HasValue: false, Value: default!, FailureMessage: failure);
    }

    public static void AssertNotContainKey<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue>? subject,
        string? subjectExpression,
        TKey unexpectedKey,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to not contain key", unexpectedKey),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (!subject.TryGetValue(unexpectedKey, out var actualValue))
        {
            return;
        }

        var failure = new Failure(
            subjectLabel,
            new Expectation("to not contain key", unexpectedKey),
            new RenderedText($"key was present with value {FormatSingleValue(actualValue)}"),
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertContainValue<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue>? subject,
        string? subjectExpression,
        TValue expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain value", expectedValue),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var valueComparer = GetComparer<TValue>();
        foreach (var pair in subject)
        {
            if (valueComparer.Equals(pair.Value, expectedValue))
            {
                return;
            }
        }

        var failure = new Failure(
            subjectLabel,
            new Expectation("to contain value", expectedValue),
            new RenderedText($"values were {FormatSortedSequence(subject.Values)}"),
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertNotContainValue<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue>? subject,
        string? subjectExpression,
        TValue unexpectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to not contain value", unexpectedValue),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var valueComparer = GetComparer<TValue>();
        foreach (var pair in subject)
        {
            if (!valueComparer.Equals(pair.Value, unexpectedValue))
            {
                continue;
            }

            var failure = new Failure(
                subjectLabel,
                new Expectation("to not contain value", unexpectedValue),
                new RenderedText($"a matching value at key {FormatSingleValue(pair.Key)}"),
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertContainEntry<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue>? subject,
        string? subjectExpression,
        TKey expectedKey,
        TValue expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        RenderedText? expectedEntry = null;

        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain entry", expectedEntry ??= new RenderedText(FormatEntry(expectedKey, expectedValue))),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (!subject.TryGetValue(expectedKey, out var actualValue))
        {
            var missingKeyFailure = new Failure(
                subjectLabel,
                new Expectation("to contain entry", expectedEntry ??= new RenderedText(FormatEntry(expectedKey, expectedValue))),
                new RenderedText($"key {FormatSingleValue(expectedKey)} was missing"),
                because);
            Fail(FailureMessageRenderer.Render(missingKeyFailure), callerFilePath, callerLineNumber);
            return;
        }

        var valueComparer = GetComparer<TValue>();
        if (valueComparer.Equals(actualValue, expectedValue))
        {
            return;
        }

        var valueMismatchFailure = new Failure(
            subjectLabel,
            new Expectation("to contain entry", expectedEntry ??= new RenderedText(FormatEntry(expectedKey, expectedValue))),
            new RenderedText($"key {FormatSingleValue(expectedKey)} had value {FormatSingleValue(actualValue)}"),
            because);
        Fail(FailureMessageRenderer.Render(valueMismatchFailure), callerFilePath, callerLineNumber);
    }

    public static void AssertNotContainEntry<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue>? subject,
        string? subjectExpression,
        TKey unexpectedKey,
        TValue unexpectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        // Keep pass-path work minimal: only format entry text when we know we are failing.
        if (subject is null)
        {
            var unexpectedEntry = new RenderedText(FormatEntry(unexpectedKey, unexpectedValue));
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to not contain entry", unexpectedEntry),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (!subject.TryGetValue(unexpectedKey, out var actualValue))
        {
            return;
        }

        var valueComparer = GetComparer<TValue>();
        if (!valueComparer.Equals(actualValue, unexpectedValue))
        {
            return;
        }

        var unexpectedEntryText = new RenderedText(FormatEntry(unexpectedKey, unexpectedValue));
        var presentEntryFailure = new Failure(
            subjectLabel,
            new Expectation("to not contain entry", unexpectedEntryText),
            new RenderedText($"matching entry was present: {FormatEntry(unexpectedKey, actualValue)}"),
            because);
        Fail(FailureMessageRenderer.Render(presentEntryFailure), callerFilePath, callerLineNumber);
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
            return;
        }

        var comparer = GetComparer<T>();
        var expectedIndex = 0;
        var matched = allowGaps
            ? ContainsInOrderAllowingGaps(subject, expectedItems, comparer, out expectedIndex)
            : ContainsInOrderWithoutGaps(subject, expectedItems, comparer);
        if (matched)
        {
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
            return;
        }

        var comparer = GetComparer<TKey>();
        var expectedIndex = 0;
        var matched = allowGaps
            ? ContainsProjectedInOrderAllowingGaps(subject, expectedItems, keySelector, comparer, out expectedIndex)
            : ContainsProjectedInOrderWithoutGaps(subject, expectedItems, keySelector, comparer);
        if (matched)
        {
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

    public static void AssertBeInAscendingOrder(
        IEnumerable? subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        AssertInOrder(
            subject,
            subjectExpression,
            because,
            expectationText: "to be in ascending order",
            failureDetailText: "first out-of-order pair",
            inOrder: (previous, current) => CompareObjects(previous, current) <= 0,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeInDescendingOrder(
        IEnumerable? subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        AssertInOrder(
            subject,
            subjectExpression,
            because,
            expectationText: "to be in descending order",
            failureDetailText: "first out-of-order pair",
            inOrder: (previous, current) => CompareObjects(previous, current) >= 0,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeInAscendingOrderByKey<T, TKey>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        Func<T, TKey> keySelector,
        IComparer<TKey>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var resolvedComparer = comparer ?? GetOrderComparer<TKey>();
        AssertInOrderByKey(
            subject,
            subjectExpression,
            keySelector,
            resolvedComparer,
            because,
            expectationText: "to be in ascending order by selected key",
            failureDetailText: "first out-of-order selected key pair",
            inOrder: (previous, current, keyComparer) => keyComparer.Compare(previous, current) <= 0,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeInDescendingOrderByKey<T, TKey>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        Func<T, TKey> keySelector,
        IComparer<TKey>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var resolvedComparer = comparer ?? GetOrderComparer<TKey>();
        AssertInOrderByKey(
            subject,
            subjectExpression,
            keySelector,
            resolvedComparer,
            because,
            expectationText: "to be in descending order by selected key",
            failureDetailText: "first out-of-order selected key pair",
            inOrder: (previous, current, keyComparer) => keyComparer.Compare(previous, current) >= 0,
            callerFilePath,
            callerLineNumber);
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

    private static void AssertInOrder(
        IEnumerable? subject,
        string? subjectExpression,
        string? because,
        string expectationText,
        string failureDetailText,
        Func<object?, object?, bool> inOrder,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation(expectationText, IncludeExpectedValue: false),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var enumerator = subject.GetEnumerator();
        try
        {
            if (!enumerator.MoveNext())
            {
                return;
            }

            var previous = enumerator.Current;
            var index = 1;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (inOrder(previous, current))
                {
                    previous = current;
                    index++;
                    continue;
                }

                var failure = new Failure(
                    subjectLabel,
                    new Expectation(expectationText, IncludeExpectedValue: false),
                    new RenderedText($"{failureDetailText} at index {index}: previous {FormatSingleValue(previous)} then current {FormatSingleValue(current)}"),
                    because);
                Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
                return;
            }
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }
    }

    private static int CompareObjects(object? previous, object? current)
    {
        if (ReferenceEquals(previous, current))
        {
            return 0;
        }

        if (previous is null)
        {
            return -1;
        }

        if (current is null)
        {
            return 1;
        }

        if (previous is IComparable comparable)
        {
            try
            {
                return comparable.CompareTo(current);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(
                    $"Values of runtime type '{previous.GetType().FullName}' do not define a compatible default ordering. Use a key-selector overload with an explicit comparer.",
                    ex);
            }
        }

        throw new InvalidOperationException(
            $"Values of runtime type '{previous.GetType().FullName}' do not define a default ordering. Use a key-selector overload with an explicit comparer.");
    }

    private static void AssertInOrderByKey<T, TKey>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        Func<T, TKey> keySelector,
        IComparer<TKey> comparer,
        string? because,
        string expectationText,
        string failureDetailText,
        Func<TKey, TKey, IComparer<TKey>, bool> inOrder,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation(expectationText, IncludeExpectedValue: false),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        using var enumerator = subject.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return;
        }

        var previousKey = keySelector(enumerator.Current);
        var index = 1;
        while (enumerator.MoveNext())
        {
            var currentKey = keySelector(enumerator.Current);
            if (inOrder(previousKey, currentKey, comparer))
            {
                previousKey = currentKey;
                index++;
                continue;
            }

            var failure = new Failure(
                subjectLabel,
                new Expectation(expectationText, IncludeExpectedValue: false),
                new RenderedText($"{failureDetailText} at index {index}: previous {FormatSingleValue(previousKey)} then current {FormatSingleValue(currentKey)}"),
                because);
            Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }
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

    private static ContainSingleResult EvaluateContainSingle<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        Func<T, bool> predicate,
        string? because)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        const string expectationText = "to contain a single item matching predicate";
        if (subject is null)
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: null,
                FailureMessage: RenderContainSingleFailure(subjectLabel, expectationText, subject, because));
        }

        var count = 0;
        object? singleItem = null;

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
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: null,
                FailureMessage: RenderContainSingleFailure(subjectLabel, expectationText, count, because));
        }

        return new ContainSingleResult(
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

    private static string RenderContainKeyFailure<TKey>(string subjectLabel, TKey expectedKey, object? actual, string? because)
    {
        var failure = new Failure(
            subjectLabel,
            new Expectation("to contain key", expectedKey),
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

    private static IComparer<T> GetOrderComparer<T>()
    {
        if (DefaultOrderComparerCache<T>.HasDefaultOrderComparer)
        {
            return DefaultOrderComparerCache<T>.DefaultComparer;
        }

        throw new InvalidOperationException(
            $"Type '{typeof(T).FullName}' does not define a default ordering. Use an overload that accepts an IComparer<{typeof(T).Name}>.");
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

    private static bool ContainsItem<T>(IReadOnlyList<T> values, T candidate, IEqualityComparer<T> comparer)
    {
        for (var i = 0; i < values.Count; i++)
        {
            if (comparer.Equals(values[i], candidate))
            {
                return true;
            }
        }

        return false;
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

    private static string FormatSortedSequence<T>(IEnumerable<T> values)
    {
        var formatter = AxiomServices.Configuration.ValueFormatter;
        var renderedValues = new List<string>();
        foreach (var value in values)
        {
            renderedValues.Add(formatter.Format(value));
        }

        renderedValues.Sort(StringComparer.Ordinal);

        if (renderedValues.Count == 0)
        {
            return "[]";
        }

        var builder = new StringBuilder();
        builder.Append('[');
        for (var i = 0; i < renderedValues.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(", ");
            }

            builder.Append(renderedValues[i]);
        }

        builder.Append(']');
        return builder.ToString();
    }

    private static string FormatEntry<TKey, TValue>(TKey key, TValue value)
    {
        return $"{FormatSingleValue(key)} => {FormatSingleValue(value)}";
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

    private static class DefaultOrderComparerCache<T>
    {
        public static readonly IComparer<T> DefaultComparer = Comparer<T>.Default;
        public static readonly bool HasDefaultOrderComparer =
            typeof(IComparable<T>).IsAssignableFrom(typeof(T)) ||
            typeof(IComparable).IsAssignableFrom(typeof(T));
    }
}
