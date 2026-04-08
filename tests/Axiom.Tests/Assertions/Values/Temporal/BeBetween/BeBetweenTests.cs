using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Temporal.BeBetween;

public sealed class BeBetweenTests
{
    [Fact]
    public void BeBetween_ReturnsContinuation_WhenDateTimeIsInsideInclusiveRange()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);

        var baseAssertions = actual.Should();
        var continuation = baseAssertions.BeBetween(actual, actual.AddMinutes(1));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeBetween_Throws_WhenDateTimeIsOutsideRange()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var lowerBound = actual.AddMinutes(1);
        var upperBound = actual.AddMinutes(2);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeBetween(lowerBound, upperBound));

        Assert.Contains("to be between [03/03/2026 10:01:00, 03/03/2026 10:02:00]", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeBetween_DoesNotThrow_WhenDateTimeOffsetIsInsideRange()
    {
        var actual = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);

        var ex = Record.Exception(() => actual.Should().BeBetween(actual.AddMinutes(-1), actual));

        Assert.Null(ex);
    }

    [Fact]
    public void BeBetween_DoesNotThrow_WhenDateOnlyIsInsideRange()
    {
        var actual = new DateOnly(2026, 03, 03);

        var ex = Record.Exception(() => actual.Should().BeBetween(actual.AddDays(-1), actual));

        Assert.Null(ex);
    }

    [Fact]
    public void BeBetween_Throws_WhenTimeOnlyIsOutsideRange()
    {
        var actual = new TimeOnly(10, 00, 00);
        var lowerBound = actual.Add(TimeSpan.FromMinutes(1));
        var upperBound = actual.Add(TimeSpan.FromMinutes(2));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeBetween(lowerBound, upperBound));

        Assert.Contains("to be between [10:01, 10:02]", ex.Message, StringComparison.Ordinal);
    }
}
