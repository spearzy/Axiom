using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.Be;

public sealed class BeTests
{
    [Fact]
    public void Be_DoesNotThrow_WhenValuesAreEqual()
    {
        var value = 42;

        var ex = Record.Exception(() => value.Should().Be(42));

        Assert.Null(ex);
    }

    [Fact]
    public void Be_Throws_WhenValuesDiffer()
    {
        var value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Be(7));

        const string expected = "Expected value to be 7, but found 42.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Be_Throws_WithReason_WhenProvided()
    {
        var value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().Be(7, "input should align with seeded data"));

        Assert.Contains("because input should align with seeded data", ex.Message);
    }

    [Fact]
    public void Be_DoesNotThrow_WhenComparerMatches()
    {
        const int value = 3;

        var ex = Record.Exception(() => value.Should().Be(5, new OddEvenMatchIntComparer()));

        Assert.Null(ex);
    }

    [Fact]
    public void Be_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        const int value = 3;
        IEqualityComparer<int>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() => value.Should().Be(5, comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    private sealed class OddEvenMatchIntComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x % 2 == y % 2;
        }

        public int GetHashCode(int obj)
        {
            return obj % 2;
        }
    }
}
