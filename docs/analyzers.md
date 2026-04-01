# Analyzers

Installing `Axiom.Assertions` gives you the Axiom Roslyn analyzers/code fixes automatically. That is the default install path for most users.

`Axiom.Analyzers` still exists as an optional standalone package if you only want the diagnostics without the runtime assertion library.

```bash
dotnet add package Axiom.Assertions
```

Install the standalone package only if you want the diagnostics on their own:

```bash
dotnet add package Axiom.Analyzers
```

The current rules focus on three high-value areas:

- ignored async Axiom assertion results
- `Batch` instances created without `using`
- high-confidence xUnit `Assert.*` migration suggestions

## Async Assertions Must Be Awaited

Rule: `AXM0001`

This rule flags ignored async Axiom assertion calls from:

- `AsyncActionAssertions`
- `AsyncFunctionAssertions<T>`
- `TaskAssertions`
- `TaskAssertions<T>`

Examples include:

- `ThrowAsync(...)`
- `NotThrowAsync()`
- `Succeed()`
- `SucceedWithin(...)`
- `BeFaultedWith(...)`

Async Axiom assertions are lazy until their returned `ValueTask` is awaited or returned. If the result is ignored, the test can appear to pass even though the assertion never executed.

## Before / After

Before:

```csharp
loader.Should().Succeed();
```

After:

```csharp
await loader.Should().Succeed();
```

The analyzer also offers a code fix in async contexts where prepending `await` is safe.

## Batch Must Be Disposed

Rule: `AXM0002`

This rule flags `Batch` instances created without `using`.

Before:

```csharp
var batch = Assert.Batch("user");
user.Name.Should().NotBeNull();
```

After:

```csharp
using var batch = Assert.Batch("user");
user.Name.Should().NotBeNull();
```

`Batch` flushes aggregated failures when it is disposed. If it is created without `using`, failures may never be emitted at the end of the scope.

The analyzer offers a code fix for the common local declaration case by converting `var batch = ...;` to `using var batch = ...;`.

## xUnit Assert Migration Suggestions

Rules:

- `AXM1001` for `Assert.Equal(expected, actual)`
- `AXM1002` for `Assert.NotEqual(expected, actual)`
- `AXM1003` for `Assert.Null(value)`
- `AXM1004` for `Assert.NotNull(value)`
- `AXM1005` for `Assert.True(condition)`
- `AXM1006` for `Assert.False(condition)`
- `AXM1007` for `Assert.Empty(subject)`
- `AXM1008` for `Assert.NotEmpty(subject)`
- `AXM1009` for `Assert.Contains(item, collection)`
- `AXM1010` for `Assert.DoesNotContain(item, collection)`
- `AXM1011` for `Assert.Single(subject)`, appending `.SingleItem` when the single item is used
- `AXM1012` for `Assert.Same(expected, actual)`
- `AXM1013` for `Assert.NotSame(expected, actual)`
- `AXM1014` for `Assert.Throws<TException>(...)`, including non-null constant `paramName` + `Action` overloads and appending `.Thrown` when the exception is used
- `AXM1015` for `Assert.IsType<T>(actual)`
- `AXM1016` for `Assert.IsAssignableFrom<T>(actual)`
- `AXM1017` for `Assert.Contains(expectedSubstring, actualString)`
- `AXM1018` for `Assert.DoesNotContain(expectedSubstring, actualString)`
- `AXM1019` for `Assert.Single(collection, predicate)`, appending `.SingleItem` when the matched item is used
- `AXM1020` for `Assert.Contains(key, dictionary)`, appending `.WhoseValue` when the associated value is used
- `AXM1021` for `Assert.DoesNotContain(key, dictionary)`

The migration support is intentionally narrow and high-confidence. It only offers diagnostics and code fixes for xUnit assertion shapes that map cleanly to Axiom's fluent API without changing value flow or subtle overload semantics.

Before:

```csharp
Assert.Equal(expected, actual);
Assert.True(condition);
Assert.Empty(values);
Assert.Contains(expected, values);
Assert.Contains("sub", actual);
Assert.Contains(key, lookup);
var found = Assert.Contains(key, lookup);
var item = Assert.Single(values);
var match = Assert.Single(values, value => value > 0);
Assert.Throws<InvalidOperationException>(() => work());
var ex = Assert.Throws<ArgumentNullException>("name", () => work());
```

After:

```csharp
actual.Should().Be(expected);
condition.Should().BeTrue();
values.Should().BeEmpty();
values.Should().Contain(expected);
actual.Should().Contain("sub");
lookup.Should().ContainKey(key);
var found = lookup.Should().ContainKey(key).WhoseValue;
var item = values.Should().ContainSingle().SingleItem;
var match = values.Should().ContainSingle(value => value > 0).SingleItem;
new Action(() => work()).Should().Throw<InvalidOperationException>();
var ex = new Action(() => work()).Should().Throw<ArgumentNullException>().WithParamName("name").Thrown;
```

The migration suggestions use semantic matching, so they only target xUnit's real `Assert` API. They do not flag custom helper classes named `Assert`.

The dictionary-key rules follow Axiom's `ContainKey` and `NotContainKey` receiver shape. They support `IDictionary<TKey, TValue>`, `IReadOnlyDictionary<TKey, TValue>`, `Dictionary<TKey, TValue>`, `ReadOnlyDictionary<TKey, TValue>`, `ConcurrentDictionary<TKey, TValue>`, and `ImmutableDictionary<TKey, TValue>`.

They also intentionally skip shapes that are not obviously semantics-preserving yet, including:

- overloads with custom comparers, precision, inspectors, or user messages
- nongeneric `Assert.Single(subject)` calls when the returned item is used
- `Assert.Throws<TException>(...)` consumed-result shapes outside the `string? paramName, Action testCode` overload
- `Assert.Throws<TException>(paramName, ...)` when `paramName` is not an obvious non-null constant string

For a broader mapping table and practical migration notes, see [Migrating to Axiom](migrating-to-axiom.md).
