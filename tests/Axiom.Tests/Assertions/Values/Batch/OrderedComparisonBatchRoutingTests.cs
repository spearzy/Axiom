using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.Batch;

public sealed class OrderedComparisonBatchRoutingTests
{
    [Fact]
    public void BeGreaterThan_OutsideBatch_ThrowsImmediately()
    {
        const int value = 1;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeGreaterThan(5));
    }

    [Fact]
    public void BeGreaterThan_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const int value = 1;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeGreaterThan(5));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeGreaterThanOrEqualTo_OutsideBatch_ThrowsImmediately()
    {
        const int value = 4;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeGreaterThanOrEqualTo(5));
    }

    [Fact]
    public void BeGreaterThanOrEqualTo_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const int value = 4;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeGreaterThanOrEqualTo(5));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeLessThan_OutsideBatch_ThrowsImmediately()
    {
        const int value = 5;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeLessThan(5));
    }

    [Fact]
    public void BeLessThan_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const int value = 5;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeLessThan(5));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeLessThanOrEqualTo_OutsideBatch_ThrowsImmediately()
    {
        const int value = 6;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeLessThanOrEqualTo(5));
    }

    [Fact]
    public void BeLessThanOrEqualTo_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const int value = 6;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeLessThanOrEqualTo(5));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeInRange_OutsideBatch_ThrowsImmediately()
    {
        const int value = 10;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeInRange(1, 5));
    }

    [Fact]
    public void BeInRange_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const int value = 10;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeInRange(1, 5));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeInRange_WithInvalidBounds_ThrowsArgumentExceptionImmediately()
    {
        const int value = 3;

        Assert.Throws<ArgumentException>(() => value.Should().BeInRange(5, 1));
    }

    [Fact]
    public void Batch_Dispose_DoesNotThrow_WhenOrderedComparisonsPass()
    {
        var ex = Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch("ordered-pass");
            6.Should().BeGreaterThan(5);
            5.Should().BeGreaterThanOrEqualTo(5);
            4.Should().BeLessThan(5);
            5.Should().BeLessThanOrEqualTo(5);
            3.Should().BeInRange(1, 5);
        });

        Assert.Null(ex);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromOrderedComparisons()
    {
        const int valueA = 1;
        const int valueB = 4;
        const int valueC = 5;
        const int valueD = 6;
        const int valueE = 10;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("ordered");
            valueA.Should().BeGreaterThan(5);
            valueB.Should().BeGreaterThanOrEqualTo(5);
            valueC.Should().BeLessThan(5);
            valueD.Should().BeLessThanOrEqualTo(5);
            valueE.Should().BeInRange(1, 5);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'ordered' failed with 5 assertion failure(s):", message);
        Assert.Contains("1) Expected valueA to be greater than 5, but found 1.", message);
        Assert.Contains("2) Expected valueB to be greater than or equal to 5, but found 4.", message);
        Assert.Contains("3) Expected valueC to be less than 5, but found 5.", message);
        Assert.Contains("4) Expected valueD to be less than or equal to 5, but found 6.", message);
        Assert.Contains("5) Expected valueE to be in range [1, 5], but found 10.", message);
    }
}
