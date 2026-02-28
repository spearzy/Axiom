namespace Axiom.Tests.EntryPoints.Should;

public sealed class GenericShouldEntryPointTests
{
    [Fact]
    public void Should_ForInt_CapturesSubjectAndExpression()
    {
        var value = 42;

        var assertions = value.Should();

        Xunit.Assert.Equal(42, assertions.Subject);
        Xunit.Assert.Equal("value", assertions.SubjectExpression);
    }

    [Fact]
    public void Should_ForBool_CapturesSubjectAndExpression()
    {
        var value = true;

        var assertions = value.Should();

        Xunit.Assert.True(assertions.Subject);
        Xunit.Assert.Equal("value", assertions.SubjectExpression);
    }
}
