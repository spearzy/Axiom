using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotSatisfy;

public sealed class NotSatisfyTests
{
    [Fact]
    public void NotSatisfy_DoesNotThrow_WhenPredicateReturnsFalse()
    {
        const int value = 42;

        var ex = Record.Exception(() => value.Should().NotSatisfy(static x => x < 40));

        Assert.Null(ex);
    }

    [Fact]
    public void NotSatisfy_Throws_WhenPredicateReturnsTrue()
    {
        const int value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotSatisfy(static x => x > 40));

        const string expected = "Expected value to not satisfy predicate, but found 42.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotSatisfy_Throws_WithReason_WhenProvided()
    {
        const int value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotSatisfy(static x => x > 40, "value should avoid the blocked rule"));

        Assert.Contains("because value should avoid the blocked rule", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotSatisfy_ThrowsArgumentNullException_WhenPredicateIsNull()
    {
        const int value = 42;
        Func<int, bool>? predicate = null;

        var ex = Assert.Throws<ArgumentNullException>(() => value.Should().NotSatisfy(predicate!));

        Assert.Equal("predicate", ex.ParamName);
    }
}
