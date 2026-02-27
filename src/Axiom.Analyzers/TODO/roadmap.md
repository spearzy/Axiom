# Analyzer Roadmap

## Candidate Rules

- Warn when assertion chains can be simplified for readability.
- Suggest Batch usage when multiple related assertions are made in the same block.
- Detect obvious contradictory assertion combinations in a single chain.

## Code Fix Direction

- Prefer focused fixes with minimal rewrites.
- Keep diagnostics deterministic and easy to suppress.

## Notes

- Real analyzer implementation is intentionally deferred.
