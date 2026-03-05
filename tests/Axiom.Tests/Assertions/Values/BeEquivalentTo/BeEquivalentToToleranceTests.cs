using Axiom.Assertions;
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
    public void GivenFloatValues_WhenWithinPerCallTolerance_ThenDoesNotThrow()
    {
        var actual = 10.00f;
        var expected = 10.01f;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.FloatTolerance = 0.02f));

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
    public void GivenHalfValues_WhenOutsidePerCallTolerance_ThenThrows()
    {
        Half actual = (Half)10.00;
        Half expected = (Half)10.02;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.HalfTolerance = 0.01f));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenDateOnlyValues_WhenWithinPerCallTolerance_ThenDoesNotThrow()
    {
        var actual = new DateOnly(2026, 03, 02);
        var expected = actual.AddDays(1);

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DateOnlyTolerance = TimeSpan.FromDays(1)));

        Assert.Null(ex);
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
    public void GivenTimeOnlyValues_WhenOutsidePerCallTolerance_ThenThrows()
    {
        var actual = new TimeOnly(10, 15, 00);
        var expected = new TimeOnly(10, 15, 02);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.TimeOnlyTolerance = TimeSpan.FromMilliseconds(500)));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenTimeOnlyValues_WhenWithinPerCallTolerance_ThenDoesNotThrow()
    {
        var actual = new TimeOnly(10, 15, 00);
        var expected = actual.Add(TimeSpan.FromMilliseconds(400));

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.TimeOnlyTolerance = TimeSpan.FromMilliseconds(500)));

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

    [Fact]
    public void GivenNaNValues_WhenComparingWithTolerance_ThenNaNMatchesNaN()
    {
        var actual = double.NaN;
        var expected = double.NaN;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DoubleTolerance = 0.01d));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenPositiveInfinityValues_WhenComparingWithTolerance_ThenTheyMatch()
    {
        var actual = double.PositiveInfinity;
        var expected = double.PositiveInfinity;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DoubleTolerance = 0.01d));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenOppositeInfinityValues_WhenComparingWithTolerance_ThenTheyDoNotMatch()
    {
        var actual = double.PositiveInfinity;
        var expected = double.NegativeInfinity;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DoubleTolerance = 0.01d));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenFloatNaNValues_WhenComparingWithTolerance_ThenNaNMatchesNaN()
    {
        var actual = float.NaN;
        var expected = float.NaN;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.FloatTolerance = 0.01f));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenFloatPositiveInfinityValues_WhenComparingWithTolerance_ThenTheyMatch()
    {
        var actual = float.PositiveInfinity;
        var expected = float.PositiveInfinity;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.FloatTolerance = 0.01f));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenFloatOppositeInfinityValues_WhenComparingWithTolerance_ThenTheyDoNotMatch()
    {
        var actual = float.PositiveInfinity;
        var expected = float.NegativeInfinity;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.FloatTolerance = 0.01f));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenHalfNaNValues_WhenComparingWithTolerance_ThenNaNMatchesNaN()
    {
        Half actual = Half.NaN;
        Half expected = Half.NaN;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.HalfTolerance = 0.01f));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenHalfPositiveInfinityValues_WhenComparingWithTolerance_ThenTheyMatch()
    {
        Half actual = Half.PositiveInfinity;
        Half expected = Half.PositiveInfinity;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.HalfTolerance = 0.01f));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenHalfOppositeInfinityValues_WhenComparingWithTolerance_ThenTheyDoNotMatch()
    {
        Half actual = Half.PositiveInfinity;
        Half expected = Half.NegativeInfinity;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.HalfTolerance = 0.01f));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenDecimalValues_WhenUsingNegativeTolerance_ThenToleranceIsNormalised()
    {
        var actual = 10.00m;
        var expected = 10.01m;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DecimalTolerance = -0.02m));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenFloatValues_WhenUsingNegativeTolerance_ThenToleranceIsNormalised()
    {
        var actual = 10.00f;
        var expected = 10.01f;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.FloatTolerance = -0.02f));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenDoubleValues_WhenUsingNegativeTolerance_ThenToleranceIsNormalised()
    {
        var actual = 10.00d;
        var expected = 10.01d;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DoubleTolerance = -0.02d));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenHalfValues_WhenUsingNegativeTolerance_ThenToleranceIsNormalised()
    {
        Half actual = (Half)10.00;
        Half expected = (Half)10.01;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.HalfTolerance = -0.02f));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenDateOnlyValues_WhenUsingNegativeTolerance_ThenToleranceIsNormalised()
    {
        var actual = new DateOnly(2026, 03, 02);
        var expected = actual.AddDays(1);

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DateOnlyTolerance = -TimeSpan.FromDays(1)));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenDateTimeValues_WhenUsingNegativeTolerance_ThenToleranceIsNormalised()
    {
        var actual = new DateTime(2026, 03, 02, 12, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMilliseconds(500);

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DateTimeTolerance = -TimeSpan.FromSeconds(1)));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenDateTimeOffsetValues_WhenUsingNegativeTolerance_ThenToleranceIsNormalised()
    {
        var actual = new DateTimeOffset(2026, 03, 02, 12, 00, 00, TimeSpan.Zero);
        var expected = actual.AddMilliseconds(500);

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DateTimeOffsetTolerance = -TimeSpan.FromSeconds(1)));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenTimeOnlyValues_WhenUsingNegativeTolerance_ThenToleranceIsNormalised()
    {
        var actual = new TimeOnly(10, 15, 02);
        var expected = new TimeOnly(10, 15, 01);

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.TimeOnlyTolerance = -TimeSpan.FromSeconds(2)));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenTimeSpanValues_WhenUsingNegativeTolerance_ThenToleranceIsNormalised()
    {
        var actual = TimeSpan.FromSeconds(10);
        var expected = TimeSpan.FromSeconds(10.5);

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.TimeSpanTolerance = -TimeSpan.FromSeconds(1)));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenDoubleValues_WhenDifferenceEqualsTolerance_ThenDoesNotThrow()
    {
        var actual = 10.00d;
        var expected = 10.01d;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DoubleTolerance = 0.01d));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenTimeSpanValues_WhenDifferenceEqualsTolerance_ThenDoesNotThrow()
    {
        var actual = TimeSpan.FromSeconds(10);
        var expected = TimeSpan.FromSeconds(10.5);

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.TimeSpanTolerance = TimeSpan.FromMilliseconds(500)));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenDateOnlyToleranceIsTimeSpanMinValue_WhenComparing_ThenThrowsArgumentOutOfRangeException()
    {
        var actual = new DateOnly(2026, 03, 03);
        var expected = actual;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DateOnlyTolerance = TimeSpan.MinValue));

        Assert.Equal("DateOnlyTolerance", ex.ParamName);
    }

    [Fact]
    public void GivenDateTimeToleranceIsTimeSpanMinValue_WhenComparing_ThenThrowsArgumentOutOfRangeException()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DateTimeTolerance = TimeSpan.MinValue));

        Assert.Equal("DateTimeTolerance", ex.ParamName);
    }

    [Fact]
    public void GivenDateTimeOffsetToleranceIsTimeSpanMinValue_WhenComparing_ThenThrowsArgumentOutOfRangeException()
    {
        var actual = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);
        var expected = actual;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.DateTimeOffsetTolerance = TimeSpan.MinValue));

        Assert.Equal("DateTimeOffsetTolerance", ex.ParamName);
    }

    [Fact]
    public void GivenTimeOnlyToleranceIsTimeSpanMinValue_WhenComparing_ThenThrowsArgumentOutOfRangeException()
    {
        var actual = new TimeOnly(10, 15, 00);
        var expected = actual;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.TimeOnlyTolerance = TimeSpan.MinValue));

        Assert.Equal("TimeOnlyTolerance", ex.ParamName);
    }

    [Fact]
    public void GivenTimeSpanToleranceIsTimeSpanMinValue_WhenComparing_ThenThrowsArgumentOutOfRangeException()
    {
        var actual = TimeSpan.FromMinutes(10);
        var expected = actual;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.TimeSpanTolerance = TimeSpan.MinValue));

        Assert.Equal("TimeSpanTolerance", ex.ParamName);
    }
}
