using System.Collections;
using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.HaveUniqueItems;

public sealed class HaveUniqueItemsComparerProviderTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void HaveUniqueItems_UsesConfiguredComparerProvider_ForArray()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());
        int[] values = [1, 3, 2];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().HaveUniqueItems());

        Assert.Equal("Expected values to have unique items, but found first duplicate item at index 1: 3.", ex.Message);
    }

    [Fact]
    public void HaveUniqueItems_UsesConfiguredComparerProvider_ForList()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());
        List<int> values = [1, 3, 2];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().HaveUniqueItems());

        Assert.Equal("Expected values to have unique items, but found first duplicate item at index 1: 3.", ex.Message);
    }

    [Fact]
    public void HaveUniqueItems_UsesConfiguredComparerProvider_ForIEnumerableInterface()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());
        IEnumerable<int> values = [1, 3, 2];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().HaveUniqueItems());

        Assert.Equal("Expected values to have unique items, but found first duplicate item at index 1: 3.", ex.Message);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_MatchesSyncGenericBehavior_WhenComparerProviderIsConfigured()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());
        int[] syncValues = [1, 3, 2];
        var asyncValues = CreateAsyncSequence(1, 3, 2);

        var syncEx = Assert.Throws<InvalidOperationException>(() => syncValues.Should().HaveUniqueItems());
        var asyncEx = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await asyncValues.Should().HaveUniqueItemsAsync());

        Assert.Contains("to have unique items", syncEx.Message, StringComparison.Ordinal);
        Assert.Contains("to have unique items", asyncEx.Message, StringComparison.Ordinal);
        Assert.Contains("first duplicate item at index 1: 3", syncEx.Message, StringComparison.Ordinal);
        Assert.Contains("first duplicate item at index 1: 3", asyncEx.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveUniqueItems_NonGenericCollection_PreservesExistingBehavior()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());
        var values = new ArrayList { 1, 3, 2 };

        var ex = Record.Exception(() => values.Should().HaveUniqueItems());

        Assert.Null(ex);
    }

    [Fact]
    public void HaveUniqueItems_UsesExplicitComparerInsteadOfConfiguredComparerProvider()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());
        int[] values = [1, 3, 2];

        var ex = Record.Exception(() => values.Should().HaveUniqueItems(EqualityComparer<int>.Default));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveUniqueItems_FallsBackToDefaultComparer_WhenProviderDoesNotSupplyOne()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new EmptyComparerProvider());
        int[] values = [1, 3, 2];

        var ex = Record.Exception(() => values.Should().HaveUniqueItems());

        Assert.Null(ex);
    }

    private static async IAsyncEnumerable<int> CreateAsyncSequence(params int[] items)
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

    private sealed class EmptyComparerProvider : IComparerProvider
    {
        public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
        {
            comparer = null;
            return false;
        }
    }
}
