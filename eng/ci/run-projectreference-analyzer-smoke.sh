#!/usr/bin/env bash

set -euo pipefail

repo_root="$(cd "$(dirname "$0")/../.." && pwd)"
assertions_project="$repo_root/src/Axiom.Assertions/Axiom.Assertions.csproj"
analyzers_project="$repo_root/src/Axiom.Analyzers/Axiom.Analyzers.csproj"
codefixes_project="$repo_root/src/Axiom.Analyzers.CodeFixes/Axiom.Analyzers.CodeFixes.csproj"

if [[ ! -f "$assertions_project" ]]; then
  echo "Could not find Axiom.Assertions project: $assertions_project"
  exit 1
fi

if [[ ! -f "$analyzers_project" ]]; then
  echo "Could not find Axiom.Analyzers project: $analyzers_project"
  exit 1
fi

if [[ ! -f "$codefixes_project" ]]; then
  echo "Could not find Axiom.Analyzers.CodeFixes project: $codefixes_project"
  exit 1
fi

smoke_root="$repo_root/.tmp/projectreference-analyzers-smoke"
trap 'rm -rf "$smoke_root"' EXIT

consumer_project="$smoke_root/Axiom.ProjectReference.Analyzers.Smoke"
error_log="$smoke_root/diagnostics.sarif"

rm -rf "$smoke_root"
mkdir -p "$smoke_root"

pushd "$smoke_root" >/dev/null
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet new classlib -n Axiom.ProjectReference.Analyzers.Smoke -f net10.0 --no-restore >/dev/null
popd >/dev/null

cat > "$consumer_project/Axiom.ProjectReference.Analyzers.Smoke.csproj" <<EOF2
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../../src/Axiom.Assertions/Axiom.Assertions.csproj" />
    <ProjectReference Include="../../../src/Axiom.Analyzers/Axiom.Analyzers.csproj"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="false"
                      SetTargetFramework="TargetFramework=netstandard2.0" />
    <ProjectReference Include="../../../src/Axiom.Analyzers.CodeFixes/Axiom.Analyzers.CodeFixes.csproj"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="false"
                      SetTargetFramework="TargetFramework=netstandard2.0" />
  </ItemGroup>
</Project>
EOF2

cat > "$consumer_project/XunitStub.cs" <<'EOF2'
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
EOF2

cat > "$consumer_project/Smoke.cs" <<'EOF2'
using Xunit;

public sealed class Smoke
{
    public void Check(int expected, int actual, string text)
    {
        Assert.Equal(expected, actual);
        Assert.Contains("sub", text);
    }
}
EOF2

build_output="$(dotnet build "$consumer_project/Axiom.ProjectReference.Analyzers.Smoke.csproj" -m:1 /nr:false /p:BuildInParallel=false /p:ErrorLog="$error_log" 2>&1)"

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
