using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.EntryPoints.Should;

public sealed class ActionShouldEntryPointTests
{
    [Fact]
    public void Should_ForAction_CapturesSubjectAndExpression()
    {
        Action action = static () => { };

        var assertions = action.Should();

        Assert.Same(action, assertions.Subject);
        Assert.Equal("action", assertions.SubjectExpression);
    }
}
