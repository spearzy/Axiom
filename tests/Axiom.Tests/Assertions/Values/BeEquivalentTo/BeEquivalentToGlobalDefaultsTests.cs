using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToGlobalDefaultsTests : IDisposable
{
    public void Dispose()
    {
        EquivalencyDefaults.Reset();
    }

    [Fact]
    public void GivenGlobalAnyOrderDefault_WhenNoPerCallConfiguration_ThenReorderedCollectionDoesNotThrow()
    {
        EquivalencyDefaults.Configure(options => options.CollectionOrder = EquivalencyCollectionOrder.Any);

        var actual = new[] { 3, 1, 2 };
        var expected = new[] { 1, 2, 3 };

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenGlobalAnyOrderDefault_WhenPerCallSetsStrict_ThenPerCallOverrideWins()
    {
        EquivalencyDefaults.Configure(options => options.CollectionOrder = EquivalencyCollectionOrder.Any);

        var actual = new[] { 3, 1, 2 };
        var expected = new[] { 1, 2, 3 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.CollectionOrder = EquivalencyCollectionOrder.Strict));

        Assert.Contains("actual[0]", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenGlobalDefaults_WhenResetCalled_ThenBuiltInDefaultsApplyAgain()
    {
        EquivalencyDefaults.Configure(options => options.CollectionOrder = EquivalencyCollectionOrder.Any);
        EquivalencyDefaults.Reset();

        var actual = new[] { 3, 1, 2 };
        var expected = new[] { 1, 2, 3 };

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("actual[0]", ex.Message, StringComparison.Ordinal);
    }
}
