using System.Runtime.CompilerServices;
using Axiom.Assertions.Authoring;
using Axiom.Assertions.Chaining;
using Axiom.Assertions.AssertionTypes;
using Axiom.Core.Failures;
using AxiomAssert = Axiom.Core.Assert;

namespace Axiom.Tests.Authoring;

public sealed class CustomAssertionAuthoringTests
{
    [Fact]
    public void CustomAssertion_CanPass_AndReturnContinuation()
    {
        var invoice = new Invoice(new DateOnly(2026, 3, 1), "GBP");

        var assertions = invoice.Should();
        var continuation = assertions.BeOverdue(new DateOnly(2026, 3, 20));

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public void CustomAssertion_CanFail_WithRenderedMessage()
    {
        var invoice = new Invoice(new DateOnly(2026, 3, 1), "EUR");

        var ex = Assert.Throws<InvalidOperationException>(() => invoice.Should().HaveCurrency("GBP"));

        Assert.Equal("Expected invoice to have currency \"GBP\", but found \"EUR\".", ex.Message);
    }

    [Fact]
    public void CustomAssertion_Failure_RoutesIntoBatch()
    {
        var invoice = new Invoice(new DateOnly(2026, 3, 1), "EUR");

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = AxiomAssert.Batch("custom");
            invoice.Should().HaveCurrency("GBP");
        });

        Assert.Equal(
            "Batch 'custom' failed with 1 assertion failure(s):\n1) Expected invoice to have currency \"GBP\", but found \"EUR\".",
            ex.Message.ReplaceLineEndings("\n"));
    }

    [Fact]
    public void CustomAssertion_UsesSubjectExpression_WhenAvailable()
    {
        var customerInvoice = new Invoice(new DateOnly(2026, 3, 1), "EUR");

        var ex = Assert.Throws<InvalidOperationException>(() => customerInvoice.Should().HaveCurrency("GBP"));

        Assert.Contains("Expected customerInvoice to have currency", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AssertionContextCreate_Throws_WhenAssertionsIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => AssertionContext.Create<string>(null!));

        Assert.Equal("assertions", ex.ParamName);
    }
}

internal static class InvoiceAssertionExtensions
{
    public static AndContinuation<ValueAssertions<Invoice>> BeOverdue(
        this ValueAssertions<Invoice> assertions,
        DateOnly today,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var context = AssertionContext.Create(assertions);

        if (context.Subject.DueDate >= today)
        {
            context.Fail(
                new Expectation("to be overdue before", today),
                context.Subject.DueDate,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return context.And();
    }

    public static AndContinuation<ValueAssertions<Invoice>> HaveCurrency(
        this ValueAssertions<Invoice> assertions,
        string expectedCurrency,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedCurrency);

        var context = AssertionContext.Create(assertions);

        if (!string.Equals(context.Subject.Currency, expectedCurrency, StringComparison.Ordinal))
        {
            context.Fail(
                new Expectation("to have currency", expectedCurrency),
                context.Subject.Currency,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return context.And();
    }
}

internal sealed record Invoice(DateOnly DueDate, string Currency);
