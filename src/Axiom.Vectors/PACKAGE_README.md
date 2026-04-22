# Axiom.Vectors

`Axiom.Vectors` is an optional extension package on top of `Axiom.Assertions`.

It adds vector, embedding, and ranked retrieval assertions for .NET tests.

## Install

```bash
dotnet add package Axiom.Vectors
```

This package is designed to be used with `Axiom.Assertions`.

## Use this package when you want

- dimension, normalization, and numeric-validity checks for vectors
- similarity and distance assertions for embeddings
- ranked retrieval assertions such as top-k, rank, recall, precision, reciprocal rank, mean reciprocal rank, and hit rate

## Install a different package when

- you only need the main fluent assertion library: install `Axiom.Assertions`
- you only need low-level batching or configuration primitives: install `Axiom.Core`
- you only need analyzers and code fixes: install `Axiom.Analyzers`
- you want JSON assertions: install `Axiom.Json` on top of `Axiom.Assertions`
- you want `HttpResponseMessage` assertions: install `Axiom.Http` on top of `Axiom.Assertions`

Documentation: [spearzy.github.io/Axiom](https://spearzy.github.io/Axiom/)
