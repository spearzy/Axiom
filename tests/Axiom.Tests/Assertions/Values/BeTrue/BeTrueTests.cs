using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.BeTrue;

public sealed class BeTrueTests
{
    [Fact]
    public void BeTrue_DoesNotThrow_WhenValueIsTrue()
    {
        const bool value = true;

        var ex = Record.Exception(() => value.Should().BeTrue());

        Assert.Null(ex);
    }

    [Fact]
    public void BeTrue_Throws_WhenFalse()
    {
        const bool value = false;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeTrue());

        const string expected = "Expected value to be True, but found False.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeTrue_Throws_WithReason_WhenProvided()
    {
        const bool value = false;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeTrue("input should be True by convention"));

        Assert.Contains("because input should be True by convention", ex.Message);
    }
}
