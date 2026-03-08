using Axiom.Core.Comparison;
using Axiom.Core.Failures;
using Axiom.Core.Formatting;

namespace Axiom.Core.Configuration;

public sealed class AxiomConfiguration
{
    private TimeSpan _regexMatchTimeout = TimeSpan.FromMilliseconds(250);

    public IComparerProvider ComparerProvider { get; set; } = DefaultComparerProvider.Instance;
    public IValueFormatter ValueFormatter { get; set; } = DefaultValueFormatter.Instance;
    public IFailureStrategy FailureStrategy { get; set; } = InvalidOperationFailureStrategy.Instance;

    public TimeSpan RegexMatchTimeout
    {
        get => _regexMatchTimeout;
        set
        {
            // Keep a finite positive timeout so assertion regex checks cannot hang a run.
            if (value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(RegexMatchTimeout), "RegexMatchTimeout must be greater than zero.");
            }

            _regexMatchTimeout = value;
        }
    }
}
