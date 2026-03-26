using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.HaveUniqueItemsByAsync;

public sealed class HaveUniqueItemsByAsyncTests : IDisposable
{
    private sealed record User(int Id, string? Email);

    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_Passes_WhenSelectedKeysAreUnique()
    {
        var values = CreateAsyncSequence(
            new User(1, "alpha@example.com"),
            new User(2, "beta@example.com"),
            new User(3, "gamma@example.com"));

        var assertions = values.Should();
        var continuation = await assertions.HaveUniqueItemsByAsync(user => user.Id);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_Throws_WhenDuplicateSelectedKeyExists()
    {
        var values = CreateAsyncSequence(
            new User(1, "alpha@example.com"),
            new User(2, "beta@example.com"),
            new User(1, "gamma@example.com"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsByAsync(user => user.Id));

        Assert.Equal(
            "Expected values to have unique items by selected key, but found first duplicate selected key at index 2: 1.",
            ex.Message);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_Throws_WhenDuplicateSelectedNullKeyExists()
    {
        var values = CreateAsyncSequence(
            new User(1, null),
            new User(2, "beta@example.com"),
            new User(3, null));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsByAsync(user => user.Email));

        Assert.Equal(
            "Expected values to have unique items by selected key, but found first duplicate selected key at index 2: <null>.",
            ex.Message);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(
            new User(1, "alpha@example.com"),
            new User(2, "beta@example.com"),
            new User(1, "gamma@example.com"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsByAsync(user => user.Id, because: "user ids must be unique"));

        Assert.Contains("because user ids must be unique", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_WithComparer_UsesSuppliedComparer()
    {
        var values = CreateAsyncSequence(
            new User(1, "ALPHA@example.com"),
            new User(2, "alpha@example.com"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsByAsync(
                user => user.Email,
                StringComparer.OrdinalIgnoreCase));

        Assert.Contains("first duplicate selected key at index 1", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_WithComparer_DoesNotThrow_WhenComparerTreatsKeysAsDistinct()
    {
        var values = CreateAsyncSequence(
            new User(1, "ALPHA@example.com"),
            new User(2, "alpha@example.com"));

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().HaveUniqueItemsByAsync(
                user => user.Email,
                StringComparer.Ordinal));

        Assert.Null(ex);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_UsesConfiguredComparerProviderForTKey()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());
        var values = CreateAsyncSequence(
            new User(1, "alpha@example.com"),
            new User(3, "beta@example.com"),
            new User(2, "gamma@example.com"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsByAsync(user => user.Id));

        Assert.Equal(
            "Expected values to have unique items by selected key, but found first duplicate selected key at index 1: 3.",
            ex.Message);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_ThrowsArgumentNullException_WhenKeySelectorIsNull()
    {
        var values = CreateAsyncSequence(new User(1, "alpha@example.com"));
        Func<User, int>? keySelector = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().HaveUniqueItemsByAsync(keySelector!));

        Assert.Equal("keySelector", ex.ParamName);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence(new User(1, "alpha@example.com"));
        IEqualityComparer<int>? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().HaveUniqueItemsByAsync(user => user.Id, comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_DoesNotThrow_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<User>();

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().HaveUniqueItemsByAsync(user => user.Id));

        Assert.Null(ex);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<User>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsByAsync(user => user.Id));

        Assert.Equal("Expected values to have unique items by selected key, but found <null>.", ex.Message);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_StopsAfterFirstDuplicateSelectedKey()
    {
        var tracking = new TrackingAsyncEnumerable<User>(
        [
            new User(1, "alpha@example.com"),
            new User(2, "beta@example.com"),
            new User(1, "gamma@example.com"),
            new User(4, "delta@example.com")
        ]);
        IAsyncEnumerable<User> values = tracking;

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsByAsync(user => user.Id));

        Assert.Equal(3, tracking.YieldCount);
        Assert.Equal(3, tracking.MoveNextCallCount);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_OutsideBatch_ThrowsImmediately()
    {
        var values = CreateAsyncSequence(
            new User(1, "alpha@example.com"),
            new User(2, "beta@example.com"),
            new User(1, "gamma@example.com"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsByAsync(user => user.Id));
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_ChainsWithAnd()
    {
        var values = CreateAsyncSequence(
            new User(1, "alpha@example.com"),
            new User(2, "beta@example.com"),
            new User(3, "gamma@example.com"));

        var continuation = await (await values.Should().HaveUniqueItemsByAsync(user => user.Id))
            .And
            .NotBeEmptyAsync();

        Assert.NotNull(continuation.And);
    }

    [Fact]
    public async Task HaveUniqueItemsByAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var values = CreateAsyncSequence(
            new User(1, "alpha@example.com"),
            new User(2, "beta@example.com"),
            new User(1, "gamma@example.com"));

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().HaveUniqueItemsByAsync(user => user.Id));

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected values to have unique items by selected key", disposeEx.Message);
        Assert.Contains("first duplicate selected key at index 2: 1", disposeEx.Message);
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
