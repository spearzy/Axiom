using System.Globalization;
using Axiom.Core.Formatting;

namespace Axiom.Vectors;

internal static class RankingAssertionHelpers
{
    private static readonly IValueFormatter Formatter = DefaultValueFormatter.Instance;

    public static string BuildFailureMessage(string subjectLabel, string expectation, string actualDetail, string? because)
        => $"Expected {subjectLabel} {expectation}{BuildReasonClause(because)}, but {actualDetail}.";

    public static string FormatMetric(double value)
        => value.ToString("G17", CultureInfo.InvariantCulture);

    public static string FormatValue(object? value)
        => Formatter.Format(value);

    public static void ValidatePositive(int value, string paramName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be greater than zero.");
        }
    }

    public static void ValidateMetric(double value, string paramName)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0d || value > 1d)
        {
            throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be a finite value between 0 and 1.");
        }
    }

    public static void ValidateTolerance(double tolerance)
    {
        if (double.IsNaN(tolerance) || double.IsInfinity(tolerance) || tolerance < 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance must be a finite value greater than or equal to zero.");
        }
    }

    public static HashSet<T> CreateRelevantSet<T>(IEnumerable<T> relevantItems, string paramName)
    {
        ArgumentNullException.ThrowIfNull(relevantItems);

        var relevantSet = new HashSet<T>(relevantItems);
        if (relevantSet.Count == 0)
        {
            throw new ArgumentException($"{paramName} must contain at least one unique item.", paramName);
        }

        return relevantSet;
    }

    public static int? FindFirstRank<T>(IReadOnlyList<T> results, T target)
    {
        var comparer = EqualityComparer<T>.Default;
        for (var index = 0; index < results.Count; index++)
        {
            if (comparer.Equals(results[index], target))
            {
                return index + 1;
            }
        }

        return null;
    }

    public static int CountUniqueRelevantHits<T>(IReadOnlyList<T> results, int k, IReadOnlySet<T> relevantItems)
    {
        var matched = new HashSet<T>();
        var limit = Math.Min(k, results.Count);
        for (var index = 0; index < limit; index++)
        {
            var item = results[index];
            if (relevantItems.Contains(item))
            {
                matched.Add(item);
            }
        }

        return matched.Count;
    }

    public static int? FindFirstRelevantRank<T>(IReadOnlyList<T> results, IReadOnlySet<T> relevantItems)
    {
        for (var index = 0; index < results.Count; index++)
        {
            if (relevantItems.Contains(results[index]))
            {
                return index + 1;
            }
        }

        return null;
    }

    private static string BuildReasonClause(string? because)
        => string.IsNullOrWhiteSpace(because) ? string.Empty : $" because {because.Trim()}";
}
