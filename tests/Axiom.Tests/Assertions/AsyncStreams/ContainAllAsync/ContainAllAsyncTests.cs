using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.ContainAllAsync;

public sealed class ContainAllAsyncTests
{
    [Fact]
    public async Task ContainAllAsync_Passes_WhenAllExpectedItemsArePresent()
    {
        var values = CreateAsyncSequence(3, 1, 2);

        var assertions = values.Should();
        var continuation = await assertions.ContainAllAsync([1, 3]);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainAllAsync_Passes_WhenExpectedItemsAreEmpty()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainAllAsync(Array.Empty<int>()));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainAllAsync_DoesNotRequireDuplicateCounts()
    {
        var values = CreateAsyncSequence(1);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainAllAsync([1, 1]));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainAllAsync_Passes_WhenComparerTreatsItemsAsEqual()
    {
        var values = CreateAsyncSequence("Alpha", "beta", "gamma");

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainAllAsync(["alpha", "GAMMA"], StringComparer.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainAllAsync_Throws_WhenExpectedItemIsMissing()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainAllAsync([1, 4]));

        Assert.Equal(
            "Expected values to contain all [1, 4], but found missing expected item at index 1: 4.",
            ex.Message);
    }

    [Fact]
    public async Task ContainAllAsync_Throws_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainAllAsync([1]));

        Assert.Equal(
            "Expected values to contain all [1], but found missing expected item at index 0: 1.",
            ex.Message);
    }

    [Fact]
    public async Task ContainAllAsync_ThrowsArgumentNullException_WhenExpectedItemsAreNull()
    {
        var values = CreateAsyncSequence(1, 2, 3);
        IEnumerable<int>? expectedItems = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainAllAsync(expectedItems!));

        Assert.Equal("expectedItems", ex.ParamName);
    }

    [Fact]
    public async Task ContainAllAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("Alpha");
        StringComparer? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainAllAsync(["alpha"], comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task ContainAllAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainAllAsync([1]));

        Assert.Equal("Expected values to contain all [1], but found <null>.", ex.Message);
    }

    [Fact]
    public async Task ContainAllAsync_StopsAfterAllExpectedItemsAreFound()
    {
        var tracking = new TrackingAsyncEnumerable<int>([1, 2, 3, 4]);
        IAsyncEnumerable<int> values = tracking;

        await values.Should().ContainAllAsync([1, 3]);

        Assert.Equal(3, tracking.YieldCount);
        Assert.Equal(3, tracking.MoveNextCallCount);
    }

    [Fact]
    public async Task ContainAllAsync_StopsAfterAllExpectedItemsAreFound_WhenUsingComparer()
    {
        var tracking = new TrackingAsyncEnumerable<string>(["Alpha", "beta", "gamma", "delta"]);
        IAsyncEnumerable<string> values = tracking;

        await values.Should().ContainAllAsync(["alpha", "GAMMA"], StringComparer.OrdinalIgnoreCase);

        Assert.Equal(3, tracking.YieldCount);
        Assert.Equal(3, tracking.MoveNextCallCount);
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
