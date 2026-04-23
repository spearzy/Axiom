#!/usr/bin/env bash

set -euo pipefail

if [[ $# -lt 1 || $# -gt 3 ]]; then
  echo "Usage: $0 <package-source-directory> [axiom-assertions-version|smoke-kind] [smoke-kind]"
  echo "Smoke kinds: xunit (default), nunit, mstest, plain"
  exit 1
fi

package_source="$1"
package_version=""
smoke_kind="xunit"
script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "$script_dir/../.." && pwd)"

if [[ $# -eq 2 ]]; then
  case "$2" in
    xunit|nunit|mstest|plain)
      smoke_kind="$2"
      ;;
    *)
      package_version="$2"
      ;;
  esac
elif [[ $# -eq 3 ]]; then
  package_version="$2"
  smoke_kind="$3"
fi

if [[ "$smoke_kind" != "xunit" && "$smoke_kind" != "nunit" && "$smoke_kind" != "mstest" && "$smoke_kind" != "plain" ]]; then
  echo "Unsupported smoke kind: $smoke_kind"
  echo "Supported smoke kinds: xunit, nunit, mstest, plain"
  exit 1
fi

if [[ ! -d "$package_source" ]]; then
  echo "Package source directory does not exist: $package_source"
  exit 1
fi

if [[ -z "$package_version" ]]; then
  package_file="$(ls "$package_source"/Axiom.Assertions.*.nupkg 2>/dev/null | head -n 1 || true)"
  if [[ -z "$package_file" ]]; then
    echo "Could not find Axiom.Assertions package in: $package_source"
    exit 1
  fi

  package_name="$(basename "$package_file")"
  package_version="${package_name#Axiom.Assertions.}"
  package_version="${package_version%.nupkg}"
fi

smoke_root="$(mktemp -d)"
trap 'rm -rf "$smoke_root"' EXIT

local_packages_cache="$smoke_root/.nuget/packages"
nuget_config="$smoke_root/NuGet.config"

cat > "$nuget_config" <<EOF2
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$package_source" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="local">
      <package pattern="Axiom.*" />
    </packageSource>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
  </packageSourceMapping>
</configuration>
EOF2

create_plain_smoke_program() {
  rm -f Program.cs
  cat > Program.cs <<'EOF2'
using System;
using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using AAssert = Axiom.Core.Assert;

"abc".Should().StartWith("a").And.EndWith("c");
42.Should().BeGreaterThan(1).And.BeInRange(40, 50);
new[] { 1, 2, 3 }.Should().Contain(2).And.NotContain(9);

var equivalencyActual = new UserSnapshot("ollie", 3);
var equivalencyExpected = new UserSnapshot("ollie", 3);
equivalencyActual.Should().BeEquivalentTo(equivalencyExpected);

try
{
    42.Should().Be(7);
    throw new Exception("Expected InvalidOperationException from fallback failure strategy.");
}
catch (InvalidOperationException ex)
{
    if (!ex.Message.Contains("Expected 42 to be 7, but found 42.", StringComparison.Ordinal))
    {
        throw new Exception($"Unexpected fallback message: {ex.Message}");
    }
}

try
{
    using (var batch = AAssert.Batch("smoke"))
    {
        "abc".Should().StartWith("z");
        1.Should().BeGreaterThan(5);
    }

    throw new Exception("Expected InvalidOperationException from fallback batch failure strategy.");
}
catch (InvalidOperationException ex)
{
    if (!ex.Message.Contains("Batch 'smoke' failed with 2 assertion failure(s):", StringComparison.Ordinal))
    {
        throw new Exception($"Unexpected fallback batch message: {ex.Message}");
    }
}

Console.WriteLine("Axiom plain consumer smoke passed.");

file sealed record UserSnapshot(string Name, int Level);
EOF2
}

copy_starter_project() {
  local starter_name="$1"
  local destination="$2"

  rm -rf "$destination"
  cp -R "$repo_root/starters/$starter_name" "$destination"
}

case "$smoke_kind" in
  xunit)
    consumer_project="$smoke_root/Axiom.Assertions.Starter.Xunit"
    copy_starter_project "Axiom.Assertions.Starter.Xunit" "$consumer_project"
    ;;
  nunit)
    consumer_project="$smoke_root/Axiom.Assertions.Starter.NUnit"
    copy_starter_project "Axiom.Assertions.Starter.NUnit" "$consumer_project"
    ;;
  mstest)
    consumer_project="$smoke_root/Axiom.Assertions.Starter.MSTest"
    copy_starter_project "Axiom.Assertions.Starter.MSTest" "$consumer_project"
    ;;
  plain)
    consumer_project="$smoke_root/Axiom.ConsumerSmoke"
    dotnet new console --framework net10.0 --output "$consumer_project" --no-restore
    ;;
esac

cd "$consumer_project"

if [[ "$smoke_kind" == "plain" ]]; then
  dotnet add package Axiom.Assertions --version "$package_version" --source "$package_source" --no-restore
  create_plain_smoke_program
  dotnet restore --configfile "$nuget_config" --packages "$local_packages_cache"
  dotnet run --configuration Release --no-restore
else
  dotnet restore --configfile "$nuget_config" --packages "$local_packages_cache" /p:AxiomAssertionsVersion="$package_version"
  dotnet test --configuration Release --no-restore /p:AxiomAssertionsVersion="$package_version"
fi
