using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncEnumerableAssertions<T>
{
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
}
