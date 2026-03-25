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
            var message = $"ActualSimilarity is unavailable because HaveCosineSimilarityTo failed with error: {failureMessage}";
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
            _assertions.Fail(
                VectorAssertions<TNumeric>.BuildFailureMessage(
                    _assertions.SubjectLabel,
                    $"to have cosine similarity to expected at least {_assertions.FormatNumeric(threshold)}",
                    _unavailableActualDetail ?? "cosine similarity could not be computed",
                    because),
                callerFilePath,
                callerLineNumber);

            return new AndContinuation<VectorAssertions<TNumeric>>(_assertions);
        }

        if (_similarity < threshold)
        {
            _assertions.Fail(
                VectorAssertions<TNumeric>.BuildFailureMessage(
                    _assertions.SubjectLabel,
                    $"to have cosine similarity to expected at least {_assertions.FormatNumeric(threshold)}",
                    $"computed cosine similarity {_assertions.FormatNumeric(_similarity)}",
                    because),
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<VectorAssertions<TNumeric>>(_assertions);
    }
}
