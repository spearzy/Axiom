namespace Axiom.Tests.Core.Comparison;

public sealed class DefaultComparerProviderTests
{
    [Fact]
    public void TryGetEqualityComparer_ReturnsTrue_ForString()
    {
        var provider = DefaultComparerProvider.Instance;

        var found = provider.TryGetEqualityComparer<string>(out var comparer);

        Xunit.Assert.True(found);
        Xunit.Assert.NotNull(comparer);
        Xunit.Assert.True(comparer!.Equals("a", "a"));
        Xunit.Assert.False(comparer.Equals("a", "b"));
    }

    [Fact]
    public void TryGetEqualityComparer_ReturnsTrue_ForInt()
    {
        var provider = DefaultComparerProvider.Instance;

        var found = provider.TryGetEqualityComparer<int>(out var comparer);

        Xunit.Assert.True(found);
        Xunit.Assert.NotNull(comparer);
        Xunit.Assert.True(comparer!.Equals(42, 42));
        Xunit.Assert.False(comparer.Equals(42, 43));
    }
}
