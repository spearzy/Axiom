using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Strings.BeNull;

public sealed class BeNullTests
{
    [Fact]
    public void BeNull_ReturnsContinuation_WhenValueIsNull()
    {
        string? value = null;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeNull();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeNull_Throws_WhenValueIsNotNull()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeNull());

        Assert.Contains("value", ex.Message, StringComparison.Ordinal);
        Assert.Contains("to be null", ex.Message, StringComparison.Ordinal);
        Assert.Contains("\"test\"", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeNull_Throws_WithReason_WhenProvided()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeNull("a missing value is required for this scenario"));

        Assert.Contains("because a missing value is required for this scenario", ex.Message, StringComparison.Ordinal);
    }
}
