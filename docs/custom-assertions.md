# Custom Assertions

Use custom assertions when the same domain rule keeps showing up and the built-in Axiom assertions start to read too mechanically.

`AssertionContext.Create(...)` is the supported entry point for building on `ValueAssertions<T>` while keeping Axiom's normal failure rendering, `Batch` aggregation, and configured failure strategy.

## The Core Pattern

Most custom assertions follow the same small shape:

1. Create a context from the incoming `ValueAssertions<T>`.
2. Read the domain object through `context.Subject`.
3. Call `context.Fail(...)` when the rule is broken.
4. Return `context.And()` so the assertion chains naturally.

```csharp
using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Authoring;
using Axiom.Assertions.Chaining;
using Axiom.Core.Failures;

public static class InvoiceAssertionExtensions
{
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
```

## Practical Example: API Responses

This is a common place where teams want a domain-specific assertion layer instead of repeating low-level value checks in every test.

```csharp
public static class ApiResponseAssertionExtensions
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

        if (!string.Equals(context.Subject.ErrorCode, expectedErrorCode, StringComparison.Ordinal))
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
```

Consumer usage stays small and readable:

```csharp
response.Should().BeSuccessful().And.HaveStatusCode(200);
failedResponse.Should().HaveErrorCode("ORDER_NOT_FOUND");
```

## Practical Example: Business Date Rules

Custom assertions also work well for business rules that are more meaningful than a raw comparison.

```csharp
public static class InvoiceAssertionExtensions
{
    public static AndContinuation<ValueAssertions<Invoice>> BeOverdueAsOf(
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
                new Expectation("to be overdue as of", today),
                context.Subject.DueDate,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return context.And();
    }
}
```

That lets a test say:

```csharp
invoice.Should().BeOverdueAsOf(today);
```

instead of spelling the rule out with lower-level date comparisons everywhere.

## Returning .And

Call `context.And()` to keep the same fluent style as built-in assertions:

```csharp
invoice.Should().HaveCurrency("GBP").And.NotBeNull();
response.Should().BeSuccessful().And.HaveStatusCode(200);
```

## Respecting Batch

Use `context.Fail(...)` instead of throwing exceptions yourself.

- Outside a `Batch`, the failure is thrown immediately.
- Inside a `Batch`, the failure is collected and reported when the root batch is disposed.

That means your custom assertions automatically behave like first-party Axiom assertions in aggregated test scenarios.

## Failure Messages And `because`

Pass an `Expectation` plus the actual value to `context.Fail(...)`. Axiom renders the message using the same formatter and subject label as the built-in assertions.

Forward `because`, `callerFilePath`, and `callerLineNumber` from your extension method so your custom assertion behaves like first-party Axiom code.

## Keeping The Surface Healthy

- Keep assertions small and domain-specific.
- Prefer expectation text that reads like a sentence, such as `to have currency`, `to be successful (2xx)`, or `to be overdue as of`.
- Put reusable domain rules into extension methods instead of building a second assertion DSL.
- Reach for custom assertions when they remove repetition or reveal intent, not just to rename a simple built-in assertion.
