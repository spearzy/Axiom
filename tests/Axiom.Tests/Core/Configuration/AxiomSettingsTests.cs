using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Core.Configuration;

public sealed class AxiomSettingsTests : IDisposable
{
    public void Dispose()
    {
        AxiomSettings.Reset();
    }

    [Fact]
    public void Configure_UpdatesCoreAndEquivalencyInOneCall()
    {
        AxiomSettings.Configure(options =>
        {
            options.Core.RegexMatchTimeout = TimeSpan.FromMilliseconds(500);
            options.Equivalency.RequireStrictRuntimeTypes = false;
        });

        Assert.Equal(TimeSpan.FromMilliseconds(500), AxiomServices.Configuration.RegexMatchTimeout);

        var actual = new ActualShape { Name = "Bob" };
        var expected = new ExpectedShape { Name = "Bob" };

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void Reset_ResetsCoreAndEquivalency()
    {
        AxiomSettings.Configure(options =>
        {
            options.Core.RegexMatchTimeout = TimeSpan.FromMilliseconds(750);
            options.Equivalency.RequireStrictRuntimeTypes = false;
        });

        AxiomSettings.Reset();

        Assert.Equal(TimeSpan.FromMilliseconds(250), AxiomServices.Configuration.RegexMatchTimeout);

        var actual = new ActualShape { Name = "Bob" };
        var expected = new ExpectedShape { Name = "Bob" };

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.Contains("Runtime types differ", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Configure_ThrowsArgumentNullException_WhenConfigureIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => AxiomSettings.Configure(null!));

        Assert.Equal("configure", ex.ParamName);
    }

    [Fact]
    public void Configure_InteroperatesWithExistingConfigurationApis()
    {
        AxiomSettings.Configure(options =>
        {
            options.Core.RegexMatchTimeout = TimeSpan.FromMilliseconds(500);
            options.Equivalency.RequireStrictRuntimeTypes = false;
        });

        AxiomServices.Configure(config => config.RegexMatchTimeout = TimeSpan.FromMilliseconds(900));
        Assert.Equal(TimeSpan.FromMilliseconds(900), AxiomServices.Configuration.RegexMatchTimeout);

        var actual = new ActualShape { Name = "Bob" };
        var expected = new ExpectedShape { Name = "Bob" };
        var structuralEx = Record.Exception(() => actual.Should().BeEquivalentTo(expected));
        Assert.Null(structuralEx);

        EquivalencyDefaults.Configure(options => options.RequireStrictRuntimeTypes = true);
        Assert.Equal(TimeSpan.FromMilliseconds(900), AxiomServices.Configuration.RegexMatchTimeout);

        var strictEx = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.Contains("Runtime types differ", strictEx.Message, StringComparison.Ordinal);
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
