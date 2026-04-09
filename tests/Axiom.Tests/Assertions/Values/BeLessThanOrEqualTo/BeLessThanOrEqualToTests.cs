using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeLessThanOrEqualTo;

public sealed class BeLessThanOrEqualToTests
{
    private static readonly IComparer<int> ReverseComparer = Comparer<int>.Create(static (left, right) => right.CompareTo(left));

    [Fact]
    public void BeLessThanOrEqualTo_DoesNotThrow_WhenValueIsLess()
    {
        const int value = 2;

        var ex = Record.Exception(() => value.Should().BeLessThanOrEqualTo(5));

        Assert.Null(ex);
    }

    [Fact]
    public void BeLessThanOrEqualTo_DoesNotThrow_WhenValueIsEqual()
    {
        const int value = 5;

        var ex = Record.Exception(() => value.Should().BeLessThanOrEqualTo(5));

        Assert.Null(ex);
    }

    [Fact]
    public void BeLessThanOrEqualTo_Throws_WhenValueIsGreater()
    {
        const int value = 6;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeLessThanOrEqualTo(5));

        const string expected = "Expected value to be less than or equal to 5, but found 6.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeLessThanOrEqualTo_Throws_WithReason_WhenProvided()
    {
        const int value = 9;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeLessThanOrEqualTo(3, "safe mode cap is 3"));

        Assert.Contains("because safe mode cap is 3", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeLessThanOrEqualTo_WithComparer_DoesNotThrow_WhenComparerTreatsValueAsLess()
    {
        const int value = 7;

        var ex = Record.Exception(() => value.Should().BeLessThanOrEqualTo(5, ReverseComparer));

        Assert.Null(ex);
    }

    [Fact]
    public void BeLessThanOrEqualTo_WithComparer_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        const int value = 3;
        IComparer<int>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() => value.Should().BeLessThanOrEqualTo(5, comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }
}
