namespace Axiom.Tests.Assertions.Values.NotBeEquivalentTo;

public sealed class NotBeEquivalentToTypedMemberMappingTests
{
    [Fact]
    public void GivenTypedTopLevelMemberMapping_WhenMappedMembersMatch_ThenNotBeEquivalentToThrows()
    {
        var actual = new ActualUser { GivenName = "Alice", Age = 36 };
        var expected = new ExpectedUser { FirstName = "Alice", Age = 36 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
                }));

        Assert.Contains("to not be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenTypedNestedMemberMapping_WhenMappedMembersDiffer_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = new ActualUser
        {
            GivenName = "Alice",
            Address = new ActualAddress { Postcode = "AB1 2CD" },
        };
        var expected = new ExpectedUser
        {
            FirstName = "Alice",
            Location = new ExpectedLocation { ZipCode = "ZZ9 9ZZ" },
        };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.Address!.Postcode, x => x.Location!.ZipCode);
                }));

        Assert.Null(ex);
    }

    private sealed class ActualUser
    {
        public string? GivenName { get; init; }
        public int Age { get; init; }
        public ActualAddress? Address { get; init; }
    }

    private sealed class ActualAddress
    {
        public string? Postcode { get; init; }
    }

    private sealed class ExpectedUser
    {
        public string? FirstName { get; init; }
        public int Age { get; init; }
        public ExpectedLocation? Location { get; init; }
    }

    private sealed class ExpectedLocation
    {
        public string? ZipCode { get; init; }
    }
}
