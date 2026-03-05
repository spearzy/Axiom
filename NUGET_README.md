# Axiom

Axiom is a .NET assertion library focused on fluent APIs, deterministic failure output, and batch-based aggregation with `Batch`.

## Install

```bash
dotnet add package Axiom.Assertions --prerelease
```

If you only need core primitives:

```bash
dotnet add package Axiom.Core --prerelease
```

## Quick Start

```csharp
using Axiom.Assertions.EntryPoints;
using Axiom.Core;

"abc".Should().StartWith("a").And.EndWith("c");

using var batch = Assert.Batch("profile");
"bob@example.com".Should().Contain("@");
"admin".Should().NotBeNull();
```

## Documentation

- GitHub repository: https://github.com/spearzy/Axiom
- Full README and examples: https://github.com/spearzy/Axiom#readme
