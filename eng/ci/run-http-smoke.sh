#!/usr/bin/env bash

set -euo pipefail

if [[ $# -lt 1 || $# -gt 2 ]]; then
  echo "Usage: $0 <package-source-directory> [axiom-http-version]"
  exit 1
fi

package_source="$1"
package_version="${2:-}"

if [[ ! -d "$package_source" ]]; then
  echo "Package source directory does not exist: $package_source"
  exit 1
fi

if [[ -z "$package_version" ]]; then
  package_file="$(ls "$package_source"/Axiom.Http.*.nupkg 2>/dev/null | head -n 1 || true)"
  if [[ -z "$package_file" ]]; then
    echo "Could not find Axiom.Http package in: $package_source"
    exit 1
  fi

  package_name="$(basename "$package_file")"
  package_version="${package_name#Axiom.Http.}"
  package_version="${package_version%.nupkg}"
fi

smoke_root="$(mktemp -d)"
trap 'rm -rf "$smoke_root"' EXIT

consumer_project="$smoke_root/Axiom.Http.Smoke"
local_packages_cache="$smoke_root/.nuget/packages"
nuget_config="$smoke_root/NuGet.config"
export NUGET_PACKAGES="$local_packages_cache"

cat > "$nuget_config" <<EOF2
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$package_source" />
  </packageSources>
</configuration>
EOF2

dotnet new console -n Axiom.Http.Smoke -f net10.0 -o "$consumer_project" --no-restore >/dev/null

cd "$consumer_project"
dotnet add package Axiom.Http --version "$package_version" --source "$package_source" --no-restore >/dev/null

cat > Program.cs <<'EOF2'
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

using var okResponse = new HttpResponseMessage(HttpStatusCode.OK)
{
    Content = new StringContent(
        """
        { "id": 1, "name": "Ada", "roles": ["admin", "author"] }
        """,
        Encoding.UTF8,
        "application/json")
};

okResponse.Headers.Add("ETag", "\"v1\"");
okResponse.Headers.Add("X-Trace", ["a", "b"]);

okResponse.Should().HaveStatusCode(HttpStatusCode.OK);
okResponse.Should().HaveStatusCode(200);
okResponse.Should().NotHaveStatusCode(HttpStatusCode.BadRequest);
okResponse.Should().HaveHeader("ETag");
okResponse.Should().HaveHeaderValue("ETag", "\"v1\"");
okResponse.Should().HaveHeaderValues("X-Trace", ["a", "b"]);
okResponse.Should().HaveContentType("application/json");
okResponse.Should().HaveContentTypeWithCharset("application/json", "utf-8");
okResponse.Should().HaveJsonBodyEquivalentTo("""{ "roles": ["admin", "author"], "name": "Ada", "id": 1.0 }""");
okResponse.Should().HaveJsonPath("$.roles[1]");
okResponse.Should().HaveJsonStringAtPath("$.name", "Ada");
okResponse.Should().HaveJsonNumberAtPath("$.id", 1m);

using var problemResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
{
    Content = new StringContent(
        """
        {
          "type": "https://example.test/problems/validation",
          "title": "Validation failed",
          "status": 400,
          "detail": "Name is required."
        }
        """,
        Encoding.UTF8,
        "application/problem+json")
};

problemResponse.Should().HaveProblemDetails();
problemResponse.Should().HaveProblemDetailsTitle("Validation failed");
problemResponse.Should().HaveProblemDetailsStatus(400);
problemResponse.Should().HaveProblemDetailsType("https://example.test/problems/validation");
problemResponse.Should().HaveProblemDetailsDetail("Name is required.");

Console.WriteLine("Axiom.Http smoke passed.");
EOF2

dotnet run --project Axiom.Http.Smoke.csproj --configfile "$nuget_config" --packages "$local_packages_cache"
