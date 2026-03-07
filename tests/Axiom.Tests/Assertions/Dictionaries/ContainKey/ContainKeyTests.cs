using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Dictionaries.ContainKey;

public sealed class ContainKeyTests
{
    [Fact]
    public void ContainKey_ReturnsContinuation_WhenKeyExists()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainKey("alpha");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainKey_ExposesWhoseValue_WhenKeyExists()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var continuation = values.Should().ContainKey("alpha");

        Assert.Equal(1, continuation.WhoseValue);
    }

    [Fact]
    public void ContainKey_IReadOnlyDictionary_ExposesWhoseValue_WhenKeyExists()
    {
        IReadOnlyDictionary<string, int> values = new Dictionary<string, int>
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var continuation = values.Should().ContainKey("beta");

        Assert.Equal(2, continuation.WhoseValue);
    }

    [Fact]
    public void ContainKey_Throws_WhenKeyDoesNotExist()
    {
        Dictionary<string, int> values = new()
        {
            ["beta"] = 2,
            ["alpha"] = 1,
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainKey("gamma"));

        const string expected = "Expected values to contain key \"gamma\", but found keys were [\"alpha\", \"beta\"].";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainKey_Throws_WithReason_WhenProvided()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainKey("gamma", "authorisation rules require a gamma key"));

        Assert.Contains("because authorisation rules require a gamma key", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void WhoseValue_ThrowsExplicitMessage_WhenContainKeyFailedInsideBatch()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
        };

        var batch = new Axiom.Core.Batch();
        var continuation = values.Should().ContainKey("gamma");

        var ex = Assert.Throws<InvalidOperationException>(() => _ = continuation.WhoseValue);

        const string failureMessage = "Expected values to contain key \"gamma\", but found keys were [\"alpha\"].";
        var expected = $"WhoseValue is unavailable because ContainKey failed with error: {failureMessage}";
        Assert.Equal(expected, ex.Message);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }
}
