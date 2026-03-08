# Equivalency

`BeEquivalentTo(...)` and `NotBeEquivalentTo(...)` let you compare object graphs structurally instead of relying on direct equality. This is the part of Axiom you use when two values should be considered the same because their shape and members match, even if they are different instances.

## Basic Usage

```csharp
using Axiom.Assertions;
using Axiom.Assertions.Equivalency;

var actual = new
{
    Name = "Ada",
    Scores = new[] { 3, 1, 2 },
    UpdatedAt = new DateTime(2026, 3, 8, 12, 0, 0, DateTimeKind.Utc)
};

var expected = new
{
    Name = "Ada",
    Scores = new[] { 1, 2, 3 },
    UpdatedAt = new DateTime(2026, 3, 8, 12, 0, 1, DateTimeKind.Utc)
};

actual.Should().BeEquivalentTo(expected, options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
    options.DateTimeTolerance = TimeSpan.FromSeconds(1);
});
```

Use `NotBeEquivalentTo(...)` when you want the inverse assertion under the same equivalency rules.

## Default Behavior

Out of the box, Axiom equivalency uses these defaults:

- strict runtime type matching
- strict collection ordering
- public properties and public fields are included
- missing members on the actual value fail the assertion
- extra members on the actual value fail the assertion
- string comparison is `StringComparison.Ordinal`
- numeric and temporal values require exact equality unless a tolerance is configured

That means `BeEquivalentTo(...)` starts from a conservative, explicit baseline and only relaxes behavior when you ask it to.

## Where To Configure It

Per assertion:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.IgnorePath("actual.UpdatedAt");
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
});
```

Project-wide defaults:

```csharp
EquivalencyDefaults.Configure(options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
    options.FailOnExtraMembers = false;
});
```

Reset project-wide defaults:

```csharp
EquivalencyDefaults.Reset();
```

Per-call configuration always overrides global defaults for that assertion.

## Comparison Precedence

When Axiom compares one leaf value against another, it applies configuration in this order:

1. configured tolerance for the leaf type
2. configured path comparer
3. `StringComparison` for string leaves
4. configured per-call type comparer
5. global comparer provider from `AxiomServices`
6. default equality

This order matters. A few practical consequences follow from it:

- tolerances win over path comparers on the same numeric or temporal leaf
- `UseComparerForPath(...)` and `UseComparerForMember(...)` can override string behavior for a specific member
- `UseComparerForType<string>(...)` is not the general way to control string leaf comparison; use `StringComparison` for broad string behavior or a path/member comparer for a specific string member

## Selecting What To Compare

### Ignore One Member Everywhere

Use `IgnoreMember(...)` when a member should be ignored regardless of where it appears in the object graph.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.IgnoreMember("UpdatedAt"));
```

### Ignore One Branch Or Path

Use `IgnorePath(...)` when you want to ignore a specific branch. Ignoring a path also ignores its children.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.IgnorePath("actual.Address"));
```

`IgnorePath(...)` uses the rooted graph path form such as `actual.Address`, and child paths under that branch are ignored automatically.

### Compare Only Selected Members

Use `OnlyCompareMember(...)` or `OnlyCompareMembers(...)` when you want equivalency to focus on a small part of the object.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.OnlyCompareMember("Name"));

actual.Should().BeEquivalentTo(expected, options =>
    options.OnlyCompareMembers(
        "Name",
        "Address.Postcode"));
```

This is useful when a test only cares about a stable subset of the graph and you want the assertion to ignore everything else.

### Control Whether Properties And Fields Participate

By default, Axiom compares public properties and public fields. You can opt out of either:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.IncludePublicProperties = true;
    options.IncludePublicFields = false;
});
```

## Comparing Different Shapes

### Allow Assignable Runtime Types

By default, runtime types must match exactly. If you want to compare across a base/derived boundary, disable strict runtime type enforcement:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.RequireStrictRuntimeTypes = false);
```

### Map Members With Different Names

When two types represent the same concept but use different member names, map them explicitly:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.MatchMemberName("GivenName", "FirstName");
});
```

This tells Axiom to compare `actual.GivenName` with `expected.FirstName`.

## Custom Equality Rules

### Compare All Leaves Of A Type With One Rule

Use `UseComparerForType<T>(...)` when all leaves of a type should be compared with the same custom rule.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.UseComparerForType<int>(new OddEvenMatchIntComparer()));
```

This is a per-assertion rule. It does not change global comparer behavior.

### Compare One Specific Path

Use `UseComparerForPath(...)` when only one location in the graph needs special handling.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.UseComparerForPath("actual.Name", StringComparer.OrdinalIgnoreCase));
```

Both absolute and relative paths are supported:

```csharp
options.UseComparerForPath("actual.Name", StringComparer.OrdinalIgnoreCase);
options.UseComparerForPath("Name", StringComparer.OrdinalIgnoreCase);
```

### Compare One Specific Member

`UseComparerForMember(...)` is an alias for path-based comparison that reads better when the intent is member-focused:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.UseComparerForMember("Name", StringComparer.OrdinalIgnoreCase));
```

### Compare Items Inside One Collection By A Custom Rule

Use `UseCollectionItemComparerForPath(...)` when a collection should be matched by item identity rather than full structural equality.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.UseCollectionItemComparerForPath("actual.Items", new LineItemSkuComparer()));
```

This is useful when collection items contain volatile details but one key member defines logical identity.

## Collection Behavior

### Collection Order

`CollectionOrder` controls whether collection order matters:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.CollectionOrder = EquivalencyCollectionOrder.Strict);

actual.Should().BeEquivalentTo(expected, options =>
    options.CollectionOrder = EquivalencyCollectionOrder.Any);
```

- `Strict` requires element order to match
- `Any` allows reordered collections to compare as equivalent

### Missing And Extra Members

When `RequireStrictRuntimeTypes = false`, different shapes can still fail because one side has members the other does not. Use these options to relax that behavior:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.FailOnMissingMembers = false;
    options.FailOnExtraMembers = false;
});
```

- `FailOnMissingMembers = false` ignores members present on the expected type but missing on the actual type
- `FailOnExtraMembers = false` ignores members present on the actual type but missing on the expected type

## Null Handling

These options let you ignore null-valued members on one side:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.IgnoreExpectedNullMembers());

actual.Should().BeEquivalentTo(expected, options =>
    options.IgnoreActualNullMembers());
```

- `IgnoreExpectedNullMembers()` skips members where the expected value is `null`
- `IgnoreActualNullMembers()` skips members where the actual value is `null`

These options also affect missing-member scenarios when the missing side would have contributed a null value.

## String Comparison

String leaves are compared using `EquivalencyOptions.StringComparison`:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.StringComparison = StringComparison.OrdinalIgnoreCase);
```

Use this when you want one string rule across the whole graph.

If you only want special handling for one string member, prefer `UseComparerForPath(...)` or `UseComparerForMember(...)`.

## Tolerances

### Available Tolerances

You can configure tolerances for:

- `FloatTolerance`
- `DoubleTolerance`
- `HalfTolerance`
- `DecimalTolerance`
- `DateOnlyTolerance`
- `DateTimeTolerance`
- `DateTimeOffsetTolerance`
- `TimeOnlyTolerance`
- `TimeSpanTolerance`

Example:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.DoubleTolerance = 0.01d;
    options.DateTimeTolerance = TimeSpan.FromSeconds(1);
});
```

### Numeric And Temporal Tolerance Rules

A few rules are worth knowing:

- if the absolute difference equals the configured tolerance, the values are treated as equivalent
- negative tolerances are normalized to their absolute value
- `TimeSpan.MinValue` is rejected for temporal tolerance options
- `NaN` only matches `NaN`
- `+Infinity` only matches `+Infinity`
- `-Infinity` only matches `-Infinity`

## Limiting Report Size

Use `MaxDifferences` to cap how many differences are rendered before the report is truncated:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.MaxDifferences = 5);
```

This is useful when large graphs would otherwise produce noisy failure output.

## Recipes

### Ignore Audit Fields

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.IgnoreMember("CreatedAt");
    options.IgnoreMember("UpdatedAt");
});
```

### Compare DTOs With Different Member Names

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.MatchMemberName("GivenName", "FirstName");
    options.MatchMemberName("FamilyName", "LastName");
});
```

### Compare Collections Without Caring About Order

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.CollectionOrder = EquivalencyCollectionOrder.Any);
```

### Compare One Member Case-Insensitively

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.StringComparison = StringComparison.Ordinal;
    options.UseComparerForMember("Name", StringComparer.OrdinalIgnoreCase);
});
```

### Compare Only The Stable Parts Of A Response

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.OnlyCompareMembers("Status", "Payload.Id", "Payload.Version"));
```

### Set Project-Wide Defaults

```csharp
EquivalencyDefaults.Configure(options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
    options.StringComparison = StringComparison.OrdinalIgnoreCase;
    options.FailOnExtraMembers = false;
});
```

Then tighten behavior for one test:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.CollectionOrder = EquivalencyCollectionOrder.Strict;
    options.StringComparison = StringComparison.Ordinal;
});
```

## Related Docs

- [README.md](../README.md)
- [assertion-reference.md](assertion-reference.md)
