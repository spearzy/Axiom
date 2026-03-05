using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.AllSatisfy;

public sealed class AllSatisfyTests
{
    [Fact]
    public void AllSatisfy_ReturnsContinuation_WhenEveryItemSatisfiesAssertions()
    {
        int[] values = [10, 20, 30];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.AllSatisfy((int item) => item.Should().BeGreaterThan(0));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void AllSatisfy_Throws_WhenAnItemFailsAssertions()
    {
        int[] values = [10, -1, 30];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().AllSatisfy((int item) => item.Should().BeGreaterThan(0)));

        Assert.Contains("Expected values to satisfy all assertions for each item (first failing index 1)", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Expected item to be greater than 0, but found -1.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AllSatisfy_Throws_WithReason_WhenProvided()
    {
        int[] values = [10, -1, 30];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().AllSatisfy((int item) => item.Should().BeGreaterThan(0), "all scores must be positive"));

        Assert.Contains("because all scores must be positive", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AllSatisfy_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values!.Should().AllSatisfy((int item) => item.Should().BeGreaterThan(0)));

        const string expected = "Expected values to satisfy all assertions for each item, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void AllSatisfy_ThrowsArgumentNullException_WhenAssertionIsNull()
    {
        int[] values = [1, 2, 3];
        Action<int>? assertion = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().AllSatisfy(assertion!));

        Assert.Equal("assertion", ex.ParamName);
    }
}
