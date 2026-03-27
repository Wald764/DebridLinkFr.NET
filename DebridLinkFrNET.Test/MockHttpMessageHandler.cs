using System.Net;

namespace DebridLinkFrNET.Test;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, MockResponse> _responses = new();

    public void AddMockResponse(string urlContains, HttpStatusCode statusCode, string content)
    {
        _responses[urlContains] = new MockResponse(statusCode, content);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var url = request.RequestUri?.ToString() ?? "";

        foreach (var (key, mock) in _responses)
        {
            if (url.Contains(key))
            {
                var response = new HttpResponseMessage(mock.StatusCode)
                {
                    Content = new StringContent(mock.Content, System.Text.Encoding.UTF8, "application/json")
                };
                return Task.FromResult(response);
            }
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{\"success\":false,\"error\":\"not_found\"}", System.Text.Encoding.UTF8, "application/json")
        });
    }

    private record MockResponse(HttpStatusCode StatusCode, string Content);
}
