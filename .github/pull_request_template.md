## Summary

Describe the change in 2-5 sentences.

## Consumer Value

What does this add or improve for library users?

- [ ] New assertion capability
- [ ] Better diagnostics / failure messages
- [ ] Better developer ergonomics
- [ ] Analyzer improvement
- [ ] Vector / embedding assertions
- [ ] Docs / discoverability
- [ ] Packaging / release alignment
- [ ] Internal-only change

If this is not consumer-facing, explain why it still belongs.

## Package Impact

Which packages are affected?

- [ ] `Axiom.Assertions`
- [ ] `Axiom.Core`
- [ ] `Axiom.Analyzers`
- [ ] `Axiom.Vectors`
- [ ] Docs site
- [ ] Release / CI only

## API Surface

List any public API additions, removals, or behavior changes.

```csharp
// example signatures or usage
```

If none:
- [ ] No public API changes

## Examples

Show the intended usage if this changes consumer-facing behavior.

```csharp
// before / after or new usage
```

## Design Notes

Capture the key design decision(s) and tradeoffs briefly.

- Decision:
- Why:
- Tradeoff / risk:

## Performance / Pay-to-Play

Does this affect pass-path cost, allocations, or hot-path behavior?

- [ ] No meaningful performance impact
- [ ] Yes, and I validated the impact
- [ ] Yes, but only on failure / opt-in paths

Notes:

## Diagnostics / Failure Output

If failure messages changed, explain what changed and why.

- [ ] No failure output changes
- [ ] Failure output changed intentionally

Notes:

## Tests

What did you add or update?

- [ ] Unit tests
- [ ] Analyzer tests
- [ ] Smoke tests
- [ ] Docs validation
- [ ] No tests needed

## Docs

Docs updated?

- [ ] README
- [ ] NuGet README
- [ ] Assertion reference
- [ ] Guide docs
- [ ] Docs site
- [ ] No docs changes needed

## Breaking Changes

- [ ] No breaking changes
- [ ] Yes

If yes, explain the break and migration path.

## Release Notes

If this lands in a release, what should the changelog/release notes say?

- Added:
- Changed:
- Fixed:

## Checklist

- [ ] The change is scoped and reviewable
- [ ] Public API shape is intentional
- [ ] Failure messages are deterministic
- [ ] Pay-to-play / pass-path impact was considered
- [ ] Tests cover the intended behavior
- [ ] Docs were updated where needed
- [ ] Changelog / release implications were considered
