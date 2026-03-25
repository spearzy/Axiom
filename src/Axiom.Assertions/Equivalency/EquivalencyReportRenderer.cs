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
            builder.Append(RenderDifference(subjectLabel, difference, valueFormatter));
        }

        if (take < differences.Count)
        {
            builder.AppendLine();
            builder.Append("+ ");
            builder.Append(differences.Count - take);
            builder.Append(" additional difference(s) omitted after reaching MaxDifferences = ");
            builder.Append(maxDifferences);
            builder.Append('.');
        }

        return builder.ToString();
    }

    private static string RenderDifference(
        string subjectLabel,
        EquivalencyDifference difference,
        IValueFormatter valueFormatter)
    {
        var builder = new StringBuilder();
        builder.Append(BuildPathLabel(subjectLabel, difference));
        builder.Append(": ");

        switch (difference.Kind)
        {
            case EquivalencyDifferenceKind.MissingMemberOnActual:
                builder.Append("member is missing on actual; expected ");
                builder.Append(valueFormatter.Format(difference.Expected));
                break;

            case EquivalencyDifferenceKind.MissingMemberOnExpected:
                builder.Append("member is present on actual but missing on expected; actual ");
                builder.Append(valueFormatter.Format(difference.Actual));
                break;

            case EquivalencyDifferenceKind.CollectionItemMissingOnActual:
                builder.Append("actual collection is missing item; expected ");
                builder.Append(valueFormatter.Format(difference.Expected));
                break;

            case EquivalencyDifferenceKind.CollectionItemExtraOnActual:
                builder.Append("actual collection contains an extra item; actual ");
                builder.Append(valueFormatter.Format(difference.Actual));
                break;

            case EquivalencyDifferenceKind.ExpectedCollectionItemNotFound:
                builder.Append("expected collection item was not found in actual collection; expected ");
                builder.Append(valueFormatter.Format(difference.Expected));
                break;

            case EquivalencyDifferenceKind.ActualCollectionContainsExtraItem:
                builder.Append("actual collection contains an extra item; actual ");
                builder.Append(valueFormatter.Format(difference.Actual));
                break;

            default:
                builder.Append("expected ");
                builder.Append(valueFormatter.Format(difference.Expected));
                builder.Append(", but found ");
                builder.Append(valueFormatter.Format(difference.Actual));
                break;
        }

        if (!string.IsNullOrWhiteSpace(difference.Detail))
        {
            builder.Append(" (");
            if (difference.Kind == EquivalencyDifferenceKind.StringMismatch)
            {
                builder.Append("string mismatch; ");
            }

            builder.Append(difference.Detail);
            builder.Append(')');
        }

        return builder.ToString();
    }

    private static string BuildPathLabel(string subjectLabel, EquivalencyDifference difference)
    {
        if (string.IsNullOrWhiteSpace(difference.ExpectedPath))
        {
            return difference.Path;
        }

        var actualRelativePath = ToRelativePath(difference.Path, subjectLabel);
        if (string.Equals(actualRelativePath, difference.ExpectedPath, StringComparison.Ordinal))
        {
            return difference.Path;
        }

        return $"{difference.Path} (compared with expected.{difference.ExpectedPath})";
    }

    private static string ToRelativePath(string path, string rootPath)
    {
        if (path.Equals(rootPath, StringComparison.Ordinal))
        {
            return string.Empty;
        }

        if (path.StartsWith($"{rootPath}.", StringComparison.Ordinal))
        {
            return path[(rootPath.Length + 1)..];
        }

        if (path.StartsWith($"{rootPath}[", StringComparison.Ordinal))
        {
            return path[rootPath.Length..];
        }

        return path;
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
