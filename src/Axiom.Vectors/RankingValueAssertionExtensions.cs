using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Vectors;

public static class RankingValueAssertionExtensions
{
    public static AndContinuation<ValueAssertions<TCollection>> ContainInTopK<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        TItem target,
        int k,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        RankingAssertionHelpers.ValidatePositive(k, nameof(k));

        var expectation = $"to contain item {RankingAssertionHelpers.FormatValue(target)} in the top {k} result(s)";
        if (!TryGetResults<TCollection, TItem>(assertions, expectation, because, callerFilePath, callerLineNumber, out var results))
        {
            return new AndContinuation<ValueAssertions<TCollection>>(assertions);
        }

        var rank = RankingAssertionHelpers.FindFirstRank(results, target);
        if (rank is null)
        {
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(
                    SubjectLabel(assertions),
                    expectation,
                    $"item {RankingAssertionHelpers.FormatValue(target)} was missing entirely",
                    because),
                callerFilePath,
                callerLineNumber);
        }
        else if (rank.Value > k)
        {
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(
                    SubjectLabel(assertions),
                    expectation,
                    $"item {RankingAssertionHelpers.FormatValue(target)} was found at rank {rank.Value}",
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveRank<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        TItem target,
        int expectedRank,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        RankingAssertionHelpers.ValidatePositive(expectedRank, nameof(expectedRank));

        var expectation = $"to have item {RankingAssertionHelpers.FormatValue(target)} at rank {expectedRank}";
        if (!TryGetResults<TCollection, TItem>(assertions, expectation, because, callerFilePath, callerLineNumber, out var results))
        {
            return new AndContinuation<ValueAssertions<TCollection>>(assertions);
        }

        var rank = RankingAssertionHelpers.FindFirstRank(results, target);
        if (rank is null)
        {
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(
                    SubjectLabel(assertions),
                    expectation,
                    $"item {RankingAssertionHelpers.FormatValue(target)} was missing entirely",
                    because),
                callerFilePath,
                callerLineNumber);
        }
        else if (rank.Value != expectedRank)
        {
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(
                    SubjectLabel(assertions),
                    expectation,
                    $"item {RankingAssertionHelpers.FormatValue(target)} was found at rank {rank.Value}",
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveRecallAt<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        int k,
        IEnumerable<TItem> relevantItems,
        double expectedRecall,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        RankingAssertionHelpers.ValidatePositive(k, nameof(k));
        RankingAssertionHelpers.ValidateMetric(expectedRecall, nameof(expectedRecall));
        RankingAssertionHelpers.ValidateTolerance(tolerance);
        var relevantSet = RankingAssertionHelpers.CreateRelevantSet(relevantItems, nameof(relevantItems));

        var expectation =
            $"to have recall@{k} equal to {RankingAssertionHelpers.FormatMetric(expectedRecall)} within tolerance {RankingAssertionHelpers.FormatMetric(tolerance)}";
        if (!TryGetResults<TCollection, TItem>(assertions, expectation, because, callerFilePath, callerLineNumber, out var results))
        {
            return new AndContinuation<ValueAssertions<TCollection>>(assertions);
        }

        var matchedCount = RankingAssertionHelpers.CountUniqueRelevantHits(results, k, relevantSet);
        var actualRecall = matchedCount / (double)relevantSet.Count;
        if (double.IsNaN(actualRecall) || Math.Abs(actualRecall - expectedRecall) > tolerance)
        {
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(
                    SubjectLabel(assertions),
                    expectation,
                    $"actual recall@{k} was {RankingAssertionHelpers.FormatMetric(actualRecall)} (matched {matchedCount} of {relevantSet.Count} relevant item(s))",
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HavePrecisionAt<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        int k,
        IEnumerable<TItem> relevantItems,
        double expectedPrecision,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        RankingAssertionHelpers.ValidatePositive(k, nameof(k));
        RankingAssertionHelpers.ValidateMetric(expectedPrecision, nameof(expectedPrecision));
        RankingAssertionHelpers.ValidateTolerance(tolerance);
        var relevantSet = RankingAssertionHelpers.CreateRelevantSet(relevantItems, nameof(relevantItems));

        var expectation =
            $"to have precision@{k} equal to {RankingAssertionHelpers.FormatMetric(expectedPrecision)} within tolerance {RankingAssertionHelpers.FormatMetric(tolerance)}";
        if (!TryGetResults<TCollection, TItem>(assertions, expectation, because, callerFilePath, callerLineNumber, out var results))
        {
            return new AndContinuation<ValueAssertions<TCollection>>(assertions);
        }

        var matchedCount = RankingAssertionHelpers.CountUniqueRelevantHits(results, k, relevantSet);
        var actualPrecision = matchedCount / (double)k;
        if (double.IsNaN(actualPrecision) || Math.Abs(actualPrecision - expectedPrecision) > tolerance)
        {
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(
                    SubjectLabel(assertions),
                    expectation,
                    $"actual precision@{k} was {RankingAssertionHelpers.FormatMetric(actualPrecision)} (matched {matchedCount} unique relevant item(s); denominator {k})",
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveReciprocalRank<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        TItem target,
        double expectedReciprocalRank,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        RankingAssertionHelpers.ValidateMetric(expectedReciprocalRank, nameof(expectedReciprocalRank));
        RankingAssertionHelpers.ValidateTolerance(tolerance);

        var expectation =
            $"to have reciprocal rank for item {RankingAssertionHelpers.FormatValue(target)} equal to {RankingAssertionHelpers.FormatMetric(expectedReciprocalRank)} within tolerance {RankingAssertionHelpers.FormatMetric(tolerance)}";
        if (!TryGetResults<TCollection, TItem>(assertions, expectation, because, callerFilePath, callerLineNumber, out var results))
        {
            return new AndContinuation<ValueAssertions<TCollection>>(assertions);
        }

        var rank = RankingAssertionHelpers.FindFirstRank(results, target);
        var actualReciprocalRank = rank is null ? 0d : 1d / rank.Value;
        if (double.IsNaN(actualReciprocalRank) || Math.Abs(actualReciprocalRank - expectedReciprocalRank) > tolerance)
        {
            var detail = rank is null
                ? $"item {RankingAssertionHelpers.FormatValue(target)} was missing entirely; actual reciprocal rank was 0"
                : $"actual reciprocal rank for item {RankingAssertionHelpers.FormatValue(target)} was {RankingAssertionHelpers.FormatMetric(actualReciprocalRank)} (first found at rank {rank.Value})";

            Fail(
                RankingAssertionHelpers.BuildFailureMessage(
                    SubjectLabel(assertions),
                    expectation,
                    detail,
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<RankingQuery<TItem>[]>> HaveMeanReciprocalRank<TItem>(
        this ValueAssertions<RankingQuery<TItem>[]> assertions,
        double expectedMeanReciprocalRank,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveMeanReciprocalRankCore<RankingQuery<TItem>[], TItem>(assertions, expectedMeanReciprocalRank, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<List<RankingQuery<TItem>>>> HaveMeanReciprocalRank<TItem>(
        this ValueAssertions<List<RankingQuery<TItem>>> assertions,
        double expectedMeanReciprocalRank,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveMeanReciprocalRankCore<List<RankingQuery<TItem>>, TItem>(assertions, expectedMeanReciprocalRank, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<IEnumerable<RankingQuery<TItem>>>> HaveMeanReciprocalRank<TItem>(
        this ValueAssertions<IEnumerable<RankingQuery<TItem>>> assertions,
        double expectedMeanReciprocalRank,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveMeanReciprocalRankCore<IEnumerable<RankingQuery<TItem>>, TItem>(assertions, expectedMeanReciprocalRank, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<ICollection<RankingQuery<TItem>>>> HaveMeanReciprocalRank<TItem>(
        this ValueAssertions<ICollection<RankingQuery<TItem>>> assertions,
        double expectedMeanReciprocalRank,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveMeanReciprocalRankCore<ICollection<RankingQuery<TItem>>, TItem>(assertions, expectedMeanReciprocalRank, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<IList<RankingQuery<TItem>>>> HaveMeanReciprocalRank<TItem>(
        this ValueAssertions<IList<RankingQuery<TItem>>> assertions,
        double expectedMeanReciprocalRank,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveMeanReciprocalRankCore<IList<RankingQuery<TItem>>, TItem>(assertions, expectedMeanReciprocalRank, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<IReadOnlyCollection<RankingQuery<TItem>>>> HaveMeanReciprocalRank<TItem>(
        this ValueAssertions<IReadOnlyCollection<RankingQuery<TItem>>> assertions,
        double expectedMeanReciprocalRank,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveMeanReciprocalRankCore<IReadOnlyCollection<RankingQuery<TItem>>, TItem>(assertions, expectedMeanReciprocalRank, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<IReadOnlyList<RankingQuery<TItem>>>> HaveMeanReciprocalRank<TItem>(
        this ValueAssertions<IReadOnlyList<RankingQuery<TItem>>> assertions,
        double expectedMeanReciprocalRank,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveMeanReciprocalRankCore<IReadOnlyList<RankingQuery<TItem>>, TItem>(assertions, expectedMeanReciprocalRank, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<TCollection>> HaveHitRateAt<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        int k,
        double expectedHitRate,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<RankingQuery<TItem>>
        => HaveHitRateAtCore<TCollection, TItem>(assertions, k, expectedHitRate, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<RankingQuery<TItem>[]>> HaveHitRateAt<TItem>(
        this ValueAssertions<RankingQuery<TItem>[]> assertions,
        int k,
        double expectedHitRate,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveHitRateAtCore<RankingQuery<TItem>[], TItem>(assertions, k, expectedHitRate, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<List<RankingQuery<TItem>>>> HaveHitRateAt<TItem>(
        this ValueAssertions<List<RankingQuery<TItem>>> assertions,
        int k,
        double expectedHitRate,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveHitRateAtCore<List<RankingQuery<TItem>>, TItem>(assertions, k, expectedHitRate, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<IEnumerable<RankingQuery<TItem>>>> HaveHitRateAt<TItem>(
        this ValueAssertions<IEnumerable<RankingQuery<TItem>>> assertions,
        int k,
        double expectedHitRate,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveHitRateAtCore<IEnumerable<RankingQuery<TItem>>, TItem>(assertions, k, expectedHitRate, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<ICollection<RankingQuery<TItem>>>> HaveHitRateAt<TItem>(
        this ValueAssertions<ICollection<RankingQuery<TItem>>> assertions,
        int k,
        double expectedHitRate,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveHitRateAtCore<ICollection<RankingQuery<TItem>>, TItem>(assertions, k, expectedHitRate, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<IList<RankingQuery<TItem>>>> HaveHitRateAt<TItem>(
        this ValueAssertions<IList<RankingQuery<TItem>>> assertions,
        int k,
        double expectedHitRate,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveHitRateAtCore<IList<RankingQuery<TItem>>, TItem>(assertions, k, expectedHitRate, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<IReadOnlyCollection<RankingQuery<TItem>>>> HaveHitRateAt<TItem>(
        this ValueAssertions<IReadOnlyCollection<RankingQuery<TItem>>> assertions,
        int k,
        double expectedHitRate,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveHitRateAtCore<IReadOnlyCollection<RankingQuery<TItem>>, TItem>(assertions, k, expectedHitRate, tolerance, because, callerFilePath, callerLineNumber);

    public static AndContinuation<ValueAssertions<IReadOnlyList<RankingQuery<TItem>>>> HaveHitRateAt<TItem>(
        this ValueAssertions<IReadOnlyList<RankingQuery<TItem>>> assertions,
        int k,
        double expectedHitRate,
        double tolerance = 0d,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        => HaveHitRateAtCore<IReadOnlyList<RankingQuery<TItem>>, TItem>(assertions, k, expectedHitRate, tolerance, because, callerFilePath, callerLineNumber);

    private static AndContinuation<ValueAssertions<TCollection>> HaveMeanReciprocalRankCore<TCollection, TItem>(
        ValueAssertions<TCollection> assertions,
        double expectedMeanReciprocalRank,
        double tolerance,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        where TCollection : IEnumerable<RankingQuery<TItem>>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        RankingAssertionHelpers.ValidateMetric(expectedMeanReciprocalRank, nameof(expectedMeanReciprocalRank));
        RankingAssertionHelpers.ValidateTolerance(tolerance);

        var expectation =
            $"to have mean reciprocal rank equal to {RankingAssertionHelpers.FormatMetric(expectedMeanReciprocalRank)} within tolerance {RankingAssertionHelpers.FormatMetric(tolerance)}";
        if (!TryGetQueries<TCollection, TItem>(assertions, expectation, because, callerFilePath, callerLineNumber, out var queries))
        {
            return new AndContinuation<ValueAssertions<TCollection>>(assertions);
        }

        if (queries.Count == 0)
        {
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(SubjectLabel(assertions), expectation, "query set was empty", because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<ValueAssertions<TCollection>>(assertions);
        }

        var reciprocalRankSum = 0d;
        foreach (var query in queries)
        {
            var rank = RankingAssertionHelpers.FindFirstRelevantRank(query.RankedResults, query.RelevantSet);
            reciprocalRankSum += rank is null ? 0d : 1d / rank.Value;
        }

        var actualMeanReciprocalRank = reciprocalRankSum / queries.Count;
        if (double.IsNaN(actualMeanReciprocalRank) || Math.Abs(actualMeanReciprocalRank - expectedMeanReciprocalRank) > tolerance)
        {
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(
                    SubjectLabel(assertions),
                    expectation,
                    $"actual mean reciprocal rank was {RankingAssertionHelpers.FormatMetric(actualMeanReciprocalRank)} across {queries.Count} quer{(queries.Count == 1 ? "y" : "ies")}",
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    private static AndContinuation<ValueAssertions<TCollection>> HaveHitRateAtCore<TCollection, TItem>(
        ValueAssertions<TCollection> assertions,
        int k,
        double expectedHitRate,
        double tolerance,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        where TCollection : IEnumerable<RankingQuery<TItem>>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        RankingAssertionHelpers.ValidatePositive(k, nameof(k));
        RankingAssertionHelpers.ValidateMetric(expectedHitRate, nameof(expectedHitRate));
        RankingAssertionHelpers.ValidateTolerance(tolerance);

        var expectation =
            $"to have hit rate@{k} equal to {RankingAssertionHelpers.FormatMetric(expectedHitRate)} within tolerance {RankingAssertionHelpers.FormatMetric(tolerance)}";
        if (!TryGetQueries<TCollection, TItem>(assertions, expectation, because, callerFilePath, callerLineNumber, out var queries))
        {
            return new AndContinuation<ValueAssertions<TCollection>>(assertions);
        }

        if (queries.Count == 0)
        {
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(SubjectLabel(assertions), expectation, "query set was empty", because),
                callerFilePath,
                callerLineNumber);
            return new AndContinuation<ValueAssertions<TCollection>>(assertions);
        }

        var hitCount = 0;
        foreach (var query in queries)
        {
            var hits = RankingAssertionHelpers.CountUniqueRelevantHits(query.RankedResults, k, query.RelevantSet);
            if (hits > 0)
            {
                hitCount++;
            }
        }

        var actualHitRate = hitCount / (double)queries.Count;
        if (double.IsNaN(actualHitRate) || Math.Abs(actualHitRate - expectedHitRate) > tolerance)
        {
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(
                    SubjectLabel(assertions),
                    expectation,
                    $"actual hit rate@{k} was {RankingAssertionHelpers.FormatMetric(actualHitRate)} ({hitCount} of {queries.Count} quer{(queries.Count == 1 ? "y" : "ies")} had a relevant hit)",
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    private static bool TryGetResults<TCollection, TItem>(
        ValueAssertions<TCollection> assertions,
        string expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber,
        out IReadOnlyList<TItem> results)
        where TCollection : IEnumerable<TItem>
    {
        if (assertions.Subject is null)
        {
            results = Array.Empty<TItem>();
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(SubjectLabel(assertions), expectation, "found <null>", because),
                callerFilePath,
                callerLineNumber);
            return false;
        }

        results = assertions.Subject is IReadOnlyList<TItem> list ? list : assertions.Subject.ToArray();
        return true;
    }

    private static bool TryGetQueries<TCollection, TItem>(
        ValueAssertions<TCollection> assertions,
        string expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber,
        out IReadOnlyList<RankingQuery<TItem>> queries)
        where TCollection : IEnumerable<RankingQuery<TItem>>
    {
        if (assertions.Subject is null)
        {
            queries = Array.Empty<RankingQuery<TItem>>();
            Fail(
                RankingAssertionHelpers.BuildFailureMessage(SubjectLabel(assertions), expectation, "found <null>", because),
                callerFilePath,
                callerLineNumber);
            return false;
        }

        queries = assertions.Subject is IReadOnlyList<RankingQuery<TItem>> list ? list : assertions.Subject.ToArray();
        return true;
    }

    private static string SubjectLabel<TCollection>(ValueAssertions<TCollection> assertions)
        => string.IsNullOrWhiteSpace(assertions.SubjectExpression) ? "<subject>" : assertions.SubjectExpression;

    private static void Fail(string message, string? callerFilePath, int callerLineNumber)
        => AssertionFailureDispatcher.Fail(message, callerFilePath, callerLineNumber);
}
