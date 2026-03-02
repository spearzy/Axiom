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

    [Fact]
    public void GivenGlobalFloatTolerance_WhenNoPerCallOverride_ThenUsesGlobalDefaults()
    {
        EquivalencyDefaults.Configure(options => options.FloatTolerance = 0.05f);

        var actual = 100.00f;
        var expected = 100.03f;

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenGlobalHalfTolerance_WhenPerCallToleranceIsStricter_ThenPerCallOverrideWins()
    {
        EquivalencyDefaults.Configure(options => options.HalfTolerance = 1.0f);

        Half actual = (Half)100.00;
        Half expected = (Half)100.50;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.HalfTolerance = 0.10f));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenGlobalDateOnlyTolerance_WhenNoPerCallOverride_ThenUsesGlobalDefaults()
    {
        EquivalencyDefaults.Configure(options => options.DateOnlyTolerance = TimeSpan.FromDays(2));

        var actual = new DateOnly(2026, 03, 02);
        var expected = new DateOnly(2026, 03, 03);

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenGlobalTimeOnlyTolerance_WhenPerCallToleranceIsStricter_ThenPerCallOverrideWins()
    {
        EquivalencyDefaults.Configure(options => options.TimeOnlyTolerance = TimeSpan.FromSeconds(2));

        var actual = new TimeOnly(10, 15, 00);
        var expected = new TimeOnly(10, 15, 01);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.TimeOnlyTolerance = TimeSpan.FromMilliseconds(500)));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenGlobalDateTimeTolerance_WhenNoPerCallOverride_ThenUsesGlobalDefaults()
    {
        EquivalencyDefaults.Configure(options => options.DateTimeTolerance = TimeSpan.FromSeconds(2));

        var actual = new DateTime(2026, 03, 02, 12, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddSeconds(1);

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenGlobalDateTimeOffsetTolerance_WhenPerCallToleranceIsStricter_ThenPerCallOverrideWins()
    {
        EquivalencyDefaults.Configure(options => options.DateTimeOffsetTolerance = TimeSpan.FromSeconds(2));

        var actual = new DateTimeOffset(2026, 03, 02, 12, 00, 00, TimeSpan.Zero);
        var expected = actual.AddSeconds(1);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DateTimeOffsetTolerance = TimeSpan.FromMilliseconds(500)));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenGlobalDecimalTolerance_WhenNoPerCallOverride_ThenUsesGlobalDefaults()
    {
        EquivalencyDefaults.Configure(options => options.DecimalTolerance = 0.05m);

        var actual = 100.00m;
        var expected = 100.03m;

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenGlobalDecimalTolerance_WhenPerCallToleranceIsStricter_ThenPerCallOverrideWins()
    {
        EquivalencyDefaults.Configure(options => options.DecimalTolerance = 0.05m);

        var actual = 100.00m;
        var expected = 100.03m;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DecimalTolerance = 0.01m));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenGlobalTimeSpanTolerance_WhenNoPerCallOverride_ThenUsesGlobalDefaults()
    {
        EquivalencyDefaults.Configure(options => options.TimeSpanTolerance = TimeSpan.FromSeconds(1));

        var actual = TimeSpan.FromSeconds(10);
        var expected = TimeSpan.FromSeconds(10.8);

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenGlobalTimeSpanTolerance_WhenPerCallToleranceIsStricter_ThenPerCallOverrideWins()
    {
        EquivalencyDefaults.Configure(options => options.TimeSpanTolerance = TimeSpan.FromSeconds(1));

        var actual = TimeSpan.FromSeconds(10);
        var expected = TimeSpan.FromSeconds(10.8);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.TimeSpanTolerance = TimeSpan.FromMilliseconds(200)));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }
}
