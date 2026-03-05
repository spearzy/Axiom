using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.HaveCount;

public sealed class HaveCountTests
{
    [Fact]
    public void HaveCount_ReturnsContinuation_WhenCountMatches()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.HaveCount(3);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void HaveCount_Throws_WhenCountDoesNotMatch()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().HaveCount(2));

        const string expected = "Expected values to have count 2, but found 3.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void HaveCount_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().HaveCount(2, "each branch must have two commits"));

        Assert.Contains("because each branch must have two commits", ex.Message);
    }
}
