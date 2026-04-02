using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncEnumerableAssertions<T>
{
    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainInOrderAsync(
        IEnumerable<T> expectedSequence,
        string? because = null,
        bool allowGaps = true,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedSequence);

        var expectedItems = MaterialiseExpectedSequence(expectedSequence);
        var expectationText = BuildContainInOrderExpectationText(allowGaps, usesSelectedKey: false);
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation(expectationText, new RenderedText(FormatSequence(expectedItems))),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        if (expectedItems.Length == 0)
        {
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var comparer = GetComparer<T>();
        var expectedIndex = 0;
        var matched = false;
        if (allowGaps)
        {
            var result = await ContainsInOrderAllowingGapsAsync(subject, expectedItems, comparer, item => item)
                .ConfigureAwait(false);
            matched = result.Matched;
            expectedIndex = result.ExpectedIndex;
        }
        else
        {
            matched = await ContainsInOrderWithoutGapsAsync(subject, expectedItems, comparer, item => item)
                .ConfigureAwait(false);
        }

        if (matched)
        {
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        Fail(
            new Failure(
                SubjectLabel(),
                new Expectation(expectationText, new RenderedText(FormatSequence(expectedItems))),
                allowGaps
                    ? new RenderedText(
                        $"missing expected item at sequence index {expectedIndex}: {FormatValue(expectedItems[expectedIndex])}")
                    : new RenderedText("missing adjacent ordered sequence"),
                because),
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }

    public async ValueTask<AndContinuation<AsyncEnumerableAssertions<T>>> ContainInOrderAsync<TKey>(
        IEnumerable<TKey> expectedSequence,
        Func<T, TKey> keySelector,
        string? because = null,
        bool allowGaps = true,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedSequence);
        ArgumentNullException.ThrowIfNull(keySelector);

        var expectedItems = MaterialiseExpectedSequence(expectedSequence);
        var expectationText = BuildContainInOrderExpectationText(allowGaps, usesSelectedKey: true);
        var subject = Subject;
        if (subject is null)
        {
            Fail(
                new Failure(
                    SubjectLabel(),
                    new Expectation(expectationText, new RenderedText(FormatSequence(expectedItems))),
                    subject,
                    because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        if (expectedItems.Length == 0)
        {
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        var comparer = GetComparer<TKey>();
        var expectedIndex = 0;
        var matched = false;
        if (allowGaps)
        {
            var result = await ContainsInOrderAllowingGapsAsync(subject, expectedItems, comparer, keySelector)
                .ConfigureAwait(false);
            matched = result.Matched;
            expectedIndex = result.ExpectedIndex;
        }
        else
        {
            matched = await ContainsInOrderWithoutGapsAsync(subject, expectedItems, comparer, keySelector)
                .ConfigureAwait(false);
        }

        if (matched)
        {
            return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
        }

        Fail(
            new Failure(
                SubjectLabel(),
                new Expectation(expectationText, new RenderedText(FormatSequence(expectedItems))),
                allowGaps
                    ? new RenderedText(
                        $"missing expected selected value at sequence index {expectedIndex}: {FormatValue(expectedItems[expectedIndex])}")
                    : new RenderedText("missing adjacent ordered sequence for selected values"),
                because),
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<AsyncEnumerableAssertions<T>>(this);
    }
}
