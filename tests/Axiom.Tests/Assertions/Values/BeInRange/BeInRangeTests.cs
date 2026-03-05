using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeInRange;

public sealed class BeInRangeTests
{
    [Fact]
    public void BeInRange_DoesNotThrow_WhenValueIsWithinInclusiveRange()
    {
        const int value = 5;

        var ex = Record.Exception(() => value.Should().BeInRange(1, 5));

        Assert.Null(ex);
    }

    [Fact]
    public void BeInRange_Throws_WhenValueIsOutsideRange()
    {
        const int value = 10;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeInRange(1, 5));

        const string expected = "Expected value to be in range [1, 5], but found 10.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeInRange_Throws_WithReason_WhenProvided()
    {
        const int value = 20;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeInRange(1, 5, "valid retries are between 1 and 5"));

        Assert.Contains("because valid retries are between 1 and 5", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeInRange_ThrowsArgumentException_WhenMinimumIsGreaterThanMaximum()
    {
        const int value = 5;

        var ex = Assert.Throws<ArgumentException>(() => value.Should().BeInRange(10, 1));

        Assert.Contains("minimum must be less than or equal to maximum.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeInRange_ThrowsArgumentException_WhenBoundsAreNotComparable()
    {
        object value = 5;

        var ex = Assert.Throws<ArgumentException>(() => value.Should().BeInRange(1, "z"));

        Assert.Contains("Range bounds must support ordering comparisons.", ex.Message, StringComparison.Ordinal);
    }
}
