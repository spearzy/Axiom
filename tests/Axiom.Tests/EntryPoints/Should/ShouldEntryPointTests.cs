using Axiom;

namespace Axiom.Tests;

public sealed class ShouldEntryPointTests
{
    [Fact]
    public void Should_CapturesSubjectAndExpression()
    {
        string? value = "abc";

        var assertions = value.Should();

        Xunit.Assert.Equal("abc", assertions.Subject);
        Xunit.Assert.Equal("value", assertions.SubjectExpression);
    }

    [Fact]
    public void Should_AllowsNullSubject()
    {
        string? value = null;

        var assertions = value.Should();

        Xunit.Assert.Null(assertions.Subject);
        Xunit.Assert.Equal("value", assertions.SubjectExpression);
    }
}
