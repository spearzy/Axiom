using System.Collections.Immutable;
using System.Composition;
using Axiom.Analyzers.XunitMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XunitAssertMigrationCodeFixProvider)), Shared]
public sealed class XunitAssertMigrationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        XunitAssertMigrationSpecs.All.Select(static spec => spec.DiagnosticId).ToImmutableArray();

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        if (root.FindNode(context.Span, getInnermostNodeForTie: true) is not InvocationExpressionSyntax invocationSyntax)
        {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (semanticModel is null ||
            semanticModel.GetOperation(invocationSyntax, context.CancellationToken) is not IInvocationOperation invocationOperation)
        {
            return;
        }

        var symbols = XunitAssertMigrationSymbols.Create(semanticModel.Compilation);
        if (!XunitAssertMigrationMatcher.TryMatch(invocationOperation, symbols, out var match))
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                GetCodeFixTitle(match),
                cancellationToken => ApplyFixAsync(context.Document, match, cancellationToken),
                equivalenceKey: match.Spec.DiagnosticId),
            context.Diagnostics);
    }

    private static async Task<Document> ApplyFixAsync(
        Document document,
        XunitAssertMigrationMatch match,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is not CompilationUnitSyntax compilationUnit)
        {
            return document;
        }

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (semanticModel is null)
        {
            return document;
        }

        var nodeToReplace = GetNodeToReplace(match);
        var replacementExpression = BuildReplacementExpression(match, semanticModel)
            .WithTriviaFrom(nodeToReplace)
            .WithAdditionalAnnotations(Formatter.Annotation);

        var rewrittenRoot = compilationUnit.ReplaceNode(nodeToReplace, replacementExpression);
        rewrittenRoot = AddUsingIfMissing(rewrittenRoot, "Axiom.Assertions");

        if (RequiresSystemNamespace(match, semanticModel))
        {
            rewrittenRoot = AddUsingIfMissing(rewrittenRoot, "System");
        }

        if (match.RequiresAssertionsExtensionsNamespace)
        {
            rewrittenRoot = AddUsingIfMissing(rewrittenRoot, "Axiom.Assertions.Extensions");
        }

        return document.WithSyntaxRoot(rewrittenRoot);
    }

    private static CompilationUnitSyntax AddUsingIfMissing(
        CompilationUnitSyntax compilationUnit,
        string namespaceName)
    {
        foreach (var usingDirective in compilationUnit.Usings)
        {
            if (usingDirective.Alias is null &&
                !usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword) &&
                usingDirective.Name?.ToString() == namespaceName)
            {
                return compilationUnit;
            }
        }

        return compilationUnit.AddUsings(
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName))
                .WithAdditionalAnnotations(Formatter.Annotation));
    }

    private static ExpressionSyntax BuildReplacementExpression(
        XunitAssertMigrationMatch match,
        SemanticModel semanticModel)
    {
        return match.Spec.Kind switch
        {
            XunitAssertMigrationKind.Be or
            XunitAssertMigrationKind.NotBe or
            XunitAssertMigrationKind.BeNull or
            XunitAssertMigrationKind.NotBeNull or
            XunitAssertMigrationKind.BeTrue or
            XunitAssertMigrationKind.BeFalse or
            XunitAssertMigrationKind.BeEmpty or
            XunitAssertMigrationKind.NotBeEmpty or
            XunitAssertMigrationKind.BeSameAs or
            XunitAssertMigrationKind.NotBeSameAs
                => XunitScalarMigrationRewriter.BuildReplacementExpression(match),

            XunitAssertMigrationKind.Contain or
            XunitAssertMigrationKind.NotContain or
            XunitAssertMigrationKind.ContainSubstring or
            XunitAssertMigrationKind.NotContainSubstring or
            XunitAssertMigrationKind.StartWith or
            XunitAssertMigrationKind.EndWith or
            XunitAssertMigrationKind.ContainKey or
            XunitAssertMigrationKind.NotContainKey
                => XunitContainmentMigrationRewriter.BuildReplacementExpression(match),

            XunitAssertMigrationKind.ContainSingle or
            XunitAssertMigrationKind.ContainSingleMatching
                => XunitSingleMigrationRewriter.BuildReplacementExpression(match),

            XunitAssertMigrationKind.Throw
                => XunitThrowsMigrationRewriter.BuildReplacementExpression(match, semanticModel),

            XunitAssertMigrationKind.ThrowExactlyAsync or
            XunitAssertMigrationKind.ThrowAsync
                => XunitAsyncThrowsMigrationRewriter.BuildReplacementExpression(match, semanticModel),

            XunitAssertMigrationKind.BeOfType or
            XunitAssertMigrationKind.BeAssignableTo
                => XunitTypeMigrationRewriter.BuildReplacementExpression(match),

            _ => throw new ArgumentOutOfRangeException(nameof(match)),
        };
    }

    private static string GetCodeFixTitle(XunitAssertMigrationMatch match)
    {
        return match.Spec.Kind switch
        {
            XunitAssertMigrationKind.Contain or
            XunitAssertMigrationKind.NotContain or
            XunitAssertMigrationKind.ContainSubstring or
            XunitAssertMigrationKind.NotContainSubstring or
            XunitAssertMigrationKind.StartWith or
            XunitAssertMigrationKind.EndWith or
            XunitAssertMigrationKind.ContainKey or
            XunitAssertMigrationKind.NotContainKey
                => XunitContainmentMigrationRewriter.GetCodeFixTitle(match),

            XunitAssertMigrationKind.ContainSingle or
            XunitAssertMigrationKind.ContainSingleMatching
                => XunitSingleMigrationRewriter.GetCodeFixTitle(match),

            XunitAssertMigrationKind.Throw
                => XunitThrowsMigrationRewriter.GetCodeFixTitle(match),

            XunitAssertMigrationKind.ThrowExactlyAsync or
            XunitAssertMigrationKind.ThrowAsync
                => XunitAsyncThrowsMigrationRewriter.GetCodeFixTitle(match),

            _ => match.Spec.CodeFixTitle,
        };
    }

    private static bool RequiresSystemNamespace(
        XunitAssertMigrationMatch match,
        SemanticModel semanticModel)
    {
        return match.Spec.Kind switch
        {
            XunitAssertMigrationKind.Throw => XunitThrowsMigrationRewriter.RequiresSystemNamespace(match, semanticModel),
            XunitAssertMigrationKind.ThrowExactlyAsync or XunitAssertMigrationKind.ThrowAsync => XunitAsyncThrowsMigrationRewriter.RequiresSystemNamespace(match, semanticModel),
            _ => false,
        };
    }

    private static SyntaxNode GetNodeToReplace(XunitAssertMigrationMatch match)
        => match.Spec.Kind is XunitAssertMigrationKind.ThrowExactlyAsync or XunitAssertMigrationKind.ThrowAsync
           ? (SyntaxNode?)match.AwaitExpressionSyntax ?? match.InvocationSyntax
           : match.InvocationSyntax;

    internal static ExpressionSyntax BuildShouldCall(
        ExpressionSyntax subjectExpression,
        string methodName,
        ExpressionSyntax? argumentExpression = null,
        TypeSyntax? typeArgumentSyntax = null)
    {
        return BuildShouldCall(subjectExpression, methodName, argumentExpression, additionalArgumentExpression: null, typeArgumentSyntax);
    }

    internal static ExpressionSyntax BuildShouldCall(
        ExpressionSyntax subjectExpression,
        string methodName,
        ExpressionSyntax? argumentExpression,
        ExpressionSyntax? additionalArgumentExpression,
        TypeSyntax? typeArgumentSyntax)
    {
        var shouldInvocation = BuildShouldInvocation(subjectExpression);
        var assertionMethod = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            shouldInvocation,
            BuildMethodName(methodName, typeArgumentSyntax));

        return BuildInvocation(assertionMethod, argumentExpression, additionalArgumentExpression);
    }

    internal static ExpressionSyntax BuildShouldInvocation(ExpressionSyntax subjectExpression)
    {
        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                PrepareSubjectExpression(subjectExpression),
                SyntaxFactory.IdentifierName("Should")),
            SyntaxFactory.ArgumentList());
    }

    internal static SimpleNameSyntax BuildMethodName(
        string methodName,
        TypeSyntax? typeArgumentSyntax = null)
    {
        if (typeArgumentSyntax is null)
        {
            return SyntaxFactory.IdentifierName(methodName);
        }

        return SyntaxFactory.GenericName(
            SyntaxFactory.Identifier(methodName),
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SingletonSeparatedList(typeArgumentSyntax.WithoutTrivia())));
    }

    internal static ExpressionSyntax BuildInvocation(
        ExpressionSyntax targetExpression,
        ExpressionSyntax? argumentExpression = null)
    {
        return BuildInvocation(targetExpression, argumentExpression, additionalArgumentExpression: null);
    }

    internal static ExpressionSyntax BuildInvocation(
        ExpressionSyntax targetExpression,
        ExpressionSyntax? argumentExpression,
        ExpressionSyntax? additionalArgumentExpression)
    {
        if (argumentExpression is null)
        {
            return SyntaxFactory.InvocationExpression(targetExpression, SyntaxFactory.ArgumentList());
        }

        if (additionalArgumentExpression is null)
        {
            return SyntaxFactory.InvocationExpression(
                targetExpression,
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(argumentExpression.WithoutTrivia()))));
        }

        return SyntaxFactory.InvocationExpression(
            targetExpression,
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(
                [
                    SyntaxFactory.Argument(argumentExpression.WithoutTrivia()),
                    SyntaxFactory.Argument(additionalArgumentExpression.WithoutTrivia()),
                ])));
    }

    internal static ExpressionSyntax AppendMemberAccess(
        ExpressionSyntax expression,
        string memberName)
    {
        return SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            expression,
            SyntaxFactory.IdentifierName(memberName));
    }

    internal static ExpressionSyntax PrepareSubjectExpression(ExpressionSyntax subjectExpression)
    {
        var withoutTrivia = subjectExpression.WithoutTrivia();
        if (NeedsParentheses(withoutTrivia))
        {
            return SyntaxFactory.ParenthesizedExpression(withoutTrivia);
        }

        return withoutTrivia;
    }

    private static bool NeedsParentheses(ExpressionSyntax expression)
    {
        return expression switch
        {
            IdentifierNameSyntax => false,
            GenericNameSyntax => false,
            MemberAccessExpressionSyntax => false,
            ElementAccessExpressionSyntax => false,
            InvocationExpressionSyntax => false,
            ObjectCreationExpressionSyntax => false,
            ImplicitObjectCreationExpressionSyntax => false,
            ThisExpressionSyntax => false,
            BaseExpressionSyntax => false,
            LiteralExpressionSyntax => false,
            ParenthesizedExpressionSyntax => false,
            _ => true,
        };
    }
}
