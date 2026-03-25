namespace Axiom.Tests.Assertions.AsyncStreams.HaveCountAsync;

public sealed class HaveCountAsyncTests
{
    [Fact]
    public async Task HaveCountAsync_Passes_WhenCountsMatch()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var assertions = values.Should();
        var continuation = await assertions.HaveCountAsync(3);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task HaveCountAsync_Throws_WhenCountsDoNotMatch()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveCountAsync(2));

        Assert.Equal("Expected values to have count 2, but found 3.", ex.Message);
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
