using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.BeInAscendingOrder;

public sealed class BeInAscendingOrderTests
{
    private sealed record User(string Email, int Rank);
    private sealed class NonComparable
    {
        public int Value { get; init; }
    }

    [Fact]
    public void BeInAscendingOrder_ReturnsContinuation_WhenItemsAreAscending()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.BeInAscendingOrder();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeInAscendingOrder_Throws_WhenItemsAreOutOfOrder()
    {
        int[] values = [1, 3, 2];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().BeInAscendingOrder());

        const string expected =
            "Expected values to be in ascending order, but found first out-of-order pair at index 2: previous 3 then current 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeInAscendingOrder_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 3, 2];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().BeInAscendingOrder("records must be sorted by id"));

        Assert.Contains("because records must be sorted by id", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeInAscendingOrder_Throws_WhenCollectionIsNull()
    {
        int[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().BeInAscendingOrder());

        const string expected = "Expected values to be in ascending order, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeInAscendingOrder_DoesNotThrow_WhenCollectionIsEmpty()
    {
        int[] values = [];

        var ex = Record.Exception(() => values.Should().BeInAscendingOrder());

        Assert.Null(ex);
    }

    [Fact]
    public void BeInAscendingOrder_WithComparer_UsesProvidedComparer()
    {
        string[] values = ["alpha", "Beta", "charlie"];

        var ex = Record.Exception(() =>
            values.Should().BeInAscendingOrder(StringComparer.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public void BeInAscendingOrder_Throws_WhenTypeHasNoDefaultOrdering()
    {
        NonComparable[] values =
        [
            new() { Value = 1 },
            new() { Value = 2 }
        ];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().BeInAscendingOrder());

        Assert.Contains("do not define a default ordering", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeInAscendingOrder_ByKey_ReturnsContinuation_WhenSelectedKeysAreAscending()
    {
        User[] users =
        [
            new("a@example.com", 1),
            new("b@example.com", 2),
            new("c@example.com", 3)
        ];

        var baseAssertions = users.Should();
        var continuation = baseAssertions.BeInAscendingOrder((User user) => user.Rank);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeInAscendingOrder_ByKey_Throws_WhenSelectedKeysAreOutOfOrder()
    {
        User[] users =
        [
            new("a@example.com", 1),
            new("b@example.com", 3),
            new("c@example.com", 2)
        ];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            users.Should().BeInAscendingOrder((User user) => user.Rank));

        const string expected =
            "Expected users to be in ascending order by selected key, but found first out-of-order selected key pair at index 2: previous 3 then current 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeInAscendingOrder_ByKey_WithComparer_UsesProvidedComparer()
    {
        User[] users =
        [
            new("a@example.com", 1),
            new("B@example.com", 2)
        ];

        var ex = Record.Exception(() =>
            users.Should().BeInAscendingOrder((User user) => user.Email, StringComparer.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public void BeInAscendingOrder_ByKey_ThrowsArgumentNullException_WhenKeySelectorIsNull()
    {
        User[] users = [new("a@example.com", 1)];
        Func<User, int>? selector = null;

        var ex = Assert.Throws<ArgumentNullException>(() => users.Should().BeInAscendingOrder(selector!));

        Assert.Equal("keySelector", ex.ParamName);
    }

    [Fact]
    public void BeInAscendingOrder_ByKey_WithComparer_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        User[] users = [new("a@example.com", 1)];
        IComparer<int>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            users.Should().BeInAscendingOrder((User user) => user.Rank, comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public void BeInAscendingOrder_WithComparer_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        string[] values = ["alpha"];
        StringComparer? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            values.Should().BeInAscendingOrder(comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }
}
