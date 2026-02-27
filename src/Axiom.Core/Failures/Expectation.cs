namespace Axiom;

public readonly record struct Expectation(string Description, object? Expected = null, bool IncludeExpectedValue = true);
