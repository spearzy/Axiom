namespace Axiom.Core.Failures;

internal static class FailureStrategyMessages
{
    internal const string NonThrowingStrategyGuard =
        "Failure strategy returned without throwing.";

    internal const string MissingFrameworkTypeTemplate =
        "Cannot use {0}: framework exception type '{1}' was not found. Ensure the corresponding test framework package is referenced.";

    internal const string MissingStringConstructorTemplate =
        "Cannot use {0}: framework exception type '{1}' must expose a constructor that accepts a message string.";
}
