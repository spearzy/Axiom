using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.OnlyContainAsync;

public sealed class OnlyContainAsyncTests
{
    [Fact]
    public async Task OnlyContainAsync_Passes_WhenEveryItemMatches()
    {
        var values = CreateAsyncSequence(2, 4, 6);

        var assertions = values.Should();
        var continuation = await assertions.OnlyContainAsync(x => x % 2 == 0);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task OnlyContainAsync_Throws_WhenFirstNonMatchingItemIsFound()
    {
        var values = CreateAsyncSequence(2, 4, 5, 6);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().OnlyContainAsync(x => x % 2 == 0));

        Assert.Equal("Expected values to only contain items matching predicate `x => x % 2 == 0` (first non-matching index 2), but found 5.", ex.Message);
    }

    [Fact]
    public async Task OnlyContainAsync_StopsAtFirstNonMatchingItem()
    {
        var tracking = new TrackingAsyncEnumerable<int>([2, 4, 5, 6]);
        IAsyncEnumerable<int> values = tracking;

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().OnlyContainAsync(x => x % 2 == 0));

        Assert.Equal(3, tracking.YieldCount);
        Assert.Equal(3, tracking.MoveNextCallCount);
    }

    [Fact]
    public async Task OnlyContainAsync_ThrowsForNullPredicate()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().OnlyContainAsync((Func<int, bool>)null!));
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
