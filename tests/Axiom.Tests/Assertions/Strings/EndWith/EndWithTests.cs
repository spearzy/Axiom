using Axiom;

namespace Axiom.Tests;

public sealed class EndWithTests
{
    [Fact]
    public void EndWith_ReturnsContinuation_WhenValueEndsWithExpectedValue()
    {
        string value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.EndWith("st");

        Xunit.Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void EndWith_Throws_WhenValueDoesNotEndWithExpectedValue()
    {
        string value = "test";

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() => value.Should().EndWith("ab"));

        Xunit.Assert.Contains("value", ex.Message);
        Xunit.Assert.Contains("end with", ex.Message);
        Xunit.Assert.Contains("ab", ex.Message);
        Xunit.Assert.Contains("test", ex.Message);
    }
}
