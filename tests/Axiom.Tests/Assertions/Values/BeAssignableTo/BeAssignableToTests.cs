using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.BeAssignableTo;

public sealed class BeAssignableToTests
{
    private class Animal;
    private sealed class Dog : Animal;

    [Fact]
    public void BeAssignableTo_DoesNotThrow_WhenValueIsExactType()
    {
        object value = "hello";

        var ex = Record.Exception(() => value.Should().BeAssignableTo<string>());

        Assert.Null(ex);
    }

    [Fact]
    public void BeAssignableTo_DoesNotThrow_WhenValueIsDerivedType()
    {
        object value = new Dog();

        var ex = Record.Exception(() => value.Should().BeAssignableTo<Animal>());

        Assert.Null(ex);
    }

    [Fact]
    public void BeAssignableTo_Throws_WhenValueIsNotAssignable()
    {
        object value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeAssignableTo<string>());

        const string expected = "Expected value to be assignable to System.String, but found 42.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeAssignableTo_Throws_WhenValueIsNull()
    {
        object? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeAssignableTo<string>());

        const string expected = "Expected value to be assignable to System.String, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeAssignableTo_Throws_WithReason_WhenProvided()
    {
        object value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeAssignableTo<string>("value should come from the text-based source"));

        Assert.Contains("because value should come from the text-based source", ex.Message, StringComparison.Ordinal);
    }
}
