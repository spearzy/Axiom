using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.BeInDescendingOrder;

public sealed class BeInDescendingOrderTests
{
    private sealed record User(string Email, int Rank);

    [Fact]
    public void BeInDescendingOrder_ReturnsContinuation_WhenItemsAreDescending()
    {
        int[] values = [3, 2, 1];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.BeInDescendingOrder();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeInDescendingOrder_Throws_WhenItemsAreOutOfOrder()
    {
        int[] values = [3, 1, 2];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().BeInDescendingOrder());

        const string expected =
            "Expected values to be in descending order, but found first out-of-order pair at index 2: previous 1 then current 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeInDescendingOrder_Throws_WithReason_WhenProvided()
    {
        int[] values = [3, 1, 2];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().BeInDescendingOrder("records must be sorted newest first"));

        Assert.Contains("because records must be sorted newest first", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeInDescendingOrder_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().BeInDescendingOrder());

        const string expected = "Expected values to be in descending order, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeInDescendingOrder_DoesNotThrow_WhenCollectionHasSingleItem()
    {
        int[] values = [3];

        var ex = Record.Exception(() => values.Should().BeInDescendingOrder());

        Assert.Null(ex);
    }

    [Fact]
    public void BeInDescendingOrder_WithComparer_UsesProvidedComparer()
    {
        string[] values = ["charlie", "Beta", "alpha"];

        var ex = Record.Exception(() =>
            values.Should().BeInDescendingOrder(StringComparer.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public void BeInDescendingOrder_ByKey_ReturnsContinuation_WhenSelectedKeysAreDescending()
    {
        User[] users =
        [
            new("c@example.com", 3),
            new("b@example.com", 2),
            new("a@example.com", 1)
        ];

        var baseAssertions = users.Should();
        var continuation = baseAssertions.BeInDescendingOrder((User user) => user.Rank);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeInDescendingOrder_ByKey_Throws_WhenSelectedKeysAreOutOfOrder()
    {
        User[] users =
        [
            new("c@example.com", 3),
            new("a@example.com", 1),
            new("b@example.com", 2)
        ];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            users.Should().BeInDescendingOrder((User user) => user.Rank));

        const string expected =
            "Expected users to be in descending order by selected key, but found first out-of-order selected key pair at index 2: previous 1 then current 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeInDescendingOrder_ByKey_WithComparer_UsesProvidedComparer()
    {
        User[] users =
        [
            new("B@example.com", 2),
            new("a@example.com", 1)
        ];

        var ex = Record.Exception(() =>
            users.Should().BeInDescendingOrder((User user) => user.Email, StringComparer.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public void BeInDescendingOrder_ByKey_ThrowsArgumentNullException_WhenKeySelectorIsNull()
    {
        User[] users = [new("a@example.com", 1)];
        Func<User, int>? selector = null;

        var ex = Assert.Throws<ArgumentNullException>(() => users.Should().BeInDescendingOrder(selector!));

        Assert.Equal("keySelector", ex.ParamName);
    }

    [Fact]
    public void BeInDescendingOrder_ByKey_WithComparer_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        User[] users = [new("a@example.com", 1)];
        IComparer<int>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            users.Should().BeInDescendingOrder((User user) => user.Rank, comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public void BeInDescendingOrder_WithComparer_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        string[] values = ["alpha"];
        StringComparer? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            values.Should().BeInDescendingOrder(comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }
}
