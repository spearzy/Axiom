using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.BeNullOrWhiteSpace;

public sealed class BeNullOrWhiteSpaceTests
{
    [Fact]
    public void BeNullOrWhiteSpace_ReturnsContinuation_WhenValueIsNull()
    {
        string? value = null;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeNullOrWhiteSpace();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeNullOrWhiteSpace_ReturnsContinuation_WhenValueIsEmpty()
    {
        const string value = "";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeNullOrWhiteSpace();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeNullOrWhiteSpace_ReturnsContinuation_WhenValueIsWhiteSpace()
    {
        const string value = "  ";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeNullOrWhiteSpace();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeNullOrWhiteSpace_Throws_WhenValueHasContent()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeNullOrWhiteSpace());

        const string expected = "Expected value to be null or white-space, but found \"test\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeNullOrWhiteSpace_Throws_WithReason_WhenProvided()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeNullOrWhiteSpace("this field should be blank during initial setup"));

        Assert.Contains("because this field should be blank during initial setup", ex.Message, StringComparison.Ordinal);
    }
}
