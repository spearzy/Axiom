using Axiom.Assertions.AssertionTypes;

namespace Axiom.Tests.Assertions.AsyncStreams.ContainSingleAsync;

public sealed class ContainSingleAsyncTests
{
    [Fact]
    public async Task ContainSingleAsync_Passes_WhenStreamHasOneItem()
    {
        var values = CreateAsyncSequence(42);

        var assertions = values.Should();
        var continuation = await assertions.ContainSingleAsync();

        Assert.Same(assertions, continuation.And);
        Assert.Equal(42, continuation.SingleItem);
    }

    [Fact]
    public async Task ContainSingleAsync_Throws_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainSingleAsync());

        Assert.Equal("Expected values to contain a single item, but found 0.", ex.Message);
    }

    [Fact]
    public async Task ContainSingleAsync_Throws_WhenStreamHasMultipleItems()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainSingleAsync());

        Assert.Equal("Expected values to contain a single item, but found at least 2 items.", ex.Message);
    }

    [Fact]
    public async Task ContainSingleAsync_Predicate_Passes_WhenExactlyOneItemMatches()
    {
        var values = CreateAsyncSequence(1, 3, 4, 5);

        var continuation = await values.Should().ContainSingleAsync(x => x % 2 == 0);

        Assert.Equal(4, continuation.SingleItem);
    }

    [Fact]
    public async Task ContainSingleAsync_Predicate_Throws_WhenNoItemsMatch()
    {
        var values = CreateAsyncSequence(1, 3, 5);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainSingleAsync(x => x % 2 == 0));

        Assert.Equal("Expected values to contain a single item matching predicate `x => x % 2 == 0`, but found 0.", ex.Message);
    }

    [Fact]
    public async Task ContainSingleAsync_Predicate_Throws_WhenMultipleItemsMatch()
    {
        var values = CreateAsyncSequence(1, 2, 4, 5);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainSingleAsync(x => x % 2 == 0));

        Assert.Equal(
            "Expected values to contain a single item matching predicate `x => x % 2 == 0`, but found at least 2 matching items (second match at index 2).",
            ex.Message);
    }

    [Fact]
    public async Task ContainSingleAsync_ExposesTypedSingleItem_ForReferenceTypes()
    {
        var values = CreateAsyncSequence(new Order(42, 19.99m));

        var continuation = await values.Should().ContainSingleAsync();
        var total = continuation.SingleItem.Total;

        Assert.Equal(19.99m, total);
    }

    [Fact]
    public async Task ShouldAsyncEnumerable_ReturnsAsyncStreamAssertions_ForConcreteWrapperType()
    {
        var values = new WrappedOrderStream([new Order(42, 19.99m)]);

        var assertions = values.ShouldAsyncEnumerable();
        var continuation = await assertions.ContainSingleAsync();

        Assert.IsType<AsyncEnumerableAssertions<Order>>(assertions);
        Assert.Equal(19.99m, continuation.SingleItem.Total);
    }

    [Fact]
    public async Task ShouldAsyncEnumerable_AllowsConcreteWrapperType_ToUsePredicateAssertions()
    {
        WrappedUserStream users = new([new User(7), new User(42), new User(100)]);

        var continuation = await users.ShouldAsyncEnumerable().ContainSingleAsync(user => user.Id == 42);

        Assert.Equal(42, continuation.SingleItem.Id);
    }

    [Fact]
    public async Task SingleItem_ThrowsExplicitMessage_WhenContainSingleAsyncFailedInsideBatch()
    {
        var values = CreateAsyncSequence(1, 2);

        using var batch = new Axiom.Core.Batch();
        var continuation = await values.Should().ContainSingleAsync();

        var ex = Assert.Throws<InvalidOperationException>(() => _ = continuation.SingleItem);

        Assert.Equal(
            "SingleItem is unavailable because ContainSingleAsync failed with error: Expected values to contain a single item, but found at least 2 items.",
            ex.Message);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public async Task ContainSingleAsync_Predicate_ThrowsForNullPredicate()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainSingleAsync((Func<int, bool>)null!));
    }

    private static async IAsyncEnumerable<T> CreateAsyncSequence<T>(params T[] items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }

    private sealed record Order(int Id, decimal Total);
    private sealed record User(int Id);

    private sealed class WrappedOrderStream(IReadOnlyList<Order> items) : IAsyncEnumerable<Order>
    {
        public async IAsyncEnumerator<Order> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            foreach (var item in items)
            {
                await Task.Yield();
                yield return item;
            }
        }
    }

    private sealed class WrappedUserStream(IReadOnlyList<User> items) : IAsyncEnumerable<User>
    {
        public async IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            foreach (var item in items)
            {
                await Task.Yield();
                yield return item;
            }
        }
    }
}
