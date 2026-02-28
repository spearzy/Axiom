namespace Axiom.Core.Failures;

public readonly record struct Failure(string Subject, Expectation Expectation, object? Actual);
