namespace Axiom.Tests.Batch;

public class BatchAssertionRoutingTests
{
    [Fact]
    public void StartWith_OutsideBatch_ThrowsImmediately()
    {
        string value = "test";

        Xunit.Assert.Throws<InvalidOperationException>(() => value.Should().StartWith("ab"));
    }

    [Fact]
    public void StartWith_InsideBatch_DoesNotThrowImmediately()
    {
        string value = "test";

        var ex = Xunit.Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch();
            value.Should().StartWith("ab");
        });

        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromAssertions()
    {
        string? value = "test";

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("strings");
            value.Should().StartWith("ab");
            value.Should().EndWith("cd");
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Xunit.Assert.Contains("Batch 'strings' failed with 2 assertion failure(s):", message);
        Xunit.Assert.Contains("1) Expected value to start with \"ab\", but found \"test\".", message);
        Xunit.Assert.Contains("2) Expected value to end with \"cd\", but found \"test\".", message);
    }

}
