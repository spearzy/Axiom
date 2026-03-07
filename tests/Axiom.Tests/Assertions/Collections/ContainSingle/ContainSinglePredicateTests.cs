using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.ContainSingle;

public sealed class ContainSinglePredicateTests
{
    [Fact]
    public void ContainSingle_WithPredicate_ReturnsContinuation_WhenExactlyOneItemMatches()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainSingle((int item) => item == 2);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainSingle_WithPredicate_ExposesSingleItem_WhenExactlyOneItemMatches()
    {
        int[] values = [10, 20, 30];

        var continuation = values.Should().ContainSingle((int item) => item == 20);
        var singleItem = Assert.IsType<int>(continuation.SingleItem);

        Assert.Equal(20, singleItem);
    }

    [Fact]
    public void ContainSingle_WithPredicate_Throws_WhenNoItemMatches()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainSingle((int item) => item == 9));

        const string expected = "Expected values to contain a single item matching predicate, but found 0.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainSingle_WithPredicate_Throws_WhenMultipleItemsMatch()
    {
        int[] values = [2, 1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainSingle((int item) => item == 2));

        const string expected = "Expected values to contain a single item matching predicate, but found 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainSingle_WithPredicate_Throws_WithReason_WhenProvided()
    {
        int[] values = [2, 1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainSingle((int item) => item == 2, "only one active record should exist"));

        Assert.Contains("because only one active record should exist", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ContainSingle_WithPredicate_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values!.Should().ContainSingle((int item) => item == 1));

        const string expected = "Expected values to contain a single item matching predicate, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainSingle_WithPredicate_ThrowsArgumentNullException_WhenPredicateIsNull()
    {
        int[] values = [1, 2, 3];
        Func<int, bool>? predicate = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().ContainSingle(predicate!));

        Assert.Equal("predicate", ex.ParamName);
    }

    [Fact]
    public void SingleItem_ThrowsExplicitMessage_WhenPredicateContainSingleFailedInsideBatch()
    {
        int[] values = [1, 2, 3];

        var batch = new Axiom.Core.Batch();
        var continuation = values.Should().ContainSingle((int item) => item == 9);

        var ex = Assert.Throws<InvalidOperationException>(() => _ = continuation.SingleItem);

        const string expected =
            "SingleItem is unavailable because ContainSingle failed with error: Expected values to contain a single item matching predicate, but found 0.";
        Assert.Equal(expected, ex.Message);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void SingleItem_ReturnsValue_WhenPredicateContainSingleSucceedsInsideBatch()
    {
        int[] values = [1, 2, 3];

        var batch = new Axiom.Core.Batch();
        var continuation = values.Should().ContainSingle((int item) => item == 2);
        var singleItem = Assert.IsType<int>(continuation.SingleItem);

        Assert.Equal(2, singleItem);
        var disposeEx = Record.Exception(() => batch.Dispose());
        Assert.Null(disposeEx);
    }
}
