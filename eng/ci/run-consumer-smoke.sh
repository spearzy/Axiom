#!/usr/bin/env bash

set -euo pipefail

if [[ $# -lt 1 || $# -gt 3 ]]; then
  echo "Usage: $0 <package-source-directory> [axiom-assertions-version|smoke-kind] [smoke-kind]"
  echo "Smoke kinds: xunit (default), nunit, mstest"
  exit 1
fi

package_source="$1"
package_version=""
smoke_kind="xunit"

if [[ $# -eq 2 ]]; then
  case "$2" in
    xunit|nunit|mstest)
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

if [[ "$smoke_kind" != "xunit" && "$smoke_kind" != "nunit" && "$smoke_kind" != "mstest" ]]; then
  echo "Unsupported smoke kind: $smoke_kind"
  echo "Supported smoke kinds: xunit, nunit, mstest"
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

consumer_project="$smoke_root/Axiom.ConsumerSmoke"
local_packages_cache="$smoke_root/.nuget/packages"
nuget_config="$smoke_root/NuGet.config"

cat > "$nuget_config" <<EOF
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
EOF

create_nunit_smoke_tests() {
  rm -f UnitTest1.cs
  cat > ConsumerSmokeTests.cs <<'EOF'
using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using AAssert = Axiom.Core.Assert;
using NUnit.Framework;

namespace Axiom.ConsumerSmoke;

public sealed class ConsumerSmokeTests
{
    [Test]
    public void StringChaining_Works()
    {
        "abc".Should().StartWith("a").And.EndWith("c");
    }

    [Test]
    public void ValueAssertions_Works()
    {
        42.Should().BeGreaterThan(1).And.BeInRange(40, 50);
        42.1d.Should().BeApproximately(42d, 0.2d);
    }

    [Test]
    public void CollectionAssertions_Works()
    {
        new[] { 1, 2, 3 }.Should().Contain(2).And.NotContain(9);
    }

    [Test]
    public void EquivalencyAssertions_Works()
    {
        var actual = new UserSnapshot("ollie", 3);
        var expected = new UserSnapshot("ollie", 3);

        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Batch_AggregatesFailures()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = AAssert.Batch("smoke");
            "abc".Should().StartWith("z");
            1.Should().BeGreaterThan(5);
        });

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex!.Message, Does.Contain("Batch 'smoke' failed with 2 assertion failure(s):"));
    }

    private sealed record UserSnapshot(string Name, int Level);
}
EOF
}

create_xunit_smoke_tests() {
  rm -f UnitTest1.cs
  cat > ConsumerSmokeTests.cs <<'EOF'
using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using AAssert = Axiom.Core.Assert;
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
            using var batch = AAssert.Batch("smoke");
            "abc".Should().StartWith("z");
            1.Should().BeGreaterThan(5);
        });

        XAssert.Contains("Batch 'smoke' failed with 2 assertion failure(s):", ex.Message, StringComparison.Ordinal);
    }

    private sealed record UserSnapshot(string Name, int Level);
}
EOF
}

create_mstest_smoke_tests() {
  rm -f UnitTest1.cs Test1.cs
  cat > ConsumerSmokeTests.cs <<'EOF'
using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using AAssert = Axiom.Core.Assert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axiom.ConsumerSmoke;

[TestClass]
public sealed class ConsumerSmokeTests
{
    [TestMethod]
    public void StringChaining_Works()
    {
        "abc".Should().StartWith("a").And.EndWith("c");
    }

    [TestMethod]
    public void ValueAssertions_Works()
    {
        42.Should().BeGreaterThan(1).And.BeInRange(40, 50);
        42.1d.Should().BeApproximately(42d, 0.2d);
    }

    [TestMethod]
    public void CollectionAssertions_Works()
    {
        new[] { 1, 2, 3 }.Should().Contain(2).And.NotContain(9);
    }

    [TestMethod]
    public void EquivalencyAssertions_Works()
    {
        var actual = new UserSnapshot("ollie", 3);
        var expected = new UserSnapshot("ollie", 3);

        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void Batch_AggregatesFailures()
    {
        InvalidOperationException? ex = null;
        try
        {
            using var batch = AAssert.Batch("smoke");
            "abc".Should().StartWith("z");
            1.Should().BeGreaterThan(5);
            Assert.Fail("Expected InvalidOperationException, but no exception was thrown.");
        }
        catch (InvalidOperationException caught)
        {
            ex = caught;
        }

        Assert.IsNotNull(ex);
        Assert.Contains("Batch 'smoke' failed with 2 assertion failure(s):", ex.Message);
    }

    private sealed record UserSnapshot(string Name, int Level);
}
EOF
}

case "$smoke_kind" in
  xunit)
    dotnet new xunit --framework net10.0 --output "$consumer_project" --no-restore
    ;;
  nunit)
    dotnet new nunit --framework net10.0 --output "$consumer_project" --no-restore
    ;;
  mstest)
    dotnet new mstest --framework net10.0 --output "$consumer_project" --no-restore
    ;;
esac

cd "$consumer_project"
dotnet add package Axiom.Assertions --version "$package_version" --source "$package_source" --no-restore

case "$smoke_kind" in
  xunit)
    create_xunit_smoke_tests
    ;;
  nunit)
    create_nunit_smoke_tests
    ;;
  mstest)
    create_mstest_smoke_tests
    ;;
esac

dotnet restore --configfile "$nuget_config" --packages "$local_packages_cache"
dotnet test --configuration Release --no-restore
