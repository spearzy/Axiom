using Axiom.Core.Modules;

namespace Axiom.Core.Configuration;

public static class AxiomServices
{
    private static AxiomConfiguration _configuration = new();

    public static AxiomConfiguration Configuration => _configuration;

    public static void Configure(Action<AxiomConfiguration> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var next = Snapshot();
        configure(next);
        Apply(next);
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

    internal static AxiomConfiguration Snapshot()
    {
        return Clone(_configuration);
    }

    internal static void Apply(AxiomConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = Clone(configuration);
    }

    private static AxiomConfiguration Clone(AxiomConfiguration source)
    {
        return new AxiomConfiguration
        {
            ComparerProvider = source.ComparerProvider,
            ValueFormatter = source.ValueFormatter,
            FailureStrategy = source.FailureStrategy,
            RegexMatchTimeout = source.RegexMatchTimeout,
        };
    }
}
