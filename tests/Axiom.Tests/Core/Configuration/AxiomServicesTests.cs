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

        Assert.Same(customFormatter, AxiomServices.Configuration.ValueFormatter);
        Assert.Same(DefaultComparerProvider.Instance, AxiomServices.Configuration.ComparerProvider);
    }

    [Fact]
    public void FailureMessageRenderer_UsesConfiguredFormatter_WhenNoExplicitFormatterProvided()
    {
        AxiomServices.Configure(c => c.ValueFormatter = new ConstantFormatter("X"));

        var failure = new Failure("value", new Expectation("to start with", "ab"), "test");
        var message = FailureMessageRenderer.Render(failure);

        const string expected = "Expected value to start with X, but found X.";
        Assert.Equal(expected, message);
    }

    [Fact]
    public void Configure_PreservesRegexTimeout_WhenUpdatingOtherSettings()
    {
        var timeout = TimeSpan.FromSeconds(2);
        AxiomServices.Configure(c => c.RegexMatchTimeout = timeout);

        AxiomServices.Configure(c => c.ValueFormatter = new ConstantFormatter("fmt"));

        Assert.Equal(timeout, AxiomServices.Configuration.RegexMatchTimeout);
    }

    [Fact]
    public void Configure_PreservesFailureStrategy_WhenUpdatingOtherSettings()
    {
        var strategy = new PassthroughFailureStrategy();
        AxiomServices.Configure(c => c.FailureStrategy = strategy);

        AxiomServices.Configure(c => c.ValueFormatter = new ConstantFormatter("fmt"));

        Assert.Same(strategy, AxiomServices.Configuration.FailureStrategy);
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

    private sealed class PassthroughFailureStrategy : IFailureStrategy
    {
        public void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0)
        {
            throw new InvalidOperationException(message);
        }
    }
}
