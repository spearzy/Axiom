using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Batch;

public sealed class ValueBatchRoutingTests
{
    [Fact]
    public void Be_OutsideBatch_ThrowsImmediately()
    {
        var value = 42;

        Assert.Throws<InvalidOperationException>(() => value.Should().Be(7));
    }

    [Fact]
    public void Be_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var value = 42;

        var ex = Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch();
            value.Should().Be(7);
        });

        Assert.NotNull(ex);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromValueAssertions()
    {
        var value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("values");
            value.Should().Be(7);
            value.Should().NotBe(42);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'values' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected value to be 7, but found 42.", message);
        Assert.Contains("2) Expected value to not be 42, but found 42.", message);
    }

    [Fact]
    public void BeFalse_OutsideBatch_ThrowsImmediately()
    {
        const bool value = true;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeFalse());
    }

    [Fact]
    public void BeFalse_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const bool value = true;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeFalse());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeTrue_OutsideBatch_ThrowsImmediately()
    {
        const bool value = false;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeTrue());
    }

    [Fact]
    public void BeTrue_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const bool value = false;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeTrue());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromBooleanAssertions()
    {
        const bool isEnabled = true;
        const bool isReady = false;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("bools");
            isEnabled.Should().BeFalse();
            isReady.Should().BeTrue();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'bools' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected isEnabled to be False, but found True.", message);
        Assert.Contains("2) Expected isReady to be True, but found False.", message);
    }

    [Fact]
    public void BeNull_OutsideBatch_ThrowsImmediately()
    {
        int? value = 1;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeNull());
    }

    [Fact]
    public void BeNull_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int? value = 1;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeNull());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotBeNull_OutsideBatch_ThrowsImmediately()
    {
        int? value = null;

        Assert.Throws<InvalidOperationException>(() => value.Should().NotBeNull());
    }

    [Fact]
    public void NotBeNull_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int? value = null;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().NotBeNull());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromNullabilityAssertions()
    {
        int? hasValue = 1;
        int? noValue = null;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("nullability");
            hasValue.Should().BeNull();
            noValue.Should().NotBeNull();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'nullability' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected hasValue to be null, but found 1.", message);
        Assert.Contains("2) Expected noValue to not be null, but found <null>.", message);
    }
}
