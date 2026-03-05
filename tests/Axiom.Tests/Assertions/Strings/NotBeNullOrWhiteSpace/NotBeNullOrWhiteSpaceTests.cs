using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.NotBeNullOrWhiteSpace;

public sealed class NotBeNullOrWhiteSpaceTests
{
    [Fact]
    public void NotBeNullOrWhiteSpace_ReturnsContinuation_WhenValueHasContent()
    {
        const string value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeNullOrWhiteSpace();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeNullOrWhiteSpace_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeNullOrWhiteSpace());

        const string expected = "Expected value to not be null or white-space, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBeNullOrWhiteSpace_Throws_WhenValueIsEmpty()
    {
        const string value = "";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeNullOrWhiteSpace());

        const string expected = "Expected value to not be null or white-space, but found \"\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBeNullOrWhiteSpace_Throws_WhenValueIsWhiteSpace()
    {
        const string value = "  ";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeNullOrWhiteSpace());

        Assert.Contains("Expected value to not be null or white-space", ex.Message, StringComparison.Ordinal);
        Assert.Contains("but found \"  \".", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotBeNullOrWhiteSpace_Throws_WithReason_WhenProvided()
    {
        const string value = " ";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotBeNullOrWhiteSpace("a display name is required"));

        Assert.Contains("because a display name is required", ex.Message, StringComparison.Ordinal);
    }
}
