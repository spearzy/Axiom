using System.Collections.Immutable;
using Axiom.Analyzers.NunitMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NunitAssertMigrationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => NunitAssertMigrationSpecs.SupportedDiagnostics;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static startContext =>
        {
            var symbols = NunitAssertMigrationSymbols.Create(startContext.Compilation);
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
        NunitAssertMigrationSymbols symbols)
    {
        var invocation = (IInvocationOperation)context.Operation;
        if (!NunitAssertMigrationMatcher.TryMatch(invocation, symbols, out var match))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(match.Spec.Rule, invocation.Syntax.GetLocation()));
    }
}
