using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Strings.NotBeNullOrEmpty;

public sealed class NotBeNullOrEmptyTests
{
    [Fact]
    public void NotBeNullOrEmpty_ReturnsContinuation_WhenValueHasContent()
    {
        const string value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeNullOrEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeNullOrEmpty_ReturnsContinuation_WhenValueIsWhiteSpace()
    {
        const string value = " ";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeNullOrEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeNullOrEmpty_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeNullOrEmpty());

        const string expected = "Expected value to not be null or empty, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBeNullOrEmpty_Throws_WhenValueIsEmpty()
    {
        const string value = "";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeNullOrEmpty());

        const string expected = "Expected value to not be null or empty, but found \"\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBeNullOrEmpty_Throws_WithReason_WhenProvided()
    {
        const string value = "";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotBeNullOrEmpty("a value must be provided"));

        Assert.Contains("because a value must be provided", ex.Message, StringComparison.Ordinal);
    }
}
