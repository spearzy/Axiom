namespace Axiom.Tests.Assertions.Collections.Contain;

public sealed class ContainTests
{
    [Fact]
    public void Contain_ReturnsContinuation_WhenItemExists()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.Contain(2);

        Xunit.Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void Contain_Throws_WhenItemDoesNotExist()
    {
        int[] values = [1, 2, 3];

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() => values.Should().Contain(9));

        const string expected = "Expected values to contain 9, but found System.Int32[].";
        Xunit.Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Contain_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2, 3];

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() =>
            values.Should().Contain(9, "the input must include an admin role"));

        Xunit.Assert.Contains("because the input must include an admin role", ex.Message);
    }
}
