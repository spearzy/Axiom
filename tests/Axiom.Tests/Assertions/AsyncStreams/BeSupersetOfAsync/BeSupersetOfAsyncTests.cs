using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.BeSupersetOfAsync;

public sealed class BeSupersetOfAsyncTests
{
    [Fact]
    public async Task BeSupersetOfAsync_Passes_WhenAllSubsetItemsArePresent()
    {
        var values = CreateAsyncSequence(3, 1, 2);

        var assertions = values.Should();
        var continuation = await assertions.BeSupersetOfAsync([1, 3]);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task BeSupersetOfAsync_Passes_WhenExpectedSubsetIsEmpty()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeSupersetOfAsync(Array.Empty<int>()));

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeSupersetOfAsync_DoesNotRequireDuplicateCounts()
    {
        var values = CreateAsyncSequence(1);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeSupersetOfAsync([1, 1]));

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeSupersetOfAsync_Passes_WhenComparerTreatsItemsAsEqual()
    {
        var values = CreateAsyncSequence("Alpha", "beta", "gamma");

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeSupersetOfAsync(["alpha", "GAMMA"], StringComparer.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeSupersetOfAsync_Throws_WhenSubsetItemIsMissing()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeSupersetOfAsync([1, 4]));

        Assert.Equal(
            "Expected values to be a superset of [1, 4], but found missing expected item at index 1: 4.",
            ex.Message);
    }

    [Fact]
    public async Task BeSupersetOfAsync_Throws_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeSupersetOfAsync([1]));

        Assert.Equal(
            "Expected values to be a superset of [1], but found missing expected item at index 0: 1.",
            ex.Message);
    }

    [Fact]
    public async Task BeSupersetOfAsync_ThrowsArgumentNullException_WhenExpectedSubsetIsNull()
    {
        var values = CreateAsyncSequence(1, 2, 3);
        IEnumerable<int>? expectedSubset = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().BeSupersetOfAsync(expectedSubset!));

        Assert.Equal("expectedSubset", ex.ParamName);
    }

    [Fact]
    public async Task BeSupersetOfAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("Alpha");
        StringComparer? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().BeSupersetOfAsync(["alpha"], comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task BeSupersetOfAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeSupersetOfAsync([1]));

        Assert.Equal("Expected values to be a superset of [1], but found <null>.", ex.Message);
    }

    [Fact]
    public async Task BeSupersetOfAsync_StopsAfterAllExpectedItemsAreFound()
    {
        var tracking = new TrackingAsyncEnumerable<int>([3, 1, 2, 4]);
        IAsyncEnumerable<int> values = tracking;

        await values.Should().BeSupersetOfAsync([1, 3]);

        Assert.Equal(2, tracking.YieldCount);
        Assert.Equal(2, tracking.MoveNextCallCount);
    }

    [Fact]
    public async Task BeSupersetOfAsync_StopsAfterAllExpectedItemsAreFound_WhenUsingComparer()
    {
        var tracking = new TrackingAsyncEnumerable<string>(["Gamma", "alpha", "beta", "delta"]);
        IAsyncEnumerable<string> values = tracking;

        await values.Should().BeSupersetOfAsync(["gamma", "ALPHA"], StringComparer.OrdinalIgnoreCase);

        Assert.Equal(2, tracking.YieldCount);
        Assert.Equal(2, tracking.MoveNextCallCount);
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
