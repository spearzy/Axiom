using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.HaveUniqueItemsAsync;

public sealed class HaveUniqueItemsAsyncTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_Passes_WhenAllItemsAreUnique()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var assertions = values.Should();
        var continuation = await assertions.HaveUniqueItemsAsync();

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_Throws_WhenDuplicateItemExists()
    {
        var values = CreateAsyncSequence(1, 2, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsAsync());

        Assert.Equal("Expected values to have unique items, but found first duplicate item at index 2: 2.", ex.Message);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_Throws_WhenDuplicateNullExists()
    {
        var values = CreateAsyncSequence<string?>(null, "x", null);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsAsync());

        Assert.Equal("Expected values to have unique items, but found first duplicate item at index 2: <null>.", ex.Message);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_Throws_WhenComparerTreatsItemsAsDuplicates()
    {
        var values = CreateAsyncSequence("alpha", "BETA", "beta");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsAsync(StringComparer.OrdinalIgnoreCase));

        Assert.Equal(
            "Expected values to have unique items, but found first duplicate item at index 2: \"beta\".",
            ex.Message);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(1, 2, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsAsync("IDs must be unique across the stream"));

        Assert.Contains("because IDs must be unique across the stream", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_DoesNotThrow_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().HaveUniqueItemsAsync());

        Assert.Null(ex);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsAsync());

        Assert.Equal("Expected values to have unique items, but found <null>.", ex.Message);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("alpha", "beta");
        IEqualityComparer<string>? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().HaveUniqueItemsAsync(comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_UsesConfiguredComparerProviderForT()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());
        var values = CreateAsyncSequence(1, 3, 2);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsAsync());

        Assert.Equal("Expected values to have unique items, but found first duplicate item at index 1: 3.", ex.Message);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_StopsAfterFirstDuplicate()
    {
        var tracking = new TrackingAsyncEnumerable<int>([1, 2, 2, 3]);
        IAsyncEnumerable<int> values = tracking;

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsAsync());

        Assert.Equal(3, tracking.YieldCount);
        Assert.Equal(3, tracking.MoveNextCallCount);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_OutsideBatch_ThrowsImmediately()
    {
        var values = CreateAsyncSequence(1, 2, 2, 3);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsAsync());
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_ChainsWithAnd()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var continuation = await (await values.Should().HaveUniqueItemsAsync())
            .And
            .NotBeEmptyAsync();

        Assert.NotNull(continuation.And);
    }

    private static async IAsyncEnumerable<T> CreateAsyncSequence<T>(params T[] items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }

    private sealed class OddEvenMatchIntComparerProvider : IComparerProvider
    {
        public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
        {
            if (typeof(T) == typeof(int))
            {
                comparer = (IEqualityComparer<T>)(object)new OddEvenMatchIntComparer();
                return true;
            }

            comparer = null;
            return false;
        }
    }

    private sealed class OddEvenMatchIntComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x % 2 == y % 2;
        }

        public int GetHashCode(int obj)
        {
            return obj % 2;
        }
    }
}
