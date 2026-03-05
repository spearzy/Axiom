using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToCycleTests
{
    [Fact]
    public void GivenSelfReferencingGraph_WhenEquivalent_ThenDoesNotThrow()
    {
        var actual = new Node { Value = 7 };
        actual.Next = actual;

        var expected = new Node { Value = 7 };
        expected.Next = expected;

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenSelfReferencingGraph_WhenLeafValueDiffers_ThenThrowsWithoutInfiniteRecursion()
    {
        var actual = new Node { Value = 7 };
        actual.Next = actual;

        var expected = new Node { Value = 8 };
        expected.Next = expected;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("actual.Value", ex.Message, StringComparison.Ordinal);
    }

    private sealed class Node
    {
        public int Value { get; init; }
        public Node? Next { get; set; }
    }
}
