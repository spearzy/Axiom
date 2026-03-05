using System.Numerics;
using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToLeafTypeTests
{
    [Fact]
    public void GivenHalfValues_WhenNotEquivalent_ThenThrows()
    {
        Half actual = (Half)1.5;
        Half expected = (Half)2.5;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenInt128Values_WhenNotEquivalent_ThenThrows()
    {
        Int128 actual = 123;
        Int128 expected = 456;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenUInt128Values_WhenNotEquivalent_ThenThrows()
    {
        UInt128 actual = 123;
        UInt128 expected = 456;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenBigIntegerValues_WithMatchingPublicFlags_WhenNotEquivalent_ThenThrows()
    {
        var actual = new BigInteger(2);
        var expected = new BigInteger(4);

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenDateOnlyValues_WhenNotEquivalent_ThenDifferenceIsReportedAtRootPath()
    {
        var actual = new DateOnly(2026, 03, 02);
        var expected = new DateOnly(2026, 03, 03);

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("actual -> expected", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenTimeOnlyValues_WhenNotEquivalent_ThenDifferenceIsReportedAtRootPath()
    {
        var actual = new TimeOnly(10, 15, 00);
        var expected = new TimeOnly(10, 15, 01);

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("actual -> expected", ex.Message, StringComparison.Ordinal);
    }
}
