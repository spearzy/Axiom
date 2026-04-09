using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

internal static partial class CollectionAssertionEngine
{
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
            AssertionFailureDispatcher.Fail(failureMessage, callerFilePath, callerLineNumber);
            return new ContainKeyResult<TValue>(HasValue: false, Value: default!, FailureMessage: failureMessage);
        }

        if (subject.TryGetValue(expectedKey, out var value))
        {
            return new ContainKeyResult<TValue>(HasValue: true, Value: value, FailureMessage: null);
        }

        var failureText = new RenderedText($"keys were {FormatSortedSequence(subject.Keys)}");
        var failure = RenderContainKeyFailure(subjectLabel, expectedKey, failureText, because);
        AssertionFailureDispatcher.Fail(failure, callerFilePath, callerLineNumber);
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertContainValue<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue>? subject,
        string? subjectExpression,
        TValue expectedValue,
        IEqualityComparer<TValue>? comparer,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var valueComparer = GetComparer(comparer);
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
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertNotContainValue<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue>? subject,
        string? subjectExpression,
        TValue unexpectedValue,
        IEqualityComparer<TValue>? comparer,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var valueComparer = GetComparer(comparer);
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertContainEntry<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue>? subject,
        string? subjectExpression,
        TKey expectedKey,
        TValue expectedValue,
        IEqualityComparer<TValue>? comparer,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (!subject.TryGetValue(expectedKey, out var actualValue))
        {
            var missingKeyFailure = new Failure(
                subjectLabel,
                new Expectation("to contain entry", expectedEntry ??= new RenderedText(FormatEntry(expectedKey, expectedValue))),
                new RenderedText($"key {FormatSingleValue(expectedKey)} was missing"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(missingKeyFailure), callerFilePath, callerLineNumber);
            return;
        }

        var valueComparer = GetComparer(comparer);
        if (valueComparer.Equals(actualValue, expectedValue))
        {
            return;
        }

        var valueMismatchFailure = new Failure(
            subjectLabel,
            new Expectation("to contain entry", expectedEntry ??= new RenderedText(FormatEntry(expectedKey, expectedValue))),
            new RenderedText($"key {FormatSingleValue(expectedKey)} had value {FormatSingleValue(actualValue)}"),
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(valueMismatchFailure), callerFilePath, callerLineNumber);
    }

    public static void AssertNotContainEntry<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue>? subject,
        string? subjectExpression,
        TKey unexpectedKey,
        TValue unexpectedValue,
        IEqualityComparer<TValue>? comparer,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (!subject.TryGetValue(unexpectedKey, out var actualValue))
        {
            return;
        }

        var valueComparer = GetComparer(comparer);
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
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(presentEntryFailure), callerFilePath, callerLineNumber);
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
}
