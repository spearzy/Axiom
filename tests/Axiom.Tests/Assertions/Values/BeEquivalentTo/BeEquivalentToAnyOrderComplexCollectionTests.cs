using Axiom.Assertions;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToAnyOrderComplexCollectionTests
{
    [Fact]
    public void GivenAnyCollectionOrder_WhenComplexItemsAreReordered_ThenDoesNotThrow()
    {
        var actual = new[]
        {
            new LineItem { Sku = "B-200", Quantity = 2 },
            new LineItem { Sku = "A-100", Quantity = 1 },
        };
        var expected = new[]
        {
            new LineItem { Sku = "A-100", Quantity = 1 },
            new LineItem { Sku = "B-200", Quantity = 2 },
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.CollectionOrder = EquivalencyCollectionOrder.Any));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenAnyCollectionOrder_WhenDuplicateComplexItemsMatchByCount_ThenDoesNotThrow()
    {
        var actual = new[]
        {
            new LineItem { Sku = "A-100", Quantity = 1 },
            new LineItem { Sku = "A-100", Quantity = 1 },
            new LineItem { Sku = "B-200", Quantity = 2 },
        };
        var expected = new[]
        {
            new LineItem { Sku = "B-200", Quantity = 2 },
            new LineItem { Sku = "A-100", Quantity = 1 },
            new LineItem { Sku = "A-100", Quantity = 1 },
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.CollectionOrder = EquivalencyCollectionOrder.Any));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenAnyCollectionOrder_WhenExpectedComplexItemIsMissing_ThenFailureIsClear()
    {
        var actual = new[]
        {
            new LineItem { Sku = "A-100", Quantity = 1 },
        };
        var expected = new[]
        {
            new LineItem { Sku = "A-100", Quantity = 1 },
            new LineItem { Sku = "B-200", Quantity = 2 },
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.CollectionOrder = EquivalencyCollectionOrder.Any));

        Assert.Contains("actual[1]", ex.Message, StringComparison.Ordinal);
        Assert.Contains("expected collection item was not found in actual collection", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenAnyCollectionOrder_WhenActualHasExtraComplexItem_ThenFailureIsClear()
    {
        var actual = new[]
        {
            new LineItem { Sku = "A-100", Quantity = 1 },
            new LineItem { Sku = "C-300", Quantity = 3 },
        };
        var expected = new[]
        {
            new LineItem { Sku = "A-100", Quantity = 1 },
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.CollectionOrder = EquivalencyCollectionOrder.Any));

        Assert.Contains("actual[1]", ex.Message, StringComparison.Ordinal);
        Assert.Contains("actual collection contains an extra item", ex.Message, StringComparison.Ordinal);
    }

    private sealed class LineItem
    {
        public string? Sku { get; init; }
        public int Quantity { get; init; }
    }
}
