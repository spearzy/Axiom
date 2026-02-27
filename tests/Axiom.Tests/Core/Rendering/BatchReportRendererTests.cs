namespace Axiom.Tests;

public sealed class BatchReportRendererTests
{
    [Fact]
    public void Render_WithName_UsesStableOrderingAndNumbering()
    {
        var report = BatchReportRenderer.Render("demo", ["first", "second"]);

        const string expected = "Batch 'demo' failed with 2 assertion failure(s):\n1) first\n2) second";
        Xunit.Assert.Equal(expected, NormaliseNewLines(report));
    }

    [Fact]
    public void Render_WithoutName_UsesDefaultHeader()
    {
        var report = BatchReportRenderer.Render(null, ["only"]);

        const string expected = "Batch failed with 1 assertion failure(s):\n1) only";
        Xunit.Assert.Equal(expected, NormaliseNewLines(report));
    }

    private static string NormaliseNewLines(string value)
    {
        return value.Replace("\r\n", "\n", StringComparison.Ordinal);
    }
}
