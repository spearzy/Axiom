using Axiom.Analyzers.MstestMigration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class MstestScalarMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(MstestAssertMigrationMatch match)
    {
        return MstestAssertMigrationCodeFixProvider.BuildShouldCall(
            match.SubjectExpression,
            GetMethodName(match.Spec.Kind),
            match.ExpectedExpression);
    }

    private static string GetMethodName(MstestAssertMigrationKind kind)
    {
        return kind switch
        {
            MstestAssertMigrationKind.Be => "Be",
            MstestAssertMigrationKind.NotBe => "NotBe",
            MstestAssertMigrationKind.BeNull => "BeNull",
            MstestAssertMigrationKind.NotBeNull => "NotBeNull",
            MstestAssertMigrationKind.BeTrue => "BeTrue",
            MstestAssertMigrationKind.BeFalse => "BeFalse",
            MstestAssertMigrationKind.BeSameAs => "BeSameAs",
            MstestAssertMigrationKind.NotBeSameAs => "NotBeSameAs",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }
}
