namespace Axiom.Tests.Assertions.Strings.Chaining;

public sealed class StringChainingTests
{
    [Fact]
    public void NotBeNull_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeNull();

        Xunit.Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void StartWith_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.StartWith("te");

        Xunit.Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void EndWith_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.EndWith("st");

        Xunit.Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void FullChain_CanBeComposed()
    {
        var value = "test";

        value.Should().StartWith("t").And.EndWith("t").And.NotBeNull();
    }
}
