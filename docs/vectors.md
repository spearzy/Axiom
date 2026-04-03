# Vectors

`Axiom.Vectors` adds vector and embedding-focused assertions without pushing that API surface into `Axiom.Assertions`.

Install it when you want to assert dimensions, numeric validity, approximate equality, dot products, distances, cosine similarity, zero-vector shape, or normalization directly on vector outputs. It builds on top of the main Axiom assertion infrastructure, so `Batch`, failure strategies, and deterministic messages behave the same way.

```bash
dotnet add package Axiom.Vectors
```

## Usage

These examples use `expected` for the matching embedding and `unrelated` for a clearly different one.

```csharp
using Axiom.Vectors;

embedding.Should().HaveDimension(1536);
embedding.Should().NotContainNaNOrInfinity();
embedding.Should().BeApproximatelyEqualTo(expected, tolerance: 0.001f);
embedding.Should().HaveDotProductWith(expected, expectedDotProduct: 1f, tolerance: 0.001f);
embedding.Should().HaveEuclideanDistanceTo(unrelated, expectedDistance: 1.4142f, tolerance: 0.001f);
embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.995f).And.BeNormalized();
embedding.Should().HaveCosineSimilarityWith(unrelated).AtMost(0.2f);
embedding.Should().HaveCosineSimilarityWith(expected).Between(0.98f, 0.999f);
embedding.Should().BeNormalized(tolerance: 0.001f);
new float[] { 0f, 0f }.Should().BeZeroVector();
embedding.Should().NotBeZeroVector();
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
embedding.Should().BeApproximatelyEqualTo(expected, tolerance: 0.001f);
```

Approximate equality fails on the first value outside tolerance and reports:

- the mismatching index
- expected value
- actual value
- absolute delta

## Dot Product

```csharp
embedding.Should().HaveDotProductWith(expected, expectedDotProduct: 1f, tolerance: 0.001f);
```

Dot-product failures report the computed dot product directly.
They include the expected value, actual dot product, and delta.

## Euclidean Distance

```csharp
embedding.Should().HaveEuclideanDistanceTo(unrelated, expectedDistance: 1.4142f, tolerance: 0.001f);
```

Distance failures report the computed Euclidean distance directly.
They include the expected value, actual distance, and delta.

## Cosine Similarity

```csharp
embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.995f);
embedding.Should().HaveCosineSimilarityWith(unrelated).AtMost(0.2f);
embedding.Should().HaveCosineSimilarityWith(expected).Between(0.98f, 0.999f).And.BeNormalized();
```

`HaveCosineSimilarityWith(...)` computes the similarity once and exposes it through `ActualSimilarity` if you want to inspect it directly:

```csharp
var similarity = embedding.Should().HaveCosineSimilarityWith(expected).ActualSimilarity;
```

Zero vectors are handled explicitly and produce deterministic failures instead of ambiguous `NaN` output.

The returned continuation exposes:

- `ActualSimilarity`
- `AtLeast(threshold)`
- `AtMost(threshold)`
- `Between(minimumThreshold, maximumThreshold)`

`HaveCosineSimilarityTo(...)` is still available as a compatibility alias, but `HaveCosineSimilarityWith(...)` is the preferred form for new code.

## Normalization

```csharp
embedding.Should().BeNormalized(tolerance: 0.001f);
```

Normalization checks the L2 norm and reports the computed norm on failure.

## Zero Vectors

```csharp
new float[] { 0f, 0f }.Should().BeZeroVector();
embedding.Should().NotBeZeroVector();
```

`BeZeroVector()` reports the first non-zero component it finds.

`NotBeZeroVector()` fails only when every component is zero. Empty vectors count as zero vectors.
