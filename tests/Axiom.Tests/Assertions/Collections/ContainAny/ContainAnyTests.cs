using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.ContainAny;

public sealed class ContainAnyTests
{
    [Fact]
    public void ContainAny_ReturnsContinuation_WhenAnyExpectedItemIsPresent_UsingParamsOverload()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainAny(9, 2, 10);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainAny_ReturnsContinuation_WhenAnyExpectedItemIsPresent_UsingEnumerableOverload()
    {
        int[] values = [1, 2, 3];
        IEnumerable<int> expectedItems = [8, 3, 9];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainAny(expectedItems);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainAny_Throws_WhenNoExpectedItemsArePresent()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainAny(9, 10));

        const string expected = "Expected values to contain any of [9, 10], but found none of the expected items were found.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainAny_Throws_WhenExpectedItemsAreEmpty()
    {
        int[] values = [1, 2, 3];
        IEnumerable<int> expectedItems = [];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainAny(expectedItems));

        const string expected = "Expected values to contain any of [], but found no expected items were provided.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainAny_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2, 3];
        IEnumerable<int> expectedItems = [9, 10];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainAny(expectedItems, "at least one approved ID must be present"));

        Assert.Contains("because at least one approved ID must be present", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ContainAny_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().ContainAny(1));

        const string expected = "Expected values to contain any of [1], but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainAny_ThrowsArgumentNullException_WhenExpectedEnumerableIsNull()
    {
        int[] values = [1, 2, 3];
        IEnumerable<int>? expectedItems = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().ContainAny(expectedItems!));

        Assert.Equal("expectedItems", ex.ParamName);
    }

    [Fact]
    public void ContainAny_ThrowsArgumentNullException_WhenExpectedParamsArrayIsNull()
    {
        int[] values = [1, 2, 3];
        int[]? expectedItems = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().ContainAny(expectedItems!));

        Assert.Equal("expectedItems", ex.ParamName);
    }
}
