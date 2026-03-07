using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Actions.Throw;

public sealed class ThrowTests
{
    [Fact]
    public void Throw_ReturnsAssertions_AndPointsBackToSameAssertions()
    {
        Action action = static () => throw new InvalidOperationException("boom");

        var baseAssertions = action.Should();
        var continuation = baseAssertions.Throw<InvalidOperationException>();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void Throw_Passes_WhenExpectedExceptionTypeIsThrown()
    {
        Action action = static () => throw new InvalidOperationException("boom");

        var ex = Record.Exception(() => action.Should().Throw<InvalidOperationException>());

        Assert.Null(ex);
    }

    [Fact]
    public void Throw_Passes_WhenDerivedExceptionTypeIsThrown()
    {
        Action action = static () => throw new ArgumentNullException("value");

        var ex = Record.Exception(() => action.Should().Throw<ArgumentException>());

        Assert.Null(ex);
    }

    [Fact]
    public void Throw_Throws_WhenNoExceptionIsThrown()
    {
        Action action = static () => { };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>());

        var expected = $"Expected action to throw {typeof(InvalidOperationException)}, but found <no exception>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Throw_Throws_WhenDifferentExceptionTypeIsThrown()
    {
        Action action = static () => throw new ArgumentException("bad");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>());

        var expected = $"Expected action to throw {typeof(InvalidOperationException)}, but found {typeof(ArgumentException)}.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Throw_Throws_WithReason_WhenProvided()
    {
        Action action = static () => { };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>("this code path must fail fast"));

        Assert.Contains("because this code path must fail fast", ex.Message);
    }
}
