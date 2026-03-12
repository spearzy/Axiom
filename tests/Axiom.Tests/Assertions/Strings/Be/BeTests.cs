using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Strings.Be;

public sealed class BeTests
{
    [Fact]
    public void Be_ReturnsContinuation_WhenValuesAreEqual()
    {
        const string value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.Be("test");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void Be_Throws_WhenValuesDiffer()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Be("prod"));

        const string expected = "Expected value to be \"prod\", but found \"test\" (first string difference; first difference at expected index 0, actual index 0; expected snippet \"prod\", actual snippet \"test\").";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Be_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Be("test"));

        const string expected = "Expected value to be \"test\", but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Be_Throws_WithReason_WhenProvided()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().Be("prod", "the environment marker must match"));

        Assert.Contains("because the environment marker must match", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Be_DoesNotThrow_WhenComparisonIgnoresCase()
    {
        const string value = "ABC";

        var ex = Record.Exception(() =>
            value.Should().Be("abc", StringComparison.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public void Be_Throws_WhenComparisonIsCaseSensitive()
    {
        const string value = "ABC";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().Be("abc", StringComparison.Ordinal));

        Assert.Contains("Expected value to be \"abc\"", ex.Message, StringComparison.Ordinal);
        Assert.Contains("first string difference", ex.Message, StringComparison.Ordinal);
    }
}
