using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotBeEquivalentTo;

public sealed class NotBeEquivalentToBatchRoutingTests
{
    [Fact]
    public void GivenNoBatch_WhenEquivalent_ThenThrowsImmediately()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 1, 2, 3 };

        Assert.Throws<InvalidOperationException>(() => actual.Should().NotBeEquivalentTo(expected));
    }

    [Fact]
    public void GivenBatch_WhenEquivalent_ThenAssertionCallSiteDoesNotThrow()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 1, 2, 3 };

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => actual.Should().NotBeEquivalentTo(expected));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void GivenBatch_WhenEquivalent_ThenDisposeThrowsCombinedFailure()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 1, 2, 3 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("not-equivalency");
            actual.Should().NotBeEquivalentTo(expected);
        });

        Assert.Contains("Batch 'not-equivalency' failed with 1 assertion failure(s):", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Expected actual to not be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenBatch_WhenNotEquivalent_ThenDisposeDoesNotThrow()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 3, 2, 1 };

        var ex = Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch("not-equivalency-pass");
            actual.Should().NotBeEquivalentTo(expected);
        });

        Assert.Null(ex);
    }
}
