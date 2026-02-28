using Axiom.Core.Comparison;
using Axiom.Core.Formatting;

namespace Axiom.Core.Configuration;

public sealed class AxiomConfiguration
{
    public IComparerProvider ComparerProvider { get; set; } = DefaultComparerProvider.Instance;
    public IValueFormatter ValueFormatter { get; set; } = DefaultValueFormatter.Instance;
}
