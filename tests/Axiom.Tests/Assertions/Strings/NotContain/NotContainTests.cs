using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.NotContain;

public sealed class NotContainTests
{
    [Fact]
    public void NotContain_ReturnsContinuation_WhenValueDoesNotContainUnexpectedSubstring()
    {
        const string value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotContain("ab");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContain_Throws_WhenValueContainsUnexpectedSubstring()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotContain("es"));

        const string expected = "Expected value to not contain \"es\", but found \"test\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContain_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotContain("a"));

        const string expected = "Expected value to not contain \"a\", but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContain_Throws_WithReason_WhenProvided()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotContain("es", "reserved sequence must not appear"));

        Assert.Contains("because reserved sequence must not appear", ex.Message, StringComparison.Ordinal);
    }
}
