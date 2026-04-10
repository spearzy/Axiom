# Axiom.Assertions

`Axiom.Assertions` is the main package for most users.

It gives you Axiom's fluent `Should()` API for values, strings, collections, dictionaries, exceptions, async code, temporal assertions, and structural equivalency. It also bundles the Axiom analyzers and code fixes in the default install path.

## Install

```bash
dotnet add package Axiom.Assertions
```

## Use this package when you want

- the main fluent assertion library for .NET tests
- deterministic failure output
- grouped failures with `Batch`
- built-in equivalency support
- bundled analyzers and code fixes

## Install a different package when

- you only want low-level primitives such as batching, formatting, or configuration: install `Axiom.Core`
- you only want the diagnostics and code fixes without runtime assertions: install `Axiom.Analyzers`
- you want vector, embedding, or retrieval assertions on top of the main package: install `Axiom.Vectors`

Documentation: [spearzy.github.io/Axiom](https://spearzy.github.io/Axiom/)
