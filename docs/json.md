---
title: JSON Assertions for .NET with Axiom.Json
description: Use Axiom.Json for deterministic JSON equivalency and path assertions over raw JSON strings, JsonDocument, and JsonElement in .NET tests.
---

# JSON Assertions

`Axiom.Json` is the optional Axiom package for deterministic JSON assertions.

It focuses on a small first wave:

- structural JSON equivalency
- simple JSON path existence checks
- simple scalar value-at-path checks

It does not add HTTP helpers, ASP.NET helpers, direct `Newtonsoft.Json` support, or a full JSONPath engine.

If your subject is `HttpResponseMessage` rather than raw JSON content, use [HTTP and API assertions](http.md). `Axiom.Http` reuses `Axiom.Json` internally for response-body JSON checks.

## Install

```bash
dotnet add package Axiom.Json
```

Use it alongside `Axiom.Assertions`:

```csharp
using Axiom.Assertions;
using Axiom.Json;
```

## Supported Inputs

The first wave supports:

- raw JSON `string`
- `JsonDocument`
- `JsonElement`

Examples:

```csharp
using System.Text.Json;
using Axiom.Assertions;
using Axiom.Json;

var actualJson = """
    { "id": 1, "name": "Bob", "roles": ["admin", "author"] }
    """;
var expectedJson = """
    { "roles": ["admin", "author"], "name": "Bob", "id": 1.0 }
    """;

actualJson.Should().BeJsonEquivalentTo(expectedJson);

using var document = JsonDocument.Parse(actualJson);
document.Should().HaveJsonPath("$.roles[1]");
document.RootElement.Should().HaveJsonStringAtPath("$.name", "Bob");
```

## Structural Equivalency

Use `BeJsonEquivalentTo(...)` and `NotBeJsonEquivalentTo(...)` when you want JSON-aware structural comparison instead of plain string equality.

```csharp
using Axiom.Assertions;
using Axiom.Json;

var actualJson = """
    {
      "customer": {
        "id": 7,
        "active": true,
        "roles": ["admin", "author"]
      }
    }
    """;

var expectedJson = """
    {
      "customer": {
        "roles": ["admin", "author"],
        "active": true,
        "id": 7.0
      }
    }
    """;

actualJson.Should().BeJsonEquivalentTo(expectedJson);
actualJson.Should().NotBeJsonEquivalentTo("""
    { "customer": { "id": 9, "active": true, "roles": ["admin", "author"] } }
    """);
```

Current semantics:

- object property order does not matter
- array order does matter
- missing properties and extra properties fail explicitly
- value-kind mismatches fail explicitly
- numeric comparison uses normalized JSON numeric values, so `1`, `1.0`, and `1e0` are treated as equivalent

## JSON Path Assertions

The first wave uses a deliberately small path syntax:

- optional root marker: `$`
- object-member traversal with `.`
- array-index traversal with `[index]`

Examples:

- `$.customer.id`
- `$.customer.roles[0]`
- `items[1].sku`

This is not a full JSONPath implementation. Property names that need escaping are out of scope for this first wave.

```csharp
using Axiom.Assertions;
using Axiom.Json;

var json = """
    {
      "customer": {
        "id": 7,
        "name": "Bob",
        "active": true,
        "roles": ["admin", "author"],
        "deletedAt": null
      }
    }
    """;

json.Should().HaveJsonPath("$.customer.roles[1]");
json.Should().NotHaveJsonPath("$.customer.email");
json.Should().HaveJsonStringAtPath("$.customer.name", "Bob");
json.Should().HaveJsonNumberAtPath("$.customer.id", 7m);
json.Should().HaveJsonBooleanAtPath("$.customer.active", true);
json.Should().HaveJsonNullAtPath("$.customer.deletedAt");
```

## Invalid JSON

For raw JSON string subjects, invalid JSON fails clearly with an `invalid JSON` parse location in the failure message.

Invalid expected JSON passed as a raw string is treated as an invalid assertion argument and throws `ArgumentException`.

## First-Wave Limits

`Axiom.Json` in `2.1.0` is intentionally narrow:

- `System.Text.Json` inputs plus raw JSON strings only
- no direct `Newtonsoft.Json` support
- no HTTP or API-response helpers in this package; use `Axiom.Http` for `HttpResponseMessage`
- no full JSONPath language
- array order is significant

For the full method list, see the [Assertion Reference](assertion-reference.md).
