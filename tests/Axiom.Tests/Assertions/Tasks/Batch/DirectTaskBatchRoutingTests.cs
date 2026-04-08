namespace Axiom.Tests.Assertions.Tasks.Batch;

public sealed class DirectTaskBatchRoutingTests
{
    [Fact]
    public async Task ThrowAsync_InsideBatch_DoesNotThrowAtAssertionCallSite_ForTaskSubject()
    {
        Task task = Task.CompletedTask;

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await task.Should().ThrowAsync<InvalidOperationException>());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public async Task NotThrowAsync_InsideBatch_DoesNotThrowAtAssertionCallSite_ForValueTaskSubject()
    {
        ValueTask task = ValueTask.FromException(new ArgumentException("bad"));

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await task.Should().NotThrowAsync());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public async Task Batch_Dispose_ThrowsCombinedFailures_ForDirectTaskSubjects()
    {
        Task noThrowTask = Task.CompletedTask;
        ValueTask fastTask = ValueTask.CompletedTask;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            using var batch = new Axiom.Core.Batch("direct task subjects");
            await noThrowTask.Should().ThrowAsync<InvalidOperationException>();
            await fastTask.Should().NotCompleteWithin(TimeSpan.FromMilliseconds(10));
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'direct task subjects' failed with 2 assertion failure(s):", message);
        Assert.Contains($"1) Expected noThrowTask to throw {typeof(InvalidOperationException)}, but found <no exception>.", message);
        Assert.Contains("2) Expected fastTask to not complete within 00:00:00.0100000, but found <completed within timeout>.", message);
    }

    [Fact]
    public async Task TaskOutcomeAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        Task completedTask = Task.CompletedTask;
        Task<int> successfulTask = Task.FromResult(42);
        Task<int> faultedTask = Task.FromException<int>(new InvalidOperationException("boom"));
        Task canceledTask = Task.FromCanceled(new CancellationToken(canceled: true));

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
        {
            await completedTask.Should().ThrowExactlyAsync<InvalidOperationException>();
            await faultedTask.Should().NotThrowAsync();
            await canceledTask.Should().Succeed();
            await successfulTask.Should().BeCanceled();
            await successfulTask.Should().BeFaultedWith<ArgumentException>();
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        var message = disposeEx.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("to throw exactly", message);
        Assert.Contains("to not throw", message);
        Assert.Contains("to succeed", message);
        Assert.Contains("to be canceled", message);
        Assert.Contains("to be faulted with", message);
    }

    [Fact]
    public async Task TaskCompletionAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        var slowCompletion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var slowFault = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<int> slowTask = slowCompletion.Task;
        Task<int> slowFaultTask = slowFault.Task;
        Task<int> fastTask = Task.FromResult(42);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
        {
            await slowTask.Should().CompleteWithin(TimeSpan.FromMilliseconds(10));
            await fastTask.Should().NotCompleteWithin(TimeSpan.FromMilliseconds(10));
            await slowTask.Should().SucceedWithin(TimeSpan.FromMilliseconds(10));
            await fastTask.Should().BeCanceledWithin(TimeSpan.FromMilliseconds(10));
            await slowFaultTask.Should().BeFaultedWithWithin<InvalidOperationException>(TimeSpan.FromMilliseconds(10));
        });

        slowCompletion.TrySetResult(42);
        slowFault.TrySetResult(0);

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        var message = disposeEx.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("to complete within 00:00:00.0100000", message);
        Assert.Contains("to not complete within 00:00:00.0100000", message);
        Assert.Contains("to succeed within 00:00:00.0100000", message);
        Assert.Contains("to be canceled within 00:00:00.0100000", message);
        Assert.Contains("to be faulted with", message);
        Assert.Contains("within 00:00:00.0100000", message);
    }

    [Fact]
    public async Task ContinuationMembers_InsideBatch_ThrowExplicitMessages_WhenBaseAssertionFailed()
    {
        Task<int> faultedTask = Task.FromException<int>(new InvalidOperationException("boom"));
        Task<int> successfulTask = Task.FromResult(42);
        var batch = new Axiom.Core.Batch();

        var successContinuation = await faultedTask.Should().Succeed();
        var faultContinuation = await successfulTask.Should().BeFaultedWith<ArgumentException>();

        var whoseResultEx = Assert.Throws<InvalidOperationException>(() => _ = successContinuation.WhoseResult);
        var thrownEx = Assert.Throws<InvalidOperationException>(() => _ = faultContinuation.Thrown);

        Assert.Equal(
            "WhoseResult is unavailable because Succeed failed with error: Expected faultedTask to succeed, but found System.InvalidOperationException.",
            whoseResultEx.Message);
        Assert.Equal(
            "Thrown is unavailable because BeFaultedWith assertion failed with error: Expected successfulTask to be faulted with System.ArgumentException, but found <completed successfully>.",
            thrownEx.Message);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        var message = disposeEx.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Expected faultedTask to succeed, but found System.InvalidOperationException.", message);
        Assert.Contains("Expected successfulTask to be faulted with System.ArgumentException, but found <completed successfully>.", message);
    }

    [Fact]
    public async Task Batch_Dispose_PreservesCallOrder_ForTaskOutcomeFailures()
    {
        Task completedTask = Task.CompletedTask;
        Task<int> successfulTask = Task.FromResult(42);
        Task<int> faultedTask = Task.FromException<int>(new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            using var batch = new Axiom.Core.Batch("task outcomes");
            await completedTask.Should().ThrowAsync<InvalidOperationException>();
            await successfulTask.Should().BeFaultedWith<ArgumentException>();
            await faultedTask.Should().Succeed();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        var throwIndex = message.IndexOf("Expected completedTask to throw", StringComparison.Ordinal);
        var faultIndex = message.IndexOf("Expected successfulTask to be faulted with", StringComparison.Ordinal);
        var succeedIndex = message.IndexOf("Expected faultedTask to succeed", StringComparison.Ordinal);

        Assert.True(throwIndex >= 0, message);
        Assert.True(faultIndex > throwIndex, message);
        Assert.True(succeedIndex > faultIndex, message);
    }
}
