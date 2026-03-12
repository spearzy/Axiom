using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Actions.NotThrow;

public sealed class NotThrowTests
{
    [Fact]
    public void NotThrow_ReturnsContinuation_WhenNoExceptionIsThrown()
    {
        Action action = static () => { };

        var baseAssertions = action.Should();
        var continuation = baseAssertions.NotThrow();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotThrow_Throws_WhenAnyExceptionIsThrown()
    {
        Action action = static () => throw new ArgumentException("bad");

        var ex = Assert.Throws<InvalidOperationException>(() => action.Should().NotThrow());

        const string expected = "Expected action to not throw, but found System.ArgumentException.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotThrow_Throws_WithReason_WhenProvided()
    {
        Action action = static () => throw new InvalidOperationException("boom");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().NotThrow("this operation should always be safe"));

        Assert.Contains("because this operation should always be safe", ex.Message, StringComparison.Ordinal);
    }
}
