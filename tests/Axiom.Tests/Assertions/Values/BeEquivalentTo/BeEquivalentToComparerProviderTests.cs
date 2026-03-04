using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToComparerProviderTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void GivenConfiguredComparerProvider_WhenComparingTopLevelLeaf_ThenUsesConfiguredComparer()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());

        var actual = 3;
        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(5));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenConfiguredComparerProvider_WhenComparingNestedLeafMember_ThenUsesConfiguredComparer()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());

        var actual = new NumberWrapper { Number = 3 };
        var expected = new NumberWrapper { Number = 5 };

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenProviderWithoutComparer_WhenComparingLeafValues_ThenFallsBackToDefaultEquality()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new EmptyComparerProvider());

        var actual = 3;
        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(5));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    private sealed class NumberWrapper
    {
        public int Number { get; init; }
    }

    private sealed class OddEvenMatchIntComparerProvider : IComparerProvider
    {
        public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
        {
            if (typeof(T) == typeof(int))
            {
                comparer = (IEqualityComparer<T>)(object)new OddEvenMatchIntComparer();
                return true;
            }

            comparer = null;
            return false;
        }
    }

    private sealed class OddEvenMatchIntComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            // Treat numbers as equivalent when they are both odd or both even.
            return x % 2 == y % 2;
        }

        public int GetHashCode(int obj)
        {
            return obj % 2;
        }
    }

    private sealed class EmptyComparerProvider : IComparerProvider
    {
        public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
        {
            comparer = null;
            return false;
        }
    }
}
