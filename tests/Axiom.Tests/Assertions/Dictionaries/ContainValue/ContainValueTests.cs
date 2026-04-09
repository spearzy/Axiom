using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Dictionaries.ContainValue;

public sealed class ContainValueTests
{
    [Fact]
    public void ContainValue_ReturnsContinuation_WhenValueExists()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainValue(2);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainValue_Throws_WhenValueDoesNotExist()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainValue(9));

        const string expected = "Expected values to contain value 9, but found values were [1, 2].";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainValue_ReturnsContinuation_WhenComparerMatchesValue()
    {
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
            ["beta"] = "Queued",
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainValue("queued", StringComparer.OrdinalIgnoreCase);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainValue_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
        };
        IEqualityComparer<string>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().ContainValue("created", comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }
}
