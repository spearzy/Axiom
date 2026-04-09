using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Dictionaries.ContainEntry;

public sealed class ContainEntryTests
{
    [Fact]
    public void ContainEntry_ReturnsContinuation_WhenMatchingEntryExists()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainEntry("beta", 2);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainEntry_DoesNotFormatExpectedEntry_WhenAssertionPasses()
    {
        Dictionary<string, ThrowingToStringValue> values = new()
        {
            ["alpha"] = new ThrowingToStringValue(1),
            ["beta"] = new ThrowingToStringValue(2),
        };

        var ex = Record.Exception(() => values.Should().ContainEntry("beta", new ThrowingToStringValue(2)));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainEntry_Throws_WhenKeyDoesNotExist()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainEntry("gamma", 9));

        const string expected = "Expected values to contain entry \"gamma\" => 9, but found key \"gamma\" was missing.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainEntry_Throws_WhenKeyExistsButValueDiffers()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainEntry("beta", 5));

        const string expected = "Expected values to contain entry \"beta\" => 5, but found key \"beta\" had value 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainEntry_ReturnsContinuation_WhenComparerMatchesValue()
    {
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
            ["beta"] = "Queued",
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainEntry("beta", "queued", StringComparer.OrdinalIgnoreCase);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainEntry_KeyLookup_StillUsesDictionaryKeySemantics_WhenValueComparerIsProvided()
    {
        Dictionary<string, string> values = new()
        {
            ["beta"] = "Queued",
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainEntry("BETA", "queued", StringComparer.OrdinalIgnoreCase));

        const string expected = "Expected values to contain entry \"BETA\" => \"queued\", but found key \"BETA\" was missing.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainEntry_KeyLookup_UsesDictionaryComparer_WhenValueComparerIsProvided()
    {
        Dictionary<string, string> values = new(StringComparer.OrdinalIgnoreCase)
        {
            ["beta"] = "Queued",
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainEntry("BETA", "queued", StringComparer.OrdinalIgnoreCase);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainEntry_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        Dictionary<string, string> values = new()
        {
            ["beta"] = "Queued",
        };
        IEqualityComparer<string>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            values.Should().ContainEntry("beta", "queued", comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    private readonly record struct ThrowingToStringValue(int Value)
    {
        public override string ToString()
        {
            throw new InvalidOperationException("ToString should not be called on pass path.");
        }
    }
}
