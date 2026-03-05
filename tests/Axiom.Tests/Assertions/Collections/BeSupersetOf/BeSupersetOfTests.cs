using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.BeSupersetOf;

public sealed class BeSupersetOfTests
{
    [Fact]
    public void BeSupersetOf_ReturnsContinuation_WhenAllExpectedItemsExist()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.BeSupersetOf([1, 2]);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeSupersetOf_ReturnsContinuation_WhenExpectedSubsetContainsDuplicates()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.BeSupersetOf([1, 1]);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeSupersetOf_ReturnsContinuation_WhenExpectedSubsetIsEmpty()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.BeSupersetOf(Array.Empty<int>());

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeSupersetOf_Throws_WhenExpectedItemIsMissing()
    {
        int[] values = [1, 2];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().BeSupersetOf([1, 2, 4]));

        const string expected = "Expected values to be a superset of [1, 2, 4], but found missing expected item at index 2: 4.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeSupersetOf_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().BeSupersetOf([1, 2, 4], "all required IDs must exist"));

        Assert.Contains("because all required IDs must exist", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeSupersetOf_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().BeSupersetOf([1]));

        const string expected = "Expected values to be a superset of [1], but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeSupersetOf_ThrowsArgumentNullException_WhenExpectedSubsetIsNull()
    {
        int[] values = [1, 2];
        int[]? expectedSubset = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().BeSupersetOf(expectedSubset!));

        Assert.Equal("expectedSubset", ex.ParamName);
    }
}
