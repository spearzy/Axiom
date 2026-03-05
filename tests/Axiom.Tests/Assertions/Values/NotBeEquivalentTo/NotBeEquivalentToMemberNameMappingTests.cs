using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotBeEquivalentTo;

public sealed class NotBeEquivalentToMemberNameMappingTests
{
    [Fact]
    public void GivenMappedMemberNames_WhenMappedMembersMatch_ThenNotBeEquivalentToThrows()
    {
        var actual = new ActualPerson { GivenName = "Ada", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Ada", Age = 36 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(nameof(ActualPerson.GivenName), nameof(ExpectedPerson.FirstName));
                }));

        Assert.Contains("to not be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenMappedMemberNames_WhenMappedMembersDiffer_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = new ActualPerson { GivenName = "Ada", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Grace", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(nameof(ActualPerson.GivenName), nameof(ExpectedPerson.FirstName));
                }));

        Assert.Null(ex);
    }

    private sealed class ActualPerson
    {
        public string? GivenName { get; init; }
        public int Age { get; init; }
    }

    private sealed class ExpectedPerson
    {
        public string? FirstName { get; init; }
        public int Age { get; init; }
    }
}
