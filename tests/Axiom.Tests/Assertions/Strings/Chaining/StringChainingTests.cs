using Axiom.Assertions;

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
    public void Contain_WithComparison_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "TEST";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.Contain("es", StringComparison.OrdinalIgnoreCase);

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
    public void BeNullOrEmpty_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = string.Empty;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeNullOrEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeNullOrEmpty_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeNullOrEmpty();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeNullOrWhiteSpace_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = " ";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeNullOrWhiteSpace();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeNullOrWhiteSpace_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeNullOrWhiteSpace();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void Match_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "AB-123";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.Match(@"^[A-Z]{2}-\d{3}$");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotMatch_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "AB-12";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotMatch(@"^[A-Z]{2}-\d{3}$");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeEquivalentTo_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = "ABC";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeEquivalentTo("abc", StringComparison.OrdinalIgnoreCase);

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
        var value = "AB-123";

        value.Should()
            .NotBeNull().And
            .NotBeNullOrWhiteSpace().And
            .NotBeNullOrEmpty().And
            .StartWith("AB").And
            .Contain("-").And
            .HaveLength(6).And
            .NotContain(" ").And
            .BeEquivalentTo("ab-123", StringComparison.OrdinalIgnoreCase).And
            .Match(@"^[A-Z]{2}-\d{3}$").And
            .NotMatch(@"^\d+$").And
            .EndWith("123").And
            .NotBeEmpty();
    }

    [Fact]
    public void ComparisonOverloadChain_CanBeComposed()
    {
        const string value = "ABC-123";

        value.Should()
            .StartWith("ab", StringComparison.OrdinalIgnoreCase).And
            .Contain("-1", StringComparison.OrdinalIgnoreCase).And
            .NotContain("zz", StringComparison.OrdinalIgnoreCase).And
            .EndWith("123", StringComparison.OrdinalIgnoreCase);
    }
}
