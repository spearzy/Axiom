using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UndisposedBatchAnalyzer : DiagnosticAnalyzer
{
    internal static readonly DiagnosticDescriptor Rule = new(
        AxiomAnalyzerIds.UndisposedBatch,
        "Dispose Axiom Batch instances",
        "The Axiom Batch created here is not disposed; use 'using' so aggregated failures are flushed",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Axiom Batch must be disposed to flush aggregated failures at the end of the scope.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static startContext =>
        {
            var symbols = BatchSymbols.Create(startContext.Compilation);
            if (!symbols.IsEnabled)
            {
                return;
            }

            startContext.RegisterSyntaxNodeAction(
                syntaxContext => AnalyzeLocalDeclaration(syntaxContext, symbols),
                SyntaxKind.LocalDeclarationStatement);

            startContext.RegisterSyntaxNodeAction(
                syntaxContext => AnalyzeExpressionStatement(syntaxContext, symbols),
                SyntaxKind.ExpressionStatement);
        });
    }

    private static void AnalyzeLocalDeclaration(
        SyntaxNodeAnalysisContext context,
        BatchSymbols symbols)
    {
        var declaration = (LocalDeclarationStatementSyntax)context.Node;
        if (declaration.UsingKeyword.IsKind(SyntaxKind.UsingKeyword))
        {
            return;
        }

        foreach (var variable in declaration.Declaration.Variables)
        {
            if (variable.Initializer is null)
            {
                continue;
            }

            var operation = context.SemanticModel.GetOperation(variable.Initializer.Value, context.CancellationToken);
            if (!symbols.IsBatchCreation(operation))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                Rule,
                variable.Initializer.Value.GetLocation()));
        }
    }

    private static void AnalyzeExpressionStatement(
        SyntaxNodeAnalysisContext context,
        BatchSymbols symbols)
    {
        var statement = (ExpressionStatementSyntax)context.Node;
        var operation = context.SemanticModel.GetOperation(statement.Expression, context.CancellationToken);
        if (!symbols.IsBatchCreation(operation))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            statement.Expression.GetLocation()));
    }

    private sealed class BatchSymbols
    {
        private readonly INamedTypeSymbol? _batchType;
        private readonly INamedTypeSymbol? _assertType;

        private BatchSymbols(
            INamedTypeSymbol? batchType,
            INamedTypeSymbol? assertType)
        {
            _batchType = batchType;
            _assertType = assertType;
        }

        public bool IsEnabled => _batchType is not null && _assertType is not null;

        public static BatchSymbols Create(Compilation compilation)
        {
            return new BatchSymbols(
                compilation.GetTypeByMetadataName("Axiom.Core.Batch"),
                compilation.GetTypeByMetadataName("Axiom.Core.Assert"));
        }

        public bool IsBatchCreation(IOperation? operation)
        {
            switch (operation)
            {
                case IObjectCreationOperation creation:
                    return SymbolEqualityComparer.Default.Equals(creation.Type, _batchType);

                case IInvocationOperation invocation:
                    return invocation.TargetMethod.Name == "Batch" &&
                           SymbolEqualityComparer.Default.Equals(invocation.TargetMethod.ContainingType, _assertType) &&
                           SymbolEqualityComparer.Default.Equals(invocation.TargetMethod.ReturnType, _batchType);

                case IConversionOperation { IsImplicit: true } conversion:
                    return IsBatchCreation(conversion.Operand);

                default:
                    return false;
            }
        }
    }
}
