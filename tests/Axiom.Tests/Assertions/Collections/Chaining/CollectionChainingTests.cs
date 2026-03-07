using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.Chaining;

public sealed class CollectionChainingTests
{
    private sealed record WorkflowStep(int Position, string Name);
    private sealed record User(int Id, string Email);

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
    public void PredicateSingleItemChain_CanBeComposed()
    {
        int[] values = [1, 2, 3];

        values.Should().NotBeEmpty().And.ContainSingle((int item) => item == 2).And.Contain(2).And.HaveCount(3);
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
    public void ItemAndAllSatisfyChain_CanBeComposed()
    {
        int[] values = [1, 2, 3];

        values.Should()
            .NotContain(9).And
            .AllSatisfy((int item) => item.Should().BeGreaterThan(0)).And
            .Contain(2);
    }

    [Fact]
    public void SatisfyRespectivelyChain_CanBeComposed()
    {
        int[] values = [2, 4, 6];

        values.Should()
            .SatisfyRespectively(
                (int item) => item.Should().BeGreaterThan(0),
                (int item) => item.Should().BeGreaterThan(3),
                (int item) => item.Should().BeGreaterThan(5)).And
            .ContainInOrder([2, 6]);
    }

    [Fact]
    public void ExactSequenceChain_CanBeComposed()
    {
        int[] values = [1, 2, 3];

        values.Should()
            .ContainExactly([1, 2, 3]).And
            .NotContain(9).And
            .HaveCount(3);
    }

    [Fact]
    public void ContainAllChain_CanBeComposed()
    {
        int[] values = [1, 2, 3];

        values.Should()
            .ContainAll(1, 3).And
            .NotContain(9).And
            .HaveCount(3);
    }

    [Fact]
    public void ContainAnyChain_CanBeComposed()
    {
        int[] values = [1, 2, 3];

        values.Should()
            .ContainAny(8, 2).And
            .ContainAll(1, 2).And
            .HaveCount(3);
    }

    [Fact]
    public void NotContainAnyChain_CanBeComposed()
    {
        int[] values = [1, 2, 3];

        values.Should()
            .NotContainAny(8, 9).And
            .ContainAny(3, 10).And
            .HaveCount(3);
    }

    [Fact]
    public void UniqueItemsChain_CanBeComposed()
    {
        int[] values = [1, 2, 3];

        values.Should()
            .HaveUniqueItems().And
            .ContainAll(1, 3).And
            .NotContainAny(8, 9);
    }

    [Fact]
    public void UniqueItemsByChain_CanBeComposed()
    {
        User[] values =
        [
            new(1, "a@example.com"),
            new(2, "b@example.com"),
            new(3, "c@example.com")
        ];

        values.Should()
            .HaveUniqueItemsBy((User user) => user.Id).And
            .HaveCount(3).And
            .NotContain((User user) => user.Email == "missing@example.com");
    }

    [Fact]
    public void SubsetChain_CanBeComposed()
    {
        int[] values = [1, 2];

        values.Should()
            .BeSubsetOf([1, 2, 3]).And
            .NotContain(9).And
            .HaveCount(2);
    }

    [Fact]
    public void SupersetChain_CanBeComposed()
    {
        int[] values = [1, 2, 3];

        values.Should()
            .BeSupersetOf([1, 2]).And
            .Contain(3).And
            .HaveCount(3);
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
