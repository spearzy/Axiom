namespace Axiom.Tests.Assertions.Collections.Chaining;

public sealed class CollectionChainingTests
{
    [Fact]
    public void FullChain_CanBeComposed()
    {
        int[] values = [1, 2, 3];

        values.Should().Contain(2).And.HaveCount(3);
    }
}
