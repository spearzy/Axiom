namespace Axiom.Core.Failures;

public readonly record struct Expectation(string Description, object? Expected = null, bool IncludeExpectedValue = true);
