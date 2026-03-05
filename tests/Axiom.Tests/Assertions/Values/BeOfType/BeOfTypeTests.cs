using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeOfType;

public sealed class BeOfTypeTests
{
    private class Animal;
    private sealed class Dog : Animal;

    [Fact]
    public void BeOfType_DoesNotThrow_WhenValueIsOfCorrectType()
    {
        const int value = 1;

        var ex = Record.Exception(() => value.Should().BeOfType<int>());

        Assert.Null(ex);
    }

    [Fact]
    public void BeOfType_ReturnsContinuation_AndPointsBackToSameAssertions()
    {
        object value = "hello";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeOfType<string>();

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeOfType_Throws_WhenTypesDiffer()
    {
        const int value = 1;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeOfType<double>());

        const string expected = "Expected value to be of type System.Double, but found 1.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeOfType_Throws_WhenValueIsNull()
    {
        object? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeOfType<string>());

        const string expected = "Expected value to be of type System.String, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeOfType_Throws_WithReason_WhenProvided()
    {
        const int value = 1;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeOfType<string>("input should align with seed data."));

        Assert.Contains("because input should align with seed data.", ex.Message);
    }

    [Fact]
    public void BeOfType_Throws_WhenRuntimeTypeIsDerivedButNotExactMatch()
    {
        Animal value = new Dog();

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeOfType<Animal>());

        Assert.Contains("Expected value to be of type", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Axiom.Tests.Assertions.Values.BeOfType.BeOfTypeTests+Animal", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Axiom.Tests.Assertions.Values.BeOfType.BeOfTypeTests+Dog", ex.Message, StringComparison.Ordinal);
    }
}
