# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project follows [Semantic Versioning](https://semver.org/).

## [2.2.0] - 2026-04-22

### Added
- Added `Axiom.Http` as an optional package for deterministic `HttpResponseMessage` assertions, including exact status-code, header, content-type, JSON body, and `ProblemDetails` checks.

### Changed
- Reused `Axiom.Json` for HTTP JSON body assertions and narrowed the internal boundary so HTTP-specific concepts stay in `Axiom.Http` while generic JSON behavior stays in `Axiom.Json`.
- Updated docs, package metadata, and release guidance to reflect the shipped first-wave HTTP surface.

### Fixed
- Tightened HTTP and JSON regression coverage so the shipped `Axiom.Http` surface and bridge behavior stay stable.

## [2.1.0] - 2026-04-22

### Added
- Added `Axiom.Json` as an optional package for deterministic JSON assertions over raw JSON `string`, `JsonDocument`, and `JsonElement`.
- Added structural JSON equivalency, simple JSON path assertions, and scalar value-at-path assertions.

### Changed
- Added package docs, metadata, and CI/release smoke coverage for the new JSON assertion package.

### Fixed
- Tightened JSON-focused regression coverage so the shipped first-wave JSON surface stays stable.

## [2.0.0] - 2026-04-21

### Added
- Expanded migration analyzers and code fixes across xUnit, NUnit, and MSTest for more high-confidence assertion shapes.
- Broadened `AssertionContext.Create(...)` so custom assertion authoring supports `StringAssertions` alongside the existing value-based path.

### Changed
- Hardened supported custom assertion authoring so `Batch`, failure rendering, subject labeling, configured failure strategy, and comparer-provider resolution behave like first-party assertions.
- Updated authoring and migration docs to match the shipped `2.0.0` surface.

### Fixed
- Tightened regression coverage around migration safety and assertion authoring behavior so the shipped flows stay aligned with the current API surface.

## [1.9.0] - 2026-04-10

### Added
- Added local comparer overloads for core value assertions, including equality and ordered comparison checks.

### Changed
- Made comparer precedence explicit across assertion APIs that support local comparers or configured comparer providers.
- Updated comparison, migration, category, and identity docs so the docs site and package guidance more clearly describe Axiom Assertions for .NET.
- Hardened NuGet metadata so `Axiom.Assertions`, `Axiom.Core`, `Axiom.Analyzers`, and `Axiom.Vectors` are more clearly differentiated for consumers.

### Fixed
- Tightened regression coverage around local comparison paths, comparer precedence, and related docs examples.

## [1.8.0] - 2026-04-09

### Added
- Added `ContainExactlyInAnyOrder(...)` for synchronous collections.
- Added local comparer overloads across collection, async collection, and dictionary value assertions.

### Changed
- Expanded collection ordering and exactness support so local equality and ordering rules can be applied per assertion.
- Updated docs and snippet coverage to reflect the current collection and dictionary assertion surface.

### Fixed
- Tightened regression coverage around collection comparer paths and related docs examples.

## [1.7.0] - 2026-04-08

### Added
- Added temporal assertions for inclusive bounds, ranges, and inverse tolerance checks across the supported date and time types.
- Added comparer-aware async collection assertions for the newer async enumerable APIs.

### Changed
- Hardened async execution and batching behavior across the async assertion surface.
- Updated docs and snippet coverage to reflect the expanded temporal and async assertion APIs.

### Fixed
- Tightened regression coverage around async batching and comparer-driven assertion paths.

## [1.6.0] - 2026-04-08

### Added
- Extended async enumerable assertions to more fully match the capabilities of the synchronous collection assertion API.
- Added fuller batching coverage for vector assertions.
- Added a small public benchmark snapshot to the docs site.

### Changed
- Improved performance in a small number of measured hot paths.
- Updated benchmark tracking so representative scenarios can be refreshed and reviewed more easily.

### Fixed
- Tightened regression coverage around async assertions, vector batching, and performance-sensitive paths.

## [1.5.0] - 2026-04-04

### Added

- Added retrieval and ranking evaluation assertions to `Axiom.Vectors`, including top-k, rank, recall@k, precision@k, reciprocal rank, mean reciprocal rank, and hit-rate assertions.

### Changed

- Hardened retrieval/ranking diagnostics, edge-case coverage, and vector docs/examples so ranking behavior and metrics are more explicit and consistent.

### Fixed

- Tightened retrieval/ranking regression coverage so documented behavior remains stable.

## [1.4.0] - 2026-04-03

### Added

- Added the next `Axiom.Vectors` assertion wave, including vector math assertions for dot product, Euclidean distance, and zero-vector checks.

### Changed

- Hardened vector diagnostics and edge-case coverage so failures around dimension mismatch, numeric output, and zero-vector behavior are more consistent.
- Updated vector docs, examples, and verification coverage to match the current vector assertion surface.

### Fixed

- Tightened vector regression coverage so documented diagnostics and behavior remain stable.

## [1.3.0] - 2026-04-03

### Added

- Added MSTest migration wave 1 for high-confidence `Assert.*` shapes.
- Added async-stream assertion support for `ContainExactlyAsync(...)`, `ContainAnyAsync(...)`, and `NotContainAnyAsync(...)`.

### Changed

- Hardened equivalency coverage and diagnostics around mappings, comparer precedence, and representative failure output.
- Updated docs, examples, and verification coverage to reflect the current async-stream and equivalency behavior.

### Fixed

- Tightened equivalency regression coverage so documented behavior and diagnostics remain stable.

## [1.2.0] - 2026-04-02

### Added

- Added NUnit migration wave 1 for high-confidence `Assert.That(...)` constraint shapes.
- Added docs snippet verification to keep docs examples compile-checked against the current codebase.

### Changed

- Improved the local development path for direct `ProjectReference` consumers so analyzer/code-fix behavior is supported more explicitly and reliably.
- Analyzer and migration docs now reflect the broader migration support and snippet verification workflow.

### Fixed

- Tightened migration and docs validation so shipped examples and analyzer guidance stay aligned with the current API surface.

## [1.1.0] - 2026-04-01

### Added

- Expanded xUnit migration analyzers and code fixes for more high-confidence `Assert.*` shapes, including consumed-result continuations where Axiom has a matching fluent continuation.
- Added xUnit dictionary-key containment migrations.
- Added xUnit `Assert.Throws<TException>(paramName, Action)` migration support.
- Added xUnit string `Assert.StartsWith(...)` and `Assert.EndsWith(...)` migration support.

### Changed

- Migration docs and analyzer docs now cover the broader xUnit migration surface.

### Fixed

- Tightened migration safety checks so suggestions only appear when the generated Axiom fluent API is valid for the actual receiver shape.

## [1.0.0] - 2026-03-29

### Fixed

- Cleaned up xUnit migration code-fix output so generated Axiom assertions avoid unnecessary fully qualified names.
- Improved the `Assert.Throws<T>(...)` migration fix output to read more naturally.

## [0.10.0] - 2026-03-29

### Added

- xUnit migration analyzers and code fixes for high-confidence `Assert.*` patterns, covering:
  - `Equal`, `NotEqual`
  - `Null`, `NotNull`
  - `True`, `False`
  - `Empty`, `NotEmpty`
  - `Contains`, `DoesNotContain`
  - `Single`
  - `Same`, `NotSame`
  - `Throws<TException>(...)` when the returned exception is not consumed
- BenchmarkDotNet-based benchmark harness for representative Axiom scenarios.
- Benchmarks docs page and migration guide docs.

### Changed

- Axiom now auto-detects the default test-framework failure strategy for xUnit, NUnit, and MSTest.
- Onboarding docs now treat setup and configuration as optional instead of required for the normal install-and-use path.
- Equivalency docs and migration guidance were tightened with clearer defaults, examples, tradeoffs, and representative diagnostics.

### Fixed

- Narrowed xUnit equality and reference migration suggestions so diagnostics are only offered when the generated Axiom fluent API is valid for the receiver type.

## [0.9.0] - 2026-03-27

### Added

- Expanded async-stream assertions with:
  - `SatisfyRespectivelyAsync(...)`
  - `HaveUniqueItemsAsync()`
  - `HaveUniqueItemsByAsync(...)`
  - `ContainInOrderAsync(...)`

### Changed

- Improved `SatisfyRespectivelyAsync(...)` failure wrapping so ordered item assertions preserve wrapper-level failing-index context and `because` under `Batch`.
- Aligned sync `HaveUniqueItems()` comparer behavior for generic collections with the async uniqueness assertions.
- Improved the vector cosine similarity API with preferred `HaveCosineSimilarityWith(...)` naming and richer threshold assertions such as `AtMost(...)` and `Between(...)`.
- Added and polished the GitHub Pages documentation site, including getting-started guidance and built-in light/dark mode support.

## [0.8.0] - 2026-03-25

### Added

- First `Axiom.Vectors` package release for vector and embedding-style assertions, including:
  - `HaveDimension(...)`
  - `NotContainNaNOrInfinity()`
  - `BeApproximatelyEqualTo(...)`
  - `HaveCosineSimilarityTo(...).AtLeast(...)`
  - `BeNormalized(...)`
- Vector `Should()` entry points for:
  - `float[]`
  - `double[]`
  - `ReadOnlyMemory<float>`
  - `ReadOnlyMemory<double>`

### Changed

- Updated release automation and package smoke coverage so `Axiom.Vectors` is packed, validated, and published alongside the other Axiom packages.
- Expanded release-facing docs and the assertion reference to treat `Axiom.Vectors` as a shipped optional package rather than a planned addition.

## [0.7.1] - 2026-03-25

### Added

- Publish `Axiom.Analyzers` as a standalone NuGet package on tagged releases.

### Changed

- Updated release automation and smoke coverage so both:
  - bundled analyzers through `Axiom.Assertions`
  - standalone `Axiom.Analyzers`
  are validated and published consistently.
- Clarified docs so `Axiom.Assertions` remains the default package and `Axiom.Analyzers` is documented as an optional standalone package.

## [0.7.0] - 2026-03-25

### Added

- Initial `Axiom.Analyzers` rule set with:
  - `AXM0001` for ignored async Axiom assertion results
  - `AXM0002` for `Batch` instances created without being disposed
- Direct `IAsyncEnumerable<T>` assertions via `AsyncEnumerableAssertions<T>`, including:
  - `BeEmptyAsync()`
  - `NotBeEmptyAsync()`
  - `HaveCountAsync(...)`
  - `ContainAsync(...)`
  - `OnlyContainAsync(...)`
  - `ContainSingleAsync(...)`
- `ShouldAsyncEnumerable()` as an explicit async-stream entry point for concrete wrapper types that would otherwise bind to generic value assertions.

### Changed

- Improved equivalency diagnostics with:
  - clearer missing-member vs extra-member reporting
  - typed mapping path context on failures
  - richer string mismatch detail inside equivalency failures
  - explicit omitted-difference summaries when `MaxDifferences` truncates output
- Bundled the Axiom analyzers/code fixes into `Axiom.Assertions` so the default consumer package lights up diagnostics automatically.

## [0.6.0] - 2026-03-23

### Added

- Typed cross-type equivalency member mapping with `MatchMember<TActual, TExpected>(...)`, including nested member-path support.
- `AxiomSettings`-level reusable modules with:
  - `IAxiomSettingsModule`
  - `AxiomSettings.UseModule(...)`
  - `AxiomSettings.UseModules(...)`
  - bridging support for existing core-only `IAxiomModule` modules through `AxiomSettings.UseModule(...)`

### Changed

- Expanded the custom assertion authoring guidance with more practical domain examples built on `AssertionContext.Create(...)`.
- Updated release-facing documentation to cover typed equivalency mapping, `AxiomSettings` modules, and the preferred `0.6.0` setup/configuration workflows.

## [0.5.0] - 2026-03-23

### Added

- Supported custom assertion authoring on `ValueAssertions<T>` with:
  - `AssertionContext.Create(...)`
  - `AssertionContext<TAssertions, TSubject>` for subject access, `.And()` continuation wiring, and routed failures
- Async function result assertions for:
  - `Func<Task<T>>`
  - `Func<ValueTask<T>>`
- New `AsyncFunctionAssertions<T>` outcome and exception assertions, including:
  - `ThrowAsync(...)` and `ThrowExactlyAsync(...)`
  - `NotThrowAsync()`
  - `CompleteWithin(...)` and `NotCompleteWithin(...)`
  - `Succeed()` and `SucceedWithin(...)`
  - `BeCanceled()` and `BeCanceledWithin(...)`
  - `BeFaultedWith<TException>()` and `BeFaultedWithWithin<TException>(...)`

### Changed

- Parameterless `ContainSingle()` now keeps a typed `SingleItem` for common generic collection subjects such as arrays, `List<T>`, and interface-typed generic collections.
- Expanded release-facing documentation with:
  - a dedicated custom assertions guide
  - updated assertion reference coverage for authoring and async function result assertions
  - refreshed README and NuGet README examples for shipped `0.5.0` APIs

## [0.4.1] - 2026-03-12

### Fixed

- Corrected the assertion reference to document the current direct task entry points and task outcome assertions.
- Updated GitHub and NuGet README async examples to show direct task success/result assertions alongside the `AxiomSettings.Configure(...)` setup guidance.

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
