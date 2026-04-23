using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using AAssert = Axiom.Core.Assert;

namespace Axiom.Assertions.Starter.NUnit;

public sealed class StarterSmokeTests
{
    [Test]
    public void NormalAssertionsInstallPath_Works()
    {
        var profile = new UserProfile("Bob", ["admin", "reviewer"]);

        profile.Name.Should().StartWith("B").And.Contain("ob");
        profile.Roles.Should().Contain("admin").And.HaveCount(2);
    }

    [Test]
    public void Batch_UsesNunitAssertionException()
    {
        var ex = Assert.Throws<AssertionException>(() =>
        {
            using var batch = AAssert.Batch("starter");
            "abc".Should().StartWith("z");
            1.Should().BeGreaterThan(5);
        });

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex!.Message, Does.Contain("Batch 'starter' failed with 2 assertion failure(s):"));
    }

    private sealed record UserProfile(string Name, string[] Roles);
}
