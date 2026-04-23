using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using AAssert = Axiom.Core.Assert;

namespace Axiom.Assertions.Starter.MSTest;

[TestClass]
public sealed class StarterSmokeTests
{
    [TestMethod]
    public void NormalAssertionsInstallPath_Works()
    {
        var profile = new UserProfile("Bob", ["admin", "reviewer"]);

        profile.Name.Should().StartWith("B").And.Contain("ob");
        profile.Roles.Should().Contain("admin").And.HaveCount(2);
    }

    [TestMethod]
    public void Batch_UsesMstestAssertionException()
    {
        AssertFailedException? ex = null;

        try
        {
            using var batch = AAssert.Batch("starter");
            "abc".Should().StartWith("z");
            1.Should().BeGreaterThan(5);
            Assert.Fail("Expected AssertFailedException, but no exception was thrown.");
        }
        catch (AssertFailedException caught)
        {
            ex = caught;
        }

        Assert.IsNotNull(ex);
        StringAssert.Contains(ex.Message, "Batch 'starter' failed with 2 assertion failure(s):");
    }

    private sealed record UserProfile(string Name, string[] Roles);
}
