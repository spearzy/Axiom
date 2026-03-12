using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Actions.ThrowAsync;

public sealed class ThrowAsyncTests
{
    [Fact]
    public async Task ThrowAsync_ReturnsAssertions_AndPointsBackToSameAssertions()
    {
        Func<Task> action = static () => Task.FromException(new InvalidOperationException("boom"));

        var baseAssertions = action.Should();
        var continuation = await baseAssertions.ThrowAsync<InvalidOperationException>();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task ThrowAsync_Passes_WhenExpectedExceptionTypeIsThrown()
    {
        Func<Task> action = static () => Task.FromException(new InvalidOperationException("boom"));

        var ex = await Record.ExceptionAsync(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>());

        Assert.Null(ex);
    }

    [Fact]
    public async Task ThrowAsync_Thrown_ReturnsTypedException_WhenExpectedExceptionTypeIsThrown()
    {
        Func<Task> action = static () => Task.FromException(new InvalidOperationException("boom"));

        var continuation = await action.Should().ThrowAsync<InvalidOperationException>();
        var thrown = continuation.Thrown;

        Assert.IsType<InvalidOperationException>(thrown);
        Assert.Equal("boom", thrown.Message);
    }

    [Fact]
    public async Task ThrowAsync_Passes_WhenDerivedExceptionTypeIsThrown()
    {
        Func<ValueTask> action = static () => ValueTask.FromException(new ArgumentNullException("value"));

        var ex = await Record.ExceptionAsync(async () =>
            await action.Should().ThrowAsync<ArgumentException>());

        Assert.Null(ex);
    }

    [Fact]
    public async Task ThrowAsync_Throws_WhenNoExceptionIsThrown()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>());

        var expected = $"Expected action to throw {typeof(InvalidOperationException)}, but found <no exception>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task ThrowAsync_Throws_WhenDifferentExceptionTypeIsThrown()
    {
        Func<Task> action = static () => Task.FromException(new ArgumentException("bad"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>());

        var expected = $"Expected action to throw {typeof(InvalidOperationException)}, but found {typeof(ArgumentException)}.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task ThrowAsync_Throws_WithReason_WhenProvided()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>("this code path must fail fast"));

        Assert.Contains("because this code path must fail fast", ex.Message);
    }

    [Fact]
    public async Task ThrowAsync_Thrown_ThrowsExplicitMessage_WhenThrowAsyncFailedInsideBatch()
    {
        Func<Task> action = static () => Task.CompletedTask;
        var batch = new Axiom.Core.Batch();

        var continuation = await action.Should().ThrowAsync<InvalidOperationException>();
        var ex = Assert.Throws<InvalidOperationException>(() => _ = continuation.Thrown);

        var failureMessage = $"Expected action to throw {typeof(InvalidOperationException)}, but found <no exception>.";
        var expected = $"Thrown is unavailable because Throw assertion failed with error: {failureMessage}";
        Assert.Equal(expected, ex.Message);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }
}
