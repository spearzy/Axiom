using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.ContainAnyAsync;

public sealed class ContainAnyAsyncTests
{
    [Fact]
    public async Task ContainAnyAsync_Passes_WhenStreamContainsOneExpectedItem()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var assertions = values.Should();
        var continuation = await assertions.ContainAnyAsync([9, 2, 10]);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainAnyAsync_Passes_WhenStreamContainsMultipleExpectedItems()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainAnyAsync([2, 3, 10]));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainAnyAsync_Throws_WhenStreamContainsNoneOfTheExpectedItems()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainAnyAsync([9, 10]));

        Assert.Equal(
            "Expected values to contain any of [9, 10], but found none of the expected items were found.",
            ex.Message);
    }

    [Fact]
    public async Task ContainAnyAsync_Passes_WhenComparerMatchesOneExpectedItem()
    {
        var values = CreateAsyncSequence("alpha", "beta", "gamma");

        var assertions = values.Should();
        var continuation = await assertions.ContainAnyAsync(
            ["DELTA", "BETA"],
            StringComparer.OrdinalIgnoreCase);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainAnyAsync_Throws_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainAnyAsync([1, 2]));

        Assert.Equal(
            "Expected values to contain any of [1, 2], but found none of the expected items were found.",
            ex.Message);
    }

    [Fact]
    public async Task ContainAnyAsync_DuplicateItems_DoNotBreakSemantics()
    {
        var values = CreateAsyncSequence(1, 1, 1);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainAnyAsync([1, 1]));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainAnyAsync_Throws_WhenExpectedItemsAreEmpty()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainAnyAsync(Array.Empty<int>()));

        Assert.Equal(
            "Expected values to contain any of [], but found no expected items were provided.",
            ex.Message);
    }

    [Fact]
    public async Task ContainAnyAsync_ThrowsArgumentNullException_WhenExpectedItemsAreNull()
    {
        var values = CreateAsyncSequence(1, 2, 3);
        IEnumerable<int>? expectedItems = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainAnyAsync(expectedItems!));

        Assert.Equal("expectedItems", ex.ParamName);
    }

    [Fact]
    public async Task ContainAnyAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("alpha", "beta");
        IEqualityComparer<string>? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainAnyAsync(["BETA"], comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task ContainAnyAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainAnyAsync([1]));

        Assert.Equal("Expected values to contain any of [1], but found <null>.", ex.Message);
    }

    [Fact]
    public async Task ContainAnyAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainAnyAsync([9, 10], "at least one approved ID must be present"));

        Assert.Contains("because at least one approved ID must be present", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ContainAnyAsync_StopsAfterFirstMatch()
    {
        var tracking = new TrackingAsyncEnumerable<int>([1, 2, 3, 4]);
        IAsyncEnumerable<int> values = tracking;

        await values.Should().ContainAnyAsync([9, 2, 10]);

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
