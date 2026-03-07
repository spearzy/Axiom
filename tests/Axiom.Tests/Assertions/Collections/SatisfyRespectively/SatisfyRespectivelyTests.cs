using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.SatisfyRespectively;

public sealed class SatisfyRespectivelyTests
{
    [Fact]
    public void SatisfyRespectively_ReturnsContinuation_WhenAllItemAssertionsPassInOrder()
    {
        int[] values = [2, 4, 6];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.SatisfyRespectively(
            (int item) => item.Should().BeGreaterThan(0),
            (int item) => item.Should().BeGreaterThan(3),
            (int item) => item.Should().BeGreaterThan(5));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void SatisfyRespectively_Throws_WhenAnItemAssertionFails()
    {
        int[] values = [10, -1, 30];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().SatisfyRespectively(
                (int item) => item.Should().BeGreaterThan(0),
                (int item) => item.Should().BeGreaterThan(0),
                (int item) => item.Should().BeGreaterThan(0)));

        Assert.Contains("Expected values to satisfy assertions respectively (failing index 1)", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Expected item to be greater than 0, but found -1.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void SatisfyRespectively_Throws_WhenCollectionHasFewerItemsThanAssertions()
    {
        int[] values = [1, 2];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().SatisfyRespectively(
                (int item) => item.Should().BeGreaterThan(0),
                (int item) => item.Should().BeGreaterThan(0),
                (int item) => item.Should().BeGreaterThan(0)));

        const string expected =
            "Expected values to satisfy assertions respectively (same order and count), but found collection had fewer items than assertions (expected 3, found 2).";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void SatisfyRespectively_Throws_WhenCollectionHasMoreItemsThanAssertions()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().SatisfyRespectively(
                (int item) => item.Should().BeGreaterThan(0),
                (int item) => item.Should().BeGreaterThan(0)));

        const string expected =
            "Expected values to satisfy assertions respectively (same order and count), but found collection had more items than assertions (expected 2, found 3).";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void SatisfyRespectively_Throws_WithReason_WhenProvided()
    {
        int[] values = [10, -1];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().SatisfyRespectively(
                "step outputs must follow contract",
                (int item) => item.Should().BeGreaterThan(0),
                (int item) => item.Should().BeGreaterThan(0)));

        Assert.Contains("because step outputs must follow contract", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void SatisfyRespectively_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values!.Should().SatisfyRespectively((int item) => item.Should().BeGreaterThan(0)));

        const string expected =
            "Expected values to satisfy assertions respectively (same order and count), but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void SatisfyRespectively_DoesNotThrow_WhenBothCollectionAndAssertionsAreEmpty()
    {
        int[] values = [];

        var ex = Record.Exception(() => values.Should().SatisfyRespectively(Array.Empty<Action<int>>()));

        Assert.Null(ex);
    }

    [Fact]
    public void SatisfyRespectively_ThrowsArgumentNullException_WhenAssertionsArrayIsNull()
    {
        int[] values = [1, 2, 3];
        Action<int>[]? assertionsForItems = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            values.Should().SatisfyRespectively(assertionsForItems!));

        Assert.Equal("assertionsForItems", ex.ParamName);
    }

    [Fact]
    public void SatisfyRespectively_ThrowsArgumentNullException_WhenAnAssertionItemIsNull()
    {
        int[] values = [1, 2];
        Action<int>? second = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            values.Should().SatisfyRespectively(
                (int item) => item.Should().BeGreaterThan(0),
                second!));

        Assert.Equal("assertionsForItems", ex.ParamName);
        Assert.Contains("assertionsForItems[1]", ex.Message, StringComparison.Ordinal);
    }
}
