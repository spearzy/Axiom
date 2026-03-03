using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.ContainInOrder;

public sealed class ContainInOrderTests
{
    [Fact]
    public void ContainInOrder_ReturnsContinuation_WhenExpectedSequenceExistsInOrder()
    {
        int[] values = [1, 2, 3, 4];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainInOrder([1, 3, 4]);

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
}
