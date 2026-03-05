using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.ContainInOrder;

public sealed class ContainInOrderTests
{
    private sealed record WorkflowStep(int Id, string Name);

    [Fact]
    public void ContainInOrder_ReturnsContinuation_WhenExpectedSequenceExistsInOrder()
    {
        int[] values = [1, 2, 3, 4];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainInOrder([1, 3, 4]);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainInOrder_ReturnsContinuation_WhenExpectedSequenceExistsAdjacently_WhenNoGapsRequired()
    {
        int[] values = [1, 2, 3, 4];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainInOrder([2, 3], allowGaps: false);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainInOrder_Throws_WhenExpectedSequenceDoesNotExistInOrder()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainInOrder([1, 3, 2]));

        const string expected = "Expected values to contain items in order [1, 3, 2], but found missing expected item at sequence index 2: 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainInOrder_Throws_WhenExpectedSequenceOnlyExistsWithGaps_AndNoGapsRequired()
    {
        int[] values = [1, 2, 3, 4];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainInOrder([1, 3], allowGaps: false));

        const string expected = "Expected values to contain items in order with no gaps [1, 3], but found missing adjacent ordered sequence.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainInOrder_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainInOrder([1, 3, 2], "events must follow the workflow order"));

        Assert.Contains("because events must follow the workflow order", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ContainInOrder_DoesNotThrow_WhenExpectedSequenceIsEmpty()
    {
        int[] values = [1, 2, 3];

        var ex = Record.Exception(() => values.Should().ContainInOrder(Array.Empty<int>()));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainInOrder_ThrowsArgumentNullException_WhenExpectedSequenceIsNull()
    {
        int[] values = [1, 2, 3];
        int[]? sequence = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().ContainInOrder(sequence!));

        Assert.Equal("expectedSequence", ex.ParamName);
    }

    [Fact]
    public void ContainInOrder_ByKey_ReturnsContinuation_WhenExpectedSelectedValuesExistInOrder()
    {
        WorkflowStep[] steps =
        [
            new(1, "validate"),
            new(2, "enrich"),
            new(3, "persist"),
            new(4, "notify")
        ];

        var baseAssertions = steps.Should();
        var continuation = baseAssertions.ContainInOrder([1, 3, 4], (WorkflowStep step) => step.Id);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainInOrder_ByKey_Throws_WhenExpectedSelectedValuesDoNotExistInOrder()
    {
        WorkflowStep[] steps =
        [
            new(1, "validate"),
            new(2, "enrich"),
            new(3, "persist")
        ];

        var ex = Assert.Throws<InvalidOperationException>(() => steps.Should().ContainInOrder([1, 3, 2], (WorkflowStep step) => step.Id));

        const string expected = "Expected steps to contain selected values in order [1, 3, 2], but found missing expected selected value at sequence index 2: 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainInOrder_ByKey_ThrowsArgumentNullException_WhenKeySelectorIsNull()
    {
        WorkflowStep[] steps =
        [
            new(1, "validate"),
            new(2, "enrich")
        ];

        Func<WorkflowStep, int>? selector = null;

        var ex = Assert.Throws<ArgumentNullException>(() => steps.Should().ContainInOrder([1], selector!));

        Assert.Equal("keySelector", ex.ParamName);
    }
}
