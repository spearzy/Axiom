using Axiom.Core.Failures;

namespace Axiom.Tests.Assertions.AsyncStreams.Batch;

public sealed class AsyncEnumerableBatchRoutingTests
{
    [Fact]
    public async Task BeEmptyAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().BeEmptyAsync());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public async Task Batch_Dispose_ThrowsCombinedFailures_ForAsyncStreams()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            using var batch = new Axiom.Core.Batch("async streams");
            await values.Should().BeEmptyAsync();
        });

        Assert.Equal(
            "Batch 'async streams' failed with 1 assertion failure(s):\n1) Expected values to be empty, but found 1."
                .Replace("\n", Environment.NewLine, StringComparison.Ordinal),
            ex.Message);
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_InsideBatch_DoesNotThrowAtAssertionCallSite_WhenStreamEndsEarly()
    {
        var values = CreateAsyncSequence(1, 2);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().SatisfyRespectivelyAsync(
                item => item.Should().BeGreaterThan(0),
                item => item.Should().BeGreaterThan(0),
                item => item.Should().BeGreaterThan(0)));

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected values to satisfy assertions respectively (same order and count)", disposeEx.Message);
        Assert.Contains("async stream had fewer items than assertions (expected 3, found 2)", disposeEx.Message);
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_InsideBatch_WrapsInnerItemFailureWithIndexAndBecause()
    {
        AssertionFailureCapture.ResetProbe();

        var values = CreateAsyncSequence(10, -1, 30);
        var thirdAssertionReached = false;

        try
        {
            using var batch = new Axiom.Core.Batch("async streams");
            var callEx = await Record.ExceptionAsync(async () =>
                await values.Should().SatisfyRespectivelyAsync(
                    "workflow should stay valid",
                    item => item.Should().BeGreaterThan(0),
                    item => item.Should().BeGreaterThan(0),
                    item =>
                    {
                        thirdAssertionReached = true;
                        item.Should().BeGreaterThan(0);
                    }));

            Assert.Null(callEx);
            Assert.False(thirdAssertionReached);
            Assert.Equal(2, AssertionFailureCapture.CaptureInvocationCount);

            var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
            Assert.Contains("Batch 'async streams' failed with 1 assertion failure(s):", disposeEx.Message);
            Assert.Contains("Expected values to satisfy assertions respectively (failing index 1) because workflow should stay valid", disposeEx.Message);
            Assert.Contains("Expected item to be greater than 0, but found -1.", disposeEx.Message);
        }
        finally
        {
            AssertionFailureCapture.ResetProbe();
        }
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_InsideBatch_WrapsDirectInvalidOperationExceptionWithIndexAndBecause()
    {
        var values = CreateAsyncSequence(10, 20, 30);
        var thirdAssertionReached = false;

        using var batch = new Axiom.Core.Batch("async streams");
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().SatisfyRespectivelyAsync(
                "workflow should stay valid",
                item => item.Should().BeGreaterThan(0),
                _ => throw new InvalidOperationException("boom"),
                _ => thirdAssertionReached = true));

        Assert.Null(callEx);
        Assert.False(thirdAssertionReached);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Batch 'async streams' failed with 1 assertion failure(s):", disposeEx.Message);
        Assert.Contains("Expected values to satisfy assertions respectively (failing index 1) because workflow should stay valid", disposeEx.Message);
        Assert.Contains("boom", disposeEx.Message);
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_InsideBatch_LetsNonInvalidOperationExceptionEscapeImmediately()
    {
        var values = CreateAsyncSequence(10, 20, 30);
        var batch = new Axiom.Core.Batch("async streams");
        var thirdAssertionReached = false;

        var ex = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await values.Should().SatisfyRespectivelyAsync(
                "workflow should stay valid",
                item => item.Should().BeGreaterThan(0),
                _ => throw new ArgumentException("boom"),
                _ => thirdAssertionReached = true));

        Assert.Equal("boom", ex.Message);
        Assert.False(thirdAssertionReached);

        var disposeEx = Record.Exception(() => batch.Dispose());
        Assert.Null(disposeEx);
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_OutsideBatch_ThrowsImmediately()
    {
        var values = CreateAsyncSequence(1, 2, 2, 3);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().HaveUniqueItemsAsync());
    }

    [Fact]
    public async Task HaveUniqueItemsAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var values = CreateAsyncSequence(1, 2, 2, 3);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().HaveUniqueItemsAsync());

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected values to have unique items", disposeEx.Message);
        Assert.Contains("first duplicate item at index 2: 2", disposeEx.Message);
    }

    [Fact]
    public async Task ContainInOrderAsync_OutsideBatch_ThrowsImmediately()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainInOrderAsync([1, 3, 2]));
    }

    [Fact]
    public async Task ContainInOrderAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().ContainInOrderAsync([1, 3, 2]));

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected values to contain items in order [1, 3, 2]", disposeEx.Message);
        Assert.Contains("missing expected item at sequence index 2: 2", disposeEx.Message);
    }

    [Fact]
    public async Task ContainInOrderAsync_ByKey_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var values = CreateAsyncSequence(
            new WorkflowEvent(WorkflowStep.Started, "start"),
            new WorkflowEvent(WorkflowStep.Running, "run"),
            new WorkflowEvent(WorkflowStep.Completed, "done"));

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().ContainInOrderAsync(
                [WorkflowStep.Started, WorkflowStep.Completed, WorkflowStep.Running],
                evt => evt.Step));

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected values to contain selected values in order [Started, Completed, Running]", disposeEx.Message);
        Assert.Contains("missing expected selected value at sequence index 2: Running", disposeEx.Message);
    }

    private static async IAsyncEnumerable<T> CreateAsyncSequence<T>(params T[] items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }

    private enum WorkflowStep
    {
        Started,
        Running,
        Completed
    }

    private sealed record WorkflowEvent(WorkflowStep Step, string Name);
}
