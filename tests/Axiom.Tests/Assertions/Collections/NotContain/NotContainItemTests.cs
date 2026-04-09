using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.NotContain;

public sealed class NotContainItemTests
{
    [Fact]
    public void NotContainItem_ReturnsContinuation_WhenUnexpectedItemIsNotPresent()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContain(9);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainItem_Throws_WhenUnexpectedItemIsPresent()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().NotContain(2));

        const string expected = "Expected values to not contain 2, but found 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContainItem_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().NotContain(2, "reserved IDs are not allowed"));

        Assert.Contains("because reserved IDs are not allowed", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotContainItem_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().NotContain(1));

        const string expected = "Expected values to not contain 1, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContainItem_Throws_WhenComparerFindsMatchingItem()
    {
        string[] values = ["Alpha", "Beta"];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().NotContain("alpha", StringComparer.OrdinalIgnoreCase));

        Assert.Equal("Expected values to not contain \"alpha\", but found \"Alpha\".", ex.Message);
    }

    [Fact]
    public void NotContainItem_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        string[] values = ["Alpha"];
        StringComparer? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            values.Should().NotContain("alpha", comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }
}
