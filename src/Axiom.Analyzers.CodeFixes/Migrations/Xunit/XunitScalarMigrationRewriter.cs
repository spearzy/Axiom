using Axiom.Analyzers.XunitMigration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class XunitScalarMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(XunitAssertMigrationMatch match)
    {
        return XunitAssertMigrationCodeFixProvider.BuildShouldCall(
            match.SubjectExpression,
            GetMethodName(match.Spec.Kind),
            match.ExpectedExpression,
            match.AdditionalArgumentExpression,
            match.TypeArgumentSyntax);
    }

    private static string GetMethodName(XunitAssertMigrationKind kind)
    {
        return kind switch
        {
            XunitAssertMigrationKind.Be => "Be",
            XunitAssertMigrationKind.NotBe => "NotBe",
            XunitAssertMigrationKind.BeNull => "BeNull",
            XunitAssertMigrationKind.NotBeNull => "NotBeNull",
            XunitAssertMigrationKind.BeTrue => "BeTrue",
            XunitAssertMigrationKind.BeFalse => "BeFalse",
            XunitAssertMigrationKind.BeEmpty => "BeEmpty",
            XunitAssertMigrationKind.NotBeEmpty => "NotBeEmpty",
            XunitAssertMigrationKind.BeSameAs => "BeSameAs",
            XunitAssertMigrationKind.NotBeSameAs => "NotBeSameAs",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }
}
