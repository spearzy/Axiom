using Axiom.Analyzers.XunitMigration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class XunitTypeMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(XunitAssertMigrationMatch match)
    {
        return XunitAssertMigrationCodeFixProvider.BuildShouldCall(
            match.SubjectExpression,
            GetMethodName(match.Spec.Kind),
            typeArgumentSyntax: match.TypeArgumentSyntax);
    }

    private static string GetMethodName(XunitAssertMigrationKind kind)
    {
        return kind switch
        {
            XunitAssertMigrationKind.BeOfType => "BeOfType",
            XunitAssertMigrationKind.BeAssignableTo => "BeAssignableTo",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }
}
