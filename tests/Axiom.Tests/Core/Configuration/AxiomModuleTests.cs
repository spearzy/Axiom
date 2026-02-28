namespace Axiom.Tests.Core.Configuration;

public sealed class AxiomModuleTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void UseModule_AppliesModuleConfiguration()
    {
        var formatter = new ConstantFormatter("module");
        var module = new TestModule(formatter);

        AxiomServices.UseModule(module);

        Xunit.Assert.Same(formatter, AxiomServices.Configuration.ValueFormatter);
        Xunit.Assert.Same(DefaultComparerProvider.Instance, AxiomServices.Configuration.ComparerProvider);
    }

    [Fact]
    public void UseModule_ThrowsForNullModule()
    {
        var ex = Xunit.Assert.Throws<ArgumentNullException>(() => AxiomServices.UseModule(null!));

        Xunit.Assert.Equal("module", ex.ParamName);
    }

    private sealed class TestModule : IAxiomModule
    {
        private readonly IValueFormatter _formatter;

        public TestModule(IValueFormatter formatter)
        {
            _formatter = formatter;
        }

        public void Configure(AxiomConfiguration configuration)
        {
            configuration.ValueFormatter = _formatter;
        }
    }

    private sealed class ConstantFormatter : IValueFormatter
    {
        private readonly string _value;

        public ConstantFormatter(string value)
        {
            _value = value;
        }

        public string Format(object? value)
        {
            return _value;
        }
    }
}
