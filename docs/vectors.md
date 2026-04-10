---
title: Vector Assertions for AI and Retrieval Tests in .NET
description: Use Axiom.Vectors for vector, embedding, and retrieval assertions in .NET tests, including similarity, normalization, and ranked retrieval quality.
---

# Vectors

`Axiom.Vectors` adds vector and embedding-focused assertions without pushing that API surface into `Axiom.Assertions`.

Install it when you want to assert dimensions, numeric validity, approximate equality, dot products, distances, cosine similarity, zero-vector shape, normalization, or ranked retrieval quality directly in tests. It builds on top of the main Axiom assertion infrastructure, so `Batch`, failure strategies, and deterministic messages behave the same way.

If you are specifically looking for retrieval and AI-oriented test scenarios, the focused page [Vector assertions for AI and retrieval tests in .NET](vector-assertions-for-ai-and-retrieval-tests-in-dotnet.md) is the quickest overview.

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

## Retrieval And Ranking

Ranking assertions use the normal `Should()` entry point from `Axiom.Assertions`.

Use `subject.Should()` for a single ordered result list, and use `subject.Should()` on `IEnumerable<RankingQuery<T>>` when you want aggregate metrics across multiple queries.

Ranks are 1-based.

```csharp
using Axiom.Assertions;
using Axiom.Vectors;

results.Should().ContainInTopK("doc-7", 2);
results.Should().HaveRank("doc-7", 2);
results.Should().HaveRecallAt(2, relevantItems, expectedRecall: 0.5);
results.Should().HavePrecisionAt(2, relevantItems, expectedPrecision: 0.5);
results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 0.5);

queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 0.75);
queries.Should().HaveHitRateAt(k: 1, expectedHitRate: 0.5);
```

| Assertion | What it checks |
| --- | --- |
| `ContainInTopK(target, k)` | Whether a specific item appears within the top `k` results |
| `HaveRank(target, expectedRank)` | The exact 1-based rank of a specific item |
| `HaveRecallAt(k, relevantItems, expectedRecall)` | How much of the relevant set appears in the top `k` results |
| `HavePrecisionAt(k, relevantItems, expectedPrecision)` | How much of the top `k` results is relevant |
| `HaveReciprocalRank(target, expectedReciprocalRank)` | `1 / rank` for the first occurrence of a target item |
| `HaveMeanReciprocalRank(expectedMeanReciprocalRank)` | The average reciprocal rank across multiple queries |
| `HaveHitRateAt(k, expectedHitRate)` | The fraction of queries with at least one relevant hit in the top `k` |

### Top-K And Rank Checks

`ContainInTopK(...)` passes when the target item appears within the allowed top `k` results.

`HaveRank(...)` checks the exact 1-based rank of the first matching item.

If the item is present but ranked too low, the failure reports the found rank. If it is missing entirely, the failure says so explicitly.

### Recall@K And Precision@K

`HaveRecallAt(...)` and `HavePrecisionAt(...)` evaluate the top `k` results against a non-empty relevant-item set.

- Duplicate ranked results do not increase the matched relevant count.
- `Precision@k` always uses `k` as the denominator, even when fewer than `k` results are returned.
- Empty relevant-item sets are rejected as invalid input.
- The ranking metric assertions default to exact comparison with `tolerance: 0`.
- Pass a tolerance explicitly when you want an approximate numeric comparison.
- When no relevant item is found in the inspected top `k`, the computed recall or precision is `0`.

### Reciprocal Rank, Mean Reciprocal Rank, And Hit Rate

`HaveReciprocalRank(...)` evaluates a single ranked result list for one target item.

Missing items produce reciprocal rank `0`, including empty result lists, and the failure message states that the item was missing.

`HaveMeanReciprocalRank(...)` and `HaveHitRateAt(...)` evaluate a query set built from `RankingQuery<T>`:

```csharp
using Axiom.Assertions;
using Axiom.Vectors;

var queries = new[]
{
    new RankingQuery<string>(["doc-2", "doc-7", "doc-5"], ["doc-2"]),
    new RankingQuery<string>(["doc-8", "doc-5", "doc-3"], ["doc-5"]),
};
```

For these aggregate metrics:

- the first relevant hit determines reciprocal rank for each query
- queries with no relevant hit contribute `0`
- each query must have a non-empty relevant-item set
