using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Temporal.Batch;

public sealed class TemporalBatchRoutingTests
{
    [Fact]
    public void BeBefore_OutsideBatch_ThrowsImmediately()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMinutes(-1);

        Assert.Throws<InvalidOperationException>(() => actual.Should().BeBefore(expected));
    }

    [Fact]
    public void BeWithin_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddSeconds(2);

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => actual.Should().BeWithin(expected, TimeSpan.FromMilliseconds(500)));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeOnOrBefore_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMinutes(-1);

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => actual.Should().BeOnOrBefore(expected));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeOnOrAfter_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMinutes(1);

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => actual.Should().BeOnOrAfter(expected));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotBeWithin_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.AddMilliseconds(400);

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
            actual.Should().NotBeWithin(expected, TimeSpan.FromMilliseconds(500)));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeBetween_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
            actual.Should().BeBetween(actual.AddMinutes(1), actual.AddMinutes(2)));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromTemporalAssertions()
    {
        var time = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var offset = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("temporal");
            time.Should().BeBefore(time.AddMinutes(-1));
            offset.Should().BeWithin(offset.AddSeconds(2), TimeSpan.FromMilliseconds(500));
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'temporal' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected time to be before 03/03/2026 09:59:00, but found 03/03/2026 10:00:00.", message);
        Assert.Contains("2) Expected offset to be within 00:00:00.5000000 of 03/03/2026 10:00:02 +00:00, but found 03/03/2026 10:00:00 +00:00.", message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromDateOnlyAndTimeOnlyAssertions()
    {
        var date = new DateOnly(2026, 03, 03);
        var time = new TimeOnly(10, 00, 00);

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("temporal-shapes");
            date.Should().BeAfter(date.AddDays(1));
            time.Should().BeWithin(time.Add(TimeSpan.FromSeconds(2)), TimeSpan.FromMilliseconds(500));
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'temporal-shapes' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected date to be after 03/04/2026, but found 03/03/2026.", message);
        Assert.Contains("2) Expected time to be within 00:00:00.5000000 of 10:00, but found 10:00.", message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromNewTemporalAssertions_InCallOrder()
    {
        var time = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);
        var offset = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);
        var date = new DateOnly(2026, 03, 03);
        var clock = new TimeOnly(10, 00, 00);

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("temporal-new");
            time.Should().BeOnOrBefore(time.AddMinutes(-1));
            offset.Should().BeOnOrAfter(offset.AddMinutes(1));
            date.Should().NotBeWithin(date.AddDays(1), TimeSpan.FromDays(2));
            clock.Should().BeBetween(clock.Add(TimeSpan.FromMinutes(1)), clock.Add(TimeSpan.FromMinutes(2)));
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'temporal-new' failed with 4 assertion failure(s):", message);

        var beOnOrBeforeIndex = message.IndexOf(
            "1) Expected time to be on or before 03/03/2026 09:59:00, but found 03/03/2026 10:00:00.",
            StringComparison.Ordinal);
        var beOnOrAfterIndex = message.IndexOf(
            "2) Expected offset to be on or after 03/03/2026 10:01:00 +00:00, but found 03/03/2026 10:00:00 +00:00.",
            StringComparison.Ordinal);
        var notBeWithinIndex = message.IndexOf(
            "3) Expected date not to be within 2.00:00:00 of 03/04/2026, but found 03/03/2026.",
            StringComparison.Ordinal);
        var beBetweenIndex = message.IndexOf(
            "4) Expected clock to be between [10:01, 10:02], but found 10:00.",
            StringComparison.Ordinal);

        Assert.True(beOnOrBeforeIndex >= 0, message);
        Assert.True(beOnOrAfterIndex > beOnOrBeforeIndex, message);
        Assert.True(notBeWithinIndex > beOnOrAfterIndex, message);
        Assert.True(beBetweenIndex > notBeWithinIndex, message);
    }
}
