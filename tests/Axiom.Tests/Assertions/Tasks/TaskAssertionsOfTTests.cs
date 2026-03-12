namespace Axiom.Tests.Assertions.Tasks;

public sealed class TaskAssertionsOfTTests
{
    [Fact]
    public async Task ThrowExactlyAsync_Passes_ForExactExceptionType_OnTaskOfT()
    {
        Task<int> task = Task.FromException<int>(new ArgumentNullException("value"));

        var ex = await Record.ExceptionAsync(async () =>
            await task.Should().ThrowExactlyAsync<ArgumentNullException>());

        Assert.Null(ex);
    }

    [Fact]
    public async Task ThrowExactlyAsync_Throws_ForBaseExceptionType_OnTaskOfT()
    {
        Task<int> task = Task.FromException<int>(new ArgumentNullException("value"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().ThrowExactlyAsync<ArgumentException>());

        var expected = $"Expected task to throw exactly {typeof(ArgumentException)}, but found {typeof(ArgumentNullException)}.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task CompleteWithin_Throws_ForTaskOfT_WhenNotCompletedInTime()
    {
        var completion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<int> task = completion.Task;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().CompleteWithin(TimeSpan.FromMilliseconds(10)));

        completion.TrySetResult(1);

        Assert.Equal("Expected task to complete within 00:00:00.0100000, but found <not completed within timeout>.", ex.Message);
    }

    [Fact]
    public async Task NotCompleteWithin_Throws_ForCompletedTaskOfT()
    {
        Task<int> task = Task.FromResult(123);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().NotCompleteWithin(TimeSpan.FromMilliseconds(10)));

        Assert.Equal("Expected task to not complete within 00:00:00.0100000, but found <completed within timeout>.", ex.Message);
    }

    [Fact]
    public async Task Succeed_Throws_ForCanceledTaskOfT()
    {
        Task<int> task = Task.FromCanceled<int>(new CancellationToken(canceled: true));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().Succeed());

        Assert.Equal("Expected task to succeed, but found <canceled>.", ex.Message);
    }

    [Fact]
    public async Task SucceedWithin_Throws_ForFaultedTaskOfT()
    {
        Task<int> task = Task.FromException<int>(new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().SucceedWithin(TimeSpan.FromMilliseconds(10)));

        Assert.Equal($"Expected task to succeed within 00:00:00.0100000, but found {typeof(InvalidOperationException)}.", ex.Message);
    }

    [Fact]
    public async Task BeCanceled_Throws_ForSuccessfulTaskOfT()
    {
        Task<int> task = Task.FromResult(42);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().BeCanceled());

        Assert.Equal("Expected task to be canceled, but found <completed successfully>.", ex.Message);
    }

    [Fact]
    public async Task BeCanceledWithin_Throws_ForTaskOfT_WhenNotCompletedInTime()
    {
        var completion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<int> task = completion.Task;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().BeCanceledWithin(TimeSpan.FromMilliseconds(10)));

        completion.TrySetResult(7);

        Assert.Equal("Expected task to be canceled within 00:00:00.0100000, but found <not completed within timeout>.", ex.Message);
    }

    [Fact]
    public async Task BeCanceledWithin_Throws_ForTaskOfT_WhenCompletedSuccessfullyWithinTimeout()
    {
        Task<int> task = Task.FromResult(7);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().BeCanceledWithin(TimeSpan.FromMilliseconds(10)));

        Assert.Equal("Expected task to be canceled within 00:00:00.0100000, but found <completed successfully>.", ex.Message);
    }

    [Fact]
    public async Task BeCanceledWithin_Passes_ForCanceledTaskOfT()
    {
        Task<int> task = Task.FromCanceled<int>(new CancellationToken(canceled: true));

        var baseAssertions = task.Should();
        var continuation = await baseAssertions.BeCanceledWithin(TimeSpan.FromMilliseconds(10));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task BeFaultedWith_Throws_ForCompletedTaskOfT()
    {
        Task<int> task = Task.FromResult(123);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().BeFaultedWith<ArgumentException>());

        Assert.Equal("Expected task to be faulted with System.ArgumentException, but found <completed successfully>.", ex.Message);
    }

    [Fact]
    public async Task BeFaultedWithWithin_Throws_ForTaskOfT_WhenNotCompletedInTime()
    {
        var completion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<int> task = completion.Task;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().BeFaultedWithWithin<InvalidOperationException>(TimeSpan.FromMilliseconds(10)));

        completion.TrySetResult(1);

        Assert.Contains("Expected task to be faulted with", ex.Message, StringComparison.Ordinal);
        Assert.Contains(typeof(InvalidOperationException).ToString(), ex.Message, StringComparison.Ordinal);
        Assert.Contains("within 00:00:00.0100000", ex.Message, StringComparison.Ordinal);
        Assert.Contains("<not completed within timeout>", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task BeFaultedWithWithin_Passes_ForFaultedTaskOfT()
    {
        Task<int> task = Task.FromException<int>(new InvalidOperationException("boom"));

        var continuation = await task.Should().BeFaultedWithWithin<InvalidOperationException>(TimeSpan.FromMilliseconds(10));

        Assert.Equal("boom", continuation.Thrown.Message);
    }
}
