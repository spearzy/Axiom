using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

namespace Axiom.Vectors;

public readonly struct CosineSimilarityAssertions<TNumeric>
    where TNumeric : struct, IFloatingPointIeee754<TNumeric>
{
    private readonly VectorAssertions<TNumeric> _assertions;
    private readonly bool _hasSimilarity;
    private readonly TNumeric _similarity;
    private readonly string? _unavailableActualDetail;

    internal CosineSimilarityAssertions(
        VectorAssertions<TNumeric> assertions,
        bool hasSimilarity,
        TNumeric similarity,
        string? unavailableActualDetail)
    {
        _assertions = assertions;
        _hasSimilarity = hasSimilarity;
        _similarity = similarity;
        _unavailableActualDetail = unavailableActualDetail;
    }

    public TNumeric ActualSimilarity
    {
        get
        {
            if (_hasSimilarity)
            {
                return _similarity;
            }

            var failureMessage = _assertions.BuildCosineSimilarityUnavailableMessage(_unavailableActualDetail);
            var message = $"ActualSimilarity is unavailable because HaveCosineSimilarityWith failed with error: {failureMessage}";
            AssertionFailureDispatcher.Throw(message);
            throw new UnreachableException();
        }
    }

    public AndContinuation<VectorAssertions<TNumeric>> AtLeast(
        TNumeric threshold,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        VectorAssertions<TNumeric>.ValidateSimilarityThreshold(threshold);

        if (!_hasSimilarity)
        {
            return FailUnavailable(
                $"to have cosine similarity with expected at least {_assertions.FormatNumeric(threshold)}",
                because,
                callerFilePath,
                callerLineNumber);
        }

        if (_similarity < threshold)
        {
            Fail(
                $"to have cosine similarity with expected at least {_assertions.FormatNumeric(threshold)}",
                $"computed cosine similarity {_assertions.FormatNumeric(_similarity)}",
                because,
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<VectorAssertions<TNumeric>>(_assertions);
    }

    public AndContinuation<VectorAssertions<TNumeric>> AtMost(
        TNumeric threshold,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        VectorAssertions<TNumeric>.ValidateSimilarityThreshold(threshold);

        if (!_hasSimilarity)
        {
            return FailUnavailable(
                $"to have cosine similarity with expected at most {_assertions.FormatNumeric(threshold)}",
                because,
                callerFilePath,
                callerLineNumber);
        }

        if (_similarity > threshold)
        {
            Fail(
                $"to have cosine similarity with expected at most {_assertions.FormatNumeric(threshold)}",
                $"computed cosine similarity {_assertions.FormatNumeric(_similarity)}",
                because,
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<VectorAssertions<TNumeric>>(_assertions);
    }

    public AndContinuation<VectorAssertions<TNumeric>> Between(
        TNumeric minimumThreshold,
        TNumeric maximumThreshold,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        VectorAssertions<TNumeric>.ValidateSimilarityRange(minimumThreshold, maximumThreshold);

        var expectation =
            $"to have cosine similarity with expected between {_assertions.FormatNumeric(minimumThreshold)} and {_assertions.FormatNumeric(maximumThreshold)} inclusive";

        if (!_hasSimilarity)
        {
            return FailUnavailable(expectation, because, callerFilePath, callerLineNumber);
        }

        if (_similarity < minimumThreshold || _similarity > maximumThreshold)
        {
            Fail(
                expectation,
                $"computed cosine similarity {_assertions.FormatNumeric(_similarity)}",
                because,
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<VectorAssertions<TNumeric>>(_assertions);
    }

    private AndContinuation<VectorAssertions<TNumeric>> FailUnavailable(
        string expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        Fail(
            expectation,
            _unavailableActualDetail ?? "cosine similarity could not be computed",
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<VectorAssertions<TNumeric>>(_assertions);
    }

    private void Fail(
        string expectation,
        string actualDetail,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        _assertions.Fail(
            VectorAssertions<TNumeric>.BuildFailureMessage(
                _assertions.SubjectLabel,
                expectation,
                actualDetail,
                because),
            callerFilePath,
            callerLineNumber);
    }
}
