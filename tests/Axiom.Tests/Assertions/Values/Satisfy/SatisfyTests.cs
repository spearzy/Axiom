using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.Satisfy;

public sealed class SatisfyTests
{
    [Fact]
    public void Satisfy_DoesNotThrow_WhenPredicateReturnsTrue()
    {
        const int value = 42;

        var ex = Record.Exception(() => value.Should().Satisfy(static x => x > 40));

        Assert.Null(ex);
    }

    [Fact]
    public void Satisfy_Throws_WhenPredicateReturnsFalse()
    {
        const int value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Satisfy(static x => x < 40));

        const string expected = "Expected value to satisfy predicate `x => x < 40`, but found 42.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Satisfy_Throws_WithReason_WhenProvided()
    {
        const int value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().Satisfy(static x => x < 40, "input should match lower guard"));

        Assert.Contains("because input should match lower guard", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Satisfy_ThrowsArgumentNullException_WhenPredicateIsNull()
    {
        const int value = 42;
        Func<int, bool>? predicate = null;

        var ex = Assert.Throws<ArgumentNullException>(() => value.Should().Satisfy(predicate!));

        Assert.Equal("predicate", ex.ParamName);
    }

    [Fact]
    public void Satisfy_UsesPredicateVariableName_InFailureMessage()
    {
        const int value = 41;
        Func<int, bool> isEven = x => x % 2 == 0;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Satisfy(isEven));

        Assert.Contains("predicate `isEven`", ex.Message, StringComparison.Ordinal);
    }
}
