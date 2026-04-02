using System.Text;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed partial class AsyncEnumerableAssertions<T>
{
    private void Fail(Failure failure, string? callerFilePath, int callerLineNumber)
    {
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
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

    private static async ValueTask<OrderedContainmentResult> ContainsInOrderAllowingGapsAsync<TExpected>(
        IAsyncEnumerable<T> subject,
        IReadOnlyList<TExpected> expectedItems,
        IEqualityComparer<TExpected> comparer,
        Func<T, TExpected> selector)
    {
        var expectedIndex = 0;
        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var selectedValue = selector(enumerator.Current);
            if (!comparer.Equals(selectedValue, expectedItems[expectedIndex]))
            {
                continue;
            }

            expectedIndex++;
            if (expectedIndex == expectedItems.Count)
            {
                return new OrderedContainmentResult(Matched: true, ExpectedIndex: expectedIndex);
            }
        }

        return new OrderedContainmentResult(Matched: false, ExpectedIndex: expectedIndex);
    }

    private static async ValueTask<bool> ContainsInOrderWithoutGapsAsync<TExpected>(
        IAsyncEnumerable<T> subject,
        IReadOnlyList<TExpected> expectedItems,
        IEqualityComparer<TExpected> comparer,
        Func<T, TExpected> selector)
    {
        // Intentionally mirrors the sync ordered-sequence matcher, but stays local here
        // so async streams keep a self-contained, single-pass implementation.
        var fallbackTable = BuildFallbackTable(expectedItems, comparer);
        var matchedCount = 0;

        await using var enumerator = subject.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var selectedValue = selector(enumerator.Current);
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

    private static TExpected[] MaterialiseExpectedSequence<TExpected>(IEnumerable<TExpected> expectedSequence)
    {
        if (expectedSequence is TExpected[] array)
        {
            return array;
        }

        var buffer = new List<TExpected>();
        foreach (var item in expectedSequence)
        {
            buffer.Add(item);
        }

        return buffer.ToArray();
    }

    private static bool ContainsItem<TValue>(
        IReadOnlyList<TValue> values,
        TValue candidate,
        IEqualityComparer<TValue> comparer)
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

    private static int[] BuildFallbackTable<TValue>(IReadOnlyList<TValue> pattern, IEqualityComparer<TValue> comparer)
    {
        var fallbackTable = new int[pattern.Count];
        var prefixLength = 0;

        for (var i = 1; i < pattern.Count; i++)
        {
            while (prefixLength > 0 && !comparer.Equals(pattern[i], pattern[prefixLength]))
            {
                prefixLength = fallbackTable[prefixLength - 1];
            }

            if (comparer.Equals(pattern[i], pattern[prefixLength]))
            {
                prefixLength++;
            }

            fallbackTable[i] = prefixLength;
        }

        return fallbackTable;
    }

    private static string FormatSequence<TValue>(IReadOnlyList<TValue> values)
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

    private readonly record struct ContainSingleResult(
        bool HasSingleItem,
        T SingleItem,
        string? FailureMessage);

    private readonly record struct OrderedContainmentResult(
        bool Matched,
        int ExpectedIndex);

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
