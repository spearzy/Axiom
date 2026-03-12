using Axiom.Assertions.Equivalency;
using Axiom.Core.Configuration;

namespace Axiom.Assertions.Configuration;

public sealed class AxiomSettingsOptions
{
    internal AxiomSettingsOptions(AxiomConfiguration core, EquivalencyOptions equivalency)
    {
        Core = core;
        Equivalency = equivalency;
    }

    public AxiomConfiguration Core { get; }
    public EquivalencyOptions Equivalency { get; }
}
