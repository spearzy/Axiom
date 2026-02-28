namespace Axiom.Core.Output;

public sealed class AssertionOutputOptions
{
    public bool Enabled { get; set; } = true;
    public bool ShowPasses { get; set; } = true;
    public bool UseColors { get; set; } = true;
    public bool IncludeSourceLine { get; set; } = true;

    public AssertionOutputOptions Clone()
    {
        return new AssertionOutputOptions
        {
            Enabled = Enabled,
            ShowPasses = ShowPasses,
            UseColors = UseColors,
            IncludeSourceLine = IncludeSourceLine,
        };
    }
}
