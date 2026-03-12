namespace Axiom.Assertions.Equivalency;

public static class EquivalencyDefaults
{
    private static readonly object Sync = new();
    private static EquivalencyOptions _options = new();

    public static void Configure(Action<EquivalencyOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        lock (Sync)
        {
            var next = _options.Clone();
            configure(next);
            _options = next;
        }
    }

    public static void Reset()
    {
        lock (Sync)
        {
            _options = new EquivalencyOptions();
        }
    }

    internal static EquivalencyOptions Snapshot()
    {
        lock (Sync)
        {
            return _options.Clone();
        }
    }

    internal static void Apply(EquivalencyOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        lock (Sync)
        {
            _options = options.Clone();
        }
    }
}
