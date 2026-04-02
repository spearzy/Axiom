using Axiom.Analyzers.XunitMigration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class XunitSingleMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(XunitAssertMigrationMatch match)
    {
        var rewrittenExpression = XunitAssertMigrationCodeFixProvider.BuildShouldCall(
            match.SubjectExpression,
            "ContainSingle",
            match.ExpectedExpression,
            match.TypeArgumentSyntax);

        if (match.AppendSingleItem)
        {
            rewrittenExpression = XunitAssertMigrationCodeFixProvider.AppendMemberAccess(rewrittenExpression, "SingleItem");
        }

        return rewrittenExpression;
    }

    public static string GetCodeFixTitle(XunitAssertMigrationMatch match)
    {
        if (match.AppendSingleItem && match.Spec.Kind is XunitAssertMigrationKind.ContainSingle)
        {
            return "Convert to 'subject.Should().ContainSingle().SingleItem'";
        }

        if (match.AppendSingleItem && match.Spec.Kind is XunitAssertMigrationKind.ContainSingleMatching)
        {
            return "Convert to 'subject.Should().ContainSingle(...).SingleItem'";
        }

        return match.Spec.CodeFixTitle;
    }
}
