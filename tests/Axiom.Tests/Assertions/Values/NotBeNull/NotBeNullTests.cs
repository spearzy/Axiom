using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotBeNull;

public sealed class NotBeNullTests
{
    [Fact]
    public void NotBeNull_DoesNotThrow_WhenValueIsNotNull()
    {
        int? value = 1;

        var ex = Record.Exception(() => value.Should().NotBeNull());

        Assert.Null(ex);
    }

    [Fact]
    public void NotBeNull_Throws_WhenValueIsNull()
    {
        int? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeNull());

        const string expected = "Expected value to not be null, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBeNull_Throws_WithReason_WhenProvided()
    {
        int? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotBeNull("cannot be null from API"));

        Assert.Contains("because cannot be null from API", ex.Message);
    }
}