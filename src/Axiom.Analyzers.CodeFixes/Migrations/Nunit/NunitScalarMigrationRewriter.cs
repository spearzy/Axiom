using Axiom.Analyzers.NunitMigration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class NunitScalarMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(NunitAssertMigrationMatch match)
    {
        return NunitAssertMigrationCodeFixProvider.BuildShouldCall(
            match.SubjectExpression,
            GetMethodName(match.Spec.Kind),
            match.ExpectedExpression);
    }

    private static string GetMethodName(NunitAssertMigrationKind kind)
    {
        return kind switch
        {
            NunitAssertMigrationKind.Be => "Be",
            NunitAssertMigrationKind.NotBe => "NotBe",
            NunitAssertMigrationKind.BeNull => "BeNull",
            NunitAssertMigrationKind.NotBeNull => "NotBeNull",
            NunitAssertMigrationKind.BeTrue => "BeTrue",
            NunitAssertMigrationKind.BeFalse => "BeFalse",
            NunitAssertMigrationKind.BeEmpty => "BeEmpty",
            NunitAssertMigrationKind.NotBeEmpty => "NotBeEmpty",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }
}
