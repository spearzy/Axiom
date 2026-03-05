using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Actions.CompleteWithin;

public sealed class CompleteWithinTests
{
    [Fact]
    public async Task CompleteWithin_ReturnsContinuation_WhenTaskCompletesWithinTimeout()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var baseAssertions = action.Should();
        var continuation = await baseAssertions.CompleteWithin(TimeSpan.Zero);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task CompleteWithin_ReturnsContinuation_WhenValueTaskCompletesWithinTimeout()
    {
        Func<ValueTask> action = static () => ValueTask.CompletedTask;

        var baseAssertions = action.Should();
        var continuation = await baseAssertions.CompleteWithin(TimeSpan.FromMilliseconds(10));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task CompleteWithin_Passes_WhenActionFaultsWithinTimeout()
    {
        Func<Task> action = static () => Task.FromException(new InvalidOperationException("boom"));

        var ex = await Record.ExceptionAsync(async () =>
            await action.Should().CompleteWithin(TimeSpan.FromMilliseconds(10)));

        Assert.Null(ex);
    }

    [Fact]
    public async Task CompleteWithin_Throws_WhenActionDoesNotCompleteWithinTimeout()
    {
        var completion = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        Func<Task> action = () => completion.Task;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().CompleteWithin(TimeSpan.FromMilliseconds(10)));

        completion.TrySetResult(null);

        const string expected = "Expected action to complete within 00:00:00.0100000, but found <not completed within timeout>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task CompleteWithin_Throws_WithReason_WhenProvided()
    {
        var completion = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        Func<Task> action = () => completion.Task;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().CompleteWithin(TimeSpan.FromMilliseconds(10), "this operation should finish quickly"));

        completion.TrySetResult(null);

        Assert.Contains("because this operation should finish quickly", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CompleteWithin_Throws_WhenTimeoutIsNegative()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await action.Should().CompleteWithin(TimeSpan.FromMilliseconds(-1)));

        Assert.Equal("timeout", ex.ParamName);
    }
}
