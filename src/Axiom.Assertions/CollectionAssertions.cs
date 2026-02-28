using System.Collections;
using Axiom.Core;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;

namespace Axiom.Assertions;

public sealed class CollectionAssertions<T>
{
    public CollectionAssertions(IEnumerable<T>? subject, string? subjectExpression)
    {
        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    public IEnumerable<T>? Subject { get; }
    public string? SubjectExpression { get; }

    public AndContinuation<CollectionAssertions<T>> Contain(T expected, string? because = null)
    {
        var subject = Subject;
        if (subject is null)
        {
            var nullFailure = new Failure(
                SubjectLabel(),
                new Expectation("to contain", expected),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure));
            return new AndContinuation<CollectionAssertions<T>>(this);
        }

        var comparer = GetComparer();
        foreach (var item in subject)
        {
            if (comparer.Equals(item, expected))
            {
                return new AndContinuation<CollectionAssertions<T>>(this);
            }
        }

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to contain", expected),
            subject,
            because);
        Fail(FailureMessageRenderer.Render(failure));
        return new AndContinuation<CollectionAssertions<T>>(this);
    }

    public AndContinuation<CollectionAssertions<T>> HaveCount(int expectedCount, string? because = null)
    {
        var subject = Subject;
        if (subject is null)
        {
            var nullFailure = new Failure(
                SubjectLabel(),
                new Expectation("to have count", expectedCount),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure));
            return new AndContinuation<CollectionAssertions<T>>(this);
        }

        var actualCount = TryGetCount(subject, out var knownCount)
            ? knownCount
            : CountItems(subject);

        if (actualCount != expectedCount)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to have count", expectedCount),
                actualCount,
                because);
            Fail(FailureMessageRenderer.Render(failure));
        }

        return new AndContinuation<CollectionAssertions<T>>(this);
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }

    private static IEqualityComparer<T> GetComparer()
    {
        if (AxiomServices.Configuration.ComparerProvider.TryGetEqualityComparer<T>(out var comparer) &&
            comparer is not null)
        {
            return comparer;
        }

        return EqualityComparer<T>.Default;
    }

    private static bool TryGetCount(IEnumerable<T> subject, out int count)
    {
        // Prefer non-enumerating count paths when collection interfaces are available.
        if (subject is ICollection<T> genericCollection)
        {
            count = genericCollection.Count;
            return true;
        }

        if (subject is IReadOnlyCollection<T> readOnlyCollection)
        {
            count = readOnlyCollection.Count;
            return true;
        }

        if (subject is ICollection nonGenericCollection)
        {
            count = nonGenericCollection.Count;
            return true;
        }

        count = 0;
        return false;
    }

    private static int CountItems(IEnumerable<T> subject)
    {
        var count = 0;
        foreach (var _ in subject)
        {
            count++;
        }

        return count;
    }

    private static void Fail(string message)
    {
        var batch = Batch.Current;
        if (batch is not null)
        {
            // Collect failures during batch execution and throw once at root dispose.
            batch.AddFailure(message);
            return;
        }

        throw new InvalidOperationException(message);
    }
}
