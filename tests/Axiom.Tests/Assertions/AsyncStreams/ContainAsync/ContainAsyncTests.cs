using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.ContainAsync;

public sealed class ContainAsyncTests
{
    [Fact]
    public async Task ContainAsync_Passes_WhenExpectedItemIsPresent()
    {
        var values = CreateAsyncSequence("alpha", "beta", "gamma");

        var assertions = values.Should();
        var continuation = await assertions.ContainAsync("beta");

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainAsync_Throws_WhenExpectedItemIsMissing()
    {
        var values = CreateAsyncSequence("alpha", "beta");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainAsync("gamma"));

        Assert.Equal("Expected values to contain \"gamma\", but found <no matching item>.", ex.Message);
    }

    [Fact]
    public async Task ContainAsync_Passes_WhenComparerMatchesItem()
    {
        var values = CreateAsyncSequence("alpha", "beta", "gamma");

        var assertions = values.Should();
        var continuation = await assertions.ContainAsync("BETA", StringComparer.OrdinalIgnoreCase);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainAsync_Predicate_Passes_WhenMatchingItemIsPresent()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var assertions = values.Should();
        var continuation = await assertions.ContainAsync(x => x > 2);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainAsync_Predicate_Throws_WhenNoMatchingItemIsPresent()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainAsync(x => x > 5));

        Assert.Equal("Expected values to contain an item matching predicate `x => x > 5`, but found <no matching item>.", ex.Message);
    }

    [Fact]
    public async Task ContainAsync_StopsAfterMatchIsFound()
    {
        var tracking = new TrackingAsyncEnumerable<int>([1, 2, 3, 4]);
        IAsyncEnumerable<int> values = tracking;

        await values.Should().ContainAsync(2);

        Assert.Equal(2, tracking.YieldCount);
        Assert.Equal(2, tracking.MoveNextCallCount);
    }

    [Fact]
    public async Task ContainAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("alpha", "beta");
        IEqualityComparer<string>? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainAsync("beta", comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task ContainAsync_Predicate_ThrowsForNullPredicate()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainAsync((Func<int, bool>)null!));
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
