using Axiom.Core.Configuration;

namespace Axiom.Core.Output;

public static class AssertionOutputWriter
{
    public static void ReportPass(
        string assertionName,
        string? subjectLabel,
        string? callerFilePath,
        int callerLineNumber)
    {
        var options = AxiomServices.Configuration.Output;
        if (!options.Enabled || !options.ShowPasses)
        {
            return;
        }

        var message = AssertionOutputRenderer.RenderPass(
            assertionName,
            subjectLabel,
            callerFilePath,
            callerLineNumber,
            options);
        Console.WriteLine(message);
    }

    public static void ReportFailure(
        string failureMessage,
        string? callerFilePath,
        int callerLineNumber)
    {
        var options = AxiomServices.Configuration.Output;
        if (!options.Enabled)
        {
            return;
        }

        var message = AssertionOutputRenderer.RenderFailure(
            failureMessage,
            callerFilePath,
            callerLineNumber,
            options);
        Console.Error.WriteLine(message);
    }
}
