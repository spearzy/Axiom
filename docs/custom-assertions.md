# Custom Assertions

Use custom assertions when a domain rule shows up often enough that the built-in Axiom surface starts to feel too low-level.

`AssertionContext.Create(...)` gives you a small supported way to build on `ValueAssertions<T>` while still using Axiom's normal failure rendering, `Batch` aggregation, and configured failure strategy.

## Basic Example

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

## Returning .And

Call `context.And()` to keep the same fluent style as built-in assertions:

```csharp
invoice.Should().HaveCurrency("GBP").And.NotBeNull();
```

## Respecting Batch

Use `context.Fail(...)` instead of throwing exceptions yourself.

- Outside a `Batch`, the failure is thrown immediately.
- Inside a `Batch`, the failure is collected and reported when the root batch is disposed.

## Failure Messages And because

Pass an `Expectation` plus the actual value to `context.Fail(...)`. Axiom renders the message using the same formatter and subject label as the built-in assertions.

Forward `because`, `callerFilePath`, and `callerLineNumber` from your extension method so your custom assertion behaves like first-party Axiom code.

## Tips

- Keep custom assertions small and domain-specific.
- Prefer clear expectation text such as `to have currency` or `to be overdue`.
- Return `context.And()` so your extensions chain naturally with existing Axiom assertions.
