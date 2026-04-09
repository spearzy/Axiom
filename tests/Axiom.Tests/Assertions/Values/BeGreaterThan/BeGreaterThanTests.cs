using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeGreaterThan;

public sealed class BeGreaterThanTests
{
    private static readonly IComparer<int> ReverseComparer = Comparer<int>.Create(static (left, right) => right.CompareTo(left));

    [Fact]
    public void BeGreaterThan_DoesNotThrow_WhenValueIsGreater()
    {
        const int value = 10;

        var ex = Record.Exception(() => value.Should().BeGreaterThan(5));

        Assert.Null(ex);
    }

    [Fact]
    public void BeGreaterThan_Throws_WhenValueIsNotGreater()
    {
        const int value = 5;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeGreaterThan(5));

        const string expected = "Expected value to be greater than 5, but found 5.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeGreaterThan_Throws_WithReason_WhenProvided()
    {
        const int value = 2;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeGreaterThan(10, "minimum valid score is 10"));

        Assert.Contains("because minimum valid score is 10", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeGreaterThan_ThrowsAssertionFailure_WhenValuesAreNotComparable()
    {
        object value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeGreaterThan("text"));

        const string expected = "Expected value to be greater than \"text\", but found 42.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeGreaterThan_WithComparer_DoesNotThrow_WhenComparerTreatsValueAsGreater()
    {
        const int value = 3;

        var ex = Record.Exception(() => value.Should().BeGreaterThan(5, ReverseComparer));

        Assert.Null(ex);
    }

    [Fact]
    public void BeGreaterThan_WithComparer_UsesExplicitComparerInsteadOfDefaultOrdering()
    {
        const int value = 7;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeGreaterThan(5, ReverseComparer));

        const string expected = "Expected value to be greater than 5, but found 7.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeGreaterThan_WithComparer_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        const int value = 3;
        IComparer<int>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() => value.Should().BeGreaterThan(5, comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }
}
