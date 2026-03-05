using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Temporal.BeWithin;

public sealed class BeWithinTests
{
    [Fact]
    public void BeWithin_ReturnsContinuation_WhenDateTimeDifferenceIsWithinTolerance()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMilliseconds(400);

        var baseAssertions = actual.Should();
        var continuation = baseAssertions.BeWithin(expected, TimeSpan.FromMilliseconds(500));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeWithin_Throws_WhenDateTimeDifferenceExceedsTolerance()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddSeconds(2);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeWithin(expected, TimeSpan.FromMilliseconds(500)));

        Assert.Contains("to be within 00:00:00.5000000 of", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeWithin_NormalisesNegativeTolerance_ForDateTime()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMilliseconds(400);

        var ex = Record.Exception(() =>
            actual.Should().BeWithin(expected, -TimeSpan.FromMilliseconds(500)));

        Assert.Null(ex);
    }

    [Fact]
    public void BeWithin_DoesNotThrow_WhenDateTimeOffsetDifferenceIsWithinTolerance()
    {
        var actual = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);
        var expected = actual.AddMilliseconds(400);

        var ex = Record.Exception(() =>
            actual.Should().BeWithin(expected, TimeSpan.FromMilliseconds(500)));

        Assert.Null(ex);
    }

    [Fact]
    public void BeWithin_ThrowsArgumentOutOfRangeException_WhenDateTimeToleranceIsTimeSpanMinValue()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            actual.Should().BeWithin(expected, TimeSpan.MinValue));

        Assert.Equal("tolerance", ex.ParamName);
    }

    [Fact]
    public void BeWithin_ThrowsArgumentOutOfRangeException_WhenDateTimeOffsetToleranceIsTimeSpanMinValue()
    {
        var actual = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);
        var expected = actual;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            actual.Should().BeWithin(expected, TimeSpan.MinValue));

        Assert.Equal("tolerance", ex.ParamName);
    }

    [Fact]
    public void BeWithin_DoesNotThrow_WhenDateOnlyDifferenceIsWithinTolerance()
    {
        var actual = new DateOnly(2026, 03, 03);
        var expected = actual.AddDays(1);

        var ex = Record.Exception(() =>
            actual.Should().BeWithin(expected, TimeSpan.FromDays(1)));

        Assert.Null(ex);
    }

    [Fact]
    public void BeWithin_Throws_WhenDateOnlyDifferenceExceedsTolerance()
    {
        var actual = new DateOnly(2026, 03, 03);
        var expected = actual.AddDays(2);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeWithin(expected, TimeSpan.FromDays(1)));

        Assert.Contains("to be within 1.00:00:00 of", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeWithin_DoesNotThrow_WhenTimeOnlyDifferenceIsWithinTolerance()
    {
        var actual = new TimeOnly(10, 00, 00);
        var expected = actual.Add(TimeSpan.FromMilliseconds(400));

        var ex = Record.Exception(() =>
            actual.Should().BeWithin(expected, TimeSpan.FromMilliseconds(500)));

        Assert.Null(ex);
    }

    [Fact]
    public void BeWithin_Throws_WhenTimeOnlyDifferenceExceedsTolerance()
    {
        var actual = new TimeOnly(10, 00, 00);
        var expected = actual.Add(TimeSpan.FromSeconds(2));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeWithin(expected, TimeSpan.FromMilliseconds(500)));

        Assert.Contains("to be within 00:00:00.5000000 of", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeWithin_DoesNotThrow_WhenTimeOnlyWrapsMidnightWithinTolerance()
    {
        var actual = new TimeOnly(23, 59, 59, 900);
        var expected = new TimeOnly(00, 00, 00, 100);

        var ex = Record.Exception(() =>
            actual.Should().BeWithin(expected, TimeSpan.FromMilliseconds(250)));

        Assert.Null(ex);
    }

    [Fact]
    public void BeWithin_ThrowsArgumentOutOfRangeException_WhenDateOnlyToleranceIsTimeSpanMinValue()
    {
        var actual = new DateOnly(2026, 03, 03);
        var expected = actual;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            actual.Should().BeWithin(expected, TimeSpan.MinValue));

        Assert.Equal("tolerance", ex.ParamName);
    }

    [Fact]
    public void BeWithin_ThrowsArgumentOutOfRangeException_WhenTimeOnlyToleranceIsTimeSpanMinValue()
    {
        var actual = new TimeOnly(10, 00, 00);
        var expected = actual;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            actual.Should().BeWithin(expected, TimeSpan.MinValue));

        Assert.Equal("tolerance", ex.ParamName);
    }
}
