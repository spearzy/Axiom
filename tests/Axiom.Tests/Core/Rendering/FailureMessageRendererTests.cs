namespace Axiom.Tests;

public sealed class FailureMessageRendererTests
{
    [Fact]
    public void Render_WithExpectedValue_FormatsStringValuesDeterministically()
    {
        var failure = new Failure(
            "value",
            new Expectation("to start with", "ab"),
            "test");

        var message = FailureMessageRenderer.Render(failure);

        const string expected = "Expected value to start with \"ab\", but found \"test\".";
        Xunit.Assert.Equal(expected, message);
    }

    [Fact]
    public void Render_WithoutExpectedValue_UsesNullTokenForActual()
    {
        var failure = new Failure(
            "value",
            new Expectation("to not be null", IncludeExpectedValue: false),
            null);

        var message = FailureMessageRenderer.Render(failure);

        const string expected = "Expected value to not be null, but found <null>.";
        Xunit.Assert.Equal(expected, message);
    }
}
