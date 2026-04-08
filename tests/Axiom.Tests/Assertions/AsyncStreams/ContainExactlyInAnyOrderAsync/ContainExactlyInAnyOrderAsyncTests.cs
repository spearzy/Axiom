using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.ContainExactlyInAnyOrderAsync;

public sealed class ContainExactlyInAnyOrderAsyncTests
{
    [Fact]
    public async Task ContainExactlyInAnyOrderAsync_Passes_WhenItemsMatchWithDifferentOrder()
    {
        var values = CreateAsyncSequence(3, 1, 2);

        var assertions = values.Should();
        var continuation = await assertions.ContainExactlyInAnyOrderAsync([1, 2, 3]);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainExactlyInAnyOrderAsync_Passes_WhenDuplicateCountsMatch()
    {
        var values = CreateAsyncSequence(1, 2, 2);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainExactlyInAnyOrderAsync([2, 1, 2]));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainExactlyInAnyOrderAsync_Passes_WhenComparerTreatsItemsAsEqual()
    {
        var values = CreateAsyncSequence("Alpha", "beta", "BETA");

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainExactlyInAnyOrderAsync(
                ["BETA", "alpha", "beta"],
                StringComparer.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainExactlyInAnyOrderAsync_Throws_WhenExpectedItemIsMissing()
    {
        var values = CreateAsyncSequence(1, 2, 2);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyInAnyOrderAsync([1, 2, 3]));

        Assert.Equal(
            "Expected values to contain exactly in any order [1, 2, 3], but found missing item 3: expected count 1 but found 0.",
            ex.Message);
    }

    [Fact]
    public async Task ContainExactlyInAnyOrderAsync_Throws_WhenUnexpectedItemIsPresent()
    {
        var values = CreateAsyncSequence(1, 2, 3, 4);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyInAnyOrderAsync([1, 2, 3]));

        Assert.Equal(
            "Expected values to contain exactly in any order [1, 2, 3], but found unexpected item 4: found count 1 but expected 0.",
            ex.Message);
    }

    [Fact]
    public async Task ContainExactlyInAnyOrderAsync_Passes_WhenBothStreamAndExpectedSequenceAreEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainExactlyInAnyOrderAsync(Array.Empty<int>()));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainExactlyInAnyOrderAsync_Throws_WhenExpectedSequenceIsEmptyButStreamIsNot()
    {
        var values = CreateAsyncSequence(1);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyInAnyOrderAsync(Array.Empty<int>()));

        Assert.Equal(
            "Expected values to contain exactly in any order [], but found unexpected item 1: found count 1 but expected 0.",
            ex.Message);
    }

    [Fact]
    public async Task ContainExactlyInAnyOrderAsync_ThrowsArgumentNullException_WhenExpectedSequenceIsNull()
    {
        var values = CreateAsyncSequence(1, 2, 3);
        int[]? expectedSequence = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainExactlyInAnyOrderAsync(expectedSequence!));

        Assert.Equal("expectedSequence", ex.ParamName);
    }

    [Fact]
    public async Task ContainExactlyInAnyOrderAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("Alpha");
        StringComparer? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainExactlyInAnyOrderAsync(["alpha"], comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task ContainExactlyInAnyOrderAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyInAnyOrderAsync([1]));

        Assert.Equal("Expected values to contain exactly in any order [1], but found <null>.", ex.Message);
    }

    [Fact]
    public async Task ContainExactlyInAnyOrderAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(1, 2, 2);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyInAnyOrderAsync([1, 2, 3], "IDs must match exactly regardless of order"));

        Assert.Contains("because IDs must match exactly regardless of order", ex.Message, StringComparison.Ordinal);
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
