using Axiom.Assertions;
using Axiom.Assertions.Extensions;

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
}
