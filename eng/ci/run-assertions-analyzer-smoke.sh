#!/usr/bin/env bash

set -euo pipefail

if [[ $# -lt 1 || $# -gt 2 ]]; then
  echo "Usage: $0 <package-source-directory> [axiom-assertions-version]"
  exit 1
fi

package_source="$1"
package_version="${2:-}"

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

consumer_project="$smoke_root/Axiom.Assertions.Analyzers.Smoke"
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

dotnet new classlib -n Axiom.Assertions.Analyzers.Smoke -f net10.0 -o "$consumer_project" --no-restore >/dev/null

cd "$consumer_project"
dotnet add package Axiom.Assertions --version "$package_version" --source "$package_source" --no-restore >/dev/null

cat > Smoke.cs <<'EOF'
using System;
using System.Threading.Tasks;
using Axiom.Assertions;

public sealed class Smoke
{
    public async Task CheckAsync()
    {
        Func<Task> work = async () => await Task.Yield();
        work.Should().NotThrowAsync();
    }
}
EOF

set +e
build_output="$(dotnet build Axiom.Assertions.Analyzers.Smoke.csproj --configfile "$nuget_config" --packages "$local_packages_cache" -warnaserror:AXM0001 2>&1)"
build_exit_code=$?
set -e

echo "$build_output"

if [[ $build_exit_code -eq 0 ]]; then
  echo "Expected AXM0001 diagnostic from the Axiom.Assertions package, but the build succeeded."
  exit 1
fi

if [[ "$build_output" != *"AXM0001"* ]]; then
  echo "Expected AXM0001 diagnostic in build output."
  exit 1
fi
