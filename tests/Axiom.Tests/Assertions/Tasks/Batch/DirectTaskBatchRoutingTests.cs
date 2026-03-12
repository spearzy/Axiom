using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

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
}
