using Axiom.Core.Modules;

namespace Axiom.Core.Configuration;

public static class AxiomServices
{
    private static AxiomConfiguration _configuration = new();

    public static AxiomConfiguration Configuration => _configuration;

    public static void Configure(Action<AxiomConfiguration> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var next = new AxiomConfiguration
        {
            ComparerProvider = _configuration.ComparerProvider,
            ValueFormatter = _configuration.ValueFormatter,
            Output = _configuration.Output.Clone(),
        };

        configure(next);
        _configuration = next;
    }

    public static void Reset()
    {
        _configuration = new AxiomConfiguration();
    }

    public static void UseModule(IAxiomModule module)
    {
        ArgumentNullException.ThrowIfNull(module);
        Configure(module.Configure);
    }
}
