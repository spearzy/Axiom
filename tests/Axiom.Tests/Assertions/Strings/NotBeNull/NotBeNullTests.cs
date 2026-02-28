namespace Axiom.Tests.Assertions.Strings.NotBeNull;

public sealed class NotBeNullTests
{
    [Fact]
    public void NotBeNull_ReturnsContinuation_WhenValueNotNull()
    {
        string? value = "test";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.NotBeNull();

        Xunit.Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotBeNull_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotBeNull());

        Xunit.Assert.Contains("value", ex.Message);
        Xunit.Assert.Contains("not be null", ex.Message);
        Xunit.Assert.Contains("<null>", ex.Message);
    }
}
