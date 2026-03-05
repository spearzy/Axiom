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
}
