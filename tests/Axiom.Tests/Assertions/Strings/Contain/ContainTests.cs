using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.Contain;

public sealed class ContainTests
{
    [Fact]
    public void Contain_ReturnsContinuation_WhenValueContainsExpectedSubstring()
    {
        const string value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.Contain("es");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void Contain_Throws_WhenValueDoesNotContainExpectedSubstring()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Contain("ab"));

        const string expected = "Expected value to contain \"ab\", but found \"test\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Contain_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Contain("a"));

        const string expected = "Expected value to contain \"a\", but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Contain_Throws_WithReason_WhenProvided()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().Contain("ab", "the generated key should include the shard prefix"));

        Assert.Contains("because the generated key should include the shard prefix", ex.Message, StringComparison.Ordinal);
    }
}
