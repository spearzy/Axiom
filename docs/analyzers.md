# Analyzers

Installing `Axiom.Assertions` gives you the Axiom Roslyn analyzers/code fixes automatically. That is the default install path for most users.

`Axiom.Analyzers` still exists as an optional standalone package if you only want the diagnostics without the runtime assertion library.

```bash
dotnet add package Axiom.Assertions
```

Install the standalone package only if you want the diagnostics on their own:

```bash
dotnet add package Axiom.Analyzers
```

The current rules focus on two high-value Axiom mistakes:

- ignored async Axiom assertion results
- `Batch` instances created without `using`

## Async Assertions Must Be Awaited

Rule: `AXM0001`

This rule flags ignored async Axiom assertion calls from:

- `AsyncActionAssertions`
- `AsyncFunctionAssertions<T>`
- `TaskAssertions`
- `TaskAssertions<T>`

Examples include:

- `ThrowAsync(...)`
- `NotThrowAsync()`
- `Succeed()`
- `SucceedWithin(...)`
- `BeFaultedWith(...)`

Async Axiom assertions are lazy until their returned `ValueTask` is awaited or returned. If the result is ignored, the test can appear to pass even though the assertion never executed.

## Before / After

Before:

```csharp
loader.Should().Succeed();
```

After:

```csharp
await loader.Should().Succeed();
```

The analyzer also offers a code fix in async contexts where prepending `await` is safe.

## Batch Must Be Disposed

Rule: `AXM0002`

This rule flags `Batch` instances created without `using`.

Before:

```csharp
var batch = Assert.Batch("user");
user.Name.Should().NotBeNull();
```

After:

```csharp
using var batch = Assert.Batch("user");
user.Name.Should().NotBeNull();
```

`Batch` flushes aggregated failures when it is disposed. If it is created without `using`, failures may never be emitted at the end of the scope.

The analyzer offers a code fix for the common local declaration case by converting `var batch = ...;` to `using var batch = ...;`.
