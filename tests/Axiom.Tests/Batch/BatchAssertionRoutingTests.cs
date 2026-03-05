using Axiom.Assertions;

namespace Axiom.Tests.Batch;

public sealed class BatchAssertionRoutingTests
{
    [Fact]
    public void StartWith_OutsideBatch_ThrowsImmediately()
    {
        string value = "test";

        Assert.Throws<InvalidOperationException>(() => value.Should().StartWith("ab"));
    }

    [Fact]
    public void StartWith_InsideBatch_DoesNotThrowImmediately()
    {
        string value = "test";

        var ex = Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch();
            value.Should().StartWith("ab");
        });

        Assert.NotNull(ex);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromAssertions()
    {
        string? value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("strings");
            value.Should().StartWith("ab");
            value.Should().EndWith("cd");
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'strings' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected value to start with \"ab\", but found \"test\".", message);
        Assert.Contains("2) Expected value to end with \"cd\", but found \"test\".", message);
    }

}
