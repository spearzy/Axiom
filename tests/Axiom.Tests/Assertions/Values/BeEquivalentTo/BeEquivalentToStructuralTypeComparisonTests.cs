namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToStructuralTypeComparisonTests
{
    [Fact]
    public void GivenUnrelatedTypesWithMatchingMemberNames_WhenStrictRuntimeTypesDisabled_ThenDoesNotThrow()
    {
        var actual = new ActualPerson { Name = "Bob", Age = 36 };
        var expected = new ExpectedPerson { Name = "Bob", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.RequireStrictRuntimeTypes = false));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenUnrelatedTypesWithMatchingMemberNames_WhenMemberValuesDiffer_ThenReportsMemberDifference()
    {
        var actual = new ActualPerson { Name = "Bob", Age = 36 };
        var expected = new ExpectedPerson { Name = "Alice", Age = 36 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.RequireStrictRuntimeTypes = false));

        Assert.Contains("actual.Name", ex.Message, StringComparison.Ordinal);
        Assert.Contains("String values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenUnrelatedTypesWithRenamedMembers_WhenMapped_ThenDoesNotThrow()
    {
        var actual = new ActualPerson { Name = "Bob", Age = 36 };
        var expected = new RenamedPerson { FirstName = "Bob", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.MatchMemberName(nameof(ActualPerson.Name), nameof(RenamedPerson.FirstName));
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenUnrelatedTypesWithRenamedMembers_WhenFailOnMissingAndExtraDisabled_ThenShapeDifferencesAreIgnored()
    {
        var actual = new ActualPerson { Name = "Bob", Age = 36 };
        var expected = new RenamedPerson { FirstName = "Bob", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenUnrelatedTypesWithRenamedMembers_WhenNoMappingAndShapeStrict_ThenThrowsForMissingMembers()
    {
        var actual = new ActualPerson { Name = "Bob", Age = 36 };
        var expected = new RenamedPerson { FirstName = "Bob", Age = 36 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.RequireStrictRuntimeTypes = false));

        Assert.Contains("Member missing on expected type.", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Member missing on actual type.", ex.Message, StringComparison.Ordinal);
    }
    
    [Fact]
    public void GivenUnrelatedTypesWithRenamedMembers_WhenRequireStrictRuntimeTypesEnabled_ThenThrowsForDifferentTypes()
    {
        var actual = new ActualPerson { Name = "Bob", Age = 36 };
        var expected = new RenamedPerson { FirstName = "Bob", Age = 36 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = true;
                    options.MatchMemberName(nameof(ActualPerson.Name), nameof(RenamedPerson.FirstName));
                }));

        Assert.Contains("Runtime types differ", ex.Message, StringComparison.Ordinal);
    }

    private sealed class ActualPerson
    {
        public string? Name { get; init; }
        public int Age { get; init; }
    }

    private sealed class ExpectedPerson
    {
        public string? Name { get; init; }
        public int Age { get; init; }
    }

    private sealed class RenamedPerson
    {
        public string? FirstName { get; init; }
        public int Age { get; init; }
    }
}
