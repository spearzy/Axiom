using Axiom.Assertions;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToMemberNameMappingTests
{
    [Fact]
    public void GivenMappedMemberNames_WhenMappedMembersMatch_ThenDoesNotThrow()
    {
        var actual = new ActualPerson { GivenName = "Bob", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Bob", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(nameof(ActualPerson.GivenName), nameof(ExpectedPerson.FirstName));
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenMappedMemberNames_WhenMappedMembersDiffer_ThenThrowsWithMappedActualPath()
    {
        var actual = new ActualPerson { GivenName = "Bob", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Alice", Age = 36 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(nameof(ActualPerson.GivenName), nameof(ExpectedPerson.FirstName));
                }));

        Assert.Contains("actual.GivenName", ex.Message, StringComparison.Ordinal);
        Assert.Contains("string mismatch;", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenMultipleMappedMemberNames_WhenAllMappedMembersMatch_ThenDoesNotThrow()
    {
        var actual = new ActualNamedPerson { GivenName = "Bob", Surname = "Smith", Age = 36 };
        var expected = new ExpectedNamedPerson { FirstName = "Bob", LastName = "Smith", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(nameof(ActualNamedPerson.GivenName), nameof(ExpectedNamedPerson.FirstName));
                    options.MatchMemberName(nameof(ActualNamedPerson.Surname), nameof(ExpectedNamedPerson.LastName));
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenSameMappedMemberNames_WhenConfiguredTwice_ThenConfigurationIsIdempotent()
    {
        var actual = new ActualPerson { GivenName = "Bob", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Bob", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(nameof(ActualPerson.GivenName), nameof(ExpectedPerson.FirstName));
                    options.MatchMemberName(nameof(ActualPerson.GivenName), nameof(ExpectedPerson.FirstName));
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenConflictingMappedMemberNamesForActualMember_WhenConfigured_ThenThrowsArgumentException()
    {
        var options = new EquivalencyOptions();

        options.MatchMemberName(nameof(ActualPerson.GivenName), nameof(ExpectedAliasPerson.FirstName));

        var ex = Assert.Throws<ArgumentException>(() =>
            options.MatchMemberName(nameof(ActualPerson.GivenName), nameof(ExpectedAliasPerson.PreferredName)));

        Assert.Contains("conflicting member-name mappings for 'GivenName'", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenConflictingMappedMemberNamesForExpectedMember_WhenConfigured_ThenThrowsArgumentException()
    {
        var options = new EquivalencyOptions();

        options.MatchMemberName(nameof(ActualPerson.GivenName), nameof(ExpectedPerson.FirstName));

        var ex = Assert.Throws<ArgumentException>(() =>
            options.MatchMemberName(nameof(ActualAliasPerson.PreferredName), nameof(ExpectedPerson.FirstName)));

        Assert.Contains("conflicting member-name mappings for 'FirstName'", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenMappedExpectedMemberNameAndIgnoreMember_WhenMappedMemberDiffers_ThenDoesNotThrow()
    {
        var actual = new ActualPerson { GivenName = "Bob", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Alice", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(nameof(ActualPerson.GivenName), nameof(ExpectedPerson.FirstName));
                    options.IgnoreMember(nameof(ExpectedPerson.FirstName));
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenNullActualMemberName_WhenMatchingMemberNames_ThenThrowsArgumentNullException()
    {
        var actual = new ActualPerson { GivenName = "Bob", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Bob", Age = 36 };

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(null!, nameof(ExpectedPerson.FirstName));
                }));

        Assert.Equal("actualMember", ex.ParamName);
    }

    [Fact]
    public void GivenNullExpectedMemberName_WhenMatchingMemberNames_ThenThrowsArgumentNullException()
    {
        var actual = new ActualPerson { GivenName = "Bob", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Bob", Age = 36 };

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(nameof(ActualPerson.GivenName), null!);
                }));

        Assert.Equal("expectedMember", ex.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyOrWhitespaceActualMemberName_WhenMatchingMemberNames_ThenThrowsArgumentException(string actualMember)
    {
        var actual = new ActualPerson { GivenName = "Bob", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Bob", Age = 36 };

        var ex = Assert.Throws<ArgumentException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(actualMember, nameof(ExpectedPerson.FirstName));
                }));

        Assert.Equal("actualMember", ex.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyOrWhitespaceExpectedMemberName_WhenMatchingMemberNames_ThenThrowsArgumentException(string expectedMember)
    {
        var actual = new ActualPerson { GivenName = "Bob", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Bob", Age = 36 };

        var ex = Assert.Throws<ArgumentException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(nameof(ActualPerson.GivenName), expectedMember);
                }));

        Assert.Equal("expectedMember", ex.ParamName);
    }

    [Fact]
    public void GivenUnrelatedLeafTypes_WhenStrictRuntimeTypesDisabled_ThenComparedByValue()
    {
        object actual = 42;
        object expected = 42L;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.RequireStrictRuntimeTypes = false));

        Assert.Contains("values differ", ex.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("runtime types differ", ex.Message, StringComparison.Ordinal);
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

    private sealed class ExpectedAliasPerson
    {
        public string? FirstName { get; init; }
        public string? PreferredName { get; init; }
        public int Age { get; init; }
    }

    private sealed class ActualNamedPerson
    {
        public string? GivenName { get; init; }
        public string? Surname { get; init; }
        public int Age { get; init; }
    }

    private sealed class ActualAliasPerson
    {
        public string? PreferredName { get; init; }
    }

    private sealed class ExpectedNamedPerson
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public int Age { get; init; }
    }
}
