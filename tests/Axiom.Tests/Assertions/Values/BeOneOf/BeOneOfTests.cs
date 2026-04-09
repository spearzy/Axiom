using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeOneOf;

public sealed class BeOneOfTests
{
    [Fact]
    public void BeOneOf_DoesNotThrow_WhenSubjectIsInExpectedSet()
    {
        const int value = 2;

        var ex = Record.Exception(() => value.Should().BeOneOf([3, 2, 1]));

        Assert.Null(ex);
    }

    [Fact]
    public void BeOneOf_Throws_WhenSubjectIsNotInExpectedSet()
    {
        const int value = 4;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeOneOf([3, 1, 2]));

        const string expected = "Expected value to be one of [1, 2, 3], but found 4.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeOneOf_Throws_WithReason_WhenProvided()
    {
        const int value = 9;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeOneOf([1, 2, 3], "status should be in the allowed range"));

        Assert.Contains("because status should be in the allowed range", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeOneOf_ThrowsArgumentNullException_WhenExpectedValuesIsNull()
    {
        const int value = 1;
        IEnumerable<int>? expectedValues = null;

        var ex = Assert.Throws<ArgumentNullException>(() => value.Should().BeOneOf(expectedValues!));

        Assert.Equal("expectedValues", ex.ParamName);
    }

    [Fact]
    public void BeOneOf_ThrowsArgumentException_WhenExpectedValuesIsEmpty()
    {
        const int value = 1;

        var ex = Assert.Throws<ArgumentException>(() => value.Should().BeOneOf(Array.Empty<int>()));

        Assert.Equal("expectedValues", ex.ParamName);
    }

    [Fact]
    public void BeOneOf_DoesNotThrow_WhenComparerMatchesExpectedSet()
    {
        const int value = 3;

        var ex = Record.Exception(() =>
            value.Should().BeOneOf([2, 4, 5], new OddEvenMatchIntComparer()));

        Assert.Null(ex);
    }

    [Fact]
    public void BeOneOf_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        const int value = 3;
        IEqualityComparer<int>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            value.Should().BeOneOf([2, 4, 5], comparer!));

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
