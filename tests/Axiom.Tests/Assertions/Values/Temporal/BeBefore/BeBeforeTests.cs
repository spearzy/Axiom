using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Temporal.BeBefore;

public sealed class BeBeforeTests
{
    [Fact]
    public void BeBefore_ReturnsContinuation_WhenDateTimeIsBeforeExpected()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMinutes(1);

        var baseAssertions = actual.Should();
        var continuation = baseAssertions.BeBefore(expected);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeBefore_Throws_WhenDateTimeIsNotBeforeExpected()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMinutes(-1);

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeBefore(expected));

        const string expectedMessage = "Expected actual to be before 03/03/2026 09:59:00, but found 03/03/2026 10:00:00.";
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public void BeBefore_Throws_WithReason_WhenProvided()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMinutes(-1);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeBefore(expected, "events should be ordered chronologically"));

        Assert.Contains("because events should be ordered chronologically", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeBefore_DoesNotThrow_WhenDateTimeOffsetIsBeforeExpected()
    {
        var actual = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);
        var expected = actual.AddMinutes(1);

        var ex = Record.Exception(() => actual.Should().BeBefore(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void BeBefore_DoesNotThrow_WhenDateOnlyIsBeforeExpected()
    {
        var actual = new DateOnly(2026, 03, 03);
        var expected = actual.AddDays(1);

        var ex = Record.Exception(() => actual.Should().BeBefore(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void BeBefore_Throws_WhenDateOnlyIsNotBeforeExpected()
    {
        var actual = new DateOnly(2026, 03, 03);
        var expected = actual.AddDays(-1);

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeBefore(expected));

        Assert.Contains("to be before", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeBefore_DoesNotThrow_WhenTimeOnlyIsBeforeExpected()
    {
        var actual = new TimeOnly(10, 00, 00);
        var expected = actual.Add(TimeSpan.FromMinutes(1));

        var ex = Record.Exception(() => actual.Should().BeBefore(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void BeBefore_Throws_WhenTimeOnlyIsNotBeforeExpected()
    {
        var actual = new TimeOnly(10, 00, 00);
        var expected = actual.Add(-TimeSpan.FromMinutes(1));

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeBefore(expected));

        Assert.Contains("to be before", ex.Message, StringComparison.Ordinal);
    }
}
