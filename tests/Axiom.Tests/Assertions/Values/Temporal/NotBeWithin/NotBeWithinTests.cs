using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Temporal.NotBeWithin;

public sealed class NotBeWithinTests
{
    [Fact]
    public void NotBeWithin_ReturnsContinuation_WhenDateTimeDifferenceExceedsTolerance()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddSeconds(2);

        var baseAssertions = actual.Should();
        var continuation = baseAssertions.NotBeWithin(expected, TimeSpan.FromMilliseconds(500));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeWithin_Throws_WhenDateTimeDifferenceIsWithinTolerance()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMilliseconds(400);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeWithin(expected, TimeSpan.FromMilliseconds(500)));

        Assert.Contains("not to be within 00:00:00.5000000 of", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotBeWithin_DoesNotThrow_WhenDateTimeOffsetDifferenceExceedsTolerance()
    {
        var actual = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);
        var expected = actual.AddSeconds(2);

        var ex = Record.Exception(() => actual.Should().NotBeWithin(expected, TimeSpan.FromMilliseconds(500)));

        Assert.Null(ex);
    }

    [Fact]
    public void NotBeWithin_DoesNotThrow_WhenDateOnlyDifferenceExceedsTolerance()
    {
        var actual = new DateOnly(2026, 03, 03);
        var expected = actual.AddDays(2);

        var ex = Record.Exception(() => actual.Should().NotBeWithin(expected, TimeSpan.FromDays(1)));

        Assert.Null(ex);
    }

    [Fact]
    public void NotBeWithin_Throws_WhenTimeOnlyWrapsMidnightWithinTolerance()
    {
        var actual = new TimeOnly(23, 59, 59, 900);
        var expected = new TimeOnly(00, 00, 00, 100);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeWithin(expected, TimeSpan.FromMilliseconds(250)));

        Assert.Contains("not to be within 00:00:00.2500000 of", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotBeWithin_ThrowsArgumentOutOfRangeException_WhenToleranceIsTimeSpanMinValue()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            actual.Should().NotBeWithin(actual, TimeSpan.MinValue));

        Assert.Equal("tolerance", ex.ParamName);
    }
}
