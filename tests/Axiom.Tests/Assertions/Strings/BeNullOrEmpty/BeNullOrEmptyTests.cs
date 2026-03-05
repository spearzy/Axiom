using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.BeNullOrEmpty;

public sealed class BeNullOrEmptyTests
{
    [Fact]
    public void BeNullOrEmpty_ReturnsContinuation_WhenValueIsNull()
    {
        string? value = null;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeNullOrEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeNullOrEmpty_ReturnsContinuation_WhenValueIsEmpty()
    {
        const string value = "";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeNullOrEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeNullOrEmpty_Throws_WhenValueHasContent()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeNullOrEmpty());

        const string expected = "Expected value to be null or empty, but found \"test\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeNullOrEmpty_Throws_WithReason_WhenProvided()
    {
        const string value = "x";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeNullOrEmpty("this field should be blank at initialisation"));

        Assert.Contains("because this field should be blank at initialisation", ex.Message, StringComparison.Ordinal);
    }
}
