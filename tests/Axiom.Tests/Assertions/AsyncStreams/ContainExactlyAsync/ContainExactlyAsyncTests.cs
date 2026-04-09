using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.ContainExactlyAsync;

public sealed class ContainExactlyAsyncTests
{
    [Fact]
    public async Task ContainExactlyAsync_Passes_WhenSequenceMatchesExactly()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var assertions = values.Should();
        var continuation = await assertions.ContainExactlyAsync([1, 2, 3]);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainExactlyAsync_Throws_WhenItemMismatchesByIndex()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyAsync([1, 9, 3]));

        Assert.Equal(
            "Expected values to contain exactly [1, 9, 3], but found item mismatch at index 1: expected 9 but found 2.",
            ex.Message);
    }

    [Fact]
    public async Task ContainExactlyAsync_Passes_WhenComparerMatchesByLocalEquality()
    {
        var values = CreateAsyncSequence("Alpha", "beta", "Gamma");

        var assertions = values.Should();
        var continuation = await assertions.ContainExactlyAsync(
            ["alpha", "BETA", "gamma"],
            StringComparer.OrdinalIgnoreCase);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainExactlyAsync_Throws_WhenStreamEndsTooEarly()
    {
        var values = CreateAsyncSequence(1, 2);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyAsync([1, 2, 3]));

        Assert.Equal(
            "Expected values to contain exactly [1, 2, 3], but found stream ended before expected item at index 2: 3.",
            ex.Message);
    }

    [Fact]
    public async Task ContainExactlyAsync_Throws_WhenStreamHasExtraItems()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyAsync([1, 2]));

        Assert.Equal(
            "Expected values to contain exactly [1, 2], but found extra item at index 2: 3.",
            ex.Message);
    }

    [Fact]
    public async Task ContainExactlyAsync_Passes_WhenBothStreamAndExpectedSequenceAreEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainExactlyAsync(Array.Empty<int>()));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainExactlyAsync_Throws_WhenEmptyStreamIsComparedWithNonEmptyExpectedSequence()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyAsync([1]));

        Assert.Equal(
            "Expected values to contain exactly [1], but found stream ended before expected item at index 0: 1.",
            ex.Message);
    }

    [Fact]
    public async Task ContainExactlyAsync_Throws_WhenNonEmptyStreamIsComparedWithEmptyExpectedSequence()
    {
        var values = CreateAsyncSequence(1);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyAsync(Array.Empty<int>()));

        Assert.Equal(
            "Expected values to contain exactly [], but found extra item at index 0: 1.",
            ex.Message);
    }

    [Fact]
    public async Task ContainExactlyAsync_ThrowsArgumentNullException_WhenExpectedSequenceIsNull()
    {
        var values = CreateAsyncSequence(1, 2, 3);
        int[]? expectedSequence = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainExactlyAsync(expectedSequence!));

        Assert.Equal("expectedSequence", ex.ParamName);
    }

    [Fact]
    public async Task ContainExactlyAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("Alpha", "beta");
        IEqualityComparer<string>? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainExactlyAsync(["alpha", "beta"], comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task ContainExactlyAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyAsync([1]));

        Assert.Equal("Expected values to contain exactly [1], but found <null>.", ex.Message);
    }

    [Fact]
    public async Task ContainExactlyAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyAsync([1, 9, 3], "normalised IDs must match exactly"));

        Assert.Contains("because normalised IDs must match exactly", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ContainExactlyAsync_StopsAfterFirstMismatch()
    {
        var tracking = new TrackingAsyncEnumerable<int>([1, 2, 3, 4]);
        IAsyncEnumerable<int> values = tracking;

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyAsync([1, 9, 3, 4]));

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
