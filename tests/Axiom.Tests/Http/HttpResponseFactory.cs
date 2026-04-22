using System.Net;
using System.Text;

namespace Axiom.Tests.Http;

internal static class HttpResponseFactory
{
    public static HttpResponseMessage Create(
        HttpStatusCode statusCode,
        string? body = null,
        string? mediaType = null)
    {
        var response = new HttpResponseMessage(statusCode);
        if (body is not null)
        {
            response.Content = mediaType is null
                ? new StringContent(body, Encoding.UTF8)
                : new StringContent(body, Encoding.UTF8, mediaType);
        }

        return response;
    }
}
