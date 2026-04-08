#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
mode="${1:-validate}"

cd "$repo_root"

if [[ "$mode" != "refresh" && "$mode" != "validate" ]]; then
  echo "Usage: bash eng/ci/run-benchmark-snapshot.sh [refresh|validate]" >&2
  exit 1
fi

if [[ "$mode" == "refresh" ]]; then
  dotnet build benchmarks/Axiom.Benchmarks/Axiom.Benchmarks.csproj -c Release

  benchmark_exe="benchmarks/Axiom.Benchmarks/bin/Release/net10.0/Axiom.Benchmarks"
  "$benchmark_exe" --filter "*ScalarAssertionBenchmarks*"
  "$benchmark_exe" --filter "*EquivalencyBenchmarks.EquivalentGraphPass*"
fi

python3 eng/ci/benchmark_snapshot.py "$mode"
