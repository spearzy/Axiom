using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Strings.Chaining;

public sealed class StringChainingTests
{
    [Fact]
    public void NotBeNull_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeNull();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void StartWith_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.StartWith("te");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void EndWith_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.EndWith("st");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void Contain_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.Contain("es");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContain_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotContain("ab");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void HaveLength_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.HaveLength(4);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeEmpty_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = string.Empty;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeEmpty_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void FullChain_CanBeComposed()
    {
        var value = "test";

        value.Should().StartWith("t").And.EndWith("t").And.NotBeNull();
    }

    [Fact]
    public void ExtendedChain_CanBeComposed()
    {
        var value = "test";

        value.Should()
            .NotBeNull().And
            .StartWith("t").And
            .Contain("es").And
            .HaveLength(4).And
            .NotContain("ab").And
            .EndWith("t").And
            .NotBeEmpty();
    }
}
