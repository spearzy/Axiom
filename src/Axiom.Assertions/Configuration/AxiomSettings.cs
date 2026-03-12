using Axiom.Assertions.Equivalency;
using Axiom.Core.Configuration;

namespace Axiom.Assertions.Configuration;

public static class AxiomSettings
{
    public static void Configure(Action<AxiomSettingsOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var core = AxiomServices.Snapshot();
        var equivalency = EquivalencyDefaults.Snapshot();
        var options = new AxiomSettingsOptions(core, equivalency);

        configure(options);

        AxiomServices.Apply(options.Core);
        EquivalencyDefaults.Apply(options.Equivalency);
    }

    public static void Reset()
    {
        AxiomServices.Reset();
        EquivalencyDefaults.Reset();
    }
}
