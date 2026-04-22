# Axiom.Json

`Axiom.Json` is an optional extension package on top of `Axiom.Assertions`.

It adds deterministic JSON assertions for raw JSON strings, `JsonDocument`, and `JsonElement` in .NET tests.

## Install

```bash
dotnet add package Axiom.Json
```

This package is designed to be used with `Axiom.Assertions`.

## Use this package when you want

- structural JSON equivalency checks with deterministic mismatch paths
- simple JSON path assertions over object members and array indexes
- JSON assertions without adding HTTP helpers or a broader API-test layer

## Install a different package when

- you only need the main fluent assertion library: install `Axiom.Assertions`
- you only need low-level batching or configuration primitives: install `Axiom.Core`
- you only need analyzers and code fixes: install `Axiom.Analyzers`
- you want `HttpResponseMessage` assertions that reuse JSON body comparison: install `Axiom.Http`
- you want vector and retrieval assertions: install `Axiom.Vectors` on top of `Axiom.Assertions`

Documentation: [spearzy.github.io/Axiom](https://spearzy.github.io/Axiom/)
