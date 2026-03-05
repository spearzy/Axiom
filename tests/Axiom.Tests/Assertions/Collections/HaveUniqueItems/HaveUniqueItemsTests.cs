using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.HaveUniqueItems;

public sealed class HaveUniqueItemsTests
{
    [Fact]
    public void HaveUniqueItems_ReturnsContinuation_WhenAllItemsAreUnique()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.HaveUniqueItems();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void HaveUniqueItems_Throws_WhenDuplicateItemExists()
    {
        int[] values = [1, 2, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().HaveUniqueItems());

        const string expected = "Expected values to have unique items, but found first duplicate item at index 2: 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void HaveUniqueItems_Throws_WhenDuplicateNullExists()
    {
        string?[] values = [null, "x", null];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().HaveUniqueItems());

        const string expected = "Expected values to have unique items, but found first duplicate item at index 2: <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void HaveUniqueItems_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().HaveUniqueItems("IDs must be unique across the result set"));

        Assert.Contains("because IDs must be unique across the result set", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveUniqueItems_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().HaveUniqueItems());

        const string expected = "Expected values to have unique items, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void HaveUniqueItems_DoesNotThrow_WhenCollectionIsEmpty()
    {
        int[] values = [];

        var ex = Record.Exception(() => values.Should().HaveUniqueItems());

        Assert.Null(ex);
    }
}
