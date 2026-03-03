using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.BeNull;

public sealed class BeNullTests
{
    [Fact]
    public void BeNull_DoesNotThrow_WhenValueIsNull()
    {
        int? value = null;

        var ex = Record.Exception(() => value.Should().BeNull());

        Assert.Null(ex);
    }

    [Fact]
    public void BeNull_Throws_WhenValueIsNotNull()
    {
        int? value = 1;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeNull());

        const string expected = "Expected value to be null, but found 1.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeNull_Throws_WithReason_WhenProvided()
    {
        int? value = 1;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeNull("can be null when db fails"));

        Assert.Contains("because can be null when db fails", ex.Message);
    }
}
