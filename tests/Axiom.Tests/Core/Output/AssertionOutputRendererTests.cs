namespace Axiom.Tests.Core.Output;

public sealed class AssertionOutputRendererTests
{
    [Fact]
    public void RenderPass_WithoutColors_IncludesAssertionSubjectAndLocation()
    {
        var options = new AssertionOutputOptions
        {
            UseColors = false,
            IncludeSourceLine = false,
        };

        var message = AssertionOutputRenderer.RenderPass(
            "Contain",
            "values",
            "/tmp/Sample.cs",
            12,
            options);

        const string expected = "PASS Contain values\n  at Sample.cs:12";
        Xunit.Assert.Equal(expected, Normalise(message));
    }

    [Fact]
    public void RenderFailure_WithoutColors_IncludesLocationAndSourceLine()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllLines(tempFile, ["first line", "second line"]);
            var options = new AssertionOutputOptions
            {
                UseColors = false,
                IncludeSourceLine = true,
            };

            var message = AssertionOutputRenderer.RenderFailure(
                "Expected values to contain 9, but found System.Int32[].",
                tempFile,
                2,
                options);

            var expectedPrefix = $"FAIL Expected values to contain 9, but found System.Int32[].\n  at {Path.GetFileName(tempFile)}:2\n  > second line";
            Xunit.Assert.Equal(expectedPrefix, Normalise(message));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void RenderFailure_WithColors_ContainsAnsiSequences()
    {
        var options = new AssertionOutputOptions
        {
            UseColors = true,
            IncludeSourceLine = false,
        };

        var message = AssertionOutputRenderer.RenderFailure(
            "Expected value to be 7, but found 42.",
            null,
            0,
            options);

        Xunit.Assert.Contains("\u001b[31m", message);
        Xunit.Assert.Contains("FAIL", message);
    }

    private static string Normalise(string value)
    {
        return value.Replace("\r\n", "\n", StringComparison.Ordinal);
    }
}
