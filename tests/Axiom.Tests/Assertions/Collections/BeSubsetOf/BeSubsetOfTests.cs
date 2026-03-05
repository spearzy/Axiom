using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.BeSubsetOf;

public sealed class BeSubsetOfTests
{
    [Fact]
    public void BeSubsetOf_ReturnsContinuation_WhenAllItemsExistInSuperset()
    {
        int[] values = [1, 2];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.BeSubsetOf([1, 2, 3]);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeSubsetOf_ReturnsContinuation_WhenSubjectContainsDuplicateItems()
    {
        int[] values = [1, 1];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.BeSubsetOf([1, 2, 3]);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeSubsetOf_ReturnsContinuation_WhenSubjectIsEmpty()
    {
        int[] values = [];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.BeSubsetOf([1, 2, 3]);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeSubsetOf_Throws_WhenItemDoesNotExistInSuperset()
    {
        int[] values = [1, 4];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().BeSubsetOf([1, 2, 3]));

        const string expected = "Expected values to be a subset of [1, 2, 3], but found missing item at index 1: 4.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeSubsetOf_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 4];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().BeSubsetOf([1, 2, 3], "all IDs must come from the approved set"));

        Assert.Contains("because all IDs must come from the approved set", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeSubsetOf_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().BeSubsetOf([1, 2, 3]));

        const string expected = "Expected values to be a subset of [1, 2, 3], but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeSubsetOf_ThrowsArgumentNullException_WhenExpectedSupersetIsNull()
    {
        int[] values = [1, 2];
        int[]? expectedSuperset = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().BeSubsetOf(expectedSuperset!));

        Assert.Equal("expectedSuperset", ex.ParamName);
    }
}
