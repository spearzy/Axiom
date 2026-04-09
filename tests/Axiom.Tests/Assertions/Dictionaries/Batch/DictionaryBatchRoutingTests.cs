using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Dictionaries.Batch;

public sealed class DictionaryBatchRoutingTests
{
    [Fact]
    public void ContainKey_OutsideBatch_ThrowsImmediately()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        Assert.Throws<InvalidOperationException>(() => values.Should().ContainKey("gamma"));
    }

    [Fact]
    public void ContainEntry_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().ContainEntry("beta", 9));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void WhoseValue_InsideBatch_ThrowsExplicitMessage_WhenContainKeyFailed()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var batch = new Axiom.Core.Batch();
        var continuation = values.Should().ContainKey("gamma");
        var ex = Assert.Throws<InvalidOperationException>(() => _ = continuation.WhoseValue);

        const string failureMessage = "Expected values to contain key \"gamma\", but found keys were [\"alpha\", \"beta\"].";
        var expected = $"WhoseValue is unavailable because ContainKey failed with error: {failureMessage}";
        Assert.Equal(expected, ex.Message);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void WhoseValue_InsideBatch_ReturnsValue_WhenContainKeyPassed()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var batch = new Axiom.Core.Batch();
        var continuation = values.Should().ContainKey("beta");

        Assert.Equal(2, continuation.WhoseValue);
        var disposeEx = Record.Exception(() => batch.Dispose());
        Assert.Null(disposeEx);
    }

    [Fact]
    public void NotContainEntry_OutsideBatch_ThrowsImmediately()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        Assert.Throws<InvalidOperationException>(() => values.Should().NotContainEntry("beta", 2));
    }

    [Fact]
    public void NotContainEntry_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().NotContainEntry("beta", 2));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotContainEntry_IReadOnlyDictionary_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        IReadOnlyDictionary<string, int> values = new Dictionary<string, int>
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().NotContainEntry("beta", 2));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void DictionaryValueComparerAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
            ["beta"] = "Queued",
        };

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
        {
            values.Should().ContainValue("missing", StringComparer.OrdinalIgnoreCase);
            values.Should().NotContainValue("queued", StringComparer.OrdinalIgnoreCase);
            values.Should().ContainEntry("beta", "missing", StringComparer.OrdinalIgnoreCase);
            values.Should().NotContainEntry("beta", "queued", StringComparer.OrdinalIgnoreCase);
        });

        Assert.Null(callEx);

        var disposeEx = Assert.Throws<InvalidOperationException>(() => batch.Dispose());
        Assert.Contains("Expected values to contain value \"missing\"", disposeEx.Message);
        Assert.Contains("Expected values to not contain value \"queued\"", disposeEx.Message);
        Assert.Contains("Expected values to contain entry \"beta\" => \"missing\"", disposeEx.Message);
        Assert.Contains("Expected values to not contain entry \"beta\" => \"queued\"", disposeEx.Message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromDictionaryAssertions()
    {
        Dictionary<string, int> values = new()
        {
            ["beta"] = 2,
            ["alpha"] = 1,
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("dictionary-rules");
            values.Should().ContainKey("gamma");
            values.Should().ContainValue(9);
            values.Should().NotContainValue(2);
            values.Should().ContainEntry("alpha", 5);
            values.Should().NotContainEntry("beta", 2);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'dictionary-rules' failed with 5 assertion failure(s):", message);
        Assert.Contains("1) Expected values to contain key \"gamma\", but found keys were [\"alpha\", \"beta\"].", message);
        Assert.Contains("2) Expected values to contain value 9, but found values were [1, 2].", message);
        Assert.Contains("3) Expected values to not contain value 2, but found a matching value at key \"beta\".", message);
        Assert.Contains("4) Expected values to contain entry \"alpha\" => 5, but found key \"alpha\" had value 1.", message);
        Assert.Contains("5) Expected values to not contain entry \"beta\" => 2, but found matching entry was present: \"beta\" => 2.", message);
    }

    [Fact]
    public void DictionaryValueComparerAssertions_InsideBatch_PreserveFailureCallOrder()
    {
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
            ["beta"] = "Queued",
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("dictionary-rules");
            values.Should().ContainValue("missing", StringComparer.OrdinalIgnoreCase);
            values.Should().NotContainValue("queued", StringComparer.OrdinalIgnoreCase);
            values.Should().ContainEntry("beta", "missing", StringComparer.OrdinalIgnoreCase);
            values.Should().NotContainEntry("beta", "queued", StringComparer.OrdinalIgnoreCase);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        var containValueIndex = message.IndexOf("1) Expected values to contain value \"missing\"", StringComparison.Ordinal);
        var notContainValueIndex = message.IndexOf("2) Expected values to not contain value \"queued\"", StringComparison.Ordinal);
        var containEntryIndex = message.IndexOf("3) Expected values to contain entry \"beta\" => \"missing\"", StringComparison.Ordinal);
        var notContainEntryIndex = message.IndexOf("4) Expected values to not contain entry \"beta\" => \"queued\"", StringComparison.Ordinal);

        Assert.True(containValueIndex >= 0, message);
        Assert.True(notContainValueIndex > containValueIndex, message);
        Assert.True(containEntryIndex > notContainValueIndex, message);
        Assert.True(notContainEntryIndex > containEntryIndex, message);
    }
}
