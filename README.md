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

## Current Features

- `Should()` entrypoints for:
  - `string?`
  - generic values (`T`)
  - `Action`
  - `Func<Task>`
  - `Func<ValueTask>`
- Fluent chaining:
  - strings: `NotBeNull()`, `StartWith(...)`, `EndWith(...)`
  - values: `Be(...)`, `NotBe(...)`
  - actions: `Throw<TException>()`
  - async actions: `ThrowAsync<TException>()`
  - collections (on enumerable values): `Contain(...)`, `HaveCount(...)`
  - `.And`
- `Batch` aggregation:
  - outside a batch, failures throw immediately,
  - inside a batch, failures are collected,
  - root batch dispose throws one combined deterministic exception.
- Target frameworks:
  - `net10.0` (primary)
  - `net8.0` (multi-target for library projects)

## Usage

### Fluent String Assertions

```csharp
"test".Should()
    .StartWith("te").And
    .EndWith("st").And
    .NotBeNull();
```

### Value Assertions

```csharp
42.Should().Be(42).And.NotBe(0);
```

### Exception Assertions

```csharp
Action sync = () => throw new InvalidOperationException();
sync.Should().Throw<InvalidOperationException>();

Func<Task> asyncAction = () => Task.FromException(new InvalidOperationException());
await asyncAction.Should().ThrowAsync<InvalidOperationException>();
```

### Collection Assertions

```csharp
int[] values = [1, 2, 3];
values.Should().Contain(2).And.HaveCount(3);
```

### Batch Aggregation

```csharp
using var batch = Assert.Batch("strings");

"test".Should().StartWith("ab");
"test".Should().EndWith("cd");
```

When the batch is disposed, Axiom throws one combined exception similar to:

```text
Batch 'strings' failed with 2 assertion failure(s):
1) Expected value to start with "ab", but found "test".
2) Expected value to end with "cd", but found "test".
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

- Analyzers scaffold: [src/Axiom.Analyzers/README.md](src/Axiom.Analyzers/README.md)
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
