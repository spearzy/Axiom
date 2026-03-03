using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.BeGreaterThanOrEqualTo;

public sealed class BeGreaterThanOrEqualToTests
{
    [Fact]
    public void BeGreaterThanOrEqualTo_DoesNotThrow_WhenValueIsGreater()
    {
        const int value = 10;

        var ex = Record.Exception(() => value.Should().BeGreaterThanOrEqualTo(5));

        Assert.Null(ex);
    }

    [Fact]
    public void BeGreaterThanOrEqualTo_DoesNotThrow_WhenValueIsEqual()
    {
        const int value = 5;

        var ex = Record.Exception(() => value.Should().BeGreaterThanOrEqualTo(5));

        Assert.Null(ex);
    }

    [Fact]
    public void BeGreaterThanOrEqualTo_Throws_WhenValueIsLess()
    {
        const int value = 4;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeGreaterThanOrEqualTo(5));

        const string expected = "Expected value to be greater than or equal to 5, but found 4.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeGreaterThanOrEqualTo_Throws_WithReason_WhenProvided()
    {
        const int value = 1;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeGreaterThanOrEqualTo(3, "threshold includes fallback mode"));

        Assert.Contains("because threshold includes fallback mode", ex.Message, StringComparison.Ordinal);
    }
}
