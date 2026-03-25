namespace Axiom.Tests.Assertions.AsyncStreams.NotBeEmptyAsync;

public sealed class NotBeEmptyAsyncTests
{
    [Fact]
    public async Task NotBeEmptyAsync_Passes_WhenStreamHasItems()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var assertions = values.Should();
        var continuation = await assertions.NotBeEmptyAsync();

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task NotBeEmptyAsync_Throws_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotBeEmptyAsync());

        Assert.Equal("Expected values to not be empty, but found 0.", ex.Message);
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
