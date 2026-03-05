using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotBeEquivalentTo;

public sealed class NotBeEquivalentToTests
{
    private sealed class Person
    {
        public string? Name { get; init; }
        public int Age { get; init; }
    }

    [Fact]
    public void GivenLeafValues_WhenEquivalent_ThenThrowsDeterministicMessage()
    {
        var value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeEquivalentTo(42));

        const string expected = "Expected value to not be equivalent to 42, but found 42.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void GivenLeafValues_WhenNotEquivalent_ThenDoesNotThrow()
    {
        var value = 42;

        var ex = Record.Exception(() => value.Should().NotBeEquivalentTo(7));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenEquivalentValues_WhenBecauseProvided_ThenReasonIsIncluded()
    {
        var value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotBeEquivalentTo(42, "the value should differ for this scenario"));

        Assert.Contains("because the value should differ for this scenario", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenStringValues_WhenCaseInsensitiveComparisonConfigured_AndValuesMatch_ThenThrows()
    {
        object value = "ABC";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotBeEquivalentTo("abc", options => options.StringComparison = StringComparison.OrdinalIgnoreCase));

        Assert.Contains("to not be equivalent to \"abc\"", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenStringValues_WhenCaseInsensitiveComparisonConfigured_AndValuesDiffer_ThenDoesNotThrow()
    {
        object value = "ABC";

        var ex = Record.Exception(() =>
            value.Should().NotBeEquivalentTo("xyz", options => options.StringComparison = StringComparison.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenNullConfigureDelegate_WhenCallingConfigureOverload_ThenThrowsArgumentNullException()
    {
        var value = 42;
        Action<Axiom.Assertions.Equivalency.EquivalencyOptions>? configure = null;

        var ex = Assert.Throws<ArgumentNullException>(() => value.Should().NotBeEquivalentTo(7, configure!));

        Assert.Equal("configure", ex.ParamName);
    }

    [Fact]
    public void GivenOnlyComparedMember_WhenSelectedMemberMatches_ThenThrowsEvenIfOtherMembersDiffer()
    {
        var actual = new Person { Name = "Alice", Age = 30 };
        var expected = new Person { Name = "Alice", Age = 99 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(expected, options => options.OnlyCompareMember(nameof(Person.Name))));

        Assert.Contains("to not be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenOnlyComparedMember_WhenSelectedMemberDiffers_ThenDoesNotThrow()
    {
        var actual = new Person { Name = "Alice", Age = 30 };
        var expected = new Person { Name = "Bob", Age = 30 };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(expected, options => options.OnlyCompareMember(nameof(Person.Name))));

        Assert.Null(ex);
    }
}
