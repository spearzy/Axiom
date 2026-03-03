using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.Chaining;

public sealed class CollectionChainingTests
{
    private sealed record WorkflowStep(int Position, string Name);

    [Fact]
    public void FullChain_CanBeComposed()
    {
        int[] values = [1, 2, 3];

        values.Should().Contain(2).And.HaveCount(3);
    }

    [Fact]
    public void SingleItemChain_CanBeComposed()
    {
        int[] values = [1];

        values.Should().NotBeEmpty().And.ContainSingle().And.Contain(1).And.HaveCount(1);
    }

    [Fact]
    public void EmptyChain_CanBeComposed()
    {
        int[] values = [];

        values.Should().BeEmpty().And.HaveCount(0);
    }

    [Fact]
    public void PredicateAndOrderChain_CanBeComposed()
    {
        int[] values = [1, 2, 3];

        values.Should()
            .NotContain((int x) => x < 0).And
            .OnlyContain((int x) => x > 0).And
            .ContainInOrder([1, 3]);
    }

    [Fact]
    public void KeySelectionOrderChain_CanBeComposed()
    {
        WorkflowStep[] steps =
        [
            new(1, "validate"),
            new(2, "enrich"),
            new(3, "persist")
        ];

        steps.Should()
            .ContainInOrder([1, 2], (WorkflowStep step) => step.Position, allowGaps: false).And
            .NotContain((WorkflowStep step) => step.Name == "archive");
    }
}
