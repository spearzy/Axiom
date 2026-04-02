using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncEnumerableAssertions<T>
{
    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> BeEmptyAsync(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to be empty", IncludeExpectedValue: false),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        await using var enumerator = subject.GetAsyncEnumerator();
        if (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to be empty", IncludeExpectedValue: false),
                    enumerator.Current,
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> NotBeEmptyAsync(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to not be empty", IncludeExpectedValue: false),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        await using var enumerator = subject.GetAsyncEnumerator();
        if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to not be empty", IncludeExpectedValue: false),
                    0,
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> HaveCountAsync(
        int expectedCount,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to have count", expectedCount),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var actualCount = 0;
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            actualCount++;
        }

        if (actualCount != expectedCount)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to have count", expectedCount),
                    actualCount,
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainAsync(
        T expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to contain", expected),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var comparer = GetComparer<T>();
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            if (comparer.Equals(enumerator.Current, expected))
            {
                return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
            }
        }

        Fail(
            new Failure(
                SubjectLabel(),
                new Expectation("to contain", expected),
                NoMatchingItemToken.Instance,
                because),
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainExactlyAsync(
        IEnumerable<T> expectedSequence,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedSequence);

        var expectedItems = MaterialiseExpectedSequence(expectedSequence);
        RenderedText? expectedSequenceText = null;
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to contain exactly", expectedSequenceText ??= new RenderedText(FormatSequence(expectedItems))),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var comparer = GetComparer<T>();
        var index = 0;
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var item = enumerator.Current;
            if (index >= expectedItems.Length)
            {
                Fail(
                    new Failure(
                        SubjectLabel(),
                        new Expectation("to contain exactly", expectedSequenceText ??= new RenderedText(FormatSequence(expectedItems))),
                        new RenderedText($"extra item at index {index}: {FormatValue(item)}"),
                        because),
                    callerFilePath,
                    callerLineNumber);
                return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
            }

            if (!comparer.Equals(item, expectedItems[index]))
            {
                Fail(
                    new Failure(
                        SubjectLabel(),
                        new Expectation("to contain exactly", expectedSequenceText ??= new RenderedText(FormatSequence(expectedItems))),
                        new RenderedText(
                            $"item mismatch at index {index}: expected {FormatValue(expectedItems[index])} but found {FormatValue(item)}"),
                        because),
                    callerFilePath,
                    callerLineNumber);
                return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
            }

            index++;
        }

        if (index < expectedItems.Length)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to contain exactly", expectedSequenceText ??= new RenderedText(FormatSequence(expectedItems))),
                    new RenderedText($"stream ended before expected item at index {index}: {FormatValue(expectedItems[index])}"),
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainAnyAsync(
        IEnumerable<T> expectedItems,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedItems);

        var expected = MaterialiseExpectedSequence(expectedItems);
        RenderedText? expectedText = null;
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to contain any of", expectedText ??= new RenderedText(FormatSequence(expected))),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        if (expected.Length == 0)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to contain any of", expectedText ??= new RenderedText(FormatSequence(expected))),
                    new RenderedText("no expected items were provided"),
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var comparer = GetComparer<T>();
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            if (ContainsItem(expected, enumerator.Current, comparer))
            {
                return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
            }
        }

        Fail(
            new Failure(
                SubjectLabel(),
                new Expectation("to contain any of", expectedText ??= new RenderedText(FormatSequence(expected))),
                new RenderedText("none of the expected items were found"),
                because),
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> NotContainAnyAsync(
        IEnumerable<T> unexpectedItems,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(unexpectedItems);

        var unexpected = MaterialiseExpectedSequence(unexpectedItems);
        RenderedText? unexpectedText = null;
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to not contain any of", unexpectedText ??= new RenderedText(FormatSequence(unexpected))),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        if (unexpected.Length == 0)
        {
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var comparer = GetComparer<T>();
        var subjectIndex = 0;
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var item = enumerator.Current;
            if (ContainsItem(unexpected, item, comparer))
            {
                Fail(
                    new Failure(
                        SubjectLabel(),
                        new Expectation("to not contain any of", unexpectedText ??= new RenderedText(FormatSequence(unexpected))),
                        new RenderedText($"first matching item at subject index {subjectIndex}: {FormatValue(item)}"),
                        because),
                    callerFilePath,
                    callerLineNumber);
                return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
            }

            subjectIndex++;
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainAsync(
        Func<T, bool> predicate,
        string? because = null,
        [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var expectationText = AssertionMessageText.BuildPredicateExpectationText(
            "to contain an item matching predicate",
            predicateExpression);
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation(expectationText, IncludeExpectedValue: false),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            if (predicate(enumerator.Current))
            {
                return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
            }
        }

        Fail(
            new Failure(
                SubjectLabel(),
                new Expectation(expectationText, IncludeExpectedValue: false),
                NoMatchingItemToken.Instance,
                because),
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> OnlyContainAsync(
        Func<T, bool> predicate,
        string? because = null,
        [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var expectationText = AssertionMessageText.BuildPredicateExpectationText(
            "to only contain items matching predicate",
            predicateExpression);
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation(expectationText, IncludeExpectedValue: false),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var index = 0;
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            if (predicate(enumerator.Current))
            {
                index++;
                continue;
            }

            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation($"{expectationText} (first non-matching index {index})", IncludeExpectedValue: false),
                    enumerator.Current,
                    because),
                callerFilePath,
                callerLineNumber);
            break;
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }
}
