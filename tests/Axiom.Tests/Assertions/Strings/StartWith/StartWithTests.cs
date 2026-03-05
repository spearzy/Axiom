using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.StartWith;

public sealed class StartWithTests
{
    [Fact]
    public void StartWith_ReturnsContinuation_WhenValueStartsWithExpectedValue()
    {
        string? value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.StartWith("te");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void StartWith_Throws_WhenValueDoesNotStartWithExpectedValue()
    {
        string? value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().StartWith("ab"));

        Assert.Contains("value", ex.Message);
        Assert.Contains("start with", ex.Message);
        Assert.Contains("ab", ex.Message);
        Assert.Contains("test", ex.Message);
    }

    [Fact]
    public void StartWith_Throws_WithReason_WhenProvided()
    {
        string? value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().StartWith("ab", "the ID prefix is required"));

        Assert.Contains("because the ID prefix is required", ex.Message);
    }
}
