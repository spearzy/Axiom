using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.ContainExactly;

public sealed class ContainExactlyTests
{
    [Fact]
    public void ContainExactly_ReturnsContinuation_WhenSequenceMatchesExactly()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainExactly([1, 2, 3]);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainExactly_DoesNotFormatExpectedSequence_WhenAssertionPasses()
    {
        ThrowingToStringValue[] values = [new(1), new(2), new(3)];
        ThrowingToStringValue[] expected = [new(1), new(2), new(3)];

        var ex = Record.Exception(() => values.Should().ContainExactly(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainExactly_Throws_WhenItemMismatchesByIndex()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainExactly([1, 9, 3]));

        const string expected =
            "Expected values to contain exactly [1, 9, 3], but found item mismatch at index 1: expected 9 but found 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainExactly_Throws_WhenSubjectContainsExtraItems()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainExactly([1, 2]));

        const string expected = "Expected values to contain exactly [1, 2], but found extra item at index 2: 3.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainExactly_Throws_WhenSubjectIsMissingItems()
    {
        int[] values = [1, 2];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainExactly([1, 2, 3]));

        const string expected = "Expected values to contain exactly [1, 2, 3], but found missing item at index 2: 3.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainExactly_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainExactly([1, 9, 3], "normalised IDs must match exactly"));

        Assert.Contains("because normalised IDs must match exactly", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ContainExactly_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().ContainExactly([1]));

        const string expected = "Expected values to contain exactly [1], but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainExactly_ThrowsArgumentNullException_WhenExpectedSequenceIsNull()
    {
        int[] values = [1, 2, 3];
        int[]? expectedSequence = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().ContainExactly(expectedSequence!));

        Assert.Equal("expectedSequence", ex.ParamName);
    }

    private readonly record struct ThrowingToStringValue(int Value)
    {
        public override string ToString()
        {
            throw new InvalidOperationException("ToString should not be called on pass path.");
        }
    }
}
