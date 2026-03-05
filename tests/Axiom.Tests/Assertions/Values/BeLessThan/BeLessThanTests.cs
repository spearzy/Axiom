using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeLessThan;

public sealed class BeLessThanTests
{
    [Fact]
    public void BeLessThan_DoesNotThrow_WhenValueIsLess()
    {
        const int value = 2;

        var ex = Record.Exception(() => value.Should().BeLessThan(5));

        Assert.Null(ex);
    }

    [Fact]
    public void BeLessThan_Throws_WhenValueIsNotLess()
    {
        const int value = 5;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeLessThan(5));

        const string expected = "Expected value to be less than 5, but found 5.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeLessThan_Throws_WithReason_WhenProvided()
    {
        const int value = 10;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeLessThan(3, "upper limit is 3"));

        Assert.Contains("because upper limit is 3", ex.Message, StringComparison.Ordinal);
    }
}
