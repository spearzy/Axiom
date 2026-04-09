using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeLessThan;

public sealed class BeLessThanTests
{
    private static readonly IComparer<int> ReverseComparer = Comparer<int>.Create(static (left, right) => right.CompareTo(left));

    [Fact]
    public void BeLessThan_DoesNotThrow_WhenValueIsLess()
    {
        const int value = 2;

        var ex = Record.Exception(() => value.Should().BeLessThan(5));

        Assert.Null(ex);
    }

    [Fact]
    public void BeLessThan_Throws_WhenValueIsNotLess()
    {
        const int value = 5;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeLessThan(5));

        const string expected = "Expected value to be less than 5, but found 5.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeLessThan_Throws_WithReason_WhenProvided()
    {
        const int value = 10;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeLessThan(3, "upper limit is 3"));

        Assert.Contains("because upper limit is 3", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeLessThan_WithComparer_DoesNotThrow_WhenComparerTreatsValueAsLess()
    {
        const int value = 7;

        var ex = Record.Exception(() => value.Should().BeLessThan(5, ReverseComparer));

        Assert.Null(ex);
    }

    [Fact]
    public void BeLessThan_WithComparer_UsesExplicitComparerInsteadOfDefaultOrdering()
    {
        const int value = 3;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeLessThan(5, ReverseComparer));

        const string expected = "Expected value to be less than 5, but found 3.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeLessThan_WithComparer_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        const int value = 3;
        IComparer<int>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() => value.Should().BeLessThan(5, comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }
}
