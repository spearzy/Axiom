using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Strings.HaveLength;

public sealed class HaveLengthTests
{
    [Fact]
    public void HaveLength_ReturnsContinuation_WhenLengthMatches()
    {
        const string value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.HaveLength(4);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void HaveLength_Throws_WhenLengthDoesNotMatch()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().HaveLength(3));

        const string expected = "Expected value to have length 3, but found 4.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void HaveLength_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().HaveLength(0));

        const string expected = "Expected value to have length 0, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void HaveLength_Throws_WithReason_WhenProvided()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().HaveLength(2, "two-character country code is required"));

        Assert.Contains("because two-character country code is required", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveLength_ThrowsArgumentOutOfRangeException_WhenExpectedLengthIsNegative()
    {
        const string value = "test";

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => value.Should().HaveLength(-1));

        Assert.Contains("expectedLength must be non-negative.", ex.Message, StringComparison.Ordinal);
    }
}
