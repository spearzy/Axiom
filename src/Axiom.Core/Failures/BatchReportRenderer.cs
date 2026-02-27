using System.Text;

namespace Axiom;

public static class BatchReportRenderer
{
    public static string Render(string? batchName, IReadOnlyList<string> failures)
    {
        var header = batchName is { Length: > 0 }
            ? $"Batch '{batchName}' failed with {failures.Count} assertion failure(s):"
            : $"Batch failed with {failures.Count} assertion failure(s):";

        var builder = new StringBuilder(header);
        for (var i = 0; i < failures.Count; i++)
        {
            builder.AppendLine();
            builder.Append(i + 1);
            builder.Append(") ");
            builder.Append(failures[i]);
        }

        return builder.ToString();
    }
}
