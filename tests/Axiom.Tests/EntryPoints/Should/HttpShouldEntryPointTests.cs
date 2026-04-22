using System.Net;
using Axiom.Http;

namespace Axiom.Tests.EntryPoints.Should;

public sealed class HttpShouldEntryPointTests
{
    [Fact]
    public void HttpResponseMessageShould_ReturnsHttpResponseAssertions()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);

        var assertions = response.Should();

        Assert.IsType<HttpResponseAssertions>(assertions);
    }
}
