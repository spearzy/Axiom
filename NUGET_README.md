# Axiom

Axiom is a .NET assertion library focused on fluent APIs, deterministic failure output, and batch-based aggregation with `Batch`.

## Install

Use an explicit version:

```bash
dotnet add package Axiom.Assertions --version 0.1.0-preview.1
```

If you only need core primitives directly:

```bash
dotnet add package Axiom.Core --version 0.1.0-preview.1
```

## Which Package?

- `Axiom.Assertions`: fluent assertions (`Should()`, chaining, string/value/collection/dictionary/exception/equivalency assertions).  
  This is the package most test projects should reference.
- `Axiom.Core`: batch aggregation and core extensibility primitives.  
  Use this directly only when you need low-level primitives without the full fluent assertion surface.

## Quick Start

```csharp
using Axiom.Assertions;
using Axiom.Core;

"abc".Should().StartWith("a").And.EndWith("c");

using var batch = Assert.Batch("profile");
"bob@example.com".Should().Contain("@");
"admin".Should().NotBeNull();
```

## Documentation

- GitHub repository: https://github.com/spearzy/Axiom
- Full README and examples: https://github.com/spearzy/Axiom#readme
