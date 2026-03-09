namespace Axiom.Core.Failures;

internal sealed record FrameworkFailureStrategyDefinition(
    string StrategyName,
    string ExceptionTypeName,
    IReadOnlyList<string> AssemblyNames);
