using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

internal static partial class CollectionAssertionEngine
{
    public static void AssertContain<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        T expected,
        IEqualityComparer<T>? comparer,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var equalityComparer = GetComparer(comparer);
        foreach (var item in subject)
        {
            if (equalityComparer.Equals(item, expected))
            {
                return;
            }
        }

        var failure = new Failure(
            subjectLabel,
            new Expectation("to contain", expected),
            subject,
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static void AssertContainAll<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<T> expectedItems,
        IEqualityComparer<T>? comparer,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (expected.Length == 0)
        {
            return;
        }

        var subjectItems = MaterialiseExpectedSequence(subject);
        var equalityComparer = GetComparer(comparer);
        for (var index = 0; index < expected.Length; index++)
        {
            if (ContainsItem(subjectItems, expected[index], equalityComparer))
            {
                continue;
            }

            var missingItemFailure = new Failure(
                subjectLabel,
                new Expectation("to contain all", expectedText ??= new RenderedText(FormatSequence(expected))),
                new RenderedText($"missing expected item at index {index}: {FormatSingleValue(expected[index])}"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(missingItemFailure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertContainAny<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<T> expectedItems,
        IEqualityComparer<T>? comparer,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (expected.Length == 0)
        {
            var noExpectedItemsFailure = new Failure(
                subjectLabel,
                new Expectation("to contain any of", expectedText ??= new RenderedText(FormatSequence(expected))),
                new RenderedText("no expected items were provided"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(noExpectedItemsFailure), callerFilePath, callerLineNumber);
            return;
        }

        var equalityComparer = GetComparer(comparer);
        foreach (var subjectItem in subject)
        {
            if (ContainsItem(expected, subjectItem, equalityComparer))
            {
                return;
            }
        }

        var missingAnyFailure = new Failure(
            subjectLabel,
            new Expectation("to contain any of", expectedText ??= new RenderedText(FormatSequence(expected))),
            new RenderedText("none of the expected items were found"),
            because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(missingAnyFailure), callerFilePath, callerLineNumber);
    }

    public static void AssertNotContainAny<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        IEnumerable<T> unexpectedItems,
        IEqualityComparer<T>? comparer,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        if (unexpected.Length == 0)
        {
            return;
        }

        var equalityComparer = GetComparer(comparer);
        var subjectIndex = 0;
        foreach (var subjectItem in subject)
        {
            if (ContainsItem(unexpected, subjectItem, equalityComparer))
            {
                var matchingUnexpectedFailure = new Failure(
                    subjectLabel,
                    new Expectation("to not contain any of", unexpectedText ??= new RenderedText(FormatSequence(unexpected))),
                    new RenderedText($"first matching item at subject index {subjectIndex}: {FormatSingleValue(subjectItem)}"),
                    because);
                AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(matchingUnexpectedFailure), callerFilePath, callerLineNumber);
                return;
            }

            subjectIndex++;
        }
    }

    public static void AssertOnlyContain<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        Func<T, bool> predicate,
        string? predicateExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        string? expectationText = null;
        string GetExpectationText() =>
            expectationText ??= AssertionMessageText.BuildPredicateExpectationText(
                "to only contain items matching predicate",
                predicateExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation(GetExpectationText(), IncludeExpectedValue: false),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
                new Expectation($"{GetExpectationText()} (first non-matching index {index})", IncludeExpectedValue: false),
                item,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertNotContain<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        Func<T, bool> predicate,
        string? predicateExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = SubjectLabel(subjectExpression);
        string? expectationText = null;
        string GetExpectationText() =>
            expectationText ??= AssertionMessageText.BuildPredicateExpectationText(
                "to not contain any item matching predicate",
                predicateExpression);
        if (subject is null)
        {
            var nullFailure = new Failure(
                subjectLabel,
                new Expectation(GetExpectationText(), IncludeExpectedValue: false),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
                new Expectation($"{GetExpectationText()} (first matching index {index})", IncludeExpectedValue: false),
                item,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return;
        }
    }

    public static void AssertNotContainItem<T>(
        IEnumerable<T>? subject,
        string? subjectExpression,
        T unexpected,
        IEqualityComparer<T>? comparer,
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
            return;
        }

        var equalityComparer = GetComparer(comparer);
        foreach (var item in subject)
        {
            if (!equalityComparer.Equals(item, unexpected))
            {
                continue;
            }

            var failure = new Failure(
                subjectLabel,
                new Expectation("to not contain", unexpected),
                item,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
                AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
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
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(nullFailure), callerFilePath, callerLineNumber);
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
                AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(fewerItemsFailure), callerFilePath, callerLineNumber);
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
                AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
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
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(moreItemsFailure), callerFilePath, callerLineNumber);
    }
}
