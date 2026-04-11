using System.Runtime.CompilerServices;
using Axiom.Assertions.Authoring;
using Axiom.Assertions.Chaining;
using Axiom.Assertions.AssertionTypes;
using AxiomAssert = Axiom.Core.Assert;

namespace Axiom.Tests.Authoring;

public sealed class PracticalCustomAssertionAuthoringTests
{
    [Fact]
    public void ApiResponseAssertions_CanPass_AndChainThroughAnd()
    {
        var response = new ApiResponse(200, "ORDER-123", null);

        var assertions = response.Should();
        var continuation = assertions.BeSuccessful();

        Assert.Same(assertions, continuation.And);
        Assert.Same(assertions, continuation.And.HaveStatusCode(200).And);
    }

    [Fact]
    public void ApiResponseAssertions_CanFail_WithNormalRenderedMessage()
    {
        var response = new ApiResponse(500, "ORDER-123", "INTERNAL_ERROR");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().BeSuccessful());

        Assert.Equal("Expected response to be successful (2xx), but found 500.", ex.Message);
    }

    [Fact]
    public void ApiResponseAssertions_Failures_RouteIntoBatch()
    {
        var response = new ApiResponse(500, "ORDER-123", "INTERNAL_ERROR");

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = AxiomAssert.Batch("api response");
            response.Should().BeSuccessful();
            response.Should().HaveErrorCode("ORDER_NOT_FOUND");
        });

        Assert.Equal(
            """
            Batch 'api response' failed with 2 assertion failure(s):
            1) Expected response to be successful (2xx), but found 500.
            2) Expected response to have error code "ORDER_NOT_FOUND", but found "INTERNAL_ERROR".
            """.ReplaceLineEndings("\n"),
            ex.Message.ReplaceLineEndings("\n"));
    }

    [Fact]
    public void BusinessDateAssertion_CanFail_WithConsumerStyleUsage()
    {
        var invoice = new BillingInvoice("INV-001", new DateOnly(2026, 3, 31));
        var today = new DateOnly(2026, 3, 20);

        var ex = Assert.Throws<InvalidOperationException>(() => invoice.Should().BeOverdueAsOf(today));

        Assert.Equal(
            "Expected invoice to be overdue as of 03/20/2026, but found 03/31/2026.",
            ex.Message);
    }
}

internal static class ApiResponseAssertionExtensions
{
    public static AndContinuation<ValueAssertions<ApiResponse>> BeSuccessful(
        this ValueAssertions<ApiResponse> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var context = AssertionContext.Create(assertions);

        if (context.Subject.StatusCode is < 200 or >= 300)
        {
            context.Fail(
                new Expectation("to be successful (2xx)", IncludeExpectedValue: false),
                context.Subject.StatusCode,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return context.And();
    }

    public static AndContinuation<ValueAssertions<ApiResponse>> HaveStatusCode(
        this ValueAssertions<ApiResponse> assertions,
        int expectedStatusCode,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var context = AssertionContext.Create(assertions);

        if (context.Subject.StatusCode != expectedStatusCode)
        {
            context.Fail(
                new Expectation("to have status code", expectedStatusCode),
                context.Subject.StatusCode,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return context.And();
    }

    public static AndContinuation<ValueAssertions<ApiResponse>> HaveErrorCode(
        this ValueAssertions<ApiResponse> assertions,
        string expectedErrorCode,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedErrorCode);

        var context = AssertionContext.Create(assertions);

        if (!context.GetEqualityComparer<string?>().Equals(context.Subject.ErrorCode, expectedErrorCode))
        {
            context.Fail(
                new Expectation("to have error code", expectedErrorCode),
                context.Subject.ErrorCode,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return context.And();
    }
}

internal static class BillingDateAssertionExtensions
{
    public static AndContinuation<ValueAssertions<BillingInvoice>> BeOverdueAsOf(
        this ValueAssertions<BillingInvoice> assertions,
        DateOnly today,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var context = AssertionContext.Create(assertions);

        if (context.Subject.DueDate >= today)
        {
            context.Fail(
                new Expectation("to be overdue as of", today),
                context.Subject.DueDate,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return context.And();
    }
}

internal sealed record ApiResponse(int StatusCode, string OrderId, string? ErrorCode);

internal sealed record BillingInvoice(string InvoiceNumber, DateOnly DueDate);
