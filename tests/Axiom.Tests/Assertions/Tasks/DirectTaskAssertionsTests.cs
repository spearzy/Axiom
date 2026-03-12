using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Tasks;

public sealed class DirectTaskAssertionsTests
{
    [Fact]
    public async Task ThrowAsync_Passes_ForFaultedTaskSubject()
    {
        Task task = Task.FromException(new InvalidOperationException("boom"));

        var ex = await Record.ExceptionAsync(async () =>
            await task.Should().ThrowAsync<InvalidOperationException>());

        Assert.Null(ex);
    }

    [Fact]
    public async Task ThrowAsync_Throws_ForCompletedTaskSubject()
    {
        Task task = Task.CompletedTask;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().ThrowAsync<InvalidOperationException>());

        var expected = $"Expected task to throw {typeof(InvalidOperationException)}, but found <no exception>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task ThrowExactlyAsync_Passes_ForFaultedValueTaskSubject()
    {
        ValueTask task = ValueTask.FromException(new ArgumentNullException("value"));

        var ex = await Record.ExceptionAsync(async () =>
            await task.Should().ThrowExactlyAsync<ArgumentNullException>());

        Assert.Null(ex);
    }

    [Fact]
    public async Task ThrowExactlyAsync_Throws_ForDerivedExceptionType_OnDirectTaskSubject()
    {
        Task task = Task.FromException(new ArgumentNullException("value"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().ThrowExactlyAsync<ArgumentException>());

        var expected = $"Expected task to throw exactly {typeof(ArgumentException)}, but found {typeof(ArgumentNullException)}.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task ThrowAsync_Passes_ForFaultedTaskOfTSubject()
    {
        Task<int> task = Task.FromException<int>(new ArgumentException("bad"));

        var ex = await Record.ExceptionAsync(async () =>
            await task.Should().ThrowAsync<ArgumentException>());

        Assert.Null(ex);
    }

    [Fact]
    public async Task NotThrowAsync_Passes_ForSuccessfulTaskOfTSubject()
    {
        Task<int> task = Task.FromResult(123);

        var baseAssertions = task.Should();
        var continuation = await baseAssertions.NotThrowAsync();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task NotThrowAsync_Throws_ForFaultedValueTaskOfTSubject()
    {
        ValueTask<int> task = ValueTask.FromException<int>(new ArgumentException("bad"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().NotThrowAsync());

        Assert.Equal("Expected task to not throw, but found System.ArgumentException.", ex.Message);
    }

    [Fact]
    public async Task CompleteWithin_Passes_ForCompletedTaskSubject()
    {
        Task task = Task.CompletedTask;

        var baseAssertions = task.Should();
        var continuation = await baseAssertions.CompleteWithin(TimeSpan.FromMilliseconds(10));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task CompleteWithin_Throws_ForTaskSubject_WhenNotCompletedInTime()
    {
        var completion = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        Task task = completion.Task;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().CompleteWithin(TimeSpan.FromMilliseconds(10)));

        completion.TrySetResult(null);

        Assert.Equal("Expected task to complete within 00:00:00.0100000, but found <not completed within timeout>.", ex.Message);
    }

    [Fact]
    public async Task NotCompleteWithin_Passes_ForTaskSubject_WhenNotCompletedInTime()
    {
        var completion = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        Task task = completion.Task;

        var baseAssertions = task.Should();
        var continuation = await baseAssertions.NotCompleteWithin(TimeSpan.FromMilliseconds(10));

        completion.TrySetResult(null);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task NotCompleteWithin_Throws_ForCompletedValueTaskSubject()
    {
        ValueTask task = ValueTask.CompletedTask;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().NotCompleteWithin(TimeSpan.FromMilliseconds(10)));

        Assert.Equal("Expected task to not complete within 00:00:00.0100000, but found <completed within timeout>.", ex.Message);
    }

    [Fact]
    public async Task CompleteWithin_Passes_ForCompletedValueTaskOfTSubject()
    {
        ValueTask<int> task = ValueTask.FromResult(5);

        var baseAssertions = task.Should();
        var continuation = await baseAssertions.CompleteWithin(TimeSpan.FromMilliseconds(10));

        Assert.Same(baseAssertions, continuation.And);
    }
}
