namespace Axiom.Tests.Assertions.AsyncStreams.BeInDescendingOrderAsync;

public sealed class BeInDescendingOrderAsyncTests
{
    [Fact]
    public async Task BeInDescendingOrderAsync_Passes_WhenItemsAreSortedDescending()
    {
        var values = CreateAsyncSequence(3, 2, 1);

        var assertions = values.Should();
        var continuation = await assertions.BeInDescendingOrderAsync();

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task BeInDescendingOrderAsync_Passes_WhenItemsContainEqualNeighbours()
    {
        var values = CreateAsyncSequence(3, 3, 2, 2);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeInDescendingOrderAsync());

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeInDescendingOrderAsync_Passes_WhenComparerDefinesOrdering()
    {
        var values = CreateAsyncSequence("ccc", "bb", "a");

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeInDescendingOrderAsync(StringLengthComparer.Instance));

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeInDescendingOrderAsync_Passes_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeInDescendingOrderAsync());

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeInDescendingOrderAsync_Throws_WhenOrderIsViolated()
    {
        var values = CreateAsyncSequence(3, 1, 2, 0);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeInDescendingOrderAsync());

        Assert.Equal(
            "Expected values to be in descending order, but found first out-of-order pair at index 2: previous 1 then current 2.",
            ex.Message);
    }

    [Fact]
    public async Task BeInDescendingOrderAsync_Throws_WhenComparerOrderIsViolated()
    {
        var values = CreateAsyncSequence("ccc", "a", "bb");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeInDescendingOrderAsync(StringLengthComparer.Instance));

        Assert.Equal(
            "Expected values to be in descending order, but found first out-of-order pair at index 2: previous \"a\" then current \"bb\".",
            ex.Message);
    }

    [Fact]
    public async Task BeInDescendingOrderAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeInDescendingOrderAsync());

        Assert.Equal("Expected values to be in descending order, but found <null>.", ex.Message);
    }

    [Fact]
    public async Task BeInDescendingOrderAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("a");
        IComparer<string>? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().BeInDescendingOrderAsync(comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task BeInDescendingOrderAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(3, 1, 2, 0);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeInDescendingOrderAsync("events should stay reverse sorted"));

        Assert.Contains("because events should stay reverse sorted", ex.Message, StringComparison.Ordinal);
    }

    private static async IAsyncEnumerable<T> CreateAsyncSequence<T>(params T[] items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }

    private sealed class StringLengthComparer : IComparer<string>
    {
        public static StringLengthComparer Instance { get; } = new();

        public int Compare(string? x, string? y)
        {
            return Comparer<int>.Default.Compare(x?.Length ?? 0, y?.Length ?? 0);
        }
    }
}
