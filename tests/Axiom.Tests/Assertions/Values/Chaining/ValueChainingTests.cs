namespace Axiom.Tests;

public sealed class ValueChainingTests
{
    [Fact]
    public void Be_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = 42;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.Be(42);

        Xunit.Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBe_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = 42;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBe(7);

        Xunit.Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void FullChain_CanBeComposed()
    {
        var value = 42;

        value.Should().Be(42).And.NotBe(0);
    }
}
