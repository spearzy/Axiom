using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.BeEmpty;

public sealed class BeEmptyTests
{
    [Fact]
    public void BeEmpty_ReturnsContinuation_WhenValueIsEmpty()
    {
        const string value = "";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeEmpty_Throws_WhenValueIsNotEmpty()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeEmpty());

        const string expected = "Expected value to be empty, but found \"test\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeEmpty_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeEmpty());

        const string expected = "Expected value to be empty, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeEmpty_Throws_WithReason_WhenProvided()
    {
        const string value = "x";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeEmpty("this field should be blank at initialisation"));

        Assert.Contains("because this field should be blank at initialisation", ex.Message, StringComparison.Ordinal);
    }
}
