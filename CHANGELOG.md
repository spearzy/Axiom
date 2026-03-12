# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project follows [Semantic Versioning](https://semver.org/).

## [0.4.0] - 2026-03-12

### Added

- Unified consumer-facing startup facade:
  - `AxiomSettings.Configure(Action<AxiomSettingsOptions>)`
  - `AxiomSettings.Reset()`
- Expression-based equivalency selectors for named types:
  - `Ignore<TSubject>(...)`
  - `OnlyCompare<TSubject>(...)`
  - `UseComparer<TSubject>(...)`
  - `UseCollectionItemComparer<TSubject>(...)`
- Direct task outcome assertions and continuations for task subjects:
  - `Succeed()`, `SucceedWithin(...)`
  - `BeCanceled()`, `BeCanceledWithin(...)`
  - `BeFaultedWith<TException>()`, `BeFaultedWithWithin<TException>(...)`

### Changed

- `RequireStrictRuntimeTypes = false` now enables structural comparison across unrelated runtime types by default; `MatchMemberName(...)` is now only needed for renamed members.
- Updated README/NuGet setup guidance to promote one setup entrypoint via `AxiomSettings.Configure(...)`.
- Updated consumer smoke coverage and test namespaces for current assertion surface.

### Fixed

- Added focused configuration and equivalency tests to preserve compatibility between `AxiomSettings`, `AxiomServices`, and `EquivalencyDefaults`.

## [0.3.0] - 2026-03-09

### Added

- Assertion failure strategy abstraction with `AxiomServices.Configure(...)` integration.
- Built-in framework failure strategies for:
  - xUnit (`XunitFailureStrategy`)
  - NUnit (`NUnitFailureStrategy`)
  - MSTest (`MSTestFailureStrategy`)
- Typed `ContainSingle(predicate)` continuation access without consumer-side casting.

### Changed

- Added predicate-expression diagnostics across value and collection predicate assertions for clearer failure output.
- Added deterministic string-difference diagnostics for string assertion failures.
- Added `StringComparison` overloads for string equality assertions:
  - `Be(expected, comparison)`
  - `NotBe(unexpected, comparison)`
- Updated batch-routing test expectations to align with the improved string diagnostics.
- Expanded README and NuGet README documentation for the updated assertion and failure strategy surfaces.

### Fixed

- Upgraded `coverlet.msbuild` in test infrastructure from `6.0.4` to `8.0.0`.

## [0.2.1] - 2026-03-08

### Added

- String equality/null assertions: `Be(expected)`, `NotBe(unexpected)`, and `BeNull()`.
- Typed throw extraction via `.Thrown` for throw assertions.
- Dictionary value extraction via `ContainKey(key).WhoseValue`.
- Collection assertion expansions:
  - `ContainSingle().SingleItem`
  - `ContainSingle(predicate)`
  - `HaveUniqueItemsBy(selector[, comparer])`
  - `SatisfyRespectively(assertions...)`
  - Collection order assertions with key selector/comparer support.
- Direct async subject entrypoints for `Task`, `Task<T>`, `ValueTask`, and `ValueTask<T>` using `Should()`.

### Changed

- Optimised collection assertion pass-path by deferring sequence/entry formatting until failure.
- Optimised ordered equivalency collection comparison to stream both enumerables side-by-side; list materialisation now only occurs for any-order comparison mode.
- Replaced C# 14 `field` usage in `SingleItem` continuation for broader compiler compatibility.

### Fixed

- Consumer smoke test template issues for MSTest/NUnit variants.
- MSTest smoke assertion argument order issue in CI validation.

## [0.2.0] - 2026-03-07

### Added

- `BeOneOf(expectedValues)` and `NotBeOneOf(unexpectedValues)` assertions.
- `Satisfy(predicate)` and `NotSatisfy(predicate)` assertions.
- Exception detail chaining assertions:
  - `WithMessage(expectedMessage[, comparison])`
  - `WithParamName(expectedParamName)`
  - `WithInnerException<TInnerException>()`
- Async exception detail chaining support on `ThrowAsync<TException>()` and `ThrowExactlyAsync<TException>()`.

### Changed

- Added `StringComparison` overloads for string assertions such as `StartWith`, `EndWith`, and `Contain`.
- Added consumer smoke tests for xUnit, NUnit, and MSTest using locally packed Axiom artefacts in CI/release validation.
- Automated GitHub release notes generation from `CHANGELOG.md`.
- Updated README method list and examples to include newly implemented assertions.

## [0.1.0] - 2026-03-05

### Added

- First stable release of `Axiom.Core` and `Axiom.Assertions`.
- `BeApproximately(expected, tolerance)` assertions for `double`, `float`, and `decimal`, including chaining and batch routing support.

### Changed

- Moved `Should()` entrypoint extensions to `Axiom.Assertions` to remove extra namespace import friction.
- Updated install guidance in GitHub and NuGet readmes to stable package commands.
- Updated CI/release dependencies (`actions/checkout`, `actions/setup-dotnet`, `actions/upload-artifact`, `actions/download-artifact`) and test tooling dependencies (`Microsoft.NET.Test.Sdk`, `coverlet.collector`, `coverlet.msbuild`) for current platform support.

## [0.1.0-preview.1] - 2026-03-05

### Added

- First public preview packages: `Axiom.Core` and `Axiom.Assertions`.
- Fluent `Should()` assertions with chaining via `.And`.
- Batch-based aggregation using `Batch` with deterministic combined failures.
- Core assertion coverage for values, strings, collections, dictionaries, exceptions, temporal values, and configurable equivalency.
