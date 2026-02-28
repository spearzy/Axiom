using Axiom.Core.Comparison;
using Axiom.Core.Formatting;
using Axiom.Core.Output;

namespace Axiom.Core.Configuration;

public sealed class AxiomConfiguration
{
    public IComparerProvider ComparerProvider { get; set; } = DefaultComparerProvider.Instance;
    public IValueFormatter ValueFormatter { get; set; } = DefaultValueFormatter.Instance;
    public AssertionOutputOptions Output { get; set; } = new();
}
