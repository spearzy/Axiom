using System.Collections;
using Axiom.Core.Failures;

namespace Axiom.Assertions;

public static class CollectionValueAssertionExtensions
{
    public static AndContinuation<ValueAssertions<TCollection>> Contain<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        TItem expected,
        string? because = null)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        var collectionAssertions = new CollectionAssertions<TItem>(assertions.Subject, assertions.SubjectExpression);
        collectionAssertions.Contain(expected, because);
        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveCount<TCollection>(
        this ValueAssertions<TCollection> assertions,
        int expectedCount,
        string? because = null)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        var subject = assertions.Subject;
        if (subject is null)
        {
            var nullFailure = new Failure(
                SubjectLabel(assertions.SubjectExpression),
                new Expectation("to have count", expectedCount),
                subject,
                because);
            Fail(FailureMessageRenderer.Render(nullFailure));
            return new AndContinuation<ValueAssertions<TCollection>>(assertions);
        }

        var actualCount = TryGetCount(subject, out var knownCount)
            ? knownCount
            : CountItems(subject);

        if (actualCount != expectedCount)
        {
            var failure = new Failure(
                SubjectLabel(assertions.SubjectExpression),
                new Expectation("to have count", expectedCount),
                actualCount,
                because);
            Fail(FailureMessageRenderer.Render(failure));
        }

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    private static bool TryGetCount(IEnumerable subject, out int count)
    {
        if (subject is ICollection collection)
        {
            count = collection.Count;
            return true;
        }

        count = 0;
        return false;
    }

    private static int CountItems(IEnumerable subject)
    {
        var count = 0;
        var enumerator = subject.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                count++;
            }
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }

        return count;
    }

    private static string SubjectLabel(string? subjectExpression)
    {
        return string.IsNullOrWhiteSpace(subjectExpression) ? "<subject>" : subjectExpression;
    }

    private static void Fail(string message)
    {
        var batch = Axiom.Core.Batch.Current;
        if (batch is not null)
        {
            batch.AddFailure(message);
            return;
        }

        throw new InvalidOperationException(message);
    }
}
