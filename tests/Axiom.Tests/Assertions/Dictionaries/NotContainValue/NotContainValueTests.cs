using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Dictionaries.NotContainValue;

public sealed class NotContainValueTests
{
    [Fact]
    public void NotContainValue_ReturnsContinuation_WhenValueDoesNotExist()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainValue(9);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainValue_Throws_WhenValueExists()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().NotContainValue(2));

        const string expected = "Expected values to not contain value 2, but found a matching value at key \"beta\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContainValue_Throws_WhenComparerMatchesValue()
    {
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
            ["beta"] = "Queued",
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().NotContainValue("queued", StringComparer.OrdinalIgnoreCase));

        const string expected = "Expected values to not contain value \"queued\", but found a matching value at key \"beta\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContainValue_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
        };
        IEqualityComparer<string>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().NotContainValue("created", comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }
}
