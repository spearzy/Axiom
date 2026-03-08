using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.EndWith;

public sealed class EndWithTests
{
    [Fact]
    public void EndWith_ReturnsContinuation_WhenValueEndsWithExpectedValue()
    {
        string value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.EndWith("st");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void EndWith_Throws_WhenValueDoesNotEndWithExpectedValue()
    {
        string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().EndWith("ab"));

        Assert.Contains("value", ex.Message);
        Assert.Contains("end with", ex.Message);
        Assert.Contains("ab", ex.Message);
        Assert.Contains("test", ex.Message);
        Assert.Contains("end comparison", ex.Message, StringComparison.Ordinal);
        Assert.Contains("first difference at expected index 0, actual index 2", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void EndWith_DoesNotThrow_WhenComparisonIgnoresCase()
    {
        const string value = "TEST";

        var ex = Record.Exception(() =>
            value.Should().EndWith("st", StringComparison.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public void EndWith_Throws_WhenComparisonIsCaseSensitive()
    {
        const string value = "TEST";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().EndWith("st", StringComparison.Ordinal));

        Assert.Contains("Expected value to end with \"st\"", ex.Message, StringComparison.Ordinal);
        Assert.Contains("end comparison", ex.Message, StringComparison.Ordinal);
    }
}
