namespace Axiom.Tests.Assertions.Values.NotBeEquivalentTo;

public sealed class NotBeEquivalentToStructuralTypeComparisonTests
{
    [Fact]
    public void GivenUnrelatedTypesWithMatchingMemberNames_WhenStrictRuntimeTypesDisabled_ThenNotBeEquivalentToThrows()
    {
        var actual = new ActualPerson { Name = "Ada", Age = 36 };
        var expected = new ExpectedPerson { Name = "Ada", Age = 36 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options => options.RequireStrictRuntimeTypes = false));

        Assert.Contains("to not be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenUnrelatedTypesWithMatchingMemberNames_WhenValuesDiffer_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = new ActualPerson { Name = "Ada", Age = 36 };
        var expected = new ExpectedPerson { Name = "Grace", Age = 36 };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options => options.RequireStrictRuntimeTypes = false));

        Assert.Null(ex);
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
}
