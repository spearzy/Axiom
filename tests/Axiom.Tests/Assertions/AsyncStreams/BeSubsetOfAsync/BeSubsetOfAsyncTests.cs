namespace Axiom.Tests.Assertions.AsyncStreams.BeSubsetOfAsync;

public sealed class BeSubsetOfAsyncTests
{
    [Fact]
    public async Task BeSubsetOfAsync_Passes_WhenAllStreamItemsAreInSuperset()
    {
        var values = CreateAsyncSequence(1, 2, 2);

        var assertions = values.Should();
        var continuation = await assertions.BeSubsetOfAsync([1, 2, 3]);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task BeSubsetOfAsync_Passes_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeSubsetOfAsync([1, 2, 3]));

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeSubsetOfAsync_Passes_WhenComparerTreatsItemsAsEqual()
    {
        var values = CreateAsyncSequence("Alpha", "beta");

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeSubsetOfAsync(["alpha", "BETA", "gamma"], StringComparer.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeSubsetOfAsync_Throws_WhenItemIsNotInSuperset()
    {
        var values = CreateAsyncSequence(1, 4, 2);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeSubsetOfAsync([1, 2, 3]));

        Assert.Equal(
            "Expected values to be a subset of [1, 2, 3], but found missing item at index 1: 4.",
            ex.Message);
    }

    [Fact]
    public async Task BeSubsetOfAsync_Throws_WhenSupersetIsEmpty()
    {
        var values = CreateAsyncSequence(1);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeSubsetOfAsync(Array.Empty<int>()));

        Assert.Equal(
            "Expected values to be a subset of [], but found missing item at index 0: 1.",
            ex.Message);
    }

    [Fact]
    public async Task BeSubsetOfAsync_ThrowsArgumentNullException_WhenExpectedSupersetIsNull()
    {
        var values = CreateAsyncSequence(1, 2, 3);
        IEnumerable<int>? expectedSuperset = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().BeSubsetOfAsync(expectedSuperset!));

        Assert.Equal("expectedSuperset", ex.ParamName);
    }

    [Fact]
    public async Task BeSubsetOfAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("Alpha");
        StringComparer? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().BeSubsetOfAsync(["alpha"], comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task BeSubsetOfAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeSubsetOfAsync([1]));

        Assert.Equal("Expected values to be a subset of [1], but found <null>.", ex.Message);
    }

    [Fact]
    public async Task BeSubsetOfAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(1, 4, 2);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeSubsetOfAsync([1, 2, 3], "stream values must stay inside the allowed range"));

        Assert.Contains("because stream values must stay inside the allowed range", ex.Message, StringComparison.Ordinal);
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
