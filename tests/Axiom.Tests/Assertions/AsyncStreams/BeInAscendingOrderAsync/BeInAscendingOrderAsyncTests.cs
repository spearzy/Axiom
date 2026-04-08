namespace Axiom.Tests.Assertions.AsyncStreams.BeInAscendingOrderAsync;

public sealed class BeInAscendingOrderAsyncTests
{
    [Fact]
    public async Task BeInAscendingOrderAsync_Passes_WhenItemsAreSortedAscending()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var assertions = values.Should();
        var continuation = await assertions.BeInAscendingOrderAsync();

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Passes_WhenItemsContainEqualNeighbours()
    {
        var values = CreateAsyncSequence(1, 1, 2, 2);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeInAscendingOrderAsync());

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Passes_WhenComparerDefinesOrdering()
    {
        var values = CreateAsyncSequence("a", "bb", "ccc");

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeInAscendingOrderAsync(StringLengthComparer.Instance));

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Passes_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeInAscendingOrderAsync());

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Throws_WhenOrderIsViolated()
    {
        var values = CreateAsyncSequence(1, 3, 2, 4);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeInAscendingOrderAsync());

        Assert.Equal(
            "Expected values to be in ascending order, but found first out-of-order pair at index 2: previous 3 then current 2.",
            ex.Message);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Throws_WhenComparerOrderIsViolated()
    {
        var values = CreateAsyncSequence("a", "ccc", "bb");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeInAscendingOrderAsync(StringLengthComparer.Instance));

        Assert.Equal(
            "Expected values to be in ascending order, but found first out-of-order pair at index 2: previous \"ccc\" then current \"bb\".",
            ex.Message);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeInAscendingOrderAsync());

        Assert.Equal("Expected values to be in ascending order, but found <null>.", ex.Message);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("a");
        IComparer<string>? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().BeInAscendingOrderAsync(comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(1, 3, 2, 4);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeInAscendingOrderAsync("events should stay sorted"));

        Assert.Contains("because events should stay sorted", ex.Message, StringComparison.Ordinal);
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
