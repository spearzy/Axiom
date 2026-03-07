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

consumer_project="$smoke_root/Axiom.ConsumerSmoke"
local_packages_cache="$smoke_root/.nuget/packages"
nuget_config="$smoke_root/NuGet.config"

dotnet new console --framework net10.0 --output "$consumer_project" --no-restore
cd "$consumer_project"

cat > "$nuget_config" <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$package_source" />
  </packageSources>
</configuration>
EOF

dotnet add package Axiom.Assertions --version "$package_version" --source "$package_source" --no-restore

cat > Program.cs <<'EOF'
using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using AAssert = Axiom.Core.Assert;

var failures = 0;

ExpectNoThrow(
    "StringChaining_Works",
    () => "abc".Should().StartWith("a").And.EndWith("c"));

ExpectNoThrow(
    "ValueAssertions_Works",
    () =>
    {
        42.Should().BeGreaterThan(1).And.BeInRange(40, 50);
        42.1d.Should().BeApproximately(42d, 0.2d);
    });

ExpectNoThrow(
    "CollectionAssertions_Works",
    () => new[] { 1, 2, 3 }.Should().Contain(2).And.NotContain(9));

ExpectNoThrow(
    "EquivalencyAssertions_Works",
    () =>
    {
        var actual = new UserSnapshot("ollie", 3);
        var expected = new UserSnapshot("ollie", 3);
        actual.Should().BeEquivalentTo(expected);
    });

ExpectThrows<InvalidOperationException>(
    "Batch_AggregatesFailures",
    () =>
    {
        using var batch = AAssert.Batch("smoke");
        "abc".Should().StartWith("z");
        1.Should().BeGreaterThan(5);
    },
    "Batch 'smoke' failed with 2 assertion failure(s):");

if (failures > 0)
{
    Console.Error.WriteLine($"Smoke test failed with {failures} failure(s).");
    return 1;
}

Console.WriteLine("Axiom consumer smoke checks passed.");
return 0;

void ExpectNoThrow(string name, Action assertion)
{
    try
    {
        assertion();
    }
    catch (Exception ex)
    {
        failures++;
        Console.Error.WriteLine($"[{name}] Expected no exception, but got {ex.GetType().Name}: {ex.Message}");
    }
}

void ExpectThrows<TException>(string name, Action assertion, string expectedMessageFragment)
    where TException : Exception
{
    try
    {
        assertion();
        failures++;
        Console.Error.WriteLine($"[{name}] Expected {typeof(TException).Name}, but no exception was thrown.");
    }
    catch (TException ex)
    {
        if (!ex.Message.Contains(expectedMessageFragment, StringComparison.Ordinal))
        {
            failures++;
            Console.Error.WriteLine(
                $"[{name}] Exception message mismatch. Expected fragment: '{expectedMessageFragment}'. Actual: '{ex.Message}'.");
        }
    }
    catch (Exception ex)
    {
        failures++;
        Console.Error.WriteLine(
            $"[{name}] Expected {typeof(TException).Name}, but got {ex.GetType().Name}: {ex.Message}");
    }
}

file sealed record UserSnapshot(string Name, int Level);
EOF

dotnet restore --configfile "$nuget_config" --packages "$local_packages_cache"
dotnet run --configuration Release --no-restore
