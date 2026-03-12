using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.EntryPoints.Should;

public sealed class AsyncShouldEntryPointTests
{
    [Fact]
    public void Should_ForTaskFunc_CapturesExpression()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var assertions = action.Should();

        Assert.NotNull(assertions.Subject);
        Assert.Equal("action", assertions.SubjectExpression);
    }

    [Fact]
    public void Should_ForValueTaskFunc_CapturesExpression()
    {
        Func<ValueTask> action = static () => ValueTask.CompletedTask;

        var assertions = action.Should();

        Assert.Same(action, assertions.Subject);
        Assert.Equal("action", assertions.SubjectExpression);
    }
}
