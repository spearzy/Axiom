---
title: HTTP and API Assertions for .NET with Axiom.Http
description: Use Axiom.Http for deterministic HttpResponseMessage assertions in .NET tests, including exact status codes, headers, content types, JSON bodies, and ProblemDetails checks.
---

# HTTP and API Assertions

`Axiom.Http` is the optional Axiom package for practical `HttpResponseMessage` assertions.

It keeps the first wave focused on common API-test needs:

- exact status-code assertions
- header presence, absence, and exact-value assertions
- focused content-type assertions
- JSON body assertions that reuse `Axiom.Json`
- minimal ProblemDetails assertions

## Install

```bash
dotnet add package Axiom.Http
```

Use it alongside `Axiom.Assertions`:

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Assertions;
using Axiom.Http;
```

## Supported Subject

The first wave is intentionally centered on `HttpResponseMessage`.

```csharp
using System.Net;
using System.Net.Http;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.OK);

var assertions = response.Should();
```

## Exact Status Codes

Exact status-code assertions are the primary shape for this package.

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.Created)
{
    Content = new StringContent("{}", Encoding.UTF8, "application/json")
};

response.Should().HaveStatusCode(HttpStatusCode.Created);
response.Should().HaveStatusCode(201);
response.Should().NotHaveStatusCode(HttpStatusCode.BadRequest);
response.Should().NotHaveStatusCode(404);
```

## Headers and Content Type

Header lookup checks both response headers and content headers.

`HaveHeaderValue(...)` expects exactly one header value.
`HaveHeaderValues(...)` checks exact count and exact order.

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.OK)
{
    Content = new StringContent("{\"id\":1}", Encoding.UTF8, "application/json")
};

response.Headers.Add("ETag", "\"v1\"");
response.Headers.Add("X-Trace", ["a", "b"]);

response.Should().HaveHeader("ETag");
response.Should().NotHaveHeader("Retry-After");
response.Should().HaveHeaderValue("ETag", "\"v1\"");
response.Should().HaveHeaderValues("X-Trace", ["a", "b"]);
response.Should().HaveContentType("application/json");
response.Should().HaveContentTypeWithCharset("application/json", "utf-8");
```

## JSON Response Bodies

JSON body assertions use `Axiom.Json`.

This means JSON body equivalency and JSON path behavior stay aligned with the JSON package:

- object property order does not matter
- array order does matter
- numeric comparison uses normalized JSON numeric values

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.OK)
{
    Content = new StringContent(
        """
        { "id": 1, "name": "Ada", "roles": ["admin", "author"] }
        """,
        Encoding.UTF8,
        "application/json")
};

var expectedJson = """
    { "roles": ["admin", "author"], "name": "Ada", "id": 1.0 }
    """;

response.Should().HaveJsonBodyEquivalentTo(expectedJson);
response.Should().HaveJsonPath("$.roles[1]");
response.Should().HaveJsonStringAtPath("$.name", "Ada");
response.Should().HaveJsonNumberAtPath("$.id", 1m);
```

If you want JSON assertions over raw JSON strings, `JsonDocument`, or `JsonElement` directly, use [JSON](json.md).

## ProblemDetails

The first wave includes small, direct ProblemDetails assertions for common error-response checks.

These assertions expect a response body with `application/problem+json`.

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
{
    Content = new StringContent(
        """
        {
          "type": "https://example.test/problems/validation",
          "title": "Validation failed",
          "status": 400,
          "detail": "Name is required."
        }
        """,
        Encoding.UTF8,
        "application/problem+json")
};

response.Should().HaveProblemDetails();
response.Should().HaveProblemDetailsTitle("Validation failed");
response.Should().HaveProblemDetailsStatus(400);
response.Should().HaveProblemDetailsType("https://example.test/problems/validation");
response.Should().HaveProblemDetailsDetail("Name is required.");
```

## First-Wave Limits

`Axiom.Http` is intentionally narrow in its first release:

- `HttpResponseMessage` only
- no ASP.NET-specific result helpers
- no test-server helpers
- no category-level status-code assertions
- no snapshot features
- no full API-client helper layer

For the full method list, see the [Assertion Reference](assertion-reference.md).
