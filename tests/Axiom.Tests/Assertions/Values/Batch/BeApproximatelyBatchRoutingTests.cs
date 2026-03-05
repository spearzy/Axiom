using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Batch;

public sealed class BeApproximatelyBatchRoutingTests
{
    [Fact]
    public void BeApproximately_OutsideBatch_ThrowsImmediately()
    {
        const double value = 10.3d;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeApproximately(10d, 0.1d));
    }

    [Fact]
    public void BeApproximately_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const double value = 10.3d;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeApproximately(10d, 0.1d));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_DoesNotThrow_WhenApproximateAssertionsPass()
    {
        var ex = Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch("approximate-pass");
            10.05d.Should().BeApproximately(10d, 0.1d);
            10.05f.Should().BeApproximately(10f, 0.1f);
            10.05m.Should().BeApproximately(10m, 0.1m);
        });

        Assert.Null(ex);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromApproximateAssertions()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("approximate");
            10.25d.Should().BeApproximately(10d, 0.1d);
            10.25f.Should().BeApproximately(10f, 0.1f);
            10.25m.Should().BeApproximately(10m, 0.1m);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'approximate' failed with 3 assertion failure(s):", message);
        Assert.Contains("1) Expected 10.25d to be within 0.1 of 10, but found 10.25.", message);
        Assert.Contains("2) Expected 10.25f to be within 0.1 of 10, but found 10.25.", message);
        Assert.Contains("3) Expected 10.25m to be within 0.1 of 10, but found 10.25.", message);
    }
}
