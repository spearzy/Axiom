using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.BeFalse;

public sealed class BeFalseTests
{
    [Fact]
    public void BeFalse_DoesNotThrow_WhenValueIsFalse()
    {
        const bool value = false;

        var ex = Record.Exception(() => value.Should().BeFalse());

        Assert.Null(ex);
    }

    [Fact]
    public void BeFalse_Throws_WhenTrue()
    {
        const bool value = true;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeFalse());

        const string expected = "Expected value to be False, but found True.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeFalse_Throws_WithReason_WhenProvided()
    {
        const bool value = true;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeFalse("input should be False by convention"));

        Assert.Contains("because input should be False by convention", ex.Message);
    }
}