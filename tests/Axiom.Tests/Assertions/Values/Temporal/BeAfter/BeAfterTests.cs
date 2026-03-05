using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Temporal.BeAfter;

public sealed class BeAfterTests
{
    [Fact]
    public void BeAfter_ReturnsContinuation_WhenDateTimeIsAfterExpected()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMinutes(-1);

        var baseAssertions = actual.Should();
        var continuation = baseAssertions.BeAfter(expected);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeAfter_Throws_WhenDateTimeIsNotAfterExpected()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMinutes(1);

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeAfter(expected));

        const string expectedMessage = "Expected actual to be after 03/03/2026 10:01:00, but found 03/03/2026 10:00:00.";
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public void BeAfter_Throws_WithReason_WhenProvided()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMinutes(1);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeAfter(expected, "events should be ordered chronologically"));

        Assert.Contains("because events should be ordered chronologically", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeAfter_DoesNotThrow_WhenDateTimeOffsetIsAfterExpected()
    {
        var actual = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);
        var expected = actual.AddMinutes(-1);

        var ex = Record.Exception(() => actual.Should().BeAfter(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void BeAfter_DoesNotThrow_WhenDateOnlyIsAfterExpected()
    {
        var actual = new DateOnly(2026, 03, 03);
        var expected = actual.AddDays(-1);

        var ex = Record.Exception(() => actual.Should().BeAfter(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void BeAfter_Throws_WhenDateOnlyIsNotAfterExpected()
    {
        var actual = new DateOnly(2026, 03, 03);
        var expected = actual.AddDays(1);

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeAfter(expected));

        Assert.Contains("to be after", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeAfter_DoesNotThrow_WhenTimeOnlyIsAfterExpected()
    {
        var actual = new TimeOnly(10, 00, 00);
        var expected = actual.Add(-TimeSpan.FromMinutes(1));

        var ex = Record.Exception(() => actual.Should().BeAfter(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void BeAfter_Throws_WhenTimeOnlyIsNotAfterExpected()
    {
        var actual = new TimeOnly(10, 00, 00);
        var expected = actual.Add(TimeSpan.FromMinutes(1));

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeAfter(expected));

        Assert.Contains("to be after", ex.Message, StringComparison.Ordinal);
    }
}
