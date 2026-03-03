# Axiom

![Axiom](Axiom.png)

[![CI](https://github.com/spearzy/Axiom/actions/workflows/ci.yml/badge.svg)](https://github.com/spearzy/Axiom/actions/workflows/ci.yml)
[![License: Apache-2.0](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

Axiom is an open-source assertion library for .NET focused on:
- low pass-path overhead (pay-for-play),
- clear failure messages (with caller expression text),
- fluent chaining via `.And`,
- batch-based aggregation via `Batch`.

## Why Axiom?

Traditional assertions can hide context:

```csharp
Assert.Equal("te", value[..2]);
// If this fails, the expression context is not always obvious.
```

Axiom-style assertions keep intent and context in one line:

```csharp
value.Should().StartWith("te");
```

Current string failure messages include the caller expression and expected/actual values, e.g.:

```text
Expected value to start with "ab", but found "test".
```

## Key Features

- Fluent `Should()` API with `.And` chaining
- `Batch` assertions for one combined failure from multiple checks
- `BeEquivalentTo(...)` for object graph comparison with configurable options
- Optional global equivalency defaults with per-call overrides
- Deterministic, testable failure messages and batch reports
- Extensible value comparison/formatting via comparer and formatter hooks
- Target frameworks: `net10.0` (primary), `net9.0`, and `net8.0`

## Implemented Assertion Methods (Current)

### Value assertions
- `Be(expected)`
- `NotBe(unexpected)`
- `BeNull()`
- `NotBeNull()`
- `BeSameAs(expectedReference)`
- `NotBeSameAs(unexpectedReference)`
- `BeOfType<TExpected>()`
- `BeAssignableTo<TExpected>()`
- `NotBeAssignableTo<TExpected>()`
- `BeGreaterThan(value)`
- `BeGreaterThanOrEqualTo(value)`
- `BeLessThan(value)`
- `BeLessThanOrEqualTo(value)`
- `BeInRange(min, max)`
- `BeEquivalentTo(expected)`
- `BeEquivalentTo(expected, configureOptions)`
- `BeTrue()` / `BeFalse()` (extension methods on `ValueAssertions<bool>`)

### String assertions
- `NotBeNull()`
- `StartWith(expectedPrefix)`
- `EndWith(expectedSuffix)`
- `Contain(expectedSubstring)`
- `NotContain(unexpectedSubstring)`
- `HaveLength(expectedLength)`
- `BeEmpty()`
- `NotBeEmpty()`

### Exception assertions
- `Throw<TException>()`
- `ThrowExactly<TException>()`
- `NotThrow()`
- `ThrowAsync<TException>()`
- `ThrowExactlyAsync<TException>()`
- `NotThrowAsync()`

### Collection assertions
- `Contain(item)`
- `HaveCount(expectedCount)`
- `BeEmpty()`
- `NotBeEmpty()`
- `ContainSingle()`
- `OnlyContain(predicate)`
- `NotContain(predicate)`
- `ContainInOrder(expectedSequence, allowGaps = true)`
- `ContainInOrder(expectedSequence, keySelector, allowGaps = true)`

### Dictionary assertions
- `ContainKey(key)`
- `NotContainKey(key)`
- `ContainValue(value)`
- `NotContainValue(value)`
- `ContainEntry(key, value)`

### Temporal assertions
- `BeBefore(expected)`
- `BeAfter(expected)`
- `BeWithin(expected, tolerance)`
for `DateTime`, `DateTimeOffset`, `DateOnly`, and `TimeOnly`.

## Usage

### Fluent String Assertions

```csharp
"test".Should()
    .StartWith("te").And
    .Contain("es").And
    .HaveLength(4).And
    .NotContain("ab").And
    .EndWith("st").And
    .NotBeEmpty().And
    .NotBeNull();
```

### Value Assertions

```csharp
42.Should()
    .Be(42).And
    .NotBe(0).And
    .BeGreaterThan(10).And
    .BeInRange(40, 50);

object value = "hello";
value.Should().BeOfType<string>().And.BeAssignableTo<object>();
```

### Equivalency Assertions

```csharp
var actual = new { Name = "Bob", Scores = new[] { 3, 1, 2 } };
var expected = new { Name = "Bob", Scores = new[] { 1, 2, 3 } };

actual.Should().BeEquivalentTo(
    expected,
    options => options.CollectionOrder = EquivalencyCollectionOrder.Any);
```

### Tolerance Rules For Non-Finite Numbers

When a tolerance is configured for `float`, `double`, or `Half`, Axiom applies explicit rules for non-finite values:

- `NaN` only matches `NaN`.
- `+Infinity` only matches `+Infinity`.
- `-Infinity` only matches `-Infinity`.

This keeps equivalency behaviour predictable for edge-case numeric inputs.

For time-based tolerances (`DateOnly`, `DateTime`, `DateTimeOffset`, `TimeOnly`, `TimeSpan`), negative values are normalised to absolute duration. `TimeSpan.MinValue` is rejected and throws `ArgumentOutOfRangeException`.

### Optional Global Equivalency Defaults

You only need this if you want project-wide defaults.  
If you do nothing, Axiom uses built-in defaults.

```csharp
using Axiom.Assertions.Equivalency;

EquivalencyDefaults.Configure(options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
    options.FailOnExtraMembers = false;
});
```

Per-call options always override global defaults:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Strict;
});
```

The `options` parameter above is an `Action<EquivalencyOptions>`.

### Custom Comparer Providers

Use a custom comparer provider when your domain needs equality rules that differ from default `.Equals(...)`.

Typical cases:
- value objects that should compare case-insensitively,
- tolerance-based equality for domain numbers,
- types from external/generated code where you cannot (or should not) change the type itself.

How Axiom uses comparer providers:
- `Be(...)` and `NotBe(...)`: uses the provider for that value type.
- `BeEquivalentTo(...)`: uses the provider for non-string leaf values during graph comparison.
- Strings in `BeEquivalentTo(...)` use `EquivalencyOptions.StringComparison`.

Implementation pattern:
- Implement `IComparerProvider`.
- Return an `IEqualityComparer<T>` for handled types.
- Return `false` for unhandled types so Axiom falls back to default equality.

```csharp
using Axiom.Core.Comparison;
using Axiom.Core.Configuration;

public sealed class OrderCodeComparerProvider : IComparerProvider
{
    public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
    {
        if (typeof(T) == typeof(OrderCode))
        {
            comparer = (IEqualityComparer<T>)(object)new OrderCodeComparer();
            return true;
        }

        comparer = null;
        return false;
    }
}

public sealed class OrderCodeComparer : IEqualityComparer<OrderCode>
{
    // Domain rule: order codes are equal ignoring case.
    public bool Equals(OrderCode? x, OrderCode? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(OrderCode obj)
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
    }
}

public sealed record OrderCode(string Value);

AxiomServices.Configure(c =>
{
    c.ComparerProvider = new OrderCodeComparerProvider();
});
```

### Exception Assertions

```csharp
Action strictThrow = () => throw new InvalidOperationException();
strictThrow.Should().ThrowExactly<InvalidOperationException>();

Action noThrow = () => { };
noThrow.Should().NotThrow();

Func<Task> asyncAction = () => Task.FromException(new InvalidOperationException());
await asyncAction.Should().ThrowAsync<Exception>();
await asyncAction.Should().ThrowExactlyAsync<InvalidOperationException>();

Func<Task> asyncNoThrow = () => Task.CompletedTask;
await asyncNoThrow.Should().NotThrowAsync();
```

### Collection Assertions

```csharp
int[] values = [1, 2, 3];
values.Should()
    .Contain(2).And
    .ContainInOrder([1, 3], allowGaps: true).And
    .HaveCount(3).And
    .NotBeEmpty();

Dictionary<string, int> scores = new()
{
    ["a"] = 1,
    ["b"] = 2
};

scores.Should()
    .ContainKey("a").And
    .ContainValue(2).And
    .ContainEntry("b", 2);
```

### Temporal Assertions

```csharp
var now = DateTime.UtcNow;
var later = now.AddMinutes(2);

later.Should().BeAfter(now).And.BeWithin(now.AddMinutes(2), TimeSpan.FromSeconds(1));
```

### Optional Coloured Assertion Output

```csharp
using Axiom.Core.Configuration;

AxiomServices.Configure(c =>
{
    c.Output.Enabled = true;
    c.Output.ShowPasses = true;
    c.Output.UseColours = true;
    c.Output.IncludeSourceLine = true;
});
```

When enabled, Axiom prints readable pass/fail output with source location (and source line for failures when available).

### Batch Assertions

Use `Batch` when you want to run several related assertions and see all failures together.

Without `Batch`, the first failing assertion throws immediately and stops execution.
With `Batch`, failures are collected and one combined exception is thrown when the root batch is disposed.

`Batch` is useful for validating multiple fields in one object or multiple expectations in one scenario.

```csharp
using var batch = Assert.Batch("user profile");

user.Name.Should().StartWith("A");
user.Email.Should().EndWith("@example.com");
user.Roles.Should().Contain("admin");
```

Disposing the root batch throws one combined deterministic message:

```text
Batch 'user profile' failed with 3 assertion failure(s):
1) ...
2) ...
3) ...
```

## Installation

Axiom is not published on NuGet yet. For now, consume it from source:

```bash
git clone https://github.com/spearzy/Axiom.git
```

Then reference the project(s) you need, for example:

```bash
dotnet add <your-test-project>.csproj reference src/Axiom.Assertions/Axiom.Assertions.csproj
```

## Planned Packages

- `Axiom.Core`
- `Axiom.Assertions`
- `Axiom.Analyzers` (scaffold only for now)
- `Axiom.Vectors` (scaffold only for now)
- `Axiom.Benchmarks`

### Scaffold Docs

- Analysers scaffold: [src/Axiom.Analyzers/README.md](src/Axiom.Analyzers/README.md)
- Vectors scaffold: [src/Axiom.Vectors/README.md](src/Axiom.Vectors/README.md)

## Design Notes

See [docs/design.md](docs/design.md) for architecture and design direction.

## Build

```bash
dotnet restore Axiom.sln
dotnet build Axiom.sln -c Release
dotnet test Axiom.sln -c Release
```

## Benchmarks

Run the current benchmark suite:

```bash
dotnet run -c Release --project benchmarks/Axiom.Benchmarks/Axiom.Benchmarks.csproj
```

Current scenarios:
- String assertions:
  - `StartWith_Pass_OutsideBatch`
  - `StartWith_Fail_OutsideBatch` (exception caught inside benchmark)
  - `StartWith_Fail_InsideBatch` (aggregated throw on batch dispose)
- Collection assertions:
  - `Contain_Pass_OutsideBatch`
  - `Contain_Fail_OutsideBatch` (exception caught inside benchmark)
  - `HaveCount_Pass_OutsideBatch`
  - `HaveCount_Fail_OutsideBatch` (exception caught inside benchmark)
  - `ContainAndHaveCount_Pass_OutsideBatch`

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

Licensed under Apache-2.0. See [LICENSE](LICENSE).
