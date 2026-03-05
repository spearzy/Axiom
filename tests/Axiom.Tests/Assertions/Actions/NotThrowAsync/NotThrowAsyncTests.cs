using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Actions.NotThrowAsync;

public sealed class NotThrowAsyncTests
{
    [Fact]
    public async Task NotThrowAsync_ReturnsContinuation_WhenNoExceptionIsThrown()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var baseAssertions = action.Should();
        var continuation = await baseAssertions.NotThrowAsync();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task NotThrowAsync_Throws_WhenAnyExceptionIsThrown()
    {
        Func<Task> action = static () => Task.FromException(new ArgumentException("bad"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().NotThrowAsync());

        const string expected = "Expected action to not throw, but found System.ArgumentException.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task NotThrowAsync_Throws_WithReason_WhenProvided()
    {
        Func<ValueTask> action = static () => ValueTask.FromException(new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().NotThrowAsync("this operation should always be safe"));

        Assert.Contains("because this operation should always be safe", ex.Message, StringComparison.Ordinal);
    }
}
