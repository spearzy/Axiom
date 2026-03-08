using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.NotBe;

public sealed class NotBeTests
{
    [Fact]
    public void NotBe_ReturnsContinuation_WhenValuesDiffer()
    {
        const string value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBe("prod");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBe_ReturnsContinuation_WhenValueIsNullAndUnexpectedHasContent()
    {
        string? value = null;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBe("prod");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBe_Throws_WhenValuesAreEqual()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBe("test"));

        const string expected = "Expected value to not be \"test\", but found \"test\" (first string difference; strings are identical).";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBe_Throws_WithReason_WhenProvided()
    {
        const string value = "test";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotBe("test", "the value must differ from the baseline"));

        Assert.Contains("because the value must differ from the baseline", ex.Message, StringComparison.Ordinal);
    }
}
