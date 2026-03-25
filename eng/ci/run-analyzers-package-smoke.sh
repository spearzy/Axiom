#!/usr/bin/env bash

set -euo pipefail

if [[ $# -ne 1 ]]; then
  echo "Usage: $0 <package-source-directory>"
  exit 1
fi

package_source="$1"

if [[ ! -d "$package_source" ]]; then
  echo "Package source directory does not exist: $package_source"
  exit 1
fi

package_file="$(ls "$package_source"/Axiom.Analyzers.*.nupkg 2>/dev/null | head -n 1 || true)"
if [[ -z "$package_file" ]]; then
  echo "Could not find Axiom.Analyzers package in: $package_source"
  exit 1
fi

package_name="$(basename "$package_file")"
package_version="${package_name#Axiom.Analyzers.}"
package_version="${package_version%.nupkg}"

smoke_root="$(mktemp -d)"
trap 'rm -rf "$smoke_root"' EXIT

consumer_project="$smoke_root/Axiom.Analyzers.Smoke"
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

dotnet new classlib -n Axiom.Analyzers.Smoke -f net10.0 -o "$consumer_project" --no-restore >/dev/null

dotnet add "$consumer_project/Axiom.Analyzers.Smoke.csproj" package Axiom.Analyzers --version "$package_version" --source "$package_source" --no-restore >/dev/null

cat > "$consumer_project/AxiomAssertionStubs.cs" <<'EOF'
using System;
using System.Threading.Tasks;

namespace Axiom.Assertions
{
    public static class ShouldExtensions
    {
        public static Axiom.Assertions.AssertionTypes.AsyncActionAssertions Should(this Action subject)
        {
            return new Axiom.Assertions.AssertionTypes.AsyncActionAssertions();
        }
    }
}

namespace Axiom.Assertions.AssertionTypes
{
    public sealed class AsyncActionAssertions
    {
        public ValueTask ThrowAsync<TException>()
            where TException : Exception
        {
            return default;
        }
    }
}
EOF

cat > "$consumer_project/Smoke.cs" <<'EOF'
using System;
using System.Threading.Tasks;
using Axiom.Assertions;

public sealed class Smoke
{
    public async Task CheckAsync()
    {
        Action act = static () => { };
        act.Should().ThrowAsync<InvalidOperationException>();
    }
}
EOF

set +e
build_output="$(dotnet build "$consumer_project/Axiom.Analyzers.Smoke.csproj" --configfile "$nuget_config" --packages "$local_packages_cache" -warnaserror:AXM0001 2>&1)"
build_exit_code=$?
set -e

echo "$build_output"

if [[ $build_exit_code -eq 0 ]]; then
  echo "Expected AXM0001 diagnostic, but the build succeeded."
  exit 1
fi

if [[ "$build_output" != *"AXM0001"* ]]; then
  echo "Expected AXM0001 diagnostic in build output."
  exit 1
fi
