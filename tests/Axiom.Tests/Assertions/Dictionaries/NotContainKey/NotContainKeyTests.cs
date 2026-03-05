using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Dictionaries.NotContainKey;

public sealed class NotContainKeyTests
{
    [Fact]
    public void NotContainKey_ReturnsContinuation_WhenKeyDoesNotExist()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainKey("gamma");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainKey_Throws_WhenKeyExists()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().NotContainKey("alpha"));

        const string expected = "Expected values to not contain key \"alpha\", but found key was present with value 1.";
        Assert.Equal(expected, ex.Message);
    }
}
