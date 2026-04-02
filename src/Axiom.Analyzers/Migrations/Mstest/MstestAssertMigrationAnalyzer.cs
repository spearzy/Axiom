using System.Collections.Immutable;
using Axiom.Analyzers.MstestMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MstestAssertMigrationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => MstestAssertMigrationSpecs.SupportedDiagnostics;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static startContext =>
        {
            var symbols = MstestAssertMigrationSymbols.Create(startContext.Compilation);
            if (!symbols.IsEnabled)
            {
                return;
            }

            startContext.RegisterOperationAction(
                operationContext => AnalyzeInvocation(operationContext, symbols),
                OperationKind.Invocation);
        });
    }

    private static void AnalyzeInvocation(
        OperationAnalysisContext context,
        MstestAssertMigrationSymbols symbols)
    {
        var invocation = (IInvocationOperation)context.Operation;
        if (!MstestAssertMigrationMatcher.TryMatch(invocation, symbols, out var match))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(match.Spec.Rule, invocation.Syntax.GetLocation()));
    }
}
