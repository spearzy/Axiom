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
error_log="$smoke_root/diagnostics.sarif"

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
using Xunit;

namespace Xunit
{
    public static class Assert
    {
        public static void Equal<T>(T expected, T actual)
        {
        }

        public static void Contains(string expectedSubstring, string actualString)
        {
        }
    }
}

public sealed class Smoke
{
    public void Check(int expected, int actual, string text)
    {
        Assert.Equal(expected, actual);
        Assert.Contains("sub", text);
    }
}
EOF

build_output="$(dotnet build Axiom.Assertions.Analyzers.Smoke.csproj --configfile "$nuget_config" --packages "$local_packages_cache" /p:ErrorLog="$error_log" 2>&1)"

echo "$build_output"

if [[ ! -f "$error_log" ]]; then
  echo "Expected SARIF diagnostics log at $error_log."
  exit 1
fi

error_log_content="$(cat "$error_log")"

if [[ "$error_log_content" != *"AXM1001"* ]]; then
  echo "Expected AXM1001 diagnostic in SARIF output."
  exit 1
fi

if [[ "$error_log_content" != *"AXM1017"* ]]; then
  echo "Expected AXM1017 diagnostic in SARIF output."
  exit 1
fi
