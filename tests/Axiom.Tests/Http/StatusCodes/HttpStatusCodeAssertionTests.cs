using System.Net;
using Axiom.Http;

namespace Axiom.Tests.Http.StatusCodes;

public sealed class HttpStatusCodeAssertionTests
{
    [Fact]
    public void HaveStatusCode_Passes_ForHttpStatusCode()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK);

        var ex = Record.Exception(() => response.Should().HaveStatusCode(HttpStatusCode.OK));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveStatusCode_Passes_ForInt()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.Created);

        var ex = Record.Exception(() => response.Should().HaveStatusCode(201));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveStatusCode_Throws_WhenActualStatusDiffers()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.NotFound);

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveStatusCode(HttpStatusCode.OK));

        Assert.Equal(
            "Expected response to have status code 200 (OK), but found 404 (NotFound).",
            ex.Message);
    }

    [Fact]
    public void NotHaveStatusCode_Throws_WhenActualStatusMatches()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.BadRequest);

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().NotHaveStatusCode(400));

        Assert.Equal(
            "Expected response to not have status code 400 (BadRequest), but found 400 (BadRequest).",
            ex.Message);
    }

    [Fact]
    public void HaveStatusCode_Throws_WhenSubjectIsNull()
    {
        HttpResponseMessage? response = null;

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveStatusCode(HttpStatusCode.OK));

        Assert.Equal("Expected response to have status code 200 (OK), but found <null>.", ex.Message);
    }
}
