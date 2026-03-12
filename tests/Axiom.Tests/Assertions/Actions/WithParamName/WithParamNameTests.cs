using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Actions.WithParamName;

public sealed class WithParamNameTests
{
    [Fact]
    public void WithParamName_DoesNotThrow_WhenParamNameMatches()
    {
        Action action = static () => throw new ArgumentNullException("value");

        var ex = Record.Exception(() =>
            action.Should().Throw<ArgumentException>().WithParamName("value"));

        Assert.Null(ex);
    }

    [Fact]
    public void WithParamName_Throws_WhenParamNameDiffers()
    {
        Action action = static () => throw new ArgumentNullException("value");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<ArgumentException>().WithParamName("other"));

        const string expected = "Expected action to have parameter name \"other\", but found \"value\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void WithParamName_Throws_WhenThrownExceptionIsNotArgumentException()
    {
        Action action = static () => throw new InvalidOperationException("boom");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>().WithParamName("value"));

        Assert.Equal(
            $"Expected action to have parameter name \"value\", but found {typeof(InvalidOperationException)}.",
            ex.Message);
    }

    [Fact]
    public void WithParamName_ThrowsArgumentNullException_WhenExpectedParamNameIsNull()
    {
        Action action = static () => throw new ArgumentNullException("value");

        var ex = Assert.Throws<ArgumentNullException>(() =>
            action.Should().Throw<ArgumentException>().WithParamName(null!));

        Assert.Equal("expectedParamName", ex.ParamName);
    }

    [Fact]
    public async Task WithParamName_DoesNotThrow_WhenAsyncParamNameMatches()
    {
        Func<ValueTask> action = static () => ValueTask.FromException(new ArgumentNullException("value"));

        var ex = await Record.ExceptionAsync(async () =>
            (await action.Should().ThrowAsync<ArgumentException>()).WithParamName("value"));

        Assert.Null(ex);
    }

    [Fact]
    public async Task WithParamName_Throws_WhenAsyncParamNameDiffers()
    {
        Func<ValueTask> action = static () => ValueTask.FromException(new ArgumentNullException("value"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            (await action.Should().ThrowAsync<ArgumentException>()).WithParamName("other"));

        const string expected = "Expected action to have parameter name \"other\", but found \"value\".";
        Assert.Equal(expected, ex.Message);
    }
}
