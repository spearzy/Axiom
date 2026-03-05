using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Temporal.Chaining;

public sealed class TemporalChainingTests
{
    [Fact]
    public void DateTimeChain_CanBeComposed()
    {
        var value = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);

        value.Should()
            .BeAfter(value.AddMinutes(-1)).And
            .BeBefore(value.AddMinutes(1)).And
            .BeWithin(value.AddMilliseconds(400), TimeSpan.FromMilliseconds(500));
    }

    [Fact]
    public void DateTimeOffsetChain_CanBeComposed()
    {
        var value = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);

        value.Should()
            .BeAfter(value.AddMinutes(-1)).And
            .BeBefore(value.AddMinutes(1)).And
            .BeWithin(value.AddMilliseconds(400), TimeSpan.FromMilliseconds(500));
    }

    [Fact]
    public void DateOnlyChain_CanBeComposed()
    {
        var value = new DateOnly(2026, 03, 03);

        value.Should()
            .BeAfter(value.AddDays(-1)).And
            .BeBefore(value.AddDays(1)).And
            .BeWithin(value.AddDays(1), TimeSpan.FromDays(1));
    }

    [Fact]
    public void TimeOnlyChain_CanBeComposed()
    {
        var value = new TimeOnly(10, 00, 00);

        value.Should()
            .BeAfter(value.Add(-TimeSpan.FromMinutes(1))).And
            .BeBefore(value.Add(TimeSpan.FromMinutes(1))).And
            .BeWithin(value.Add(TimeSpan.FromMilliseconds(400)), TimeSpan.FromMilliseconds(500));
    }
}
