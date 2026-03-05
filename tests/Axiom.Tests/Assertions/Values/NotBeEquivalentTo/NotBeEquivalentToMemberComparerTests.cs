using System.Collections;
using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotBeEquivalentTo;

public sealed class NotBeEquivalentToMemberComparerTests
{
    [Fact]
    public void GivenMemberComparer_WhenStringMemberDiffersByCaseButComparerTreatsAsEqual_ThenNotBeEquivalentToThrows()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "abc", Age = 30 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options =>
                {
                    options.StringComparison = StringComparison.Ordinal;
                    options.UseComparerForMember(nameof(Person.Name), StringComparer.OrdinalIgnoreCase);
                }));

        Assert.Contains("to not be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenMemberComparer_WhenMatchingValuesButComparerTreatsAsDifferent_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "ABC", Age = 30 };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options => options.UseComparerForMember(nameof(Person.Name), new AlwaysFalseObjectComparer())));

        Assert.Null(ex);
    }

    private sealed class Person
    {
        public string? Name { get; init; }
        public int Age { get; init; }
    }

    private sealed class AlwaysFalseObjectComparer : IEqualityComparer
    {
        public new bool Equals(object? x, object? y)
        {
            return false;
        }

        public int GetHashCode(object obj)
        {
            return 0;
        }
    }
}
