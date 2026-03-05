using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToBatchRoutingTests
{
    private sealed class Person
    {
        public string? Name { get; init; }
        public int Age { get; init; }
    }

    [Fact]
    public void GivenNoBatch_WhenNotEquivalent_ThenThrowsImmediately()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 3, 2, 1 };

        Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));
    }

    [Fact]
    public void GivenBatch_WhenNotEquivalent_ThenAssertionCallSiteDoesNotThrow()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 3, 2, 1 };

        var batch = new Axiom.Core.Batch();
        try
        {
            var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));
            Assert.Null(ex);
        }
        finally
        {
            _ = Record.Exception(batch.Dispose);
        }
    }

    [Fact]
    public void GivenBatch_WhenNotEquivalent_ThenDisposeThrowsCombinedFailure()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 3, 2, 1 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("equivalency");
            actual.Should().BeEquivalentTo(expected);
        });

        Assert.Contains("Batch 'equivalency' failed with 1 assertion failure(s):", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Expected actual to be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenBatch_WhenOnlyComparedMemberDiffers_ThenDisposeThrowsCombinedFailure()
    {
        var actual = new Person { Name = "Alice", Age = 30 };
        var expected = new Person { Name = "Bob", Age = 99 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("equivalency-only-member");
            actual.Should().BeEquivalentTo(expected, options => options.OnlyCompareMember(nameof(Person.Name)));
        });

        Assert.Contains("Batch 'equivalency-only-member' failed with 1 assertion failure(s):", ex.Message, StringComparison.Ordinal);
        Assert.Contains("actual.Name", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenBatch_WhenOnlyComparedMemberMatches_ThenDisposeDoesNotThrow()
    {
        var actual = new Person { Name = "Alice", Age = 30 };
        var expected = new Person { Name = "Alice", Age = 99 };

        var ex = Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch("equivalency-only-member-pass");
            actual.Should().BeEquivalentTo(expected, options => options.OnlyCompareMember(nameof(Person.Name)));
        });

        Assert.Null(ex);
    }
}
