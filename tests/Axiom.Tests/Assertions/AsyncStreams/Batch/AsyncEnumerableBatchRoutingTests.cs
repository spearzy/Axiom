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
    public async Task ContainExactlyAsync_OutsideBatch_ThrowsImmediately()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainExactlyAsync([1, 9, 3]));
    }

    [Fact]
    public async Task ContainExactlyAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().ContainExactlyAsync([1, 9, 3]));

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected values to contain exactly [1, 9, 3]", disposeEx.Message);
        Assert.Contains("item mismatch at index 1: expected 9 but found 2", disposeEx.Message);
    }

    [Fact]
    public async Task ContainAnyAsync_OutsideBatch_ThrowsImmediately()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().ContainAnyAsync([9, 10]));
    }

    [Fact]
    public async Task ContainAnyAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().ContainAnyAsync([9, 10]));

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected values to contain any of [9, 10]", disposeEx.Message);
        Assert.Contains("none of the expected items were found", disposeEx.Message);
    }

    [Fact]
    public async Task NotContainAnyAsync_OutsideBatch_ThrowsImmediately()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().NotContainAnyAsync([9, 2, 10]));
    }

    [Fact]
    public async Task NotContainAnyAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await values.Should().NotContainAnyAsync([9, 2, 10]));

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected values to not contain any of [9, 2, 10]", disposeEx.Message);
        Assert.Contains("first matching item at subject index 1: 2", disposeEx.Message);
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

    [Fact]
    public async Task AsyncEnumerableComparerAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        static IAsyncEnumerable<string> CreateValues()
        {
            return CreateAsyncSequence("Alpha", "beta", "beta");
        }

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
        {
            await CreateValues().Should().ContainAsync("GAMMA", StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().ContainExactlyAsync(
                ["alpha", "gamma", "beta"],
                StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().ContainAnyAsync(
                ["DELTA", "EPSILON"],
                StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().NotContainAnyAsync(
                ["THETA", "BETA"],
                StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().HaveUniqueItemsAsync(StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().ContainInOrderAsync(
                ["ALPHA", "GAMMA"],
                StringComparer.OrdinalIgnoreCase);
            //item is the same element returned as the selected key
            await CreateValues().Should().ContainInOrderAsync(["ALPHA", "GAMMA"], item => item,
                StringComparer.OrdinalIgnoreCase);
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("to contain \"GAMMA\"", disposeEx.Message);
        Assert.Contains("<no matching item>", disposeEx.Message);
        Assert.Contains("to contain exactly [\"alpha\", \"gamma\", \"beta\"]", disposeEx.Message);
        Assert.Contains("item mismatch at index 1: expected \"gamma\" but found \"beta\"", disposeEx.Message);
        Assert.Contains("to contain any of [\"DELTA\", \"EPSILON\"]", disposeEx.Message);
        Assert.Contains("none of the expected items were found", disposeEx.Message);
        Assert.Contains("to not contain any of [\"THETA\", \"BETA\"]", disposeEx.Message);
        Assert.Contains("first matching item at subject index 1: \"beta\"", disposeEx.Message);
        Assert.Contains("to have unique items", disposeEx.Message);
        Assert.Contains("first duplicate item at index 2: \"beta\"", disposeEx.Message);
        Assert.Contains("to contain items in order [\"ALPHA\", \"GAMMA\"]", disposeEx.Message);
        Assert.Contains("missing expected item at sequence index 1: \"GAMMA\"", disposeEx.Message);
        Assert.Contains("to contain selected values in order [\"ALPHA\", \"GAMMA\"]", disposeEx.Message);
        Assert.Contains("missing expected selected value at sequence index 1: \"GAMMA\"", disposeEx.Message);
    }

    [Fact]
    public async Task AsyncEnumerableComparerAssertions_InsideBatch_PreserveFailureCallOrder_ForContainmentAndUniqueness()
    {
        static IAsyncEnumerable<string> CreateValues()
        {
            return CreateAsyncSequence("Alpha", "beta", "beta");
        }

        using var batch = new Axiom.Core.Batch("async streams");
        var callEx = await Record.ExceptionAsync(async () =>
        {
            await CreateValues().Should().ContainAsync("GAMMA", StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().NotContainAnyAsync(["THETA", "BETA"], StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().HaveUniqueItemsAsync(StringComparer.OrdinalIgnoreCase);
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        var containIndex = disposeEx.Message.IndexOf("to contain \"GAMMA\"", StringComparison.Ordinal);
        var notContainAnyIndex = disposeEx.Message.IndexOf("to not contain any of [\"THETA\", \"BETA\"]", StringComparison.Ordinal);
        var uniqueIndex = disposeEx.Message.IndexOf("to have unique items", StringComparison.Ordinal);

        Assert.True(containIndex >= 0, disposeEx.Message);
        Assert.True(notContainAnyIndex > containIndex, disposeEx.Message);
        Assert.True(uniqueIndex > notContainAnyIndex, disposeEx.Message);
    }

    [Fact]
    public async Task NewAsyncContainmentAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        static IAsyncEnumerable<int> CreateValues()
        {
            return CreateAsyncSequence(1, 2, 2);
        }

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
        {
            await CreateValues().Should().ContainExactlyInAnyOrderAsync([1, 2, 3]);
            await CreateValues().Should().ContainAllAsync([1, 4]);
            await CreateValues().Should().NotContainAsync(2);
            await CreateValues().Should().BeSubsetOfAsync([1]);
            await CreateValues().Should().BeSupersetOfAsync([1, 4]);
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("to contain exactly in any order [1, 2, 3]", disposeEx.Message);
        Assert.Contains("missing item 3: expected count 1 but found 0", disposeEx.Message);
        Assert.Contains("to contain all [1, 4]", disposeEx.Message);
        Assert.Contains("missing expected item at index 1: 4", disposeEx.Message);
        Assert.Contains("to not contain 2", disposeEx.Message);
        Assert.Contains("first matching item at subject index 1: 2", disposeEx.Message);
        Assert.Contains("to be a subset of [1]", disposeEx.Message);
        Assert.Contains("missing item at index 1: 2", disposeEx.Message);
        Assert.Contains("to be a superset of [1, 4]", disposeEx.Message);
        Assert.Contains("missing expected item at index 1: 4", disposeEx.Message);
    }

    [Fact]
    public async Task NewAsyncContainmentAssertions_WithComparer_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        static IAsyncEnumerable<string> CreateValues()
        {
            return CreateAsyncSequence("Alpha", "beta", "beta");
        }

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
        {
            await CreateValues().Should().ContainExactlyInAnyOrderAsync(
                ["alpha", "beta", "gamma"],
                StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().ContainAllAsync(["alpha", "delta"], StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().NotContainAsync("BETA", StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().BeSubsetOfAsync(["alpha"], StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().BeSupersetOfAsync(["alpha", "delta"], StringComparer.OrdinalIgnoreCase);
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("to contain exactly in any order [\"alpha\", \"beta\", \"gamma\"]", disposeEx.Message);
        Assert.Contains("missing item \"gamma\": expected count 1 but found 0", disposeEx.Message);
        Assert.Contains("to contain all [\"alpha\", \"delta\"]", disposeEx.Message);
        Assert.Contains("missing expected item at index 1: \"delta\"", disposeEx.Message);
        Assert.Contains("to not contain \"BETA\"", disposeEx.Message);
        Assert.Contains("first matching item at subject index 1: \"beta\"", disposeEx.Message);
        Assert.Contains("to be a subset of [\"alpha\"]", disposeEx.Message);
        Assert.Contains("missing item at index 1: \"beta\"", disposeEx.Message);
        Assert.Contains("to be a superset of [\"alpha\", \"delta\"]", disposeEx.Message);
        Assert.Contains("missing expected item at index 1: \"delta\"", disposeEx.Message);
    }

    [Fact]
    public async Task AsyncEnumerableOrderingAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        var ascending = CreateAsyncSequence(1, 3, 2);
        var descending = CreateAsyncSequence(3, 1, 2);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
        {
            await ascending.Should().BeInAscendingOrderAsync();
            await descending.Should().BeInDescendingOrderAsync();
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected ascending to be in ascending order", disposeEx.Message);
        Assert.Contains("first out-of-order pair at index 2: previous 3 then current 2", disposeEx.Message);
        Assert.Contains("Expected descending to be in descending order", disposeEx.Message);
        Assert.Contains("first out-of-order pair at index 2: previous 1 then current 2", disposeEx.Message);
    }

    [Fact]
    public async Task AsyncEnumerableOrderingAssertions_WithComparer_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        var ascending = CreateAsyncSequence("a", "ccc", "bb");
        var descending = CreateAsyncSequence("ccc", "a", "bb");

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
        {
            await ascending.Should().BeInAscendingOrderAsync(StringLengthComparer.Instance);
            await descending.Should().BeInDescendingOrderAsync(StringLengthComparer.Instance);
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected ascending to be in ascending order", disposeEx.Message);
        Assert.Contains("first out-of-order pair at index 2: previous \"ccc\" then current \"bb\"", disposeEx.Message);
        Assert.Contains("Expected descending to be in descending order", disposeEx.Message);
        Assert.Contains("first out-of-order pair at index 2: previous \"a\" then current \"bb\"", disposeEx.Message);
    }

    [Fact]
    public async Task AsyncEnumerableComparerAssertions_InsideBatch_PreserveFailureCallOrder()
    {
        static IAsyncEnumerable<string> CreateValues()
        {
            return CreateAsyncSequence("Alpha", "beta", "beta");
        }

        static IAsyncEnumerable<string> CreateDescending()
        {
            return CreateAsyncSequence("ccc", "a", "bb");
        }

        using var batch = new Axiom.Core.Batch("async streams");
        var callEx = await Record.ExceptionAsync(async () =>
        {
            await CreateValues().Should().ContainAllAsync(["alpha", "delta"], StringComparer.OrdinalIgnoreCase);
            await CreateValues().Should().NotContainAsync("BETA", StringComparer.OrdinalIgnoreCase);
            await CreateDescending().Should().BeInDescendingOrderAsync(StringLengthComparer.Instance);
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        var containAllIndex = disposeEx.Message.IndexOf("to contain all [\"alpha\", \"delta\"]", StringComparison.Ordinal);
        var notContainIndex = disposeEx.Message.IndexOf("to not contain \"BETA\"", StringComparison.Ordinal);
        var descendingIndex = disposeEx.Message.IndexOf("to be in descending order", StringComparison.Ordinal);

        Assert.True(containAllIndex >= 0, disposeEx.Message);
        Assert.True(notContainIndex > containAllIndex, disposeEx.Message);
        Assert.True(descendingIndex > notContainIndex, disposeEx.Message);
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

    private sealed class StringLengthComparer : IComparer<string>
    {
        public static StringLengthComparer Instance { get; } = new();

        public int Compare(string? x, string? y)
        {
            return Comparer<int>.Default.Compare(x?.Length ?? 0, y?.Length ?? 0);
        }
    }
}
