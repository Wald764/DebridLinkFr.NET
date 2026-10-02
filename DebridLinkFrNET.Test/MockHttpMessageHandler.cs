using System.Net;

namespace DebridLinkFrNET.Test;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, MockResponse> _responses = new();
    private readonly System.Collections.Concurrent.ConcurrentQueue<RecordedRequest> _requests = new();

    /// <summary>
    /// Requests received by the handler, in order, with their body already read.
    /// </summary>
    public IReadOnlyList<RecordedRequest> Requests => _requests.ToList();

    /// <summary>
    /// When set, every request throws this exception (simulates a network failure).
    /// </summary>
    public Exception? ExceptionToThrow { get; set; }

    public void AddMockResponse(string urlContains, HttpStatusCode statusCode, string content)
    {
        _responses[urlContains] = new MockResponse(statusCode, content);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var url = request.RequestUri?.ToString() ?? "";
        var body = request.Content != null ? await request.Content.ReadAsStringAsync(cancellationToken) : null;
        _requests.Enqueue(new RecordedRequest(request.Method, url, request.Headers.Authorization?.ToString(), body));

        if (ExceptionToThrow != null)
        {
            throw ExceptionToThrow;
        }

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

    public record RecordedRequest(HttpMethod Method, string Url, string? Authorization, string? Body = null)
    {
        public Uri Uri => new(Url);
    }

    private record MockResponse(HttpStatusCode StatusCode, string Content);
}
