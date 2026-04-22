using System.Net;
using System.Text;
using Axiom.Http;

namespace Axiom.Tests.Http.ContentTypes;

public sealed class HttpContentTypeAssertionTests
{
    [Fact]
    public void HaveContentType_Passes_WhenMediaTypeMatches()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{}", "application/json");

        var ex = Record.Exception(() => response.Should().HaveContentType("application/json"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveContentType_WithCharset_Passes_WhenMediaTypeAndCharsetMatch()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{}", "application/json");

        var ex = Record.Exception(() => response.Should().HaveContentTypeWithCharset("application/json", "utf-8"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveContentType_Throws_WhenMediaTypeDiffers()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{}", "application/problem+json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveContentType("application/json"));

        Assert.Equal(
            "Expected response to have content type application/json, but found application/problem+json; charset=utf-8.",
            ex.Message);
    }

    [Fact]
    public void HaveContentType_WithCharset_Throws_WhenCharsetDiffers()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.Latin1, "application/json")
        };

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveContentTypeWithCharset("application/json", "utf-8"));

        Assert.Equal(
            "Expected response to have content type application/json; charset=utf-8, but found application/json; charset=iso-8859-1.",
            ex.Message);
    }

    [Fact]
    public void HaveContentType_Throws_WhenContentTypeIsMissing()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(Encoding.UTF8.GetBytes("{}"))
        };

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveContentType("application/json"));

        Assert.Equal("Expected response to have content type application/json, but found no content type.", ex.Message);
    }
}
