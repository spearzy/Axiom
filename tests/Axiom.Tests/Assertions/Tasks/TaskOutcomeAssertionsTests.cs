using System.Threading.Tasks.Sources;

namespace Axiom.Tests.Assertions.Tasks;

public sealed class TaskOutcomeAssertionsTests
{
    [Fact]
    public async Task Succeed_Passes_ForCompletedTaskSubject()
    {
        Task task = Task.CompletedTask;

        var baseAssertions = task.Should();
        var continuation = await baseAssertions.Succeed();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task Succeed_Throws_ForFaultedTaskSubject()
    {
        Task task = Task.FromException(new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().Succeed());

        Assert.Equal("Expected task to succeed, but found System.InvalidOperationException.", ex.Message);
    }

    [Fact]
    public async Task SucceedWithin_ReturnsResult_ForCompletedTaskOfTSubject()
    {
        Task<int> task = Task.FromResult(42);

        var continuation = await task.Should().SucceedWithin(TimeSpan.FromMilliseconds(10));

        Assert.Equal(42, continuation.WhoseResult);
    }

    [Fact]
    public async Task BeCanceled_Passes_ForCanceledTaskSubject()
    {
        Task task = Task.FromCanceled(new CancellationToken(canceled: true));

        var baseAssertions = task.Should();
        var continuation = await baseAssertions.BeCanceled();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public async Task BeFaultedWith_Passes_And_ExposesThrown_ForFaultedTaskSubject()
    {
        Task task = Task.FromException(new ArgumentNullException("value"));

        var continuation = await task.Should().BeFaultedWith<ArgumentNullException>();

        Assert.Equal("value", continuation.Thrown.ParamName);
    }

    [Fact]
    public async Task SucceedWithin_Throws_ForTaskOfT_WhenNotCompletedInTime()
    {
        var completion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<int> task = completion.Task;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await task.Should().SucceedWithin(TimeSpan.FromMilliseconds(10)));

        completion.TrySetResult(5);

        Assert.Equal("Expected task to succeed within 00:00:00.0100000, but found <not completed within timeout>.", ex.Message);
    }

    [Fact]
    public async Task Succeed_ForDirectValueTaskOfT_FromSingleUseSource_CachesResultWithoutDoubleConsumption()
    {
        var source = new SingleUseValueTaskSource<int>(42);
        ValueTask<int> task = new(source, source.Version);

        var continuation = await task.Should().Succeed();

        Assert.Equal(42, continuation.WhoseResult);
        Assert.Equal(42, continuation.WhoseResult);
        Assert.Equal(1, source.GetResultCalls);
    }

    [Fact]
    public async Task WhoseResult_ThrowsHelpfulMessage_WhenSucceedFailed_InsideBatch()
    {
        Task<int> task = Task.FromException<int>(new InvalidOperationException("boom"));

        var batch = new Axiom.Core.Batch();
        var continuation = await task.Should().Succeed();

        var ex = Assert.Throws<InvalidOperationException>(() => _ = continuation.WhoseResult);
        var batchEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());

        Assert.Equal(
            "WhoseResult is unavailable because Succeed failed with error: Expected task to succeed, but found System.InvalidOperationException.",
            ex.Message);
        Assert.Contains("Expected task to succeed, but found System.InvalidOperationException.", batchEx.Message);
    }

    private sealed class SingleUseValueTaskSource<TResult> : IValueTaskSource<TResult>
    {
        private ManualResetValueTaskSourceCore<TResult> _core;
        private int _getResultCalls;

        public SingleUseValueTaskSource(TResult result)
        {
            _core = new ManualResetValueTaskSourceCore<TResult>
            {
                RunContinuationsAsynchronously = true
            };
            _core.SetResult(result);
        }

        public int GetResultCalls => _getResultCalls;

        public short Version => _core.Version;

        public TResult GetResult(short token)
        {
            if (Interlocked.Increment(ref _getResultCalls) > 1)
            {
                throw new InvalidOperationException("ValueTask result was consumed more than once.");
            }

            return _core.GetResult(token);
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return _core.GetStatus(token);
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            _core.OnCompleted(continuation, state, token, flags);
        }
    }
}
