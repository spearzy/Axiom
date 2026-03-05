using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.BeEmpty;

public sealed class BeEmptyTests
{
    [Fact]
    public void BeEmpty_ReturnsContinuation_WhenCollectionIsEmpty()
    {
        int[] values = [];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.BeEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeEmpty_Throws_WhenCollectionIsNotEmpty()
    {
        int[] values = [1];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().BeEmpty());

        const string expected = "Expected values to be empty, but found 1.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeEmpty_Throws_WithReason_WhenProvided()
    {
        int[] values = [1];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().BeEmpty("this list must start empty for the test setup"));

        Assert.Contains("because this list must start empty for the test setup", ex.Message, StringComparison.Ordinal);
    }
}
