using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.BeEmptyAsync;

public sealed class BeEmptyAsyncTests
{
    [Fact]
    public async Task BeEmptyAsync_Passes_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var assertions = values.Should();
        var continuation = await assertions.BeEmptyAsync();

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task BeEmptyAsync_Throws_WhenStreamHasItems()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeEmptyAsync());

        Assert.Equal("Expected values to be empty, but found 1.", ex.Message);
    }

    [Fact]
    public async Task BeEmptyAsync_StopsAfterFirstItem_WhenStreamIsNotEmpty()
    {
        var tracking = new TrackingAsyncEnumerable<int>([1, 2, 3]);
        IAsyncEnumerable<int> values = tracking;

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeEmptyAsync());

        Assert.Equal(1, tracking.YieldCount);
        Assert.Equal(1, tracking.MoveNextCallCount);
    }

    [Fact]
    public async Task BeEmptyAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeEmptyAsync());

        Assert.Equal("Expected values to be empty, but found <null>.", ex.Message);
    }

    private static async IAsyncEnumerable<T> CreateAsyncSequence<T>(params T[] items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }
}
