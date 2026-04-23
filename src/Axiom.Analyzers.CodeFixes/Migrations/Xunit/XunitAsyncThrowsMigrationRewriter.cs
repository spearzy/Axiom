using Axiom.Analyzers.XunitMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class XunitAsyncThrowsMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(
        XunitAssertMigrationMatch match,
        SemanticModel semanticModel)
    {
        var actionExpression = CanUseDirectThrowAsyncReceiver(match.SubjectExpression, semanticModel)
            ? XunitAssertMigrationCodeFixProvider.PrepareSubjectExpression(match.SubjectExpression)
            : SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName("Func<System.Threading.Tasks.Task>"),
                SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(match.SubjectExpression.WithoutTrivia()))), initializer: null);

        var rewrittenExpression = XunitAssertMigrationCodeFixProvider.BuildShouldCall(
            actionExpression,
            GetMethodName(match.Spec.Kind),
            typeArgumentSyntax: match.TypeArgumentSyntax);

        var awaitedExpression = SyntaxFactory.AwaitExpression(rewrittenExpression);
        ExpressionSyntax replacementExpression = awaitedExpression;

        if (match.ExpectedExpression is not null)
        {
            replacementExpression = XunitAssertMigrationCodeFixProvider.BuildInvocation(
                XunitAssertMigrationCodeFixProvider.AppendMemberAccess(
                    SyntaxFactory.ParenthesizedExpression(replacementExpression),
                    "WithParamName"),
                match.ExpectedExpression);
        }

        if (match.AppendThrown)
        {
            if (replacementExpression == awaitedExpression)
            {
                replacementExpression = SyntaxFactory.ParenthesizedExpression(replacementExpression);
            }

            replacementExpression = XunitAssertMigrationCodeFixProvider.AppendMemberAccess(replacementExpression, "Thrown");
        }

        return replacementExpression;
    }

    public static string GetCodeFixTitle(XunitAssertMigrationMatch match)
    {
        var methodName = GetMethodName(match.Spec.Kind);
        if (match.AppendThrown)
        {
            return match.ExpectedExpression is not null
                ? $"Convert to '(await ...Should().{methodName}<TException>()).WithParamName(...).Thrown'"
                : $"Convert to '(await ...Should().{methodName}<TException>()).Thrown'";
        }

        return match.ExpectedExpression is not null
            ? $"Convert to '(await ...Should().{methodName}<TException>()).WithParamName(...)'"
            : match.Spec.CodeFixTitle;
    }

    public static bool RequiresSystemNamespace(
        XunitAssertMigrationMatch match,
        SemanticModel semanticModel)
    {
        return !CanUseDirectThrowAsyncReceiver(match.SubjectExpression, semanticModel);
    }

    private static string GetMethodName(XunitAssertMigrationKind kind)
    {
        return kind switch
        {
            XunitAssertMigrationKind.ThrowExactlyAsync => "ThrowExactlyAsync",
            XunitAssertMigrationKind.ThrowAsync => "ThrowAsync",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    private static bool CanUseDirectThrowAsyncReceiver(
        ExpressionSyntax subjectExpression,
        SemanticModel semanticModel)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(subjectExpression);
        if (symbolInfo.Symbol is IMethodSymbol)
        {
            return false;
        }

        foreach (var candidateSymbol in symbolInfo.CandidateSymbols)
        {
            if (candidateSymbol is IMethodSymbol)
            {
                return false;
            }
        }

        var typeInfo = semanticModel.GetTypeInfo(subjectExpression);
        var funcType = semanticModel.Compilation.GetTypeByMetadataName("System.Func`1");
        var taskType = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
        if (funcType is null || taskType is null)
        {
            return false;
        }

        var funcOfTaskType = funcType.Construct(taskType);
        return SymbolEqualityComparer.Default.Equals(typeInfo.Type, funcOfTaskType) ||
               SymbolEqualityComparer.Default.Equals(typeInfo.ConvertedType, funcOfTaskType);
    }
}
