using System.Collections;
using System.Text;
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
        var expectedText = new RenderedText(FormatSequence(expected));
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain all", expectedText),
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
                new Expectation("to contain all", expectedText),
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
        var expectedText = new RenderedText(FormatSequence(expected));
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain any of", expectedText),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (expected.Length == 0)
        {
            var noExpectedItemsFailure = new Failure(
                subjectLabel,
                new Expectation("to contain any of", expectedText),
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
            new Expectation("to contain any of", expectedText),
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
        var unexpectedText = new RenderedText(FormatSequence(unexpected));
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to not contain any of", unexpectedText),
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
                    new Expectation("to not contain any of", unexpectedText),
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

        var seen = new HashSet<object?>();
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
        var expectedSequenceText = new RenderedText(FormatSequence(expectedItems));
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain exactly", expectedSequenceText),
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
                    new Expectation("to contain exactly", expectedSequenceText),
                    new RenderedText($"extra item at index {index}: {FormatSingleValue(item)}"),
                    because);
                Fail(FailureMessageRenderer.Render(extraItemFailure), callerFilePath, callerLineNumber);
                return;
            }

            if (!comparer.Equals(item, expectedItems[index]))
            {
                var mismatchFailure = new Failure(
                    subjectLabel,
                    new Expectation("to contain exactly", expectedSequenceText),
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
                new Expectation("to contain exactly", expectedSequenceText),
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
        var supersetText = new RenderedText(FormatSequence(supersetItems));
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to be a subset of", supersetText),
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
                new Expectation("to be a subset of", supersetText),
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
        var subsetText = new RenderedText(FormatSequence(subsetItems));
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to be a superset of", subsetText),
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
                new Expectation("to be a superset of", subsetText),
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
        var expectedEntry = new RenderedText(FormatEntry(expectedKey, expectedValue));
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation("to contain entry", expectedEntry),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (!subject.TryGetValue(expectedKey, out var actualValue))
        {
            var missingKeyFailure = new Failure(
                subjectLabel,
                new Expectation("to contain entry", expectedEntry),
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
            new Expectation("to contain entry", expectedEntry),
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

    private static string RenderContainSingleFailure(string subjectLabel, object? actual, string? because)
    {
        var failure = new Failure(
            subjectLabel,
            new Expectation("to contain a single item", IncludeExpectedValue: false),
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
}
