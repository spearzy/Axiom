namespace Axiom.Tests.Assertions.AsyncStreams.Batch;

public sealed class AsyncEnumerableBatchRoutingTests
{
    [Fact]
    public async Task BeEmptyAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().BeEmptyAsync());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public async Task Batch_Dispose_ThrowsCombinedFailures_ForAsyncStreams()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            using var batch = new Axiom.Core.Batch("async streams");
            await values.Should().BeEmptyAsync();
        });

        Assert.Equal(
            "Batch 'async streams' failed with 1 assertion failure(s):\n1) Expected values to be empty, but found 1."
                .Replace("\n", Environment.NewLine, StringComparison.Ordinal),
            ex.Message);
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_InsideBatch_DoesNotThrowAtAssertionCallSite_WhenStreamEndsEarly()
    {
        var values = CreateAsyncSequence(1, 2);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().SatisfyRespectivelyAsync(
                item => item.Should().BeGreaterThan(0),
                item => item.Should().BeGreaterThan(0),
                item => item.Should().BeGreaterThan(0)));

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected values to satisfy assertions respectively (same order and count)", disposeEx.Message);
        Assert.Contains("async stream had fewer items than assertions (expected 3, found 2)", disposeEx.Message);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_OutsideBatch_ThrowsImmediately()
    {
        var values = CreateAsyncSequence(1, 2, 2, 3);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsAsync());
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var values = CreateAsyncSequence(1, 2, 2, 3);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().HaveUniqueItemsAsync());

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected values to have unique items", disposeEx.Message);
        Assert.Contains("first duplicate item at index 2: 2", disposeEx.Message);
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
