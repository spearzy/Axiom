using System.Net;
using Axiom.Http;

namespace Axiom.Tests.Http.ProblemDetails;

public sealed class ProblemDetailsAssertionTests
{
    [Fact]
    public void HaveProblemDetails_Passes_ForProblemDetailsResponse()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.BadRequest,
            """
            {
              "type": "https://example.test/problems/validation",
              "title": "Validation failed",
              "status": 400,
              "detail": "Name is required."
            }
            """,
            "application/problem+json");

        var ex = Record.Exception(() =>
        {
            response.Should().HaveProblemDetails();
            response.Should().HaveProblemDetailsTitle("Validation failed");
            response.Should().HaveProblemDetailsStatus(400);
            response.Should().HaveProblemDetailsType("https://example.test/problems/validation");
            response.Should().HaveProblemDetailsDetail("Name is required.");
        });

        Assert.Null(ex);
    }

    [Fact]
    public void HaveProblemDetails_Throws_WhenContentTypeIsNotProblemJson()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.BadRequest,
            "{ \"title\": \"Validation failed\", \"status\": 400 }",
            "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveProblemDetails());

        Assert.Equal(
            "Expected response to have problem details, but found application/json; charset=utf-8.",
            ex.Message);
    }

    [Fact]
    public void HaveProblemDetails_Throws_WhenRequiredMembersAreMissing()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.BadRequest,
            "{ \"status\": 400 }",
            "application/problem+json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveProblemDetails());

        Assert.Equal(
            "Expected response to have problem details, but found missing path $.title.",
            ex.Message);
    }

    [Fact]
    public void HaveProblemDetailsStatus_Throws_WhenStatusDiffers()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.UnprocessableEntity,
            "{ \"title\": \"Validation failed\", \"status\": 422 }",
            "application/problem+json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveProblemDetailsStatus(400));

        Assert.Equal(
            "Expected response problem details body to have JSON number at path $.status equal to 400, but found JSON number 422 at $.status.",
            ex.Message);
    }
}
