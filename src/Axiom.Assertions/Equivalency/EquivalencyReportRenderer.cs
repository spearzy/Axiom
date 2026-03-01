using System.Text;
using Axiom.Core.Configuration;
using Axiom.Core.Formatting;

namespace Axiom.Assertions.Equivalency;

internal static class EquivalencyReportRenderer
{
    public static string Render(
        string subjectLabel,
        object? expected,
        IReadOnlyList<EquivalencyDifference> differences,
        int maxDifferences,
        string? because,
        IValueFormatter? formatter = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subjectLabel);
        ArgumentNullException.ThrowIfNull(differences);

        if (differences.Count == 0)
        {
            throw new ArgumentException("At least one difference is required.", nameof(differences));
        }

        var valueFormatter = formatter ?? AxiomServices.Configuration.ValueFormatter;
        var reasonClause = RenderReasonClause(because);

        var builder = new StringBuilder();
        builder.Append("Expected ");
        builder.Append(subjectLabel);
        builder.Append(" to be equivalent to ");
        builder.Append(valueFormatter.Format(expected));
        builder.Append(reasonClause);
        builder.Append(", but found ");
        builder.Append(differences.Count);
        builder.Append(" difference(s):");

        var take = maxDifferences <= 0
            ? differences.Count
            : Math.Min(maxDifferences, differences.Count);

        for (var i = 0; i < take; i++)
        {
            var difference = differences[i];

            builder.AppendLine();
            builder.Append(i + 1);
            builder.Append(") ");
            builder.Append(difference.Path);
            builder.Append(" -> expected ");
            builder.Append(valueFormatter.Format(difference.Expected));
            builder.Append(", but found ");
            builder.Append(valueFormatter.Format(difference.Actual));

            if (!string.IsNullOrWhiteSpace(difference.Detail))
            {
                builder.Append(" (");
                builder.Append(difference.Detail);
                builder.Append(')');
            }
        }

        if (take < differences.Count)
        {
            builder.AppendLine();
            builder.Append("+ ");
            builder.Append(differences.Count - take);
            builder.Append(" more difference(s).");
        }

        return builder.ToString();
    }

    private static string RenderReasonClause(string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return string.Empty;
        }

        return $" because {reason.Trim()}";
    }
}
