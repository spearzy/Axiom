using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToToleranceTests : IDisposable
{
    public void Dispose()
    {
        EquivalencyDefaults.Reset();
    }

    [Fact]
    public void GivenDecimalValues_WhenNoToleranceConfigured_ThenRequiresExactEquality()
    {
        var actual = 10.00m;
        var expected = 10.01m;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenDecimalValues_WhenWithinPerCallTolerance_ThenDoesNotThrow()
    {
        var actual = 10.00m;
        var expected = 10.01m;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DecimalTolerance = 0.02m));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenDoubleValues_WhenOutsidePerCallTolerance_ThenThrows()
    {
        var actual = 10.00d;
        var expected = 10.02d;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DoubleTolerance = 0.01d));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenDateTimeValues_WhenWithinPerCallTolerance_ThenDoesNotThrow()
    {
        var actual = new DateTime(2026, 03, 02, 12, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMilliseconds(500);

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DateTimeTolerance = TimeSpan.FromSeconds(1)));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenTimeSpanValues_WhenOutsidePerCallTolerance_ThenThrows()
    {
        var actual = TimeSpan.FromSeconds(10);
        var expected = TimeSpan.FromSeconds(12);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.TimeSpanTolerance = TimeSpan.FromMilliseconds(500)));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenGlobalDoubleTolerance_WhenNoPerCallOverride_ThenUsesGlobalDefaults()
    {
        EquivalencyDefaults.Configure(options => options.DoubleTolerance = 0.05d);

        var actual = 100.00d;
        var expected = 100.03d;

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenGlobalDoubleTolerance_WhenPerCallToleranceIsStricter_ThenPerCallOverrideWins()
    {
        EquivalencyDefaults.Configure(options => options.DoubleTolerance = 0.05d);

        var actual = 100.00d;
        var expected = 100.03d;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DoubleTolerance = 0.01d));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }
}
