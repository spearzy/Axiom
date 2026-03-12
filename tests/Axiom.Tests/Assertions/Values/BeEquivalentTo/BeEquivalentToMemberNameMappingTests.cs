using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToMemberNameMappingTests
{
    [Fact]
    public void GivenMappedMemberNames_WhenMappedMembersMatch_ThenDoesNotThrow()
    {
        var actual = new ActualPerson { GivenName = "Ada", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Ada", Age = 36 };

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
        var actual = new ActualPerson { GivenName = "Ada", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Grace", Age = 36 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(nameof(ActualPerson.GivenName), nameof(ExpectedPerson.FirstName));
                }));

        Assert.Contains("actual.GivenName", ex.Message, StringComparison.Ordinal);
        Assert.Contains("String values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenNullActualMemberName_WhenMatchingMemberNames_ThenThrowsArgumentNullException()
    {
        var actual = new ActualPerson { GivenName = "Ada", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Ada", Age = 36 };

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
        var actual = new ActualPerson { GivenName = "Ada", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Ada", Age = 36 };

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
        var actual = new ActualPerson { GivenName = "Ada", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Ada", Age = 36 };

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
        var actual = new ActualPerson { GivenName = "Ada", Age = 36 };
        var expected = new ExpectedPerson { FirstName = "Ada", Age = 36 };

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

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("Runtime types differ", ex.Message, StringComparison.Ordinal);
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
