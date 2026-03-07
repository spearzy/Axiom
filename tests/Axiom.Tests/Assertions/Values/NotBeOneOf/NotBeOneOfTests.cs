using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotBeOneOf;

public sealed class NotBeOneOfTests
{
    [Fact]
    public void NotBeOneOf_DoesNotThrow_WhenSubjectIsNotInUnexpectedSet()
    {
        const int value = 9;

        var ex = Record.Exception(() => value.Should().NotBeOneOf([1, 2, 3]));

        Assert.Null(ex);
    }

    [Fact]
    public void NotBeOneOf_Throws_WhenSubjectIsInUnexpectedSet()
    {
        const int value = 2;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeOneOf([3, 1, 2]));

        const string expected = "Expected value to not be one of [1, 2, 3], but found 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBeOneOf_Throws_WithReason_WhenProvided()
    {
        const int value = 2;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotBeOneOf([1, 2, 3], "current mode must avoid blocked values"));

        Assert.Contains("because current mode must avoid blocked values", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotBeOneOf_ThrowsArgumentNullException_WhenUnexpectedValuesIsNull()
    {
        const int value = 1;
        IEnumerable<int>? unexpectedValues = null;

        var ex = Assert.Throws<ArgumentNullException>(() => value.Should().NotBeOneOf(unexpectedValues!));

        Assert.Equal("unexpectedValues", ex.ParamName);
    }

    [Fact]
    public void NotBeOneOf_ThrowsArgumentException_WhenUnexpectedValuesIsEmpty()
    {
        const int value = 1;

        var ex = Assert.Throws<ArgumentException>(() => value.Should().NotBeOneOf(Array.Empty<int>()));

        Assert.Equal("unexpectedValues", ex.ParamName);
    }
}
