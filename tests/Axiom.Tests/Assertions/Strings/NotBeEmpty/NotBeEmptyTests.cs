using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Strings.NotBeEmpty;

public sealed class NotBeEmptyTests
{
    [Fact]
    public void NotBeEmpty_ReturnsContinuation_WhenValueIsNotEmpty()
    {
        const string value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeEmpty_Throws_WhenValueIsEmpty()
    {
        const string value = "";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeEmpty());

        const string expected = "Expected value to not be empty, but found \"\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBeEmpty_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeEmpty());

        const string expected = "Expected value to not be empty, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBeEmpty_Throws_WithReason_WhenProvided()
    {
        const string value = "";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotBeEmpty("external id is mandatory"));

        Assert.Contains("because external id is mandatory", ex.Message, StringComparison.Ordinal);
    }
}
