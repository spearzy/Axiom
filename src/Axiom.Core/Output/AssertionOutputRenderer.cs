using System.Text;

namespace Axiom.Core.Output;

public static class AssertionOutputRenderer
{
    private const string AnsiReset = "\u001b[0m";
    private const string AnsiGreen = "\u001b[32m";
    private const string AnsiRed = "\u001b[31m";
    private const string AnsiCyan = "\u001b[36m";
    private const string AnsiDim = "\u001b[2m";

    public static string RenderPass(
        string assertionName,
        string? subjectLabel,
        string? callerFilePath,
        int callerLineNumber,
        AssertionOutputOptions options)
    {
        var builder = new StringBuilder();
        builder.Append(Colorize("PASS", AnsiGreen, options.UseColors));
        builder.Append(' ');
        builder.Append(assertionName);
        builder.Append(' ');
        builder.Append(string.IsNullOrWhiteSpace(subjectLabel) ? "<subject>" : subjectLabel);

        AppendLocation(builder, callerFilePath, callerLineNumber, options.UseColors);
        return builder.ToString();
    }

    public static string RenderFailure(
        string failureMessage,
        string? callerFilePath,
        int callerLineNumber,
        AssertionOutputOptions options)
    {
        var builder = new StringBuilder();
        builder.Append(Colorize("FAIL", AnsiRed, options.UseColors));
        builder.Append(' ');
        builder.Append(failureMessage);

        AppendLocation(builder, callerFilePath, callerLineNumber, options.UseColors);
        if (options.IncludeSourceLine)
        {
            AppendSourceLine(builder, callerFilePath, callerLineNumber, options.UseColors);
        }

        return builder.ToString();
    }

    private static void AppendLocation(
        StringBuilder builder,
        string? callerFilePath,
        int callerLineNumber,
        bool useColors)
    {
        if (string.IsNullOrWhiteSpace(callerFilePath) || callerLineNumber <= 0)
        {
            return;
        }

        builder.AppendLine();
        builder.Append(Colorize("  at ", AnsiDim, useColors));
        builder.Append(Colorize(Path.GetFileName(callerFilePath), AnsiCyan, useColors));
        builder.Append(':');
        builder.Append(callerLineNumber);
    }

    private static void AppendSourceLine(
        StringBuilder builder,
        string? callerFilePath,
        int callerLineNumber,
        bool useColors)
    {
        var sourceLine = TryReadSourceLine(callerFilePath, callerLineNumber);
        if (string.IsNullOrWhiteSpace(sourceLine))
        {
            return;
        }

        builder.AppendLine();
        builder.Append(Colorize("  > ", AnsiDim, useColors));
        builder.Append(sourceLine);
    }

    private static string? TryReadSourceLine(string? callerFilePath, int callerLineNumber)
    {
        if (string.IsNullOrWhiteSpace(callerFilePath) || callerLineNumber <= 0 || !File.Exists(callerFilePath))
        {
            return null;
        }

        try
        {
            using var reader = new StreamReader(callerFilePath);
            for (var currentLine = 1; currentLine < callerLineNumber; currentLine++)
            {
                if (reader.ReadLine() is null)
                {
                    return null;
                }
            }

            return reader.ReadLine()?.Trim();
        }
        catch
        {
            return null;
        }
    }

    private static string Colorize(string value, string color, bool useColors)
    {
        return useColors ? $"{color}{value}{AnsiReset}" : value;
    }
}
