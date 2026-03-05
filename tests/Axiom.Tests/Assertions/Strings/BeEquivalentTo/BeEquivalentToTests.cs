using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.BeEquivalentTo;

public sealed class BeEquivalentToTests
{
    [Fact]
    public void BeEquivalentTo_ReturnsContinuation_WhenValuesAreEquivalentUsingComparison()
    {
        const string value = "ABC";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeEquivalentTo("abc", StringComparison.OrdinalIgnoreCase);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeEquivalentTo_ReturnsContinuation_WhenBothValuesAreNull()
    {
        string? value = null;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeEquivalentTo(null!, StringComparison.Ordinal);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeEquivalentTo_Throws_WhenValuesAreNotEquivalentForComparison()
    {
        const string value = "ABC";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeEquivalentTo("abc", StringComparison.Ordinal));

        const string expected = "Expected value to be equivalent to \"abc\", but found \"ABC\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeEquivalentTo_Throws_WhenValueIsNullAndExpectedHasContent()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeEquivalentTo("abc", StringComparison.OrdinalIgnoreCase));

        const string expected = "Expected value to be equivalent to \"abc\", but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeEquivalentTo_Throws_WithReason_WhenProvided()
    {
        const string value = "ABC";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeEquivalentTo("abc", StringComparison.Ordinal, "normalisation should be strict here"));

        Assert.Contains("because normalisation should be strict here", ex.Message, StringComparison.Ordinal);
    }
}
