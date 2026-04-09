using Axiom.Tests.Assertions.AsyncStreams.TestSupport;

namespace Axiom.Tests.Assertions.AsyncStreams.ContainInOrderAsync;

public sealed class ContainInOrderAsyncTests : IDisposable
{
    private enum WorkflowStep
    {
        Started,
        Running,
        Completed
    }

    private sealed record WorkflowEvent(WorkflowStep Step, string Name);

    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public async Task ContainInOrderAsync_ReturnsContinuation_WhenExpectedSequenceExistsInOrder()
    {
        var values = CreateAsyncSequence(1, 2, 3, 4);

        var assertions = values.Should();
        var continuation = await assertions.ContainInOrderAsync([1, 3, 4]);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainInOrderAsync_ReturnsContinuation_WhenExpectedSequenceExistsAdjacently_WhenNoGapsRequired()
    {
        var values = CreateAsyncSequence(1, 2, 3, 4);

        var assertions = values.Should();
        var continuation = await assertions.ContainInOrderAsync([2, 3], allowGaps: false);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainInOrderAsync_Throws_WhenExpectedSequenceDoesNotExistInOrder()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainInOrderAsync([1, 3, 2]));

        Assert.Equal(
            "Expected values to contain items in order [1, 3, 2], but found missing expected item at sequence index 2: 2.",
            ex.Message);
    }

    [Fact]
    public async Task ContainInOrderAsync_Throws_WhenExpectedSequenceOnlyExistsWithGaps_AndNoGapsRequired()
    {
        var values = CreateAsyncSequence(1, 2, 3, 4);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainInOrderAsync([1, 3], allowGaps: false));

        Assert.Equal(
            "Expected values to contain items in order with no gaps [1, 3], but found missing adjacent ordered sequence.",
            ex.Message);
    }

    [Fact]
    public async Task ContainInOrderAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainInOrderAsync(
                [1, 3, 2],
                because: "events must follow the workflow order"));

        Assert.Contains("because events must follow the workflow order", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ContainInOrderAsync_DoesNotThrow_WhenExpectedSequenceIsEmpty()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainInOrderAsync(Array.Empty<int>()));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainInOrderAsync_ThrowsArgumentNullException_WhenExpectedSequenceIsNull()
    {
        var values = CreateAsyncSequence(1, 2, 3);
        int[]? sequence = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainInOrderAsync(sequence!));

        Assert.Equal("expectedSequence", ex.ParamName);
    }

    [Fact]
    public async Task ContainInOrderAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainInOrderAsync([1]));

        Assert.Equal("Expected values to contain items in order [1], but found <null>.", ex.Message);
    }

    [Fact]
    public async Task ContainInOrderAsync_UsesConfiguredComparerProviderForT()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new CaseInsensitiveStringComparerProvider());
        var values = CreateAsyncSequence("started", "COMPLETED", "Finished");

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainInOrderAsync(["STARTED", "completed"]));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainInOrderAsync_Passes_WhenComparerMatchesExpectedSequence()
    {
        var values = CreateAsyncSequence("started", "COMPLETED", "Finished");

        var assertions = values.Should();
        var continuation = await assertions.ContainInOrderAsync(
            ["STARTED", "completed"],
            StringComparer.OrdinalIgnoreCase);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainInOrderAsync_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence("started", "completed");
        IEqualityComparer<string>? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainInOrderAsync(["STARTED"], comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task ContainInOrderAsync_ByKey_ReturnsContinuation_WhenExpectedSelectedValuesExistInOrder()
    {
        var values = CreateAsyncSequence(
            new WorkflowEvent(WorkflowStep.Started, "start"),
            new WorkflowEvent(WorkflowStep.Running, "run"),
            new WorkflowEvent(WorkflowStep.Completed, "done"));

        var assertions = values.Should();
        var continuation = await assertions.ContainInOrderAsync(
            [WorkflowStep.Started, WorkflowStep.Completed],
            evt => evt.Step);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainInOrderAsync_ByKey_Throws_WhenExpectedSelectedValuesDoNotExistInOrder()
    {
        var values = CreateAsyncSequence(
            new WorkflowEvent(WorkflowStep.Started, "start"),
            new WorkflowEvent(WorkflowStep.Running, "run"),
            new WorkflowEvent(WorkflowStep.Completed, "done"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainInOrderAsync(
                [WorkflowStep.Started, WorkflowStep.Completed, WorkflowStep.Running],
                evt => evt.Step));

        Assert.Equal(
            "Expected values to contain selected values in order [Started, Completed, Running], but found missing expected selected value at sequence index 2: Running.",
            ex.Message);
    }

    [Fact]
    public async Task ContainInOrderAsync_ByKey_Throws_WhenExpectedSelectedValuesOnlyExistWithGaps_AndNoGapsRequired()
    {
        var values = CreateAsyncSequence(
            new WorkflowEvent(WorkflowStep.Started, "start"),
            new WorkflowEvent(WorkflowStep.Running, "run"),
            new WorkflowEvent(WorkflowStep.Completed, "done"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainInOrderAsync(
                [WorkflowStep.Started, WorkflowStep.Completed],
                evt => evt.Step,
                allowGaps: false));

        Assert.Equal(
            "Expected values to contain selected values in order with no gaps [Started, Completed], but found missing adjacent ordered sequence for selected values.",
            ex.Message);
    }

    [Fact]
    public async Task ContainInOrderAsync_ByKey_ThrowsArgumentNullException_WhenKeySelectorIsNull()
    {
        var values = CreateAsyncSequence(new WorkflowEvent(WorkflowStep.Started, "start"));
        Func<WorkflowEvent, WorkflowStep>? keySelector = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainInOrderAsync([WorkflowStep.Started], keySelector!));

        Assert.Equal("keySelector", ex.ParamName);
    }

    [Fact]
    public async Task ContainInOrderAsync_ByKey_UsesConfiguredComparerProviderForTKey()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new CaseInsensitiveStringComparerProvider());
        var values = CreateAsyncSequence(
            new WorkflowEvent(WorkflowStep.Started, "STARTED"),
            new WorkflowEvent(WorkflowStep.Running, "RUNNING"),
            new WorkflowEvent(WorkflowStep.Completed, "completed"));

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().ContainInOrderAsync(
                ["started", "COMPLETED"],
                evt => evt.Name));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ContainInOrderAsync_ByKey_Passes_WhenComparerMatchesSelectedValues()
    {
        var values = CreateAsyncSequence(
            new WorkflowEvent(WorkflowStep.Started, "STARTED"),
            new WorkflowEvent(WorkflowStep.Running, "RUNNING"),
            new WorkflowEvent(WorkflowStep.Completed, "completed"));

        var assertions = values.Should();
        var continuation = await assertions.ContainInOrderAsync(
            ["started", "COMPLETED"],
            evt => evt.Name,
            StringComparer.OrdinalIgnoreCase);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task ContainInOrderAsync_ByKey_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        var values = CreateAsyncSequence(new WorkflowEvent(WorkflowStep.Started, "start"));
        IEqualityComparer<string>? comparer = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().ContainInOrderAsync(["start"], evt => evt.Name, comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public async Task ContainInOrderAsync_ChainsWithAnd()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var continuation = await (await values.Should().ContainInOrderAsync([1, 3]))
            .And
            .NotBeEmptyAsync();

        Assert.NotNull(continuation.And);
    }

    [Fact]
    public async Task ContainInOrderAsync_StopsEnumeratingAfterSuccessfulMatch()
    {
        var tracking = new TrackingAsyncEnumerable<int>([1, 3, 4, 5]);
        IAsyncEnumerable<int> values = tracking;

        await values.Should().ContainInOrderAsync([1, 3]);

        Assert.Equal(2, tracking.YieldCount);
        Assert.Equal(2, tracking.MoveNextCallCount);
    }

    private static async IAsyncEnumerable<T> CreateAsyncSequence<T>(params T[] items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }

    private sealed class CaseInsensitiveStringComparerProvider : IComparerProvider
    {
        public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
        {
            if (typeof(T) == typeof(string))
            {
                comparer = (IEqualityComparer<T>)StringComparer.OrdinalIgnoreCase;
                return true;
            }

            comparer = null;
            return false;
        }
    }
}
