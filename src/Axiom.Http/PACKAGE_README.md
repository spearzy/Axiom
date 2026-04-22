# Axiom.Http

`Axiom.Http` is an optional extension package on top of `Axiom.Assertions` and `Axiom.Json`.

It adds deterministic `HttpResponseMessage` assertions for status codes, headers, content types, JSON bodies, and ProblemDetails-style error responses in .NET tests.

## Install

```bash
dotnet add package Axiom.Http
```

This package is designed to be used with `Axiom.Assertions` and reuses `Axiom.Json` for JSON body assertions.

## Use this package when you want

- exact `HttpResponseMessage` status code assertions
- practical header and content-type assertions for API tests
- JSON body assertions without rebuilding JSON comparison logic
- focused ProblemDetails assertions for common error-response tests

## Install a different package when

- you only need the main fluent assertion library: install `Axiom.Assertions`
- you only need low-level batching or configuration primitives: install `Axiom.Core`
- you only need analyzers and code fixes: install `Axiom.Analyzers`
- you only need JSON document or raw JSON assertions without HTTP response helpers: install `Axiom.Json`
- you want vector and retrieval assertions: install `Axiom.Vectors` on top of `Axiom.Assertions`

Documentation: [spearzy.github.io/Axiom](https://spearzy.github.io/Axiom/)
