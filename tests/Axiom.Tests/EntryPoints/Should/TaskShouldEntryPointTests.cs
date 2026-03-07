using Axiom.Assertions;

namespace Axiom.Tests.EntryPoints.Should;

public sealed class TaskShouldEntryPointTests
{
    [Fact]
    public void Should_ForTask_CapturesExpression()
    {
        Task work = Task.CompletedTask;

        var assertions = work.Should();

        Assert.NotNull(assertions.Subject);
        Assert.Equal("work", assertions.SubjectExpression);
    }

    [Fact]
    public void Should_ForTaskOfT_CapturesExpression()
    {
        Task<int> work = Task.FromResult(42);

        var assertions = work.Should();

        Assert.NotNull(assertions.Subject);
        Assert.Equal("work", assertions.SubjectExpression);
    }

    [Fact]
    public void Should_ForValueTask_CapturesExpression()
    {
        ValueTask work = ValueTask.CompletedTask;

        var assertions = work.Should();

        Assert.NotNull(assertions.Subject);
        Assert.Equal("work", assertions.SubjectExpression);
    }

    [Fact]
    public void Should_ForValueTaskOfT_CapturesExpression()
    {
        ValueTask<int> work = ValueTask.FromResult(42);

        var assertions = work.Should();

        Assert.NotNull(assertions.Subject);
        Assert.Equal("work", assertions.SubjectExpression);
    }
}
