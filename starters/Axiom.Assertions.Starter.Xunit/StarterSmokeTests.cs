using System;
using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using AAssert = Axiom.Core.Assert;
using XAssert = Xunit.Assert;
using Xunit.Sdk;

namespace Axiom.Assertions.Starter.Xunit;

public sealed class StarterSmokeTests
{
    [Fact]
    public void NormalAssertionsInstallPath_Works()
    {
        var profile = new UserProfile("Bob", ["admin", "reviewer"]);

        profile.Name.Should().StartWith("B").And.Contain("ob");
        profile.Roles.Should().Contain("admin").And.HaveCount(2);
    }

    [Fact]
    public void Batch_UsesXunitException()
    {
        var ex = XAssert.Throws<XunitException>(() =>
        {
            using var batch = AAssert.Batch("starter");
            "abc".Should().StartWith("z");
            1.Should().BeGreaterThan(5);
        });

        XAssert.Contains("Batch 'starter' failed with 2 assertion failure(s):", ex.Message, StringComparison.Ordinal);
    }

    private sealed record UserProfile(string Name, string[] Roles);
}
