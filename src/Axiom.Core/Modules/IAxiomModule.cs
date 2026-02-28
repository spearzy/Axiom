using Axiom.Core.Configuration;

namespace Axiom.Core.Modules;

public interface IAxiomModule
{
    void Configure(AxiomConfiguration configuration);
}
