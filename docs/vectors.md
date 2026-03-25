# Vectors

`Axiom.Vectors` adds vector and embedding-focused assertions without pushing that API surface into `Axiom.Assertions`.

Install it when you want to assert dimensions, numeric validity, approximate equality, cosine similarity, or normalization directly on vector outputs. It builds on top of the main Axiom assertion infrastructure, so `Batch`, failure strategies, and deterministic messages behave the same way.

```bash
dotnet add package Axiom.Vectors
```

## Usage

```csharp
using Axiom.Vectors;

embedding.Should().HaveDimension(1536);
embedding.Should().NotContainNaNOrInfinity();
embedding.Should().BeApproximatelyEqualTo(expected, tolerance: 1e-5f);
embedding.Should().HaveCosineSimilarityTo(expected).AtLeast(0.995f);
embedding.Should().BeNormalized(tolerance: 1e-5f);
```

Supported subject shapes in the first release:

- `float[]`
- `double[]`
- `ReadOnlyMemory<float>`
- `ReadOnlyMemory<double>`

The package exposes `Should()` entry points for those shapes and returns:

- `VectorAssertions<float>`
- `VectorAssertions<double>`

## Dimension

```csharp
embedding.Should().HaveDimension(1536);
```

## NaN And Infinity Checks

```csharp
embedding.Should().NotContainNaNOrInfinity();
```

Axiom stops at the first invalid value and reports the offending index.

## Approximate Equality

```csharp
embedding.Should().BeApproximatelyEqualTo(expected, tolerance: 1e-5f);
```

Approximate equality fails on the first value outside tolerance and reports:

- the mismatching index
- expected value
- actual value
- absolute delta

## Cosine Similarity

```csharp
embedding.Should().HaveCosineSimilarityTo(expected).AtLeast(0.995f);
```

`HaveCosineSimilarityTo(...)` computes the similarity once and exposes it through `ActualSimilarity` if you want to inspect it directly:

```csharp
var similarity = embedding.Should().HaveCosineSimilarityTo(expected).ActualSimilarity;
```

Zero vectors are handled explicitly and produce deterministic failures instead of ambiguous `NaN` output.

The returned continuation exposes:

- `ActualSimilarity`
- `AtLeast(threshold)`

## Normalization

```csharp
embedding.Should().BeNormalized(tolerance: 1e-5f);
```

Normalization checks the L2 norm and reports the computed norm on failure.
