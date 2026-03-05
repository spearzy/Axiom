using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.EndWith;

public sealed class EndWithTests
{
    [Fact]
    public void EndWith_ReturnsContinuation_WhenValueEndsWithExpectedValue()
    {
        string value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.EndWith("st");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void EndWith_Throws_WhenValueDoesNotEndWithExpectedValue()
    {
        string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().EndWith("ab"));

        Assert.Contains("value", ex.Message);
        Assert.Contains("end with", ex.Message);
        Assert.Contains("ab", ex.Message);
        Assert.Contains("test", ex.Message);
    }
}
