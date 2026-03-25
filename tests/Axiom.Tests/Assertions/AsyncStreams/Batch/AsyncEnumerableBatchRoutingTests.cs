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

    private static async IAsyncEnumerable<T> CreateAsyncSequence<T>(params T[] items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }
}
