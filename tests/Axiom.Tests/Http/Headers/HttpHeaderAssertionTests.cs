using System.Net;
using Axiom.Http;

namespace Axiom.Tests.Http.Headers;

public sealed class HttpHeaderAssertionTests
{
    [Fact]
    public void HaveHeader_Passes_ForResponseHeader()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK);
        response.Headers.Add("ETag", "\"v1\"");

        var ex = Record.Exception(() => response.Should().HaveHeader("ETag"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveHeader_Passes_ForContentHeader()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{}", "application/json");

        var ex = Record.Exception(() => response.Should().HaveHeader("Content-Type"));

        Assert.Null(ex);
    }

    [Fact]
    public void NotHaveHeader_Passes_WhenHeaderIsAbsent()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK);

        var ex = Record.Exception(() => response.Should().NotHaveHeader("Retry-After"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveHeaderValue_Passes_WhenSingleExactValueMatches()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK);
        response.Headers.Add("ETag", "\"v1\"");

        var ex = Record.Exception(() => response.Should().HaveHeaderValue("ETag", "\"v1\""));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveHeaderValues_Passes_WhenExactValueSequenceMatches()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK);
        response.Headers.Add("X-Trace", ["a", "b"]);

        var ex = Record.Exception(() => response.Should().HaveHeaderValues("X-Trace", ["a", "b"]));

        Assert.Null(ex);
    }

    [Fact]
    public void NotHaveHeader_Throws_WhenHeaderIsPresent()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK);
        response.Headers.Add("ETag", "\"v1\"");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().NotHaveHeader("ETag"));

        Assert.Equal(
            "Expected response to not have header ETag, but found header ETag with values [\"\\\"v1\\\"\"].",
            ex.Message);
    }

    [Fact]
    public void HaveHeaderValue_Throws_WhenHeaderHasMultipleValues()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK);
        response.Headers.Add("X-Trace", ["a", "b"]);

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveHeaderValue("X-Trace", "a"));

        Assert.Equal(
            "Expected response to have header X-Trace value \"a\", but found header X-Trace with values [\"a\", \"b\"].",
            ex.Message);
    }
}
