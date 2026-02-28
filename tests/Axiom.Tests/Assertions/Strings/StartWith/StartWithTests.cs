namespace Axiom.Tests.Assertions.Strings.StartWith;

public sealed class StartWithTests
{
    [Fact]
    public void StartWith_ReturnsContinuation_WhenValueStartsWithExpectedValue()
    {
        string? value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.StartWith("te");

        Xunit.Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void StartWith_Throws_WhenValueDoesNotStartWithExpectedValue()
    {
        string? value = "test";

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() => value.Should().StartWith("ab"));

        Xunit.Assert.Contains("value", ex.Message);
        Xunit.Assert.Contains("start with", ex.Message);
        Xunit.Assert.Contains("ab", ex.Message);
        Xunit.Assert.Contains("test", ex.Message);
    }
}
