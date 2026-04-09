using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.NotContainAnyAsync;

public sealed class NotContainAnyAsyncTests
{
    [Fact]
    public async Task NotContainAnyAsync_Passes_WhenStreamContainsNoneOfTheUnexpectedItems()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var assertions = values.Should();
        var continuation = await assertions.NotContainAnyAsync([8, 9, 10]);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task NotContainAnyAsync_Throws_WhenStreamContainsAnUnexpectedItem()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAnyAsync([9, 2, 10]));

        Assert.Equal(
            "Expected values to not contain any of [9, 2, 10], but found first matching item at subject index 1: 2.",
            ex.Message);
    }

    [Fact]
    public async Task NotContainAnyAsync_Throws_WhenComparerMatchesAnUnexpectedItem()
    {
        var values = CreateAsyncSequence("alpha", "beta", "gamma");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAnyAsync(["DELTA", "BETA"], StringComparer.OrdinalIgnoreCase));

        Assert.Equal(
            "Expected values to not contain any of [\"DELTA\", \"BETA\"], but found first matching item at subject index 1: \"beta\".",
            ex.Message);
    }

    [Fact]
    public async Task NotContainAnyAsync_Failure_IdentifiesTheUnexpectedItem()
    {
        var values = CreateAsyncSequence("alpha", "beta", "gamma");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAnyAsync(["delta", "beta"]));

        Assert.Contains("first matching item at subject index 1: \"beta\"", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task NotContainAnyAsync_Passes_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().NotContainAnyAsync([1, 2]));

        Assert.Null(ex);
    }

    [Fact]
    public async Task NotContainAnyAsync_DoesNotThrow_WhenUnexpectedItemsAreEmpty()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().NotContainAnyAsync(Array.Empty<int>()));

        Assert.Null(ex);
    }

    [Fact]
    public async Task NotContainAnyAsync_ThrowsArgumentNullException_WhenUnexpectedItemsAreNull()
    {
        var values = CreateAsyncSequence(1, 2, 3);
        IEnumerable<int>? unexpectedItems = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().NotContainAnyAsync(unexpectedItems!));

        Assert.Equal("unexpectedItems", ex.ParamName);
    }

    [Fact]
    public async Task NotContainAnyAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("alpha", "beta");
        IEqualityComparer<string>? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().NotContainAnyAsync(["BETA"], comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task NotContainAnyAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAnyAsync([1]));

        Assert.Equal("Expected values to not contain any of [1], but found <null>.", ex.Message);
    }

    [Fact]
    public async Task NotContainAnyAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAnyAsync([9, 2], "restricted IDs must never be present"));

        Assert.Contains("because restricted IDs must never be present", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task NotContainAnyAsync_StopsAtFirstUnexpectedMatch()
    {
        var tracking = new TrackingAsyncEnumerable<int>([1, 2, 3, 4]);
        IAsyncEnumerable<int> values = tracking;

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAnyAsync([8, 2, 9]));

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
