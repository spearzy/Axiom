using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using System.Collections;

namespace Axiom.Tests.Assertions.Collections.ContainSingle;

public sealed class ContainSingleTests
{
    [Fact]
    public void ContainSingle_ReturnsContinuation_WhenCollectionHasOneItem()
    {
        int[] values = [1];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainSingle();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainSingle_ExposesSingleItem_WhenCollectionHasOneItem()
    {
        int[] values = [42];

        var continuation = values.Should().ContainSingle();
        var singleItem = continuation.SingleItem;

        Assert.Equal(42, singleItem);
    }

    [Fact]
    public void ContainSingle_ExposesStronglyTypedSingleItem_ForGenericValueTypes()
    {
        List<int> values = [42];

        var continuation = values.Should().ContainSingle();
        int singleItem = continuation.SingleItem;

        Assert.Equal(42, singleItem);
    }

    [Fact]
    public void ContainSingle_ExposesStronglyTypedSingleItem_ForGenericReferenceTypes()
    {
        List<Order> values = [new(42, 19.99m)];

        var continuation = values.Should().ContainSingle();
        var total = continuation.SingleItem.Total;

        Assert.Equal(19.99m, total);
    }

    [Fact]
    public void ContainSingle_ExposesStronglyTypedSingleItem_ForInterfaceTypedGenericCollections()
    {
        IReadOnlyList<Order> values = [new(7, 12.50m)];

        var continuation = values.Should().ContainSingle();
        decimal total = continuation.SingleItem.Total;

        Assert.Equal(12.50m, total);
    }

    [Fact]
    public void ContainSingle_FallsBackToObjectSingleItem_ForNonGenericCollections()
    {
        var values = new ArrayList { 42 };

        var continuation = values.Should().ContainSingle();
        var singleItem = Assert.IsType<int>(continuation.SingleItem);

        Assert.Equal(42, singleItem);
    }

    [Fact]
    public void ContainSingle_Throws_WhenCollectionIsEmpty()
    {
        int[] values = [];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainSingle());

        const string expected = "Expected values to contain a single item, but found 0.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainSingle_Throws_WhenCollectionHasMultipleItems()
    {
        int[] values = [1, 2];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainSingle());

        const string expected = "Expected values to contain a single item, but found 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainSingle_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainSingle("this scenario expects exactly one record"));

        Assert.Contains("because this scenario expects exactly one record", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void SingleItem_ThrowsExplicitMessage_WhenContainSingleFailedInsideBatch()
    {
        int[] values = [1, 2];

        var batch = new Axiom.Core.Batch();
        var continuation = values.Should().ContainSingle();

        var ex = Assert.Throws<InvalidOperationException>(() => _ = continuation.SingleItem);

        const string expected = "SingleItem is unavailable because ContainSingle failed with error: Expected values to contain a single item, but found 2.";
        Assert.Equal(expected, ex.Message);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void SingleItem_ReturnsValue_WhenContainSingleSucceedsInsideBatch()
    {
        int[] values = [7];

        var batch = new Axiom.Core.Batch();
        var continuation = values.Should().ContainSingle();
        var singleItem = continuation.SingleItem;

        Assert.Equal(7, singleItem);
        var disposeEx = Record.Exception(() => batch.Dispose());
        Assert.Null(disposeEx);
    }

    private sealed record Order(int Id, decimal Total);
}
