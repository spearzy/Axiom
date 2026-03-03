using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Chaining;

public sealed class ValueChainingTests
{
    [Fact]
    public void Be_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = 42;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.Be(42);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBe_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = 42;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBe(7);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void FullChain_CanBeComposed()
    {
        var value = 42;

        value.Should().Be(42).And.NotBe(0);
    }

    [Fact]
    public void BeFalse_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        const bool value = false;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeFalse();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeFalseBoolChain_CanBeComposed()
    {
        const bool value = false;

        value.Should().BeFalse().And.NotBe(true);
    }

    [Fact]
    public void BeTrue_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        const bool value = true;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeTrue();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeTrueBoolChain_CanBeComposed()
    {
        const bool value = true;

        value.Should().BeTrue().And.NotBe(false);
    }

    [Fact]
    public void BeNull_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        int? value = null;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeNull();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeNull_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        int? value = 42;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeNull();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NullabilityChain_CanBeComposed()
    {
        int? value = 42;

        value.Should().NotBeNull().And.Be(42);
    }
}
