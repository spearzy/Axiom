using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.ContainExactlyInAnyOrder;

public sealed class ContainExactlyInAnyOrderTests
{
    [Fact]
    public void ContainExactlyInAnyOrder_ReturnsContinuation_WhenItemsMatchWithDifferentOrder()
    {
        int[] values = [3, 1, 2];

        var assertions = values.Should();
        var continuation = assertions.ContainExactlyInAnyOrder([1, 2, 3]);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_Passes_WhenDuplicateCountsMatch()
    {
        int[] values = [1, 2, 2];

        var ex = Record.Exception(() => values.Should().ContainExactlyInAnyOrder([2, 1, 2]));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_Passes_WhenComparerTreatsItemsAsEqual()
    {
        string[] values = ["Alpha", "beta", "BETA"];

        var ex = Record.Exception(() =>
            values.Should().ContainExactlyInAnyOrder(
                ["BETA", "alpha", "beta"],
                StringComparer.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_DoesNotFormatExpectedSequence_WhenAssertionPasses()
    {
        ThrowingToStringValue[] values = [new(1), new(2), new(3)];
        ThrowingToStringValue[] expected = [new(3), new(1), new(2)];

        var ex = Record.Exception(() => values.Should().ContainExactlyInAnyOrder(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_Throws_WhenExpectedItemIsMissing()
    {
        int[] values = [1, 2, 2];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainExactlyInAnyOrder([1, 2, 3]));

        Assert.Equal(
            "Expected values to contain exactly in any order [1, 2, 3], but found missing item 3: expected count 1 but found 0.",
            ex.Message);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_Throws_WhenUnexpectedItemIsPresent()
    {
        int[] values = [1, 2, 3, 4];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainExactlyInAnyOrder([1, 2, 3]));

        Assert.Equal(
            "Expected values to contain exactly in any order [1, 2, 3], but found unexpected item 4: found count 1 but expected 0.",
            ex.Message);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_Throws_WhenDuplicateCountsDiffer()
    {
        int[] values = [1, 2, 2];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainExactlyInAnyOrder([1, 2, 3]));

        Assert.DoesNotContain("unexpected item 2", ex.Message, StringComparison.Ordinal);
        Assert.Contains("missing item 3", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_Passes_WhenBothSequencesAreEmpty()
    {
        int[] values = [];

        var ex = Record.Exception(() => values.Should().ContainExactlyInAnyOrder(Array.Empty<int>()));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_Throws_WhenExpectedSequenceIsEmptyButSubjectIsNot()
    {
        int[] values = [1];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainExactlyInAnyOrder(Array.Empty<int>()));

        Assert.Equal(
            "Expected values to contain exactly in any order [], but found unexpected item 1: found count 1 but expected 0.",
            ex.Message);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2, 2];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainExactlyInAnyOrder([1, 2, 3], "normalised IDs must match exactly regardless of order"));

        Assert.Contains(
            "because normalised IDs must match exactly regardless of order",
            ex.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().ContainExactlyInAnyOrder([1]));

        Assert.Equal("Expected values to contain exactly in any order [1], but found <null>.", ex.Message);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_ThrowsArgumentNullException_WhenExpectedSequenceIsNull()
    {
        int[] values = [1, 2, 3];
        int[]? expectedSequence = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().ContainExactlyInAnyOrder(expectedSequence!));

        Assert.Equal("expectedSequence", ex.ParamName);
    }

    [Fact]
    public void ContainExactlyInAnyOrder_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        string[] values = ["Alpha"];
        StringComparer? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            values.Should().ContainExactlyInAnyOrder(["alpha"], comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    private readonly record struct ThrowingToStringValue(int Value)
    {
        public override string ToString()
        {
            throw new InvalidOperationException("ToString should not be called on pass path.");
        }
    }
}
