using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Actions.NotCompleteWithin;

public sealed class NotCompleteWithinTests
{
    [Fact]
    public async Task NotCompleteWithin_ReturnsContinuation_WhenTaskDoesNotCompleteInTime()
    {
        var completion = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        Func<Task> action = () => completion.Task;

        var baseAssertions = action.Should();
        var continuation = await baseAssertions.NotCompleteWithin(TimeSpan.FromMilliseconds(10));

        completion.TrySetResult(null);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task NotCompleteWithin_ReturnsContinuation_WhenValueTaskDoesNotCompleteInTime()
    {
        var completion = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        Func<ValueTask> action = () => new ValueTask(completion.Task);

        var baseAssertions = action.Should();
        var continuation = await baseAssertions.NotCompleteWithin(TimeSpan.FromMilliseconds(10));

        completion.TrySetResult(null);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task NotCompleteWithin_Throws_WhenActionCompletesWithinTimeout()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().NotCompleteWithin(TimeSpan.FromMilliseconds(10)));

        const string expected = "Expected action to not complete within 00:00:00.0100000, but found <completed within timeout>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task NotCompleteWithin_Throws_WhenActionFaultsWithinTimeout()
    {
        Func<Task> action = static () => Task.FromException(new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().NotCompleteWithin(TimeSpan.FromMilliseconds(10)));

        const string expected = "Expected action to not complete within 00:00:00.0100000, but found <completed within timeout>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task NotCompleteWithin_Throws_WithReason_WhenProvided()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().NotCompleteWithin(TimeSpan.FromMilliseconds(10), "this operation must remain pending"));

        Assert.Contains("because this operation must remain pending", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task NotCompleteWithin_Throws_WhenTimeoutIsNegative()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await action.Should().NotCompleteWithin(TimeSpan.FromMilliseconds(-1)));

        Assert.Equal("timeout", ex.ParamName);
    }
}
