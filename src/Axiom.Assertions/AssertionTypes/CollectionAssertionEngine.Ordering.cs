using System.Collections;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

internal static partial class CollectionAssertionEngine
{
    public static void AssertContainInOrder<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<T> expectedSequence,
        IEqualityComparer<T>? comparer,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (expectedItems.Length == 0)
        {
            return;
        }

        var equalityComparer = GetComparer(comparer);
        var expectedIndex = 0;
        var matched = allowGaps
            ? ContainsInOrderAllowingGaps(subject, expectedItems, equalityComparer, out expectedIndex)
            : ContainsInOrderWithoutGaps(subject, expectedItems, equalityComparer);
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
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertContainInOrderByKey<T, TKey>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<TKey> expectedSequence,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (expectedItems.Length == 0)
        {
            return;
        }

        var equalityComparer = GetComparer(comparer);
        var expectedIndex = 0;
        var matched = allowGaps
            ? ContainsProjectedInOrderAllowingGaps(subject, expectedItems, keySelector, equalityComparer, out expectedIndex)
            : ContainsProjectedInOrderWithoutGaps(subject, expectedItems, keySelector, equalityComparer);
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
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
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

    public static void AssertBeInAscendingOrder<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IComparer<T> comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        AssertInOrder(
            subject,
            subjectExpression,
            comparer,
            because,
            expectationText: "to be in ascending order",
            failureDetailText: "first out-of-order pair",
            inOrder: (previous, current, resolvedComparer) => resolvedComparer.Compare(previous, current) <= 0,
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

    public static void AssertBeInDescendingOrder<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IComparer<T> comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        AssertInOrder(
            subject,
            subjectExpression,
            comparer,
            because,
            expectationText: "to be in descending order",
            failureDetailText: "first out-of-order pair",
            inOrder: (previous, current, resolvedComparer) => resolvedComparer.Compare(previous, current) >= 0,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
                AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
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

    private static void AssertInOrder<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IComparer<T> comparer,
        string? because,
        string expectationText,
        string failureDetailText,
        Func<T, T, IComparer<T>, bool> inOrder,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        using var enumerator = subject.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return;
        }

        var previous = enumerator.Current;
        var index = 1;
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            if (inOrder(previous, current, comparer))
            {
                previous = current;
                index++;
                continue;
            }

            var failure = new Failure(
                subjectLabel,
                new Expectation(expectationText, IncludeExpectedValue: false),
                new RenderedText(
                    $"{failureDetailText} at index {index}: previous {FormatSingleValue(previous)} then current {FormatSingleValue(current)}"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }
    }
}
