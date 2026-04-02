using Axiom.Analyzers.XunitMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class XunitThrowsMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(
        XunitAssertMigrationMatch match,
        SemanticModel semanticModel)
    {
        var actionExpression = CanUseDirectThrowReceiver(match.SubjectExpression, semanticModel)
            ? XunitAssertMigrationCodeFixProvider.PrepareSubjectExpression(match.SubjectExpression)
            : SyntaxFactory.ObjectCreationExpression(
                SyntaxFactory.IdentifierName("Action"),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(match.SubjectExpression.WithoutTrivia()))),
                initializer: null);

        var rewrittenExpression = XunitAssertMigrationCodeFixProvider.BuildShouldCall(
            actionExpression,
            "Throw",
            typeArgumentSyntax: match.TypeArgumentSyntax);

        if (match.ExpectedExpression is not null)
        {
            rewrittenExpression = XunitAssertMigrationCodeFixProvider.BuildInvocation(
                XunitAssertMigrationCodeFixProvider.AppendMemberAccess(rewrittenExpression, "WithParamName"),
                match.ExpectedExpression);
        }

        if (match.AppendThrown)
        {
            rewrittenExpression = XunitAssertMigrationCodeFixProvider.AppendMemberAccess(rewrittenExpression, "Thrown");
        }

        return rewrittenExpression;
    }

    public static string GetCodeFixTitle(XunitAssertMigrationMatch match)
    {
        if (match.AppendThrown)
        {
            return "Convert to '.Should().Throw<TException>().WithParamName(...).Thrown'";
        }

        if (match.ExpectedExpression is not null)
        {
            return "Convert to '.Should().Throw<TException>().WithParamName(...)'";
        }

        return match.Spec.CodeFixTitle;
    }

    public static bool RequiresSystemNamespace(
        XunitAssertMigrationMatch match,
        SemanticModel semanticModel)
    {
        return !CanUseDirectThrowReceiver(match.SubjectExpression, semanticModel);
    }

    private static bool CanUseDirectThrowReceiver(
        ExpressionSyntax subjectExpression,
        SemanticModel semanticModel)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(subjectExpression);
        if (symbolInfo.Symbol is IMethodSymbol ||
            symbolInfo.CandidateSymbols.Any(static symbol => symbol is IMethodSymbol))
        {
            return false;
        }

        var typeInfo = semanticModel.GetTypeInfo(subjectExpression);
        var actionType = semanticModel.Compilation.GetTypeByMetadataName("System.Action");
        if (actionType is null)
        {
            return false;
        }

        return SymbolEqualityComparer.Default.Equals(typeInfo.Type, actionType) ||
               SymbolEqualityComparer.Default.Equals(typeInfo.ConvertedType, actionType);
    }
}
