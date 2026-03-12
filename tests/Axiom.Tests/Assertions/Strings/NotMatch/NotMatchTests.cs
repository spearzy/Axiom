using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Strings.NotMatch;

public sealed class NotMatchTests
{
    private const string SlowRegexPattern = "^(a+)+$";
    private static readonly string SlowRegexInput = new string('a', 250_000) + "X";

    [Fact]
    public void NotMatch_ReturnsContinuation_WhenValueDoesNotMatchPattern()
    {
        const string value = "AB-12";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotMatch(@"^[A-Z]{2}-\d{3}$");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotMatch_Throws_WhenValueMatchesPattern()
    {
        const string value = "AB-123";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotMatch(@"^[A-Z]{2}-\d{3}$"));

        const string expected = "Expected value to not match regex \"^[A-Z]{2}-\\d{3}$\", but found \"AB-123\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotMatch_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotMatch(@"\w+"));

        const string expected = "Expected value to not match regex \"\\w+\", but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotMatch_Throws_WithReason_WhenProvided()
    {
        const string value = "AB-123";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotMatch(@"^[A-Z]{2}-\d{3}$", "legacy IDs should not pass the new format"));

        Assert.Contains("because legacy IDs should not pass the new format", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotMatch_ThrowsArgumentException_WhenPatternIsInvalid()
    {
        const string value = "AB-123";

        var ex = Assert.Throws<ArgumentException>(() => value.Should().NotMatch("["));

        Assert.Equal("pattern", ex.ParamName);
        Assert.Contains("Invalid regex pattern", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotMatch_ThrowsArgumentOutOfRangeException_WhenTimeoutIsNotPositive()
    {
        const string value = "AB-123";

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            value.Should().NotMatch(@"^[A-Z]{2}-\d{3}$", TimeSpan.FromMilliseconds(-1)));

        Assert.Equal("timeout", ex.ParamName);
    }

    [Fact]
    public void NotMatch_UsesPerCallTimeoutOverride_WhenRegexEvaluationTimesOut()
    {
        var value = SlowRegexInput;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotMatch(SlowRegexPattern, TimeSpan.FromMilliseconds(1)));

        Assert.Contains("regex evaluation timed out after 1 ms", ex.Message, StringComparison.Ordinal);
    }
}
