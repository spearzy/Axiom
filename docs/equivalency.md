# Equivalency

Use `Be(...)` when direct equality is the thing you care about.

Use `BeEquivalentTo(...)` when you care about the shape and values of an object graph.

That usually means DTOs, API responses, snapshots, projections, and other values where instance identity does not matter but member-by-member comparison does.

## When To Use `Be(...)` vs `BeEquivalentTo(...)`

Use `Be(...)` for simple values and for types that already define the exact equality semantics you want.

```csharp
order.Total.Should().Be(129.48m);
status.Should().Be(OrderStatus.Submitted);
```

Use `BeEquivalentTo(...)` when spelling the assertion as many scalar checks would be noisy or would hide the real comparison shape.

```csharp
actual.Should().BeEquivalentTo(expected);
```

A good rule is:

- if the important question is "are these two values equal?", use `Be(...)`
- if the important question is "does this object graph match the expected shape and values?", use `BeEquivalentTo(...)`

## Default Rules

Axiom starts from a strict baseline.

By default:

- runtime types must match exactly
- collection order matters
- public properties and public fields participate
- missing members on actual fail the assertion
- extra members on actual fail the assertion
- string leaves use `StringComparison.Ordinal`
- numeric and temporal leaves require exact equality unless a tolerance is configured

That means `BeEquivalentTo(...)` does not try to guess what you meant. You loosen the rules explicitly when the test needs it.

## A Normal First Equivalency Assertion

This is a typical first use:

```csharp
using Axiom.Assertions;
using Axiom.Assertions.Equivalency;

var actual = new
{
    Name = "Bob",
    Scores = new[] { 3, 1, 2 },
    UpdatedAt = new DateTime(2026, 3, 8, 12, 0, 0, DateTimeKind.Utc)
};

var expected = new
{
    Name = "Bob",
    Scores = new[] { 1, 2, 3 },
    UpdatedAt = new DateTime(2026, 3, 8, 12, 0, 1, DateTimeKind.Utc)
};

actual.Should().BeEquivalentTo(expected, options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
    options.DateTimeTolerance = TimeSpan.FromSeconds(1);
});
```

This keeps the assertion at the level of the object graph while making the two deliberate exceptions obvious:

- collection order does not matter here
- a one-second timestamp difference is acceptable here

## Comparing Different Shapes

### Cross-Type Comparison With The Same Member Names

If the two values are different runtime types but the member names already line up, start here:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.RequireStrictRuntimeTypes = false);
```

This is the simplest cross-type setup.

### Renamed Members

If the types represent the same concept but some members were renamed, use typed member mapping.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
    options.MatchMember<ActualUser, ExpectedUser>(x => x.Address.Postcode, x => x.Location.ZipCode);
});
```

Prefer `MatchMember<TActual, TExpected>(...)` when the types are known at compile time. It is the clearest option and it fails early for invalid selectors.

Use `MatchMemberName(...)` when the mapping has to come from strings:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.MatchMemberName("GivenName", "FirstName");
});
```

Both mapping styles are one-to-one. Repeating the same mapping is fine, but conflicting remaps fail fast instead of silently overriding an earlier mapping.

Member mapping precedence is:

1. typed `MatchMember<TActual, TExpected>(...)`
2. `MatchMemberName(...)`
3. same-name matching

If both typed and name-based mappings could apply, typed mapping wins.

## Choosing What To Compare

### Ignore A Branch Or Member

Use ignore rules when part of the graph is volatile and not relevant to the test.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.Ignore<OrderSnapshot>(x => x.Metadata.RequestId));
```

You can also ignore by path:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.IgnorePath("actual.Metadata.RequestId"));
```

For named types, prefer expression selectors. They survive renames better than string paths.

### Compare Only A Stable Subset

Use `OnlyCompare(...)` when the test intentionally cares about only part of the graph.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.OnlyCompare<OrderSnapshot>(x => x.Id, x => x.Total, x => x.Status));
```

This is often clearer than comparing the whole graph and ignoring many unrelated members.

### Missing And Extra Members

When you disable strict runtime type matching, shape differences can still fail because one side has members the other side does not.

If that strictness is not useful for a specific test, loosen it explicitly:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.FailOnMissingMembers = false;
    options.FailOnExtraMembers = false;
});
```

This is a common fit for cross-type DTO comparisons where the graph shapes are similar but not identical.

## Comparers, Strings, And Tolerances

Axiom applies leaf-level rules in this order:

1. configured tolerance for the leaf type
2. configured path comparer
3. `StringComparison` for string leaves
4. configured per-call type comparer
5. global comparer provider from `AxiomServices`
6. default equality

The order is significant. Later rules do not override earlier ones.

### String Comparison

Use `StringComparison` when one string rule should apply across the whole graph.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.StringComparison = StringComparison.OrdinalIgnoreCase);
```

If you only want special handling for one member, use a member or path comparer instead.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.UseComparer<Person>(x => x.Name, StringComparer.OrdinalIgnoreCase));
```

### Type, Member, And Path Comparers

Use these when the equality rule is domain-specific and narrower than the whole graph:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.UseComparerForType<int>(new OddEvenMatchIntComparer()));

actual.Should().BeEquivalentTo(expected, options =>
    options.UseComparerForMember("Name", StringComparer.OrdinalIgnoreCase));

actual.Should().BeEquivalentTo(expected, options =>
    options.UseComparerForPath("actual.Name", StringComparer.OrdinalIgnoreCase));
```

### Tolerances

Use tolerances when the value really is approximate.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.DoubleTolerance = 0.01d;
    options.DateTimeTolerance = TimeSpan.FromSeconds(1);
});
```

Available tolerance options include:

- `FloatTolerance`
- `DoubleTolerance`
- `HalfTolerance`
- `DecimalTolerance`
- `DateOnlyTolerance`
- `DateTimeTolerance`
- `DateTimeOffsetTolerance`
- `TimeOnlyTolerance`
- `TimeSpanTolerance`

## Collection Behavior

`CollectionOrder` controls whether collections are compared strictly by position or as any-order sets of items.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.CollectionOrder = EquivalencyCollectionOrder.Strict);

actual.Should().BeEquivalentTo(expected, options =>
    options.CollectionOrder = EquivalencyCollectionOrder.Any);
```

Use `Strict` when the order itself is part of the behavior.

Use `Any` when the collection is logically unordered and position is noise.

For collection members that need item identity rules instead of full structural matching, use a collection-item comparer:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.UseCollectionItemComparer<Order>(x => x.Items, new LineItemSkuComparer()));
```

## Representative Failure Output

These snippets show what Axiom reports when equivalency fails.

### Nested Member Mismatch

A plain nested mismatch stays rooted on the actual-side path.

```text
Expected actual to be equivalent to expected, but found 1 difference(s):
1) actual.Address.Postcode: expected "ZZ9 9ZZ", but found "AB1 2CD" (string mismatch; ...)
```

### Renamed Member Mapping Mismatch

When a typed mapping is involved, Axiom keeps the actual-side path and adds the expected-side path it was compared against.

```text
Expected actual to be equivalent to expected, but found 1 difference(s):
1) actual.Address.Postcode (compared with expected.Location.ZipCode): expected "ZZ9 9ZZ", but found "AB1 2CD" (string mismatch; ...)
```

### Ordered Collection Mismatch

With strict ordering, the report points at the element index that diverged.

```text
Expected actual to be equivalent to expected, but found 3 difference(s):
1) actual[0]: expected 1, but found 3
```

### `MaxDifferences` Truncation

When the report is truncated, the truncation is explicit.

```text
Expected actual to be equivalent to expected, but found 4 difference(s):
1) actual.Age: expected 30, but found 31
2) actual.Name: expected "Bob", but found "Alice" (string mismatch; ...)
+ 2 additional difference(s) omitted after reaching MaxDifferences = 2.
```

## Reading The Report

A few things are worth knowing when you read an equivalency failure:

- paths are reported from the actual-side graph
- mapped members show the expected-side path only when it differs from the actual-side path
- missing and extra members say explicitly which side is missing what
- string mismatches include extra detail instead of only saying the values differ
- `MaxDifferences` keeps the report deterministic and tells you what was omitted

If the graph is large, start with the first reported difference. The report is ordered and stable, so the first few lines usually explain the real problem quickly.

## Project-Wide Defaults

Per-call configuration is usually the clearest place to start.

If a team keeps applying the same equivalency rules in many tests, move them into shared defaults:

```csharp
AxiomSettings.Configure(options =>
{
    options.Equivalency.CollectionOrder = EquivalencyCollectionOrder.Any;
    options.Equivalency.StringComparison = StringComparison.OrdinalIgnoreCase;
    options.Equivalency.FailOnExtraMembers = false;
});
```

`EquivalencyDefaults.Configure(...)` is still available when you want to work with equivalency defaults in isolation, but `AxiomSettings.Configure(...)` is the better general entry point for shared project configuration.

## Practical Migration Notes

If you are moving tests from another assertion style:

- start with `actual.Should().BeEquivalentTo(expected)`
- add `RequireStrictRuntimeTypes = false` only when the types really are different shapes of the same concept
- add `MatchMember(...)` only for the members that were renamed
- loosen member-presence rules only when the shape difference is intentional
- use tolerances and comparers to model real domain semantics, not to hide unclear expectations

For broader migration guidance, see [Migrating to Axiom](migrating-to-axiom.md).

For the full option catalog, see [assertion-reference.md](assertion-reference.md).
