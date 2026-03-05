using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Strings.Match;

public sealed class MatchTests : IDisposable
{
    private const string SlowRegexPattern = "^(a+)+$";
    private static readonly string SlowRegexInput = new string('a', 250_000) + "X";

    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void Match_ReturnsContinuation_WhenValueMatchesPattern()
    {
        const string value = "AB-123";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.Match(@"^[A-Z]{2}-\d{3}$");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void Match_Throws_WhenValueDoesNotMatchPattern()
    {
        const string value = "AB-12";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Match(@"^[A-Z]{2}-\d{3}$"));

        const string expected = "Expected value to match regex \"^[A-Z]{2}-\\d{3}$\", but found \"AB-12\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Match_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Match(@"\w+"));

        const string expected = "Expected value to match regex \"\\w+\", but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Match_Throws_WithReason_WhenProvided()
    {
        const string value = "AB-12";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().Match(@"^[A-Z]{2}-\d{3}$", "ticket IDs must include 3 digits"));

        Assert.Contains("because ticket IDs must include 3 digits", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Match_ThrowsArgumentException_WhenPatternIsInvalid()
    {
        const string value = "AB-123";

        var ex = Assert.Throws<ArgumentException>(() => value.Should().Match("["));

        Assert.Equal("pattern", ex.ParamName);
        Assert.Contains("Invalid regex pattern", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Match_ThrowsArgumentOutOfRangeException_WhenTimeoutIsNotPositive()
    {
        const string value = "AB-123";

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            value.Should().Match(@"^[A-Z]{2}-\d{3}$", TimeSpan.Zero));

        Assert.Equal("timeout", ex.ParamName);
    }

    [Fact]
    public void Match_ThrowsArgumentOutOfRangeException_WhenConfiguredRegexTimeoutIsNotPositive()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            AxiomServices.Configure(config => config.RegexMatchTimeout = TimeSpan.Zero));

        Assert.Equal("RegexMatchTimeout", ex.ParamName);
    }

    [Fact]
    public void Match_UsesPerCallTimeoutOverride_WhenRegexEvaluationTimesOut()
    {
        var value = SlowRegexInput;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().Match(SlowRegexPattern, TimeSpan.FromMilliseconds(1)));

        Assert.Contains("regex evaluation timed out after 1 ms", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Match_UsesConfiguredGlobalTimeout_WhenRegexEvaluationTimesOut()
    {
        AxiomServices.Configure(config => config.RegexMatchTimeout = TimeSpan.FromMilliseconds(2));
        var value = SlowRegexInput;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Match(SlowRegexPattern));

        Assert.Contains("regex evaluation timed out after 2 ms", ex.Message, StringComparison.Ordinal);
    }
}
