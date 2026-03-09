namespace Axiom.Core.Failures;

internal static class FrameworkFailureStrategyDefinitions
{
    internal static FrameworkFailureStrategyDefinition Xunit { get; } = new(
        StrategyName: nameof(XunitFailureStrategy),
        ExceptionTypeName: "Xunit.Sdk.XunitException",
        AssemblyNames: ["xunit.assert", "xunit.v3.assert"]);

    internal static FrameworkFailureStrategyDefinition NUnit { get; } = new(
        StrategyName: nameof(NUnitFailureStrategy),
        ExceptionTypeName: "NUnit.Framework.AssertionException",
        AssemblyNames: ["nunit.framework"]);

    internal static FrameworkFailureStrategyDefinition MSTest { get; } = new(
        StrategyName: nameof(MSTestFailureStrategy),
        ExceptionTypeName: "Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException",
        AssemblyNames:
        [
            "Microsoft.VisualStudio.TestPlatform.TestFramework",
            "MSTest.TestFramework"
        ]);
}
