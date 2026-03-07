using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Actions.WithInnerException;

public sealed class WithInnerExceptionTests
{
    [Fact]
    public void WithInnerException_DoesNotThrow_WhenInnerExceptionMatches()
    {
        Action action = static () => throw new InvalidOperationException(
            "outer",
            new ArgumentException("inner"));

        var ex = Record.Exception(() =>
            action.Should().Throw<InvalidOperationException>().WithInnerException<ArgumentException>());

        Assert.Null(ex);
    }

    [Fact]
    public void WithInnerException_Throws_WhenInnerExceptionTypeDiffers()
    {
        Action action = static () => throw new InvalidOperationException(
            "outer",
            new ArgumentException("inner"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>().WithInnerException<ArgumentNullException>());

        Assert.Equal(
            $"Expected action to have inner exception {typeof(ArgumentNullException)}, but found {typeof(ArgumentException)}.",
            ex.Message);
    }

    [Fact]
    public void WithInnerException_Throws_WhenNoInnerExceptionExists()
    {
        Action action = static () => throw new InvalidOperationException("outer");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>().WithInnerException<ArgumentException>());

        Assert.Equal(
            $"Expected action to have inner exception {typeof(ArgumentException)}, but found <no inner exception>.",
            ex.Message);
    }

    [Fact]
    public async Task WithInnerException_DoesNotThrow_WhenAsyncInnerExceptionMatches()
    {
        Func<Task> action = static () => Task.FromException(
            new InvalidOperationException("outer", new ArgumentException("inner")));

        var ex = await Record.ExceptionAsync(async () =>
            (await action.Should().ThrowAsync<InvalidOperationException>()).WithInnerException<ArgumentException>());

        Assert.Null(ex);
    }

    [Fact]
    public async Task WithInnerException_Throws_WhenAsyncInnerExceptionTypeDiffers()
    {
        Func<Task> action = static () => Task.FromException(
            new InvalidOperationException("outer", new ArgumentException("inner")));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            (await action.Should().ThrowAsync<InvalidOperationException>()).WithInnerException<ArgumentNullException>());

        Assert.Equal(
            $"Expected action to have inner exception {typeof(ArgumentNullException)}, but found {typeof(ArgumentException)}.",
            ex.Message);
    }
}
