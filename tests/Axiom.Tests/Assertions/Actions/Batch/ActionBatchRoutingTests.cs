using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Actions.Batch;

public sealed class ActionBatchRoutingTests
{
    [Fact]
    public void Throw_OutsideBatch_ThrowsImmediately()
    {
        Action action = static () => { };

        Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>());
    }

    [Fact]
    public void Throw_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Action action = static () => { };

        var ex = Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch();
            action.Should().Throw<InvalidOperationException>();
        });

        Assert.NotNull(ex);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromActionAssertions()
    {
        Action noThrow = static () => { };
        Action wrongThrow = static () => throw new ArgumentException("bad");

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("actions");
            noThrow.Should().Throw<InvalidOperationException>();
            wrongThrow.Should().Throw<InvalidOperationException>();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'actions' failed with 2 assertion failure(s):", message);
        Assert.Contains($"1) Expected noThrow to throw {typeof(InvalidOperationException)}, but found <no exception>.", message);
        Assert.Contains($"2) Expected wrongThrow to throw {typeof(InvalidOperationException)}, but found {typeof(ArgumentException)}.", message);
    }

    [Fact]
    public void ThrowExactly_OutsideBatch_ThrowsImmediately()
    {
        Action action = static () => { };

        Assert.Throws<InvalidOperationException>(() =>
            action.Should().ThrowExactly<InvalidOperationException>());
    }

    [Fact]
    public void ThrowExactly_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Action action = static () => { };

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => action.Should().ThrowExactly<InvalidOperationException>());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotThrow_OutsideBatch_ThrowsImmediately()
    {
        Action action = static () => throw new ArgumentException("bad");

        Assert.Throws<InvalidOperationException>(() => action.Should().NotThrow());
    }

    [Fact]
    public void NotThrow_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Action action = static () => throw new ArgumentException("bad");

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => action.Should().NotThrow());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromStrictAndNotThrowAssertions()
    {
        Action noThrow = static () => { };
        Action hasThrow = static () => throw new ArgumentException("bad");

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("action-extended");
            noThrow.Should().ThrowExactly<InvalidOperationException>();
            hasThrow.Should().NotThrow();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'action-extended' failed with 2 assertion failure(s):", message);
        Assert.Contains($"1) Expected noThrow to throw exactly {typeof(InvalidOperationException)}, but found <no exception>.", message);
        Assert.Contains("2) Expected hasThrow to not throw, but found System.ArgumentException.", message);
    }
}
