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

dotnet new xunit --framework net10.0 --output "$smoke_root/Axiom.ConsumerSmoke" --no-restore
cd "$smoke_root/Axiom.ConsumerSmoke"

dotnet add package Axiom.Assertions --version "$package_version" --source "$package_source"

rm -f UnitTest1.cs
cat > ConsumerSmokeTests.cs <<'EOF'
using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using Axiom.Core;
using XAssert = Xunit.Assert;

namespace Axiom.ConsumerSmoke;

public sealed class ConsumerSmokeTests
{
    [Fact]
    public void StringChaining_Works()
    {
        "abc".Should().StartWith("a").And.EndWith("c");
    }

    [Fact]
    public void ValueAssertions_Works()
    {
        42.Should().BeGreaterThan(1).And.BeInRange(40, 50);
        42.1d.Should().BeApproximately(42d, 0.2d);
    }

    [Fact]
    public void CollectionAssertions_Works()
    {
        new[] { 1, 2, 3 }.Should().Contain(2).And.NotContain(9);
    }

    [Fact]
    public void EquivalencyAssertions_Works()
    {
        var actual = new UserSnapshot("ollie", 3);
        var expected = new UserSnapshot("ollie", 3);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Batch_AggregatesFailures()
    {
        var ex = XAssert.Throws<InvalidOperationException>(() =>
        {
            using var batch = Assert.Batch("smoke");
            "abc".Should().StartWith("z");
            1.Should().BeGreaterThan(5);
        });

        XAssert.Contains("Batch 'smoke' failed with 2 assertion failure(s):", ex.Message, StringComparison.Ordinal);
    }

    private sealed record UserSnapshot(string Name, int Level);
}
EOF

dotnet test --configuration Release
