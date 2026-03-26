using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;
namespace Axiom.Assertions.AssertionTypes;

public sealed class AsyncEnumerableAssertions<T>(IAsyncEnumerable<T>? subject, string? subjectExpression)
{
    public IAsyncEnumerable<T>? Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;

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

    public async ValueTask<ContainSingleContinuation<AsyncEnumerableAssertions<T>, T>> ContainSingleAsync(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var result = await EvaluateContainSingleAsync(because).ConfigureAwait(false);
        if (result.FailureMessage is not null)
        {
            AssertionFailureDispatcher.Fail(result.FailureMessage, callerFilePath, callerLineNumber);
        }

        return new ContainSingleContinuation<AsyncEnumerableAssertions<T>, T>(
            this,
            result.HasSingleItem,
            result.SingleItem,
            result.FailureMessage,
            "ContainSingleAsync");
    }

    public async ValueTask<ContainSingleContinuation<AsyncEnumerableAssertions<T>, T>> ContainSingleAsync(
        Func<T, bool> predicate,
        string? because = null,
        [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var result = await EvaluateContainSingleAsync(predicate, predicateExpression, because).ConfigureAwait(false);
        if (result.FailureMessage is not null)
        {
            AssertionFailureDispatcher.Fail(result.FailureMessage, callerFilePath, callerLineNumber);
        }

        return new ContainSingleContinuation<AsyncEnumerableAssertions<T>, T>(
            this,
            result.HasSingleItem,
            result.SingleItem,
            result.FailureMessage,
            "ContainSingleAsync");
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> HaveUniqueItemsAsync(
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
                    new Expectation("to have unique items", IncludeExpectedValue: false),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var seen = new HashSet<T>(GetComparer<T>());
        var index = 0;
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var item = enumerator.Current;
            if (seen.Add(item))
            {
                index++;
                continue;
            }

            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to have unique items", IncludeExpectedValue: false),
                    new RenderedText($"first duplicate item at index {index}: {FormatValue(item)}"),
                    because),
                callerFilePath,
                callerLineNumber);
            break;
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> HaveUniqueItemsByAsync<TKey>(
        Func<T, TKey> keySelector,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        return EvaluateHaveUniqueItemsByAsync(keySelector, comparer: null, because, callerFilePath, callerLineNumber);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> HaveUniqueItemsByAsync<TKey>(
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(comparer, nameof(comparer));

        return await EvaluateHaveUniqueItemsByAsync(keySelector, (IEqualityComparer<TKey>)comparer, because, callerFilePath, callerLineNumber)
            .ConfigureAwait(false);
    }

    private async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> EvaluateHaveUniqueItemsByAsync<TKey>(
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer,
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
                    new Expectation("to have unique items by selected key", IncludeExpectedValue: false),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var seen = new HashSet<TKey>(comparer ?? GetComparer<TKey>());
        var index = 0;
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var key = keySelector(enumerator.Current);
            if (seen.Add(key))
            {
                index++;
                continue;
            }

            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to have unique items by selected key", IncludeExpectedValue: false),
                    new RenderedText($"first duplicate selected key at index {index}: {FormatValue(key)}"),
                    because),
                callerFilePath,
                callerLineNumber);
            break;
        }

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> SatisfyRespectivelyAsync(
        params Action<T>[] assertionsForItems)
    {
        return SatisfyRespectivelyAsync(because: null, assertionsForItems);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> SatisfyRespectivelyAsync(
        string? because,
        params Action<T>[] assertionsForItems)
    {
        ArgumentNullException.ThrowIfNull(assertionsForItems);

        for (var index = 0; index < assertionsForItems.Length; index++)
        {
            if (assertionsForItems[index] is null)
            {
                throw new ArgumentNullException(nameof(assertionsForItems), $"assertionsForItems[{index}] must not be null.");
            }
        }

        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation("to satisfy assertions respectively (same order and count)", IncludeExpectedValue: false),
                    subject,
                    because),
                callerFilePath: null,
                callerLineNumber: 0);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var expectedCount = assertionsForItems.Length;
        await using var enumerator = subject.GetAsyncEnumerator();

        for (var index = 0; index < expectedCount; index++)
        {
            if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                Fail(
                    new Failure(
                        SubjectLabel(),
                        new Expectation("to satisfy assertions respectively (same order and count)", IncludeExpectedValue: false),
                        new RenderedText($"async stream had fewer items than assertions (expected {expectedCount}, found {index})"),
                        because),
                    callerFilePath: null,
                    callerLineNumber: 0);
                return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
            }

            try
            {
                assertionsForItems[index](enumerator.Current);
            }
            catch (InvalidOperationException ex)
            {
                Fail(
                    new Failure(
                        SubjectLabel(),
                        new Expectation($"to satisfy assertions respectively (failing index {index})", IncludeExpectedValue: false),
                        new RenderedText(ex.Message),
                        because),
                    callerFilePath: null,
                    callerLineNumber: 0);
                return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
            }
        }

        if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var actualCount = expectedCount + 1;
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            actualCount++;
        }

        Fail(
            new Failure(
                SubjectLabel(),
                new Expectation("to satisfy assertions respectively (same order and count)", IncludeExpectedValue: false),
                new RenderedText($"async stream had more items than assertions (expected {expectedCount}, found {actualCount})"),
                because),
            callerFilePath: null,
            callerLineNumber: 0);

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    private async ValueTask<ContainSingleResult> EvaluateContainSingleAsync(string? because)
    {
        var subject = Subject;
        if (subject is null)
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure("to contain a single item", subject, because));
        }

        await using var enumerator = subject.GetAsyncEnumerator();
        if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure("to contain a single item", 0, because));
        }

        var singleItem = enumerator.Current;
        if (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(
                    "to contain a single item",
                    AtLeastTwoItemsToken.Instance,
                    because));
        }

        return new ContainSingleResult(
            HasSingleItem: true,
            SingleItem: singleItem,
            FailureMessage: null);
    }

    private async ValueTask<ContainSingleResult> EvaluateContainSingleAsync(
        Func<T, bool> predicate,
        string? predicateExpression,
        string? because)
    {
        var expectationText = AssertionMessageText.BuildPredicateExpectationText(
            "to contain a single item matching predicate",
            predicateExpression);
        var subject = Subject;
        if (subject is null)
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(expectationText, subject, because));
        }

        var index = 0;
        var hasSingleItem = false;
        T singleItem = default!;

        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            if (!predicate(enumerator.Current))
            {
                index++;
                continue;
            }

            if (!hasSingleItem)
            {
                hasSingleItem = true;
                singleItem = enumerator.Current;
                index++;
                continue;
            }

            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(
                    expectationText,
                    new AtLeastTwoMatchingItemsToken(index),
                    because));
        }

        if (!hasSingleItem)
        {
            return new ContainSingleResult(
                HasSingleItem: false,
                SingleItem: default!,
                FailureMessage: RenderContainSingleFailure(expectationText, 0, because));
        }

        return new ContainSingleResult(
            HasSingleItem: true,
            SingleItem: singleItem,
            FailureMessage: null);
    }

    private void Fail(Failure failure, string? callerFilePath, int callerLineNumber)
    {
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }

    private static IEqualityComparer<TValue> GetComparer<TValue>()
    {
        if (AxiomServices.Configuration.ComparerProvider.TryGetEqualityComparer<TValue>(out var comparer) &&
            comparer is not null)
        {
            return comparer;
        }

        return EqualityComparer<TValue>.Default;
    }

    private string RenderContainSingleFailure(string expectationText, object? actual, string? because)
    {
        var failure = new Failure(
            SubjectLabel(),
            new Expectation(expectationText, IncludeExpectedValue: false),
            actual,
            because);
        return FailureMessageRenderer.Render(failure);
    }

    private readonly record struct RenderedText(string Text)
    {
        public override string ToString()
        {
            return Text;
        }
    }

    private static string FormatValue<TValue>(TValue value)
    {
        return AxiomServices.Configuration.ValueFormatter.Format(value);
    }

    private readonly record struct ContainSingleResult(
        bool HasSingleItem,
        T SingleItem,
        string? FailureMessage);

    private sealed class NoMatchingItemToken
    {
        public static NoMatchingItemToken Instance { get; } = new();

        public override string ToString()
        {
            return "<no matching item>";
        }
    }

    private sealed class AtLeastTwoItemsToken
    {
        public static AtLeastTwoItemsToken Instance { get; } = new();

        public override string ToString()
        {
            return "at least 2 items";
        }
    }

    private sealed record AtLeastTwoMatchingItemsToken(int Index)
    {
        public override string ToString()
        {
            return $"at least 2 matching items (second match at index {Index})";
        }
    }
}
