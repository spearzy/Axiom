using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Chaining;

public sealed class ValueChainingTests
{
    private sealed class Marker(string id)
    {
        public string Id { get; } = id;
    }

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
    public void BeOneOf_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = 42;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeOneOf([1, 42, 99]);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeOneOf_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = 42;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeOneOf([1, 2, 3]);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void Satisfy_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = 42;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.Satisfy(static x => x > 40);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotSatisfy_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = 42;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotSatisfy(static x => x < 40);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void FullChain_CanBeComposed()
    {
        var value = 42;

        value.Should().Be(42).And.NotBe(0);
    }

    [Fact]
    public void OneOfChain_CanBeComposed()
    {
        var value = 42;

        value.Should()
            .BeOneOf([41, 42, 43]).And
            .NotBeOneOf([1, 2, 3]).And
            .Be(42);
    }

    [Fact]
    public void PredicateChain_CanBeComposed()
    {
        var value = 42;

        value.Should()
            .Satisfy(static x => x > 40).And
            .NotSatisfy(static x => x < 40).And
            .Be(42);
    }

    [Fact]
    public void NotBeEquivalentTo_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = 42;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeEquivalentTo(7);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void EquivalencyInequalityChain_CanBeComposed()
    {
        var value = 42;

        value.Should().NotBeEquivalentTo(7).And.Be(42);
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

    [Fact]
    public void BeSameAs_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = new Marker("x");

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeSameAs(value);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ReferenceChain_CanBeComposed()
    {
        var value = new Marker("x");

        value.Should().BeSameAs(value).And.NotBeNull();
    }

    [Fact]
    public void NotBeSameAs_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        var value = new Marker("x");
        var other = new Marker("y");

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeSameAs(other);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ReferenceInequalityChain_CanBeComposed()
    {
        var value = new Marker("x");
        var other = new Marker("y");

        value.Should().NotBeSameAs(other).And.NotBeNull();
    }

    [Fact]
    public void BeOfType_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        object value = "hello";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeOfType<string>();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void TypeChain_CanBeComposed()
    {
        object value = "hello";

        value.Should().BeOfType<string>().And.NotBeNull();
    }

    [Fact]
    public void BeAssignableTo_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        object value = "hello";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeAssignableTo<object>();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeAssignableTo_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        object value = 42;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeAssignableTo<string>();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void AssignableTypeChain_CanBeComposed()
    {
        object value = "hello";

        value.Should().BeAssignableTo<object>().And.NotBeAssignableTo<int>();
    }

    [Fact]
    public void BeGreaterThan_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        const int value = 10;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeGreaterThan(5);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeGreaterThanOrEqualTo_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        const int value = 10;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeGreaterThanOrEqualTo(10);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeLessThan_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        const int value = 3;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeLessThan(5);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeLessThanOrEqualTo_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        const int value = 3;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeLessThanOrEqualTo(3);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeInRange_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        const int value = 3;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeInRange(1, 5);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeApproximately_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        const double value = 10.05d;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeApproximately(10d, 0.1d);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void OrderedComparisonChain_CanBeComposed()
    {
        const int value = 5;

        value.Should()
            .BeGreaterThan(1).And
            .BeGreaterThanOrEqualTo(5).And
            .BeLessThan(10).And
            .BeLessThanOrEqualTo(5).And
            .BeInRange(1, 5);
    }

    [Fact]
    public void ApproximateComparisonChain_CanBeComposed()
    {
        const double value = 10.05d;

        value.Should()
            .BeApproximately(10d, 0.1d).And
            .BeGreaterThan(10d);
    }
}
