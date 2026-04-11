using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Authoring;
using Axiom.Assertions.Chaining;
using Axiom.Assertions.Extensions;
using AxiomAssert = Axiom.Core.Assert;

namespace Axiom.Tests.Authoring;

public sealed class AssertionContextReceiverSupportTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void AssertionContextCreate_StringAssertions_Throws_WhenAssertionsIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => AssertionContext.Create((StringAssertions)null!));

        Assert.Equal("assertions", ex.ParamName);
    }

    [Fact]
    public void StringCustomAssertion_CanPass_AndChainThroughAnd()
    {
        const string route = "/orders/123";

        var assertions = route.Should();
        var continuation = assertions.HaveSegmentCount(2);

        Assert.Same(assertions, continuation.And);
        Assert.Same(assertions, continuation.And.StartWith("/orders").And);
    }

    [Fact]
    public void StringCustomAssertion_CanFail_WithRenderedMessage()
    {
        const string route = "/orders/123";

        var ex = Assert.Throws<InvalidOperationException>(() => route.Should().HaveSegmentCount(3));

        Assert.Equal("Expected route to have segment count 3, but found 2.", ex.Message);
    }

    [Fact]
    public void CollectionCustomAssertion_CanPass_AndChainThroughAnd()
    {
        var approvals = new[] { "alpha", "beta", "gamma" };

        var assertions = approvals.Should();
        var continuation = assertions.HaveCountAtLeast(3);

        Assert.Same(assertions, continuation.And);
        Assert.Same(assertions, continuation.And.Contain("alpha").And);
    }

    [Fact]
    public void CollectionCustomAssertion_Failure_RoutesIntoBatch()
    {
        var approvals = new[] { "alpha", "beta" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = AxiomAssert.Batch("authoring");
            approvals.Should().HaveCountAtLeast(3);
        });

        Assert.Equal(
            "Batch 'authoring' failed with 1 assertion failure(s):\n1) Expected approvals to have at least 3, but found 2.",
            ex.Message.ReplaceLineEndings("\n"));
    }

    [Fact]
    public void CustomAssertion_Failure_UsesConfiguredFailureStrategy_OutsideBatch()
    {
        var strategy = new AuthoringTestFailureStrategy();
        AxiomServices.Configure(c => c.FailureStrategy = strategy);

        const string route = "/orders/123";
        var ex = Assert.Throws<AuthoringTestFailureException>(() => route.Should().HaveSegmentCount(3));

        Assert.Equal("Expected route to have segment count 3, but found 2.", ex.Message);
        Assert.Contains(ex, strategy.Failures);
    }

    [Fact]
    public void StringCustomAssertion_UsesSubjectLabel_WhenAvailable()
    {
        const string orderRoute = "/orders/123";

        var ex = Assert.Throws<InvalidOperationException>(() => orderRoute.Should().HaveSegmentCount(3));

        Assert.Contains("Expected orderRoute to have segment count 3, but found 2.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CustomAssertions_InsideNestedBatches_AggregateIntoRootBatch()
    {
        const string route = "/orders/123";
        var approvals = new[] { "alpha", "beta" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var outer = AxiomAssert.Batch("outer");
            using (AxiomAssert.Batch("inner"))
            {
                route.Should().HaveSegmentCount(3);
            }

            approvals.Should().HaveCountAtLeast(3);
        });

        Assert.Equal(
            """
            Batch 'outer' failed with 2 assertion failure(s):
            1) Expected route to have segment count 3, but found 2.
            2) Expected approvals to have at least 3, but found 2.
            """.ReplaceLineEndings("\n"),
            ex.Message.ReplaceLineEndings("\n"));
    }

    [Fact]
    public void StringCustomAssertion_UsesConfiguredComparerProvider_WhenAuthorUsesContextComparer()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new CaseInsensitiveStringComparerProvider());

        var ex = Record.Exception(() => "READY".Should().MatchConfiguredText("ready"));

        Assert.Null(ex);
    }
}

internal static class StringAuthoringAssertionExtensions
{
    public static AndContinuation<StringAssertions> HaveSegmentCount(
        this StringAssertions assertions,
        int expectedSegmentCount,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var context = AssertionContext.Create(assertions);
        var actualSegmentCount = CountSegments(context.Subject);

        if (actualSegmentCount != expectedSegmentCount)
        {
            context.Fail(
                new Expectation("to have segment count", expectedSegmentCount),
                actualSegmentCount,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return context.And();
    }

    public static AndContinuation<StringAssertions> MatchConfiguredText(
        this StringAssertions assertions,
        string expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expected);

        var context = AssertionContext.Create(assertions);
        if (!context.GetEqualityComparer<string?>().Equals(context.Subject, expected))
        {
            context.Fail(
                new Expectation("to match configured text", expected),
                context.Subject,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return context.And();
    }

    private static int CountSegments(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return 0;
        }

        var segmentCount = 0;
        var inSegment = false;

        foreach (var character in value)
        {
            if (character == '/')
            {
                inSegment = false;
                continue;
            }

            if (!inSegment)
            {
                segmentCount++;
                inSegment = true;
            }
        }

        return segmentCount;
    }
}

internal sealed class AuthoringTestFailureStrategy : IFailureStrategy
{
    public List<AuthoringTestFailureException> Failures { get; } = new();

    public void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0)
    {
        var ex = new AuthoringTestFailureException(message, callerFilePath, callerLineNumber);
        Failures.Add(ex);
        throw ex;
    }
}

internal sealed class AuthoringTestFailureException : Exception
{
    public AuthoringTestFailureException(string message, string? callerFilePath, int callerLineNumber)
        : base(message)
    {
        CallerFilePath = callerFilePath;
        CallerLineNumber = callerLineNumber;
    }

    public string? CallerFilePath { get; }
    public int CallerLineNumber { get; }
}

internal static class CollectionAuthoringAssertionExtensions
{
    public static AndContinuation<ValueAssertions<TCollection>> HaveCountAtLeast<TCollection>(
        this ValueAssertions<TCollection> assertions,
        int minimumCount,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : System.Collections.IEnumerable
    {
        var context = AssertionContext.Create(assertions);
        var actualCount = CountItems(context.Subject);

        if (actualCount < minimumCount)
        {
            context.Fail(
                new Expectation("to have at least", minimumCount),
                actualCount,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return context.And();
    }

    private static int CountItems(System.Collections.IEnumerable? subject)
    {
        if (subject is null)
        {
            return 0;
        }

        var count = 0;
        foreach (var _ in subject)
        {
            count++;
        }

        return count;
    }
}
