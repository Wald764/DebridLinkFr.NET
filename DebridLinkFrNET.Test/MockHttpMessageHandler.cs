using System.Net;

namespace DebridLinkFrNET.Test;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, MockResponse> _responses = new();

    /// <summary>
    /// Requests received by the handler, in order, with their body already read.
    /// </summary>
    public List<RecordedRequest> Requests { get; } = new();

    public void AddMockResponse(string urlContains, HttpStatusCode statusCode, string content)
    {
        _responses[urlContains] = new MockResponse(statusCode, content);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var url = request.RequestUri?.ToString() ?? "";
        var body = request.Content != null ? await request.Content.ReadAsStringAsync(cancellationToken) : null;
        Requests.Add(new RecordedRequest(request.Method, request.RequestUri!, body));

        foreach (var (key, mock) in _responses)
        {
            if (url.Contains(key))
            {
                var response = new HttpResponseMessage(mock.StatusCode)
                {
                    Content = new StringContent(mock.Content, System.Text.Encoding.UTF8, "application/json")
                };
                return response;
            }
        }

        return new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{\"success\":false,\"error\":\"not_found\"}", System.Text.Encoding.UTF8, "application/json")
        };
    }

    public record RecordedRequest(HttpMethod Method, Uri Uri, string? Body);

    private record MockResponse(HttpStatusCode StatusCode, string Content);
}
