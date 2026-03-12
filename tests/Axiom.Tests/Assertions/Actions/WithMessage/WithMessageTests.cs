using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Actions.WithMessage;

public sealed class WithMessageTests
{
    [Fact]
    public void WithMessage_DoesNotThrow_WhenMessageMatches()
    {
        Action action = static () => throw new InvalidOperationException("boom");

        var ex = Record.Exception(() =>
            action.Should().Throw<InvalidOperationException>().WithMessage("boom"));

        Assert.Null(ex);
    }

    [Fact]
    public void WithMessage_Throws_WhenMessageDiffers()
    {
        Action action = static () => throw new InvalidOperationException("boom");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>().WithMessage("other"));

        const string expected = "Expected action to have exception message \"other\", but found \"boom\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void WithMessage_Throws_WithInheritedReason()
    {
        Action action = static () => throw new InvalidOperationException("boom");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>("operation must fail").WithMessage("other"));

        Assert.Contains("because operation must fail", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void WithMessage_UsesConfiguredComparison()
    {
        Action action = static () => throw new InvalidOperationException("BOOM");

        var ex = Record.Exception(() =>
            action.Should().Throw<InvalidOperationException>().WithMessage("boom", StringComparison.OrdinalIgnoreCase));

        Assert.Null(ex);
    }

    [Fact]
    public void WithMessage_ThrowsArgumentNullException_WhenExpectedMessageIsNull()
    {
        Action action = static () => throw new InvalidOperationException("boom");

        var ex = Assert.Throws<ArgumentNullException>(() =>
            action.Should().Throw<InvalidOperationException>().WithMessage(null!));

        Assert.Equal("expectedMessage", ex.ParamName);
    }

    [Fact]
    public async Task WithMessage_DoesNotThrow_WhenAsyncMessageMatches()
    {
        Func<Task> action = static () => Task.FromException(new InvalidOperationException("boom"));

        var ex = await Record.ExceptionAsync(async () =>
            (await action.Should().ThrowAsync<InvalidOperationException>()).WithMessage("boom"));

        Assert.Null(ex);
    }

    [Fact]
    public async Task WithMessage_Throws_WhenAsyncMessageDiffers()
    {
        Func<Task> action = static () => Task.FromException(new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            (await action.Should().ThrowAsync<InvalidOperationException>()).WithMessage("other"));

        const string expected = "Expected action to have exception message \"other\", but found \"boom\".";
        Assert.Equal(expected, ex.Message);
    }
}
