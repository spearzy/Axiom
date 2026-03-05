using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotBeSameAs;

public sealed class NotBeSameAsTests
{
    private sealed class Marker(string id)
    {
        public string Id { get; } = id;

        public override string ToString()
        {
            return $"Marker({Id})";
        }
    }

    [Fact]
    public void NotBeSameAs_ReturnsContinuation_WhenDifferentReferences()
    {
        var first = new Marker("one");
        var second = new Marker("two");

        var baseAssertions = first.Should();
        var continuation = baseAssertions.NotBeSameAs(second);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeSameAs_Throws_WhenSameReference()
    {
        var marker = new Marker("one");

        var ex = Assert.Throws<InvalidOperationException>(() => marker.Should().NotBeSameAs(marker));

        const string expected = "Expected marker to not be same reference as Marker(one), but found Marker(one).";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBeSameAs_Throws_WithReason_WhenProvided()
    {
        var marker = new Marker("one");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            marker.Should().NotBeSameAs(marker, "service should return a new object each call"));

        Assert.Contains("because service should return a new object each call", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotBeSameAs_DoesNotThrow_WhenOneReferenceIsNull()
    {
        var marker = new Marker("one");
        Marker? value = null;

        var ex = Record.Exception(() => value.Should().NotBeSameAs(marker));

        Assert.Null(ex);
    }

    [Fact]
    public void NotBeSameAs_Throws_WhenBothReferencesAreNull()
    {
        Marker? value = null;
        Marker? unexpected = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeSameAs(unexpected));

        const string expected = "Expected value to not be same reference as <null>, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }
}
