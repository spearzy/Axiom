using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncEnumerableAssertions<T>
{
    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainExactlyInAnyOrderAsync(
        IEnumerable<T> expectedSequence,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return ContainExactlyInAnyOrderCoreAsync(expectedSequence, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainExactlyInAnyOrderAsync(
        IEnumerable<T> expectedSequence,
        IEqualityComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        return ContainExactlyInAnyOrderCoreAsync(expectedSequence, comparer, because, callerFilePath, callerLineNumber);
    }

    private async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainExactlyInAnyOrderCoreAsync(
        IEnumerable<T> expectedSequence,
        IEqualityComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        ArgumentNullException.ThrowIfNull(expectedSequence);

        var expectedItems = MaterialiseExpectedSequence(expectedSequence);
        RenderedText? expectedText = null;
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to contain exactly in any order", expectedText ??= new RenderedText(FormatSequence(expectedItems))),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var actualItems = await MaterialiseSubjectSequenceAsync(subject).ConfigureAwait(false);
        var equalityComparer = GetComparer(comparer);

        var seenExpected = new HashSet<T>(equalityComparer);
        for (var index = 0; index < expectedItems.Length; index++)
        {
            var expectedItem = expectedItems[index];
            if (!seenExpected.Add(expectedItem))
            {
                continue;
            }

            var expectedCount = CountOccurrences(expectedItems, expectedItem, equalityComparer);
            var actualCount = CountOccurrences(actualItems, expectedItem, equalityComparer);
            if (actualCount >= expectedCount)
            {
                continue;
            }

            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to contain exactly in any order", expectedText ??= new RenderedText(FormatSequence(expectedItems))),
                    new RenderedText($"missing item {FormatValue(expectedItem)}: expected count {expectedCount} but found {actualCount}"),
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var seenActual = new HashSet<T>(equalityComparer);
        for (var index = 0; index < actualItems.Length; index++)
        {
            var actualItem = actualItems[index];
            if (!seenActual.Add(actualItem))
            {
                continue;
            }

            var actualCount = CountOccurrences(actualItems, actualItem, equalityComparer);
            var expectedCount = CountOccurrences(expectedItems, actualItem, equalityComparer);
            if (expectedCount >= actualCount)
            {
                continue;
            }

            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to contain exactly in any order", expectedText ??= new RenderedText(FormatSequence(expectedItems))),
                    new RenderedText($"unexpected item {FormatValue(actualItem)}: found count {actualCount} but expected {expectedCount}"),
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainAllAsync(
        IEnumerable<T> expectedItems,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return ContainAllCoreAsync(expectedItems, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainAllAsync(
        IEnumerable<T> expectedItems,
        IEqualityComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        return ContainAllCoreAsync(expectedItems, comparer, because, callerFilePath, callerLineNumber);
    }

    private async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainAllCoreAsync(
        IEnumerable<T> expectedItems,
        IEqualityComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
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
                    new Expectation("to contain all", expectedText ??= new RenderedText(FormatSequence(expected))),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        if (expected.Length == 0)
        {
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var equalityComparer = GetComparer(comparer);
        var remainingExpected = new HashSet<T>(expected, equalityComparer);
        await using var enumerator = subject.GetAsyncEnumerator();
        while (remainingExpected.Count > 0 && await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            remainingExpected.Remove(enumerator.Current);
        }

        for (var index = 0; index < expected.Length; index++)
        {
            if (!remainingExpected.Contains(expected[index]))
            {
                continue;
            }

            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to contain all", expectedText ??= new RenderedText(FormatSequence(expected))),
                    new RenderedText($"missing expected item at index {index}: {FormatValue(expected[index])}"),
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> NotContainAsync(
        T unexpected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return NotContainCoreAsync(unexpected, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> NotContainAsync(
        T unexpected,
        IEqualityComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        return NotContainCoreAsync(unexpected, comparer, because, callerFilePath, callerLineNumber);
    }

    private async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> NotContainCoreAsync(
        T unexpected,
        IEqualityComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to not contain", unexpected),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var equalityComparer = GetComparer(comparer);
        var subjectIndex = 0;
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            if (!equalityComparer.Equals(enumerator.Current, unexpected))
            {
                subjectIndex++;
                continue;
            }

            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to not contain", unexpected),
                    new RenderedText($"first matching item at subject index {subjectIndex}: {FormatValue(enumerator.Current)}"),
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> BeSubsetOfAsync(
        IEnumerable<T> expectedSuperset,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return BeSubsetOfCoreAsync(expectedSuperset, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> BeSubsetOfAsync(
        IEnumerable<T> expectedSuperset,
        IEqualityComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        return BeSubsetOfCoreAsync(expectedSuperset, comparer, because, callerFilePath, callerLineNumber);
    }

    private async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> BeSubsetOfCoreAsync(
        IEnumerable<T> expectedSuperset,
        IEqualityComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        ArgumentNullException.ThrowIfNull(expectedSuperset);

        var supersetItems = MaterialiseExpectedSequence(expectedSuperset);
        RenderedText? supersetText = null;
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to be a subset of", supersetText ??= new RenderedText(FormatSequence(supersetItems))),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var equalityComparer = GetComparer(comparer);
        var superset = new HashSet<T>(supersetItems, equalityComparer);
        var index = 0;
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var item = enumerator.Current;
            if (superset.Contains(item))
            {
                index++;
                continue;
            }

            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to be a subset of", supersetText ??= new RenderedText(FormatSequence(supersetItems))),
                    new RenderedText($"missing item at index {index}: {FormatValue(item)}"),
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> BeSupersetOfAsync(
        IEnumerable<T> expectedSubset,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return BeSupersetOfCoreAsync(expectedSubset, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> BeSupersetOfAsync(
        IEnumerable<T> expectedSubset,
        IEqualityComparer<T> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        return BeSupersetOfCoreAsync(expectedSubset, comparer, because, callerFilePath, callerLineNumber);
    }

    private async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> BeSupersetOfCoreAsync(
        IEnumerable<T> expectedSubset,
        IEqualityComparer<T>? comparer,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        ArgumentNullException.ThrowIfNull(expectedSubset);

        var subsetItems = MaterialiseExpectedSequence(expectedSubset);
        RenderedText? subsetText = null;
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to be a superset of", subsetText ??= new RenderedText(FormatSequence(subsetItems))),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        if (subsetItems.Length == 0)
        {
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var equalityComparer = GetComparer(comparer);
        var foundItems = new HashSet<T>(equalityComparer);
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            foundItems.Add(enumerator.Current);
            if (foundItems.Count >= subsetItems.Length && subsetItems.All(foundItems.Contains))
            {
                return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
            }
        }

        for (var index = 0; index < subsetItems.Length; index++)
        {
            if (foundItems.Contains(subsetItems[index]))
            {
                continue;
            }

            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to be a superset of", subsetText ??= new RenderedText(FormatSequence(subsetItems))),
                    new RenderedText($"missing expected item at index {index}: {FormatValue(subsetItems[index])}"),
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }
}
