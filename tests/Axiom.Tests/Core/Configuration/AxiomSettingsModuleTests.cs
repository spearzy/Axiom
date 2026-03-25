using Axiom.Assertions.Configuration;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Core.Configuration;

public sealed class AxiomSettingsModuleTests : IDisposable
{
    public void Dispose()
    {
        AxiomSettings.Reset();
        AxiomServices.Reset();
        EquivalencyDefaults.Reset();
    }

    [Fact]
    public void UseModule_AppliesSettingsModuleToCoreAndEquivalency()
    {
        var module = new ApiTestSettingsModule(
            TimeSpan.FromMilliseconds(500),
            requireStrictRuntimeTypes: false);

        AxiomSettings.UseModule(module);

        Assert.Equal(TimeSpan.FromMilliseconds(500), AxiomServices.Configuration.RegexMatchTimeout);

        var actual = new ActualShape { Name = "Alice" };
        var expected = new ExpectedShape { Name = "Alice" };

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void UseModules_AppliesModulesInOrder()
    {
        AxiomSettings.UseModules(
            new ApiTestSettingsModule(TimeSpan.FromMilliseconds(500), requireStrictRuntimeTypes: true),
            new ApiTestSettingsModule(TimeSpan.FromMilliseconds(900), requireStrictRuntimeTypes: false));

        Assert.Equal(TimeSpan.FromMilliseconds(900), AxiomServices.Configuration.RegexMatchTimeout);

        var actual = new ActualShape { Name = "Alice" };
        var expected = new ExpectedShape { Name = "Alice" };

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void UseModule_BridgesCoreOnlyModuleThroughAxiomSettings()
    {
        var module = new RegexTimeoutModule(TimeSpan.FromMilliseconds(700));

        AxiomSettings.UseModule(module);

        Assert.Equal(TimeSpan.FromMilliseconds(700), AxiomServices.Configuration.RegexMatchTimeout);
    }

    [Fact]
    public void UseModule_ThrowsArgumentNullException_WhenSettingsModuleIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => AxiomSettings.UseModule((IAxiomSettingsModule)null!));

        Assert.Equal("module", ex.ParamName);
    }

    [Fact]
    public void UseModules_ThrowsArgumentNullException_WhenSettingsModulesArrayIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => AxiomSettings.UseModules((IAxiomSettingsModule[])null!));

        Assert.Equal("modules", ex.ParamName);
    }

    [Fact]
    public void UseModules_ThrowsArgumentNullException_WhenSettingsModulesContainsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            AxiomSettings.UseModules(
                new ApiTestSettingsModule(TimeSpan.FromMilliseconds(500), requireStrictRuntimeTypes: false),
                null!));

        Assert.Equal("modules", ex.ParamName);
    }

    [Fact]
    public void UseModule_ThrowsArgumentNullException_WhenCoreModuleIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => AxiomSettings.UseModule((IAxiomModule)null!));

        Assert.Equal("module", ex.ParamName);
    }

    [Fact]
    public void UseModule_InteroperatesWithExistingConfigurationApis()
    {
        AxiomSettings.UseModule(new ApiTestSettingsModule(TimeSpan.FromMilliseconds(500), requireStrictRuntimeTypes: false));

        AxiomServices.Configure(configuration => configuration.RegexMatchTimeout = TimeSpan.FromMilliseconds(800));
        EquivalencyDefaults.Configure(options => options.RequireStrictRuntimeTypes = true);

        Assert.Equal(TimeSpan.FromMilliseconds(800), AxiomServices.Configuration.RegexMatchTimeout);

        var actual = new ActualShape { Name = "Alice" };
        var expected = new ExpectedShape { Name = "Alice" };

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("runtime types differ", ex.Message, StringComparison.Ordinal);
    }

    private sealed class ApiTestSettingsModule : IAxiomSettingsModule
    {
        private readonly TimeSpan _regexMatchTimeout;
        private readonly bool _requireStrictRuntimeTypes;

        public ApiTestSettingsModule(TimeSpan regexMatchTimeout, bool requireStrictRuntimeTypes)
        {
            _regexMatchTimeout = regexMatchTimeout;
            _requireStrictRuntimeTypes = requireStrictRuntimeTypes;
        }

        public void Configure(AxiomSettingsOptions options)
        {
            options.Core.RegexMatchTimeout = _regexMatchTimeout;
            options.Equivalency.RequireStrictRuntimeTypes = _requireStrictRuntimeTypes;
        }
    }

    private sealed class RegexTimeoutModule : IAxiomModule
    {
        private readonly TimeSpan _regexMatchTimeout;

        public RegexTimeoutModule(TimeSpan regexMatchTimeout)
        {
            _regexMatchTimeout = regexMatchTimeout;
        }

        public void Configure(AxiomConfiguration configuration)
        {
            configuration.RegexMatchTimeout = _regexMatchTimeout;
        }
    }

    private sealed class ActualShape
    {
        public string? Name { get; init; }
    }

    private sealed class ExpectedShape
    {
        public string? Name { get; init; }
    }
}
