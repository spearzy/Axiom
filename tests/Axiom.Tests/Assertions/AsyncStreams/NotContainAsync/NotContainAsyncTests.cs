using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.NotContainAsync;

public sealed class NotContainAsyncTests
{
    [Fact]
    public async Task NotContainAsync_Passes_WhenUnexpectedItemIsAbsent()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var assertions = values.Should();
        var continuation = await assertions.NotContainAsync(9);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task NotContainAsync_Throws_WhenUnexpectedItemIsPresent()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAsync(2));

        Assert.Equal(
            "Expected values to not contain 2, but found first matching item at subject index 1: 2.",
            ex.Message);
    }

    [Fact]
    public async Task NotContainAsync_Throws_WhenComparerFindsAMatch()
    {
        var values = CreateAsyncSequence("Alpha", "beta", "gamma");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAsync("BETA", StringComparer.OrdinalIgnoreCase));

        Assert.Equal(
            "Expected values to not contain \"BETA\", but found first matching item at subject index 1: \"beta\".",
            ex.Message);
    }

    [Fact]
    public async Task NotContainAsync_ReportsFirstMatchingOccurrence_WhenDuplicatesExist()
    {
        var values = CreateAsyncSequence(1, 2, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAsync(2));

        Assert.Contains("first matching item at subject index 1: 2", ex.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("index 2", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task NotContainAsync_Passes_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().NotContainAsync(1));

        Assert.Null(ex);
    }

    [Fact]
    public async Task NotContainAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAsync(1));

        Assert.Equal("Expected values to not contain 1, but found <null>.", ex.Message);
    }

    [Fact]
    public async Task NotContainAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("Alpha");
        StringComparer? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().NotContainAsync("alpha", comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task NotContainAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAsync(2, "blocked IDs must never appear"));

        Assert.Contains("because blocked IDs must never appear", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task NotContainAsync_StopsAtFirstMatch()
    {
        var tracking = new TrackingAsyncEnumerable<int>([1, 2, 2, 3]);
        IAsyncEnumerable<int> values = tracking;

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAsync(2));

        Assert.Equal(2, tracking.YieldCount);
        Assert.Equal(2, tracking.MoveNextCallCount);
    }

    [Fact]
    public async Task NotContainAsync_StopsAtFirstMatch_WhenUsingComparer()
    {
        var tracking = new TrackingAsyncEnumerable<string>(["Alpha", "beta", "BETA", "gamma"]);
        IAsyncEnumerable<string> values = tracking;

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAsync("beta", StringComparer.OrdinalIgnoreCase));

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
