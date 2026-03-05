using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotBe;

public sealed class NotBeTests
{
    [Fact]
    public void NotBe_DoesNotThrow_WhenValuesDiffer()
    {
        var value = 42;

        var ex = Record.Exception(() => value.Should().NotBe(7));

        Assert.Null(ex);
    }

    [Fact]
    public void NotBe_Throws_WhenValuesAreEqual()
    {
        var value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBe(42));

        const string expected = "Expected value to not be 42, but found 42.";
        Assert.Equal(expected, ex.Message);
    }
}
