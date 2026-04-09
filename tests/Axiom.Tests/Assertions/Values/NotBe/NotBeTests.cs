using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotBe;

public sealed class NotBeTests
{
    [Fact]
    public void NotBe_DoesNotThrow_WhenValuesDiffer()
    {
        var value = 42;

        var ex = Record.Exception(() => value.Should().NotBe(7));

        Assert.Null(ex);
    }

    [Fact]
    public void NotBe_Throws_WhenValuesAreEqual()
    {
        var value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBe(42));

        const string expected = "Expected value to not be 42, but found 42.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBe_Throws_WhenComparerMatches()
    {
        const int value = 3;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBe(5, new OddEvenMatchIntComparer()));

        const string expected = "Expected value to not be 5, but found 3.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBe_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        const int value = 3;
        IEqualityComparer<int>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() => value.Should().NotBe(5, comparer!));

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
