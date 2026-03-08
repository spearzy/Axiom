namespace Axiom.Assertions.AssertionTypes;

internal static class AssertionMessageText
{
    public static string BuildPredicateExpectationText(string prefix, string? predicateExpression)
    {
        if (string.IsNullOrWhiteSpace(predicateExpression))
        {
            return $"{prefix} <predicate>";
        }

        var expression = predicateExpression.Trim();
        if (expression.StartsWith("static ", StringComparison.Ordinal))
        {
            expression = expression["static ".Length..].TrimStart();
        }

        return $"{prefix} `{expression}`";
    }
}
