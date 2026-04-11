using Axiom.Assertions.Chaining;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;

namespace Axiom.Assertions.Authoring;

public readonly struct AssertionContext<TAssertions, TSubject>
{
    internal AssertionContext(TAssertions assertions, TSubject subject, string subjectLabel)
    {
        Assertions = assertions;
        Subject = subject;
        SubjectLabel = subjectLabel;
    }

    public TAssertions Assertions { get; }

    public TSubject Subject { get; }

    public string SubjectLabel { get; }

    public AndContinuation<TAssertions> And()
    {
        return new AndContinuation<TAssertions>(Assertions);
    }

    public IEqualityComparer<TValue> GetEqualityComparer<TValue>()
    {
        if (AxiomServices.Configuration.ComparerProvider.TryGetEqualityComparer<TValue>(out var comparer) &&
            comparer is not null)
        {
            return comparer;
        }

        return EqualityComparer<TValue>.Default;
    }

    public void Fail(
        Expectation expectation,
        object? actual,
        string? because = null,
        string? callerFilePath = null,
        int callerLineNumber = 0)
    {
        var failure = new Failure(SubjectLabel, expectation, actual, because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }
}
