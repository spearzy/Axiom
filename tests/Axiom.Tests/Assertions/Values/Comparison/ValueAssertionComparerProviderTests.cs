using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.Comparison;

public sealed class ValueAssertionComparerProviderTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void Be_UsesConfiguredComparerProvider()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());

        var value = 3;
        var ex = Record.Exception(() => value.Should().Be(5));

        Assert.Null(ex);
    }

    [Fact]
    public void NotBe_UsesConfiguredComparerProvider()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new OddEvenMatchIntComparerProvider());

        var value = 3;
        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBe(7));

        const string expected = "Expected value to not be 7, but found 3.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Be_FallsBackToDefaultComparer_WhenProviderDoesNotSupplyOne()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new EmptyComparerProvider());

        var value = 42;
        var ex = Record.Exception(() => value.Should().Be(42));

        Assert.Null(ex);
    }

    private sealed class OddEvenMatchIntComparerProvider : IComparerProvider
    {
        public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
        {
            // This provider only handles int assertions for this test fixture.
            if (typeof(T) == typeof(int))
            {
                // The interface is generic, so we cast the int comparer to IEqualityComparer<T>
                // after proving T is int.
                comparer = (IEqualityComparer<T>)(object)new OddEvenMatchIntComparer();
                return true;
            }

            // Returning false tells Axiom to fall back to its default comparer.
            comparer = null;
            return false;
        }
    }

    private sealed class OddEvenMatchIntComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            // Two numbers are "equal" when they share parity (both odd or both even).
            return x % 2 == y % 2;
        }

        public int GetHashCode(int obj)
        {
            // Hash code must match Equals semantics: only two buckets (odd/even).
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
