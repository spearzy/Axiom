using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToTests
{
    [Fact]
    public void GivenLeafValues_WhenEquivalent_ThenDoesNotThrow()
    {
        var value = 42;

        var ex = Record.Exception(() => value.Should().BeEquivalentTo(42));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenLeafValues_WhenNotEquivalent_ThenThrowsDeterministicReport()
    {
        var value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeEquivalentTo(7));

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Expected value to be equivalent to 7, but found 1 difference(s):", message, StringComparison.Ordinal);
        Assert.Contains("1) value: expected 7, but found 42", message, StringComparison.Ordinal);
        Assert.Contains("(values differ)", message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenStringValues_WhenStringComparisonConfigured_ThenUsesConfiguredComparison()
    {
        object value = "ABC";

        var ex = Record.Exception(() =>
            value.Should().BeEquivalentTo("abc", options => options.StringComparison = StringComparison.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenDifferentRuntimeTypes_WhenUsingDefaultOptions_ThenThrows()
    {
        object value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeEquivalentTo(42L));

        Assert.Contains("runtime types differ", ex.Message, StringComparison.Ordinal);
        Assert.Contains("System.Int64", ex.Message, StringComparison.Ordinal);
        Assert.Contains("System.Int32", ex.Message, StringComparison.Ordinal);
    }
}
