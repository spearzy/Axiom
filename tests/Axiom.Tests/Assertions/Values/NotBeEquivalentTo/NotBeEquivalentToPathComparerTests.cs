using System.Collections;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.NotBeEquivalentTo;

public sealed class NotBeEquivalentToPathComparerTests
{
    [Fact]
    public void GivenPathComparer_WhenStringMemberDiffersByCaseButComparerTreatsAsEqual_ThenNotBeEquivalentToThrows()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "abc", Age = 30 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options =>
                {
                    options.StringComparison = StringComparison.Ordinal;
                    options.UseComparerForPath("actual.Name", StringComparer.OrdinalIgnoreCase);
                }));

        Assert.Contains("to not be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenPathComparer_WhenMatchingMemberValuesButComparerTreatsAsDifferent_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "ABC", Age = 30 };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options => options.UseComparerForPath("actual.Name", new AlwaysFalseObjectComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenRelativePathComparer_WhenMatchingMemberValuesButComparerTreatsAsDifferent_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "ABC", Age = 30 };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options => options.UseComparerForPath(nameof(Person.Name), new AlwaysFalseObjectComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenSameRootReference_WhenChildPathComparerReturnsFalse_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = new Person { Name = "ABC", Age = 30 };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(
                actual,
                options => options.UseComparerForPath("actual.Name", new AlwaysFalseObjectComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenToleranceAndPathComparerOnSameLeaf_WhenValueIsWithinTolerance_ThenToleranceWinsForNotBeEquivalentTo()
    {
        var actual = new ScoreHolder { Score = 10.0 };
        var expected = new ScoreHolder { Score = 10.2 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options =>
                {
                    options.DoubleTolerance = 0.5;
                    options.UseComparerForPath("actual.Score", new AlwaysFalseObjectComparer());
                }));

        Assert.Contains("to not be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenToleranceAndPathComparerOnSameLeaf_WhenValueIsOutsideTolerance_ThenToleranceStillWinsForNotBeEquivalentTo()
    {
        var actual = new ScoreHolder { Score = 10.0 };
        var expected = new ScoreHolder { Score = 11.0 };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(
                expected,
                options =>
                {
                    options.DoubleTolerance = 0.5;
                    options.UseComparerForPath("actual.Score", new AlwaysTrueObjectComparer());
                }));

        Assert.Null(ex);
    }

    private sealed class Person
    {
        public string? Name { get; init; }
        public int Age { get; init; }
    }

    private sealed class ScoreHolder
    {
        public double Score { get; init; }
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

    private sealed class AlwaysTrueObjectComparer : IEqualityComparer
    {
        public new bool Equals(object? x, object? y)
        {
            return true;
        }

        public int GetHashCode(object obj)
        {
            return 0;
        }
    }
}
