using System.Net;
using System.Text.Json;
using Axiom.Http;

namespace Axiom.Tests.Http.Json;

public sealed class HttpJsonBodyAssertionTests
{
    [Fact]
    public void HaveJsonBodyEquivalentTo_Passes_WhenBodyIsEquivalent()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            """
            { "id": 1, "name": "Ada", "roles": ["admin", "author"] }
            """,
            "application/json");

        const string expected = """
            { "roles": ["admin", "author"], "name": "Ada", "id": 1.0 }
            """;

        var ex = Record.Exception(() => response.Should().HaveJsonBodyEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonPath_AndScalarAssertions_Pass_WhenBodyMatches()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            """
            { "user": { "name": "Ada", "active": true, "score": 1e0, "deletedAt": null } }
            """,
            "application/json");

        var ex = Record.Exception(() =>
        {
            response.Should().HaveJsonPath("$.user.name");
            response.Should().HaveJsonStringAtPath("$.user.name", "Ada");
            response.Should().HaveJsonBooleanAtPath("$.user.active", true);
            response.Should().HaveJsonNumberAtPath("$.user.score", 1m);
            response.Should().HaveJsonNullAtPath("$.user.deletedAt");
        });

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonBodyEquivalentTo_Throws_WithJsonMismatchDetail()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"id\": 2 }", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonBodyEquivalentTo("{ \"id\": 1 }"));

        Assert.Equal(
            "Expected response JSON body to be JSON equivalent to {\"id\":1}, but found JSON value mismatch at $.id: expected 1 but found 2.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonPath_Throws_WhenResponseHasNoContent()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK);

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonPath("$.id"));

        Assert.Equal("Expected response to have JSON path $.id, but found no response content.", ex.Message);
    }

    [Fact]
    public void HaveJsonBodyEquivalentTo_Passes_ForJsonDocumentExpectedInput()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"id\": 1 }", "application/json");
        using var expected = JsonDocument.Parse("{ \"id\": 1.0 }");

        var ex = Record.Exception(() => response.Should().HaveJsonBodyEquivalentTo(expected));

        Assert.Null(ex);
    }
}
