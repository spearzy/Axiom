namespace Axiom;

public static class FailureMessageRenderer
{
    public static string Render(Failure failure, IValueFormatter? formatter = null)
    {
        var valueFormatter = formatter ?? DefaultValueFormatter.Instance;
        var expectation = failure.Expectation;

        if (expectation.IncludeExpectedValue)
        {
            return $"Expected {failure.Subject} {expectation.Description} {valueFormatter.Format(expectation.Expected)}, but found {valueFormatter.Format(failure.Actual)}.";
        }

        return $"Expected {failure.Subject} {expectation.Description}, but found {valueFormatter.Format(failure.Actual)}.";
    }
}
