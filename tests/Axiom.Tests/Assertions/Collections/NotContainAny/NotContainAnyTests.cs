using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.NotContainAny;

public sealed class NotContainAnyTests
{
    [Fact]
    public void NotContainAny_ReturnsContinuation_WhenNoUnexpectedItemsArePresent_UsingParamsOverload()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainAny(9, 10);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainAny_ReturnsContinuation_WhenNoUnexpectedItemsArePresent_UsingEnumerableOverload()
    {
        int[] values = [1, 2, 3];
        IEnumerable<int> unexpectedItems = [8, 9, 10];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainAny(unexpectedItems);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainAny_Throws_WhenUnexpectedItemIsPresent()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().NotContainAny(9, 2, 10));

        const string expected =
            "Expected values to not contain any of [9, 2, 10], but found first matching item at subject index 1: 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContainAny_DoesNotThrow_WhenUnexpectedItemsAreEmpty()
    {
        int[] values = [1, 2, 3];
        IEnumerable<int> unexpectedItems = [];

        var ex = Record.Exception(() => values.Should().NotContainAny(unexpectedItems));

        Assert.Null(ex);
    }

    [Fact]
    public void NotContainAny_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2, 3];
        IEnumerable<int> unexpectedItems = [9, 2];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().NotContainAny(unexpectedItems, "restricted IDs must never be present"));

        Assert.Contains("because restricted IDs must never be present", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotContainAny_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().NotContainAny(1));

        const string expected = "Expected values to not contain any of [1], but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContainAny_ThrowsArgumentNullException_WhenUnexpectedEnumerableIsNull()
    {
        int[] values = [1, 2, 3];
        IEnumerable<int>? unexpectedItems = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().NotContainAny(unexpectedItems!));

        Assert.Equal("unexpectedItems", ex.ParamName);
    }

    [Fact]
    public void NotContainAny_ThrowsArgumentNullException_WhenUnexpectedParamsArrayIsNull()
    {
        int[] values = [1, 2, 3];
        int[]? unexpectedItems = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().NotContainAny(unexpectedItems!));

        Assert.Equal("unexpectedItems", ex.ParamName);
    }
}
