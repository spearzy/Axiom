using System.Collections;
using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotBeEquivalentTo;

public sealed class NotBeEquivalentToCollectionItemComparerTests
{
    [Fact]
    public void GivenCollectionItemComparer_WhenItemsMatchByConfiguredRule_ThenNotBeEquivalentToThrows()
    {
        var actual = new Order
        {
            Items =
            [
                new() { Sku = "A-1", Quantity = 1 },
                new() { Sku = "B-2", Quantity = 2 },
            ]
        };
        var expected = new Order
        {
            Items =
            [
                new() { Sku = "A-1", Quantity = 100 },
                new() { Sku = "B-2", Quantity = 200 },
            ]
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options => options.UseCollectionItemComparerForPath("actual.Items", new LineItemSkuComparer())));

        Assert.Contains("to not be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenCollectionItemComparer_WhenItemsDoNotMatchByConfiguredRule_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = new Order
        {
            Items =
            [
                new() { Sku = "A-1", Quantity = 1 },
                new() { Sku = "B-2", Quantity = 2 },
            ]
        };
        var expected = new Order
        {
            Items =
            [
                new() { Sku = "A-1", Quantity = 1 },
                new() { Sku = "Z-9", Quantity = 2 },
            ]
        };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options => options.UseCollectionItemComparerForPath("actual.Items", new LineItemSkuComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenSharedCollectionReference_WhenCollectionItemComparerReturnsFalse_ThenNotBeEquivalentToDoesNotThrow()
    {
        LineItem[] sharedItems =
        [
            new() { Sku = "A-1", Quantity = 1 },
            new() { Sku = "B-2", Quantity = 2 },
        ];

        var actual = new Order { Items = sharedItems };
        var expected = new Order { Items = sharedItems };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options => options.UseCollectionItemComparerForPath("actual.Items", new AlwaysFalseItemComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenSameRootReference_WhenCollectionItemComparerReturnsFalse_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = new Order
        {
            Items =
            [
                new() { Sku = "A-1", Quantity = 1 },
                new() { Sku = "B-2", Quantity = 2 },
            ]
        };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(
                actual,
                options => options.UseCollectionItemComparerForPath("actual.Items", new AlwaysFalseItemComparer())));

        Assert.Null(ex);
    }

    private sealed class Order
    {
        public LineItem[] Items { get; init; } = [];
    }

    private sealed class LineItem
    {
        public string? Sku { get; init; }
        public int Quantity { get; init; }
    }

    private sealed class LineItemSkuComparer : IEqualityComparer
    {
        public new bool Equals(object? x, object? y)
        {
            if (x is null || y is null)
            {
                return x is null && y is null;
            }

            if (x is not LineItem left || y is not LineItem right)
            {
                return object.Equals(x, y);
            }

            return string.Equals(left.Sku, right.Sku, StringComparison.Ordinal);
        }

        public int GetHashCode(object obj)
        {
            if (obj is LineItem item)
            {
                return StringComparer.Ordinal.GetHashCode(item.Sku ?? string.Empty);
            }

            return obj.GetHashCode();
        }
    }

    private sealed class AlwaysFalseItemComparer : IEqualityComparer
    {
        public new bool Equals(object? x, object? y)
        {
            return false;
        }

        public int GetHashCode(object obj)
        {
            return 0;
        }
    }
}
