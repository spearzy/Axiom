using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeSameAs;

public sealed class BeSameAsTests
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
    public void BeSameAs_ReturnsContinuation_WhenSameReference()
    {
        var marker = new Marker("one");

        var baseAssertions = marker.Should();
        var continuation = baseAssertions.BeSameAs(marker);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeSameAs_Throws_WhenDifferentReferences()
    {
        var first = new Marker("one");
        var second = new Marker("two");

        var ex = Assert.Throws<InvalidOperationException>(() => first.Should().BeSameAs(second));

        const string expected = "Expected first to be same reference as Marker(two), but found Marker(one).";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeSameAs_Throws_WithReason_WhenProvided()
    {
        var first = new Marker("one");
        var second = new Marker("two");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            first.Should().BeSameAs(second, "both variables should point to the exact same object"));

        Assert.Contains("because both variables should point to the exact same object", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeSameAs_DoesNotThrow_WhenBothReferencesAreNull()
    {
        object? value = null;
        object? expected = null;

        var ex = Record.Exception(() => value.Should().BeSameAs(expected));

        Assert.Null(ex);
    }
}
