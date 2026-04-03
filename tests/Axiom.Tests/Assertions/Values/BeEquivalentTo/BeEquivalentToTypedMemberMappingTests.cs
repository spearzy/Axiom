using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToTypedMemberMappingTests
{
    [Fact]
    public void GivenTypedTopLevelMemberMapping_WhenMembersMatch_ThenDoesNotThrow()
    {
        var actual = new ActualUser { GivenName = "Alice", Age = 36 };
        var expected = new ExpectedUser { FirstName = "Alice", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenTypedNestedMemberMapping_WhenMembersDiffer_ThenThrowsWithActualSidePath()
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

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.Address!.Postcode, x => x.Location!.ZipCode);
                }));

        Assert.Contains("actual.Address.Postcode", ex.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("actual.Location.ZipCode", ex.Message, StringComparison.Ordinal);
        Assert.Contains("string mismatch;", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenMultipleTypedMappings_WhenAllMappedMembersMatch_ThenDoesNotThrow()
    {
        var actual = new ActualUser
        {
            GivenName = "Alice",
            Surname = "Bob",
            Address = new ActualAddress { Postcode = "AB1 2CD" },
            Age = 36,
        };
        var expected = new ExpectedUser
        {
            FirstName = "Alice",
            LastName = "Bob",
            Location = new ExpectedLocation { ZipCode = "AB1 2CD" },
            Age = 36,
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.Surname, x => x.LastName);
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.Address!.Postcode, x => x.Location!.ZipCode);
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenMultipleTypedNestedMappingsUnderSameParent_WhenAllMappedMembersMatch_ThenDoesNotThrow()
    {
        var actual = new NestedActualUser
        {
            Address = new NestedActualAddress { Postcode = "AB1 2CD", CountryCode = "GB" },
        };
        var expected = new NestedExpectedUser
        {
            Location = new NestedExpectedLocation { ZipCode = "AB1 2CD", IsoCountryCode = "GB" },
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMember<NestedActualUser, NestedExpectedUser>(x => x.Address!.Postcode, x => x.Location!.ZipCode);
                    options.MatchMember<NestedActualUser, NestedExpectedUser>(x => x.Address!.CountryCode, x => x.Location!.IsoCountryCode);
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenTypedMemberMapping_WhenStrictRuntimeTypesDisabled_ThenUnrelatedTypesAreComparedStructurally()
    {
        var actual = new ActualUser { GivenName = "Alice", Age = 36 };
        var expected = new ExpectedUser { FirstName = "Alice", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenInvalidActualSelector_WhenMatchingTypedMembers_ThenThrowsArgumentException()
    {
        var options = new EquivalencyOptions();

        var ex = Assert.Throws<ArgumentException>(() =>
            options.MatchMember<ActualUser, ExpectedUser>(
                x => x.GivenName!.ToUpperInvariant(),
                x => x.FirstName));

        Assert.Equal("actualMemberSelector", ex.ParamName);
    }

    [Fact]
    public void GivenInvalidExpectedSelector_WhenMatchingTypedMembers_ThenThrowsArgumentException()
    {
        var options = new EquivalencyOptions();

        var ex = Assert.Throws<ArgumentException>(() =>
            options.MatchMember<ActualUser, ExpectedUser>(
                x => x.GivenName,
                x => x.FirstName!.ToUpperInvariant()));

        Assert.Equal("expectedMemberSelector", ex.ParamName);
    }

    [Fact]
    public void GivenMismatchedSelectorDepths_WhenMatchingTypedMembers_ThenThrowsArgumentException()
    {
        var options = new EquivalencyOptions();

        var ex = Assert.Throws<ArgumentException>(() =>
            options.MatchMember<ActualUser, ExpectedUser>(
                x => x.Address!,
                x => x.Location!.ZipCode));

        Assert.Equal(
            "actualMemberSelector and expectedMemberSelector must select member paths with the same depth.",
            ex.Message);
    }

    [Fact]
    public void GivenConflictingTypedNestedMappings_WhenConfigured_ThenThrowsArgumentException()
    {
        var options = new EquivalencyOptions();

        options.MatchMember<ConflictingActualUser, ConflictingExpectedUser>(
            x => x.Address!.Postcode,
            x => x.Location!.ZipCode);

        var ex = Assert.Throws<ArgumentException>(() =>
            options.MatchMember<ConflictingActualUser, ConflictingExpectedUser>(
                x => x.Address!.CountryCode,
                x => x.Region!.IsoCountryCode));

        Assert.Contains("conflicting typed member mappings for 'Address'", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenFlatMemberNameMapping_WhenUsedWithoutTypedMapping_ThenStillWorks()
    {
        var actual = new ActualUser { GivenName = "Alice", Age = 36 };
        var expected = new ExpectedUser { FirstName = "Alice", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMemberName(nameof(ActualUser.GivenName), nameof(ExpectedUser.FirstName));
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenTypedMemberMappingAndIgnoredActualPath_WhenMappedMemberDiffers_ThenDoesNotThrow()
    {
        var actual = new ActualUser
        {
            Address = new ActualAddress { Postcode = "AB1 2CD" },
        };
        var expected = new ExpectedUser
        {
            Location = new ExpectedLocation { ZipCode = "ZZ9 9ZZ" },
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.Address!.Postcode, x => x.Location!.ZipCode);
                    options.Ignore<ActualUser>(x => x.Address!.Postcode);
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenTypedMemberMappingAndIgnoredExpectedMemberName_WhenMappedMemberDiffers_ThenDoesNotThrow()
    {
        var actual = new ActualUser { GivenName = "Alice" };
        var expected = new ExpectedUser { FirstName = "Bob" };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
                    options.IgnoreMember(nameof(ExpectedUser.FirstName));
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenTypedAndFlatMappings_WhenBothCouldApply_ThenTypedMappingWins()
    {
        var actual = new ActualUser { GivenName = "Alice", Age = 36 };
        var expected = new ExpectedUser { FirstName = "Alice", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMemberName(nameof(ActualUser.GivenName), "PreferredName");
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
                }));

        Assert.Null(ex);
    }

    private sealed class ActualUser
    {
        public string? GivenName { get; init; }
        public string? Surname { get; init; }
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
        public string? LastName { get; init; }
        public int Age { get; init; }
        public ExpectedLocation? Location { get; init; }
    }

    private sealed class ExpectedLocation
    {
        public string? ZipCode { get; init; }
    }

    private sealed class NestedActualUser
    {
        public NestedActualAddress? Address { get; init; }
    }

    private sealed class NestedActualAddress
    {
        public string? Postcode { get; init; }
        public string? CountryCode { get; init; }
    }

    private sealed class NestedExpectedUser
    {
        public NestedExpectedLocation? Location { get; init; }
    }

    private sealed class NestedExpectedLocation
    {
        public string? ZipCode { get; init; }
        public string? IsoCountryCode { get; init; }
    }

    private sealed class ConflictingActualUser
    {
        public ConflictingActualAddress? Address { get; init; }
    }

    private sealed class ConflictingActualAddress
    {
        public string? Postcode { get; init; }
        public string? CountryCode { get; init; }
    }

    private sealed class ConflictingExpectedUser
    {
        public ConflictingExpectedLocation? Location { get; init; }
        public ConflictingExpectedRegion? Region { get; init; }
    }

    private sealed class ConflictingExpectedLocation
    {
        public string? ZipCode { get; init; }
    }

    private sealed class ConflictingExpectedRegion
    {
        public string? IsoCountryCode { get; init; }
    }
}
