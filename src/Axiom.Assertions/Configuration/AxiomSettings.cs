using Axiom.Assertions.Equivalency;
using Axiom.Core.Configuration;
using Axiom.Core.Modules;

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

    public static void UseModule(IAxiomSettingsModule module)
    {
        ArgumentNullException.ThrowIfNull(module);

        Configure(module.Configure);
    }

    public static void UseModules(params IAxiomSettingsModule[] modules)
    {
        ArgumentNullException.ThrowIfNull(modules);

        Configure(options =>
        {
            foreach (var module in modules)
            {
                ArgumentNullException.ThrowIfNull(module, nameof(modules));
                module.Configure(options);
            }
        });
    }

    public static void UseModule(IAxiomModule module)
    {
        ArgumentNullException.ThrowIfNull(module);

        Configure(options => module.Configure(options.Core));
    }
}
