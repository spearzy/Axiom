# Axiom.Core

`Axiom.Core` contains low-level Axiom primitives such as batching, failure modelling, formatting, and configuration.

It is not the default install path for most test projects. Most users should install `Axiom.Assertions` instead.

## Install

```bash
dotnet add package Axiom.Core
```

## Use this package when you want

- `Batch` without the full fluent assertion surface
- low-level formatting or failure infrastructure
- advanced integration or extension work on top of Axiom

## Install a different package when

- you want the main fluent assertion library: install `Axiom.Assertions`
- you want analyzers and code fixes only: install `Axiom.Analyzers`
- you want JSON assertions: install `Axiom.Json` on top of `Axiom.Assertions`
- you want `HttpResponseMessage` assertions: install `Axiom.Http` on top of `Axiom.Assertions`
- you want vector and retrieval assertions: install `Axiom.Vectors` on top of `Axiom.Assertions`

Documentation: [spearzy.github.io/Axiom](https://spearzy.github.io/Axiom/)
