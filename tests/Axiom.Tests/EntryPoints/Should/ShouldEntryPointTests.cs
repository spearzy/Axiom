
using Axiom.Assertions;
using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.EntryPoints.Should;

public sealed class ShouldEntryPointTests
{
    [Fact]
    public void Should_CapturesSubjectAndExpression()
    {
        string? value = "abc";

        var assertions = value.Should();

        Assert.Equal("abc", assertions.Subject);
        Assert.Equal("value", assertions.SubjectExpression);
    }

    [Fact]
    public void Should_AllowsNullSubject()
    {
        string? value = null;

        var assertions = value.Should();

        Assert.Null(assertions.Subject);
        Assert.Equal("value", assertions.SubjectExpression);
    }
}
