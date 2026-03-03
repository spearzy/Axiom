using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.NotBeAssignableTo;

public sealed class NotBeAssignableToTests
{
    private class Animal;
    private sealed class Dog : Animal;

    [Fact]
    public void NotBeAssignableTo_DoesNotThrow_WhenValueIsNotAssignable()
    {
        object value = 42;

        var ex = Record.Exception(() => value.Should().NotBeAssignableTo<string>());

        Assert.Null(ex);
    }

    [Fact]
    public void NotBeAssignableTo_DoesNotThrow_WhenValueIsNull()
    {
        object? value = null;

        var ex = Record.Exception(() => value.Should().NotBeAssignableTo<string>());

        Assert.Null(ex);
    }

    [Fact]
    public void NotBeAssignableTo_Throws_WhenValueIsExactType()
    {
        object value = "hello";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeAssignableTo<string>());

        const string expected = "Expected value to not be assignable to System.String, but found \"hello\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotBeAssignableTo_Throws_WhenValueIsDerivedType()
    {
        object value = new Dog();

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().NotBeAssignableTo<Animal>());

        Assert.Contains("Expected value to not be assignable to", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Axiom.Tests.Assertions.Values.NotBeAssignableTo.NotBeAssignableToTests+Animal", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Axiom.Tests.Assertions.Values.NotBeAssignableTo.NotBeAssignableToTests+Dog", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotBeAssignableTo_Throws_WithReason_WhenProvided()
    {
        object value = "hello";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().NotBeAssignableTo<object>("this should remain a value-only payload"));

        Assert.Contains("because this should remain a value-only payload", ex.Message, StringComparison.Ordinal);
    }
}
