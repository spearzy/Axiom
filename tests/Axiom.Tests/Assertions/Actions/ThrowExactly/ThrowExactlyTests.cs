using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Actions.ThrowExactly;

public sealed class ThrowExactlyTests
{
    [Fact]
    public void ThrowExactly_ReturnsContinuation_WhenExpectedExactExceptionTypeIsThrown()
    {
        Action action = static () => throw new InvalidOperationException("boom");

        var baseAssertions = action.Should();
        var continuation = baseAssertions.ThrowExactly<InvalidOperationException>();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ThrowExactly_Throws_WhenNoExceptionIsThrown()
    {
        Action action = static () => { };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().ThrowExactly<InvalidOperationException>());

        var expected = $"Expected action to throw exactly {typeof(InvalidOperationException)}, but found <no exception>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ThrowExactly_Throws_WhenDerivedExceptionTypeIsThrown()
    {
        Action action = static () => throw new ArgumentNullException("value");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().ThrowExactly<ArgumentException>());

        var expected = $"Expected action to throw exactly {typeof(ArgumentException)}, but found {typeof(ArgumentNullException)}.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ThrowExactly_Throws_WithReason_WhenProvided()
    {
        Action action = static () => { };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().ThrowExactly<InvalidOperationException>("only this exact exception is valid"));

        Assert.Contains("because only this exact exception is valid", ex.Message, StringComparison.Ordinal);
    }
}
