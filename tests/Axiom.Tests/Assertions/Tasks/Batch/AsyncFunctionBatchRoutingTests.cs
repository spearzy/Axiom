namespace Axiom.Tests.Assertions.Tasks.Batch;

public sealed class AsyncFunctionBatchRoutingTests
{
    [Fact]
    public async Task AsyncFunctionOutcomeAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        Func<Task<int>> noThrow = static () => Task.FromResult(42);
        Func<Task<int>> faulted = static () => Task.FromException<int>(new InvalidOperationException("boom"));
        Func<Task<int>> canceled = static () => Task.FromCanceled<int>(new CancellationToken(canceled: true));

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
        {
            await noThrow.Should().ThrowAsync<InvalidOperationException>();
            await noThrow.Should().ThrowExactlyAsync<InvalidOperationException>();
            await faulted.Should().NotThrowAsync();
            await faulted.Should().Succeed();
            await noThrow.Should().BeCanceled();
            await noThrow.Should().BeFaultedWith<ArgumentException>();
            await canceled.Should().Succeed();
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        var message = disposeEx.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Expected noThrow to throw System.InvalidOperationException", message);
        Assert.Contains("Expected noThrow to throw exactly System.InvalidOperationException", message);
        Assert.Contains("Expected faulted to not throw, but found System.InvalidOperationException.", message);
        Assert.Contains("Expected faulted to succeed, but found System.InvalidOperationException.", message);
        Assert.Contains("Expected noThrow to be canceled, but found <completed successfully>.", message);
        Assert.Contains("Expected noThrow to be faulted with System.ArgumentException, but found <completed successfully>.", message);
        Assert.Contains("Expected canceled to succeed, but found <canceled>.", message);
    }

    [Fact]
    public async Task AsyncFunctionCompletionAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        var slowCompletion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var slowCancel = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var slowFault = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

        Func<Task<int>> slow = () => slowCompletion.Task;
        Func<Task<int>> fast = static () => Task.FromResult(42);
        Func<Task<int>> slowCanceled = () => slowCancel.Task;
        Func<Task<int>> slowFaulted = () => slowFault.Task;

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
        {
            await slow.Should().CompleteWithin(TimeSpan.FromMilliseconds(10));
            await fast.Should().NotCompleteWithin(TimeSpan.FromMilliseconds(10));
            await slow.Should().SucceedWithin(TimeSpan.FromMilliseconds(10));
            await fast.Should().BeCanceledWithin(TimeSpan.FromMilliseconds(10));
            await slowFaulted.Should().BeFaultedWithWithin<InvalidOperationException>(TimeSpan.FromMilliseconds(10));
            await slowCanceled.Should().BeCanceledWithin(TimeSpan.FromMilliseconds(10));
        });

        slowCompletion.TrySetResult(42);
        slowCancel.TrySetCanceled();
        slowFault.TrySetResult(0);

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        var message = disposeEx.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Expected slow to complete within 00:00:00.0100000, but found <not completed within timeout>.", message);
        Assert.Contains("Expected fast to not complete within 00:00:00.0100000, but found <completed within timeout>.", message);
        Assert.Contains("Expected slow to succeed within 00:00:00.0100000, but found <not completed within timeout>.", message);
        Assert.Contains("Expected fast to be canceled within 00:00:00.0100000, but found <completed successfully>.", message);
        Assert.Contains("Expected slowFaulted to be faulted with", message);
        Assert.Contains("<not completed within timeout>", message);
        Assert.Contains("Expected slowCanceled to be canceled within 00:00:00.0100000, but found <not completed within timeout>.", message);
    }

    [Fact]
    public async Task ContinuationMembers_InsideBatch_ThrowExplicitMessages_WhenBaseAssertionFailed()
    {
        Func<Task<int>> noThrow = static () => Task.FromResult(42);
        Func<Task<int>> faulted = static () => Task.FromException<int>(new InvalidOperationException("boom"));
        var batch = new Axiom.Core.Batch();

        var thrownContinuation = await noThrow.Should().ThrowAsync<ArgumentException>();
        var successContinuation = await faulted.Should().Succeed();

        var thrownEx = Assert.Throws<InvalidOperationException>(() => _ = thrownContinuation.Thrown);
        var whoseResultEx = Assert.Throws<InvalidOperationException>(() => _ = successContinuation.WhoseResult);

        Assert.Equal(
            "Thrown is unavailable because Throw assertion failed with error: Expected noThrow to throw System.ArgumentException, but found <no exception>.",
            thrownEx.Message);
        Assert.Equal(
            "WhoseResult is unavailable because Succeed failed with error: Expected faulted to succeed, but found System.InvalidOperationException.",
            whoseResultEx.Message);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        var message = disposeEx.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Expected noThrow to throw System.ArgumentException, but found <no exception>.", message);
        Assert.Contains("Expected faulted to succeed, but found System.InvalidOperationException.", message);
    }

    [Fact]
    public async Task Batch_Dispose_PreservesCallOrder_ForAsyncFunctionFailures()
    {
        Func<Task<int>> noThrow = static () => Task.FromResult(42);
        Func<Task<int>> fast = static () => Task.FromResult(1);
        Func<Task<int>> faulted = static () => Task.FromException<int>(new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            using var batch = new Axiom.Core.Batch("async functions");
            await noThrow.Should().ThrowAsync<InvalidOperationException>();
            await fast.Should().BeFaultedWith<ArgumentException>();
            await faulted.Should().NotThrowAsync();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        var throwIndex = message.IndexOf("Expected noThrow to throw", StringComparison.Ordinal);
        var faultIndex = message.IndexOf("Expected fast to be faulted with", StringComparison.Ordinal);
        var notThrowIndex = message.IndexOf("Expected faulted to not throw", StringComparison.Ordinal);

        Assert.True(throwIndex >= 0, message);
        Assert.True(faultIndex > throwIndex, message);
        Assert.True(notThrowIndex > faultIndex, message);
    }
}
