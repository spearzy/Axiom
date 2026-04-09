using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.ContainAll;

public sealed class ContainAllTests
{
    [Fact]
    public void ContainAll_ReturnsContinuation_WhenAllItemsArePresent_UsingParamsOverload()
    {
        int[] values = [1, 2, 3, 4];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainAll(1, 3, 4);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainAll_ReturnsContinuation_WhenAllItemsArePresent_UsingEnumerableOverload()
    {
        int[] values = [1, 2, 3, 4];
        IEnumerable<int> expectedItems = [1, 2, 4];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainAll(expectedItems);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainAll_DoesNotFormatExpectedItems_WhenAssertionPasses()
    {
        ThrowingToStringValue[] values = [new(1), new(2), new(3)];
        IEnumerable<ThrowingToStringValue> expectedItems = [new(1), new(3)];

        var ex = Record.Exception(() => values.Should().ContainAll(expectedItems));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainAll_Throws_WhenAnyExpectedItemIsMissing()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainAll(1, 9));

        const string expected = "Expected values to contain all [1, 9], but found missing expected item at index 1: 9.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainAll_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2, 3];
        IEnumerable<int> expectedItems = [1, 9];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainAll(expectedItems, "all mandatory IDs must be present"));

        Assert.Contains("because all mandatory IDs must be present", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ContainAll_DoesNotThrow_WhenExpectedItemsAreEmpty()
    {
        int[] values = [1, 2, 3];
        IEnumerable<int> expectedItems = [];

        var ex = Record.Exception(() => values.Should().ContainAll(expectedItems));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainAll_ThrowsArgumentNullException_WhenExpectedEnumerableIsNull()
    {
        int[] values = [1, 2, 3];
        IEnumerable<int>? expectedItems = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().ContainAll(expectedItems!));

        Assert.Equal("expectedItems", ex.ParamName);
    }

    [Fact]
    public void ContainAll_ThrowsArgumentNullException_WhenExpectedParamsArrayIsNull()
    {
        int[] values = [1, 2, 3];
        int[]? expectedItems = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().ContainAll(expectedItems!));

        Assert.Equal("expectedItems", ex.ParamName);
    }

    [Fact]
    public void ContainAll_Passes_WhenComparerTreatsItemsAsEqual()
    {
        string[] values = ["Alpha", "Beta", "Gamma"];
        IEnumerable<string> expectedItems = ["alpha", "gamma"];

        var ex = Record.Exception(() =>
            values.Should().ContainAll(expectedItems, StringComparer.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainAll_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        string[] values = ["Alpha", "Beta"];
        StringComparer? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            values.Should().ContainAll(["alpha"], comparer!));

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
