namespace Axiom.Tests.Core.Configuration;

public sealed class AxiomServicesTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void Configure_UpdatesOnlySpecifiedSetting()
    {
        var customFormatter = new ConstantFormatter("fmt");

        AxiomServices.Configure(c => c.ValueFormatter = customFormatter);
        AxiomServices.Configure(c => c.ComparerProvider = DefaultComparerProvider.Instance);

        Xunit.Assert.Same(customFormatter, AxiomServices.Configuration.ValueFormatter);
        Xunit.Assert.Same(DefaultComparerProvider.Instance, AxiomServices.Configuration.ComparerProvider);
    }

    [Fact]
    public void FailureMessageRenderer_UsesConfiguredFormatter_WhenNoExplicitFormatterProvided()
    {
        AxiomServices.Configure(c => c.ValueFormatter = new ConstantFormatter("X"));

        var failure = new Failure("value", new Expectation("to start with", "ab"), "test");
        var message = FailureMessageRenderer.Render(failure);

        const string expected = "Expected value to start with X, but found X.";
        Xunit.Assert.Equal(expected, message);
    }

    [Fact]
    public void Configure_ClonesOutputOptions_InsteadOfReusingReference()
    {
        AxiomServices.Configure(c =>
        {
            c.Output.Enabled = true;
            c.Output.ShowPasses = true;
        });

        var before = AxiomServices.Configuration.Output;
        AxiomServices.Configure(c => c.ValueFormatter = new ConstantFormatter("fmt"));

        var after = AxiomServices.Configuration.Output;
        Xunit.Assert.NotSame(before, after);
        Xunit.Assert.True(after.Enabled);
        Xunit.Assert.True(after.ShowPasses);
    }

    private sealed class ConstantFormatter : IValueFormatter
    {
        private readonly string _text;

        public ConstantFormatter(string text)
        {
            _text = text;
        }

        public string Format(object? value) => _text;
    }
}
