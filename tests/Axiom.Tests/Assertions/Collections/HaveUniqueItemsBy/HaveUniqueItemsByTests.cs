using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.HaveUniqueItemsBy;

public sealed class HaveUniqueItemsByTests
{
    private sealed record User(int Id, string? Email);

    [Fact]
    public void HaveUniqueItemsBy_ReturnsContinuation_WhenSelectedKeysAreUnique()
    {
        User[] values =
        [
            new(1, "alpha@example.com"),
            new(2, "beta@example.com"),
            new(3, "gamma@example.com")
        ];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.HaveUniqueItemsBy((User user) => user.Id);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void HaveUniqueItemsBy_Throws_WhenDuplicateSelectedKeyExists()
    {
        User[] values =
        [
            new(1, "alpha@example.com"),
            new(2, "beta@example.com"),
            new(1, "gamma@example.com")
        ];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().HaveUniqueItemsBy((User user) => user.Id));

        const string expected =
            "Expected values to have unique items by selected key, but found first duplicate selected key at index 2: 1.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void HaveUniqueItemsBy_Throws_WhenDuplicateSelectedNullKeyExists()
    {
        User[] values =
        [
            new(1, null),
            new(2, "beta@example.com"),
            new(3, null)
        ];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().HaveUniqueItemsBy((User user) => user.Email));

        const string expected =
            "Expected values to have unique items by selected key, but found first duplicate selected key at index 2: <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void HaveUniqueItemsBy_WithComparer_Throws_WhenComparerTreatsKeysAsEqual()
    {
        User[] values =
        [
            new(1, "ALPHA@example.com"),
            new(2, "alpha@example.com")
        ];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().HaveUniqueItemsBy((User user) => user.Email, StringComparer.OrdinalIgnoreCase));

        Assert.Contains("first duplicate selected key at index 1", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveUniqueItemsBy_WithComparer_DoesNotThrow_WhenComparerTreatsKeysAsDistinct()
    {
        User[] values =
        [
            new(1, "ALPHA@example.com"),
            new(2, "alpha@example.com")
        ];

        var ex = Record.Exception(() =>
            values.Should().HaveUniqueItemsBy((User user) => user.Email, StringComparer.Ordinal));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveUniqueItemsBy_Throws_WithReason_WhenProvided()
    {
        User[] values =
        [
            new(1, "alpha@example.com"),
            new(2, "beta@example.com"),
            new(1, "gamma@example.com")
        ];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().HaveUniqueItemsBy((User user) => user.Id, "user ids must be unique"));

        Assert.Contains("because user ids must be unique", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveUniqueItemsBy_Throws_WhenCollectionIsNull()
    {
        User[]? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values!.Should().HaveUniqueItemsBy((User user) => user.Id));

        const string expected = "Expected values to have unique items by selected key, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void HaveUniqueItemsBy_DoesNotThrow_WhenCollectionIsEmpty()
    {
        User[] values = [];

        var ex = Record.Exception(() => values.Should().HaveUniqueItemsBy((User user) => user.Id));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveUniqueItemsBy_ThrowsArgumentNullException_WhenKeySelectorIsNull()
    {
        User[] values = [new(1, "alpha@example.com")];
        Func<User, int>? selector = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            values.Should().HaveUniqueItemsBy(selector!));

        Assert.Equal("keySelector", ex.ParamName);
    }

    [Fact]
    public void HaveUniqueItemsBy_WithComparer_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        User[] values = [new(1, "alpha@example.com")];
        IEqualityComparer<int>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            values.Should().HaveUniqueItemsBy((User user) => user.Id, comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }
}
