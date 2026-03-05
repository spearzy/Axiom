using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Actions.ThrowExactlyAsync;

public sealed class ThrowExactlyAsyncTests
{
    [Fact]
    public async Task ThrowExactlyAsync_ReturnsContinuation_WhenExpectedExactExceptionTypeIsThrown()
    {
        Func<Task> action = static () => Task.FromException(new InvalidOperationException("boom"));

        var baseAssertions = action.Should();
        var continuation = await baseAssertions.ThrowExactlyAsync<InvalidOperationException>();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task ThrowExactlyAsync_Throws_WhenNoExceptionIsThrown()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowExactlyAsync<InvalidOperationException>());

        var expected = $"Expected action to throw exactly {typeof(InvalidOperationException)}, but found <no exception>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task ThrowExactlyAsync_Throws_WhenDerivedExceptionTypeIsThrown()
    {
        Func<ValueTask> action = static () => ValueTask.FromException(new ArgumentNullException("value"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowExactlyAsync<ArgumentException>());

        var expected = $"Expected action to throw exactly {typeof(ArgumentException)}, but found {typeof(ArgumentNullException)}.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task ThrowExactlyAsync_Throws_WithReason_WhenProvided()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowExactlyAsync<InvalidOperationException>("only this exact exception is valid"));

        Assert.Contains("because only this exact exception is valid", ex.Message, StringComparison.Ordinal);
    }
}
