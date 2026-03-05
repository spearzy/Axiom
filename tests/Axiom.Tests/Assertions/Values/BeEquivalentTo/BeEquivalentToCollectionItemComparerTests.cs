using System.Collections;
using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToCollectionItemComparerTests
{
    [Fact]
    public void GivenCollectionItemComparerForAbsolutePath_WhenItemsMatchByConfiguredRule_ThenDoesNotThrow()
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

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.UseCollectionItemComparerForPath("actual.Items", new LineItemSkuComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenCollectionItemComparerForRelativePath_WhenItemsMatchByConfiguredRule_ThenDoesNotThrow()
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

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.UseCollectionItemComparerForPath(nameof(Order.Items), new LineItemSkuComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenAnyOrderAndCollectionItemComparer_WhenItemsAreReordered_ThenDoesNotThrow()
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
                new() { Sku = "B-2", Quantity = 200 },
                new() { Sku = "A-1", Quantity = 100 },
            ]
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.CollectionOrder = EquivalencyCollectionOrder.Any;
                    options.UseCollectionItemComparerForPath("actual.Items", new LineItemSkuComparer());
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenCollectionItemComparer_WhenItemsDoNotMatchByConfiguredRule_ThenThrows()
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

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.UseCollectionItemComparerForPath("actual.Items", new LineItemSkuComparer())));

        Assert.Contains("actual.Items[1]", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenSharedCollectionReference_WhenCollectionItemComparerReturnsFalse_ThenBeEquivalentToThrows()
    {
        LineItem[] sharedItems =
        [
            new() { Sku = "A-1", Quantity = 1 },
            new() { Sku = "B-2", Quantity = 2 },
        ];

        var actual = new Order { Items = sharedItems };
        var expected = new Order { Items = sharedItems };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.UseCollectionItemComparerForPath("actual.Items", new AlwaysFalseItemComparer())));

        Assert.Contains("actual.Items[0]", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenSameRootReference_WhenCollectionItemComparerReturnsFalse_ThenBeEquivalentToThrows()
    {
        var actual = new Order
        {
            Items =
            [
                new() { Sku = "A-1", Quantity = 1 },
                new() { Sku = "B-2", Quantity = 2 },
            ]
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                actual,
                options => options.UseCollectionItemComparerForPath("actual.Items", new AlwaysFalseItemComparer())));

        Assert.Contains("actual.Items[0]", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenNullComparer_WhenConfiguringCollectionItemComparer_ThenThrowsArgumentNullException()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 1, 2, 3 };

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.UseCollectionItemComparerForPath("actual", null!)));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public void GivenNullPath_WhenConfiguringCollectionItemComparer_ThenThrowsArgumentNullException()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 1, 2, 3 };

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.UseCollectionItemComparerForPath(null!, StringComparer.Ordinal)));

        Assert.Equal("path", ex.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyOrWhitespacePath_WhenConfiguringCollectionItemComparer_ThenThrowsArgumentException(string path)
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 1, 2, 3 };

        var ex = Assert.Throws<ArgumentException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.UseCollectionItemComparerForPath(path, StringComparer.Ordinal)));

        Assert.Equal("path", ex.ParamName);
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
