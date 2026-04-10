using Axiom.Analyzers.XunitMigration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class XunitContainmentMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(XunitAssertMigrationMatch match)
    {
        var rewrittenExpression = XunitAssertMigrationCodeFixProvider.BuildShouldCall(
            match.SubjectExpression,
            GetMethodName(match.Spec.Kind),
            match.ExpectedExpression,
            match.AdditionalArgumentExpression,
            match.TypeArgumentSyntax);

        if (match.AppendWhoseValue)
        {
            rewrittenExpression = XunitAssertMigrationCodeFixProvider.AppendMemberAccess(rewrittenExpression, "WhoseValue");
        }

        return rewrittenExpression;
    }

    public static string GetCodeFixTitle(XunitAssertMigrationMatch match)
    {
        if (match.AppendWhoseValue && match.Spec.Kind is XunitAssertMigrationKind.ContainKey)
        {
            return "Convert to 'dictionary.Should().ContainKey(key).WhoseValue'";
        }

        return match.Spec.CodeFixTitle;
    }

    private static string GetMethodName(XunitAssertMigrationKind kind)
    {
        return kind switch
        {
            XunitAssertMigrationKind.Contain => "Contain",
            XunitAssertMigrationKind.NotContain => "NotContain",
            XunitAssertMigrationKind.ContainSubstring => "Contain",
            XunitAssertMigrationKind.NotContainSubstring => "NotContain",
            XunitAssertMigrationKind.StartWith => "StartWith",
            XunitAssertMigrationKind.EndWith => "EndWith",
            XunitAssertMigrationKind.ContainKey => "ContainKey",
            XunitAssertMigrationKind.NotContainKey => "NotContainKey",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }
}
