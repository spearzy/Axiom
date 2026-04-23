# Axiom.Analyzers

`Axiom.Analyzers` contains the Roslyn analyzers and code fixes for Axiom.

It is a special-case package. It does not include the runtime fluent assertion APIs. Use it when you want the diagnostics only.

## Install

```bash
dotnet add package Axiom.Analyzers
```

## Use this package when you want

- Axiom diagnostics and code fixes without the runtime assertion library
- migration suggestions for supported xUnit, NUnit, and MSTest assertion shapes
- checks for common Axiom assertion mistakes in the editor and build

## Install a different package when

- you want the main fluent assertion library and the analyzers together: install `Axiom.Assertions`
- you only need low-level Axiom primitives: install `Axiom.Core`
- you want JSON assertions: install `Axiom.Json` on top of `Axiom.Assertions`
- you want `HttpResponseMessage` assertions: install `Axiom.Http` on top of `Axiom.Assertions`
- you want vector and retrieval assertions: install `Axiom.Vectors` on top of `Axiom.Assertions`

Documentation: [spearzy.github.io/Axiom](https://spearzy.github.io/Axiom/)
Getting started: [Getting Started](https://spearzy.github.io/Axiom/getting-started/)
