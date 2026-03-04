using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Dictionaries.NotContainEntry;

public sealed class NotContainEntryTests
{
    [Fact]
    public void NotContainEntry_ReturnsContinuation_WhenKeyDoesNotExist()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainEntry("gamma", 9);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainEntry_ReturnsContinuation_WhenKeyExistsButValueDiffers()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainEntry("beta", 9);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainEntry_DoesNotFormatEntry_WhenKeyIsMissing()
    {
        Dictionary<int, ThrowingToStringValue> values = new()
        {
            [1] = new ThrowingToStringValue(1),
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainEntry(2, new ThrowingToStringValue(2));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainEntry_DoesNotFormatEntry_WhenKeyExistsButValueDiffers()
    {
        Dictionary<int, ThrowingToStringValue> values = new()
        {
            [1] = new ThrowingToStringValue(1),
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainEntry(1, new ThrowingToStringValue(2));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainEntry_IReadOnlyDictionary_ReturnsContinuation_WhenKeyDoesNotExist()
    {
        IReadOnlyDictionary<string, int> values = new Dictionary<string, int>
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainEntry("gamma", 9);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainEntry_IReadOnlyDictionary_Throws_WhenMatchingEntryExists()
    {
        IReadOnlyDictionary<string, int> values = new Dictionary<string, int>
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().NotContainEntry("beta", 2));

        const string expected = "Expected values to not contain entry \"beta\" => 2, but found matching entry was present: \"beta\" => 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContainEntry_Throws_WhenMatchingEntryExists()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().NotContainEntry("beta", 2));

        const string expected = "Expected values to not contain entry \"beta\" => 2, but found matching entry was present: \"beta\" => 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContainEntry_Throws_WhenDictionaryIsNull()
    {
        Dictionary<string, int>? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().NotContainEntry("beta", 2));

        const string expected = "Expected values to not contain entry \"beta\" => 2, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContainEntry_Throws_WithReason_WhenProvided()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().NotContainEntry("beta", 2, "deprecated mappings must be removed"));

        Assert.Contains("because deprecated mappings must be removed", ex.Message, StringComparison.Ordinal);
    }

    private sealed class ThrowingToStringValue(int id)
    {
        public int Id { get; } = id;

        public override string ToString()
        {
            throw new InvalidOperationException("ToString should not be called on pass path.");
        }
    }
}
