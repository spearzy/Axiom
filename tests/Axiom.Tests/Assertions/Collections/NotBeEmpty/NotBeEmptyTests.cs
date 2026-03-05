using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.NotBeEmpty;

public sealed class NotBeEmptyTests
{
    [Fact]
    public void NotBeEmpty_ReturnsContinuation_WhenCollectionHasItems()
    {
        int[] values = [1];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotBeEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeEmpty_Throws_WhenCollectionIsEmpty()
    {
        int[] values = [];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().NotBeEmpty());

        const string expected = "Expected values to not be empty, but found 0.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBeEmpty_Throws_WithReason_WhenProvided()
    {
        int[] values = [];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().NotBeEmpty("a user must have at least one role"));

        Assert.Contains("because a user must have at least one role", ex.Message, StringComparison.Ordinal);
    }
}
