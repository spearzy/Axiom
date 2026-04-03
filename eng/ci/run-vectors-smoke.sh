#!/usr/bin/env bash

set -euo pipefail

if [[ $# -lt 1 || $# -gt 2 ]]; then
  echo "Usage: $0 <package-source-directory> [axiom-vectors-version]"
  exit 1
fi

package_source="$1"
package_version="${2:-}"

if [[ ! -d "$package_source" ]]; then
  echo "Package source directory does not exist: $package_source"
  exit 1
fi

if [[ -z "$package_version" ]]; then
  package_file="$(ls "$package_source"/Axiom.Vectors.*.nupkg 2>/dev/null | head -n 1 || true)"
  if [[ -z "$package_file" ]]; then
    echo "Could not find Axiom.Vectors package in: $package_source"
    exit 1
  fi

  package_name="$(basename "$package_file")"
  package_version="${package_name#Axiom.Vectors.}"
  package_version="${package_version%.nupkg}"
fi

smoke_root="$(mktemp -d)"
trap 'rm -rf "$smoke_root"' EXIT

consumer_project="$smoke_root/Axiom.Vectors.Smoke"
local_packages_cache="$smoke_root/.nuget/packages"
nuget_config="$smoke_root/NuGet.config"

cat > "$nuget_config" <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$package_source" />
  </packageSources>
</configuration>
EOF

dotnet new console -n Axiom.Vectors.Smoke -f net10.0 -o "$consumer_project" --no-restore >/dev/null

cd "$consumer_project"
dotnet add package Axiom.Vectors --version "$package_version" --source "$package_source" --no-restore >/dev/null

cat > Program.cs <<'EOF'
using System;
using Axiom.Vectors;

var embedding = new float[] { 1f, 0f, 0f };
var expected = new float[] { 1f, 0f, 0f };
var unrelated = new float[] { 0f, 1f, 0f };
ReadOnlyMemory<float> embeddingMemory = embedding;
ReadOnlyMemory<float> expectedMemory = expected;
ReadOnlyMemory<float> unrelatedMemory = unrelated;
ReadOnlyMemory<float> zero = new float[] { 0f, 0f, 0f };

embedding.Should().HaveDimension(3);
embedding.Should().NotContainNaNOrInfinity();
embedding.Should().BeApproximatelyEqualTo(expected, tolerance: 0.001f);
embedding.Should().HaveDotProductWith(expected, expectedDotProduct: 1f, tolerance: 0.001f);
embedding.Should().HaveEuclideanDistanceTo(unrelated, expectedDistance: 1.4142135f, tolerance: 0.001f);
embedding.Should().HaveCosineSimilarityTo(expected).AtLeast(0.999f);
embedding.Should().BeNormalized(tolerance: 0.001f);
embedding.Should().NotBeZeroVector();

embeddingMemory.Should().HaveDotProductWith(expectedMemory, expectedDotProduct: 1f, tolerance: 0.001f);
embeddingMemory.Should().HaveEuclideanDistanceTo(unrelatedMemory, expectedDistance: 1.4142135f, tolerance: 0.001f);
zero.Should().BeZeroVector();

Console.WriteLine("Axiom.Vectors smoke passed.");
EOF

dotnet run --project Axiom.Vectors.Smoke.csproj --configfile "$nuget_config" --packages "$local_packages_cache"
