using Axiom.Assertions.EntryPoints;
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
}
