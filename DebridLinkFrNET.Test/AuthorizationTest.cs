using System.Net;
using Xunit;

namespace DebridLinkFrNET.Test;

public class AuthorizationTest
{
    [Fact]
    public async Task Authorization_IsSentPerRequest()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("account/infos", HttpStatusCode.OK, Setup.Responses.AccountInfos);

        await client.Account.Infos();

        var request = Assert.Single(handler.Requests);
        Assert.Equal($"Bearer {Setup.FakeApiKey}", request.Authorization);
    }

    [Fact]
    public async Task Authorization_DoesNotTouchSharedHttpClientDefaultHeaders()
    {
        var handler = new MockHttpMessageHandler();
        handler.AddMockResponse("account/infos", HttpStatusCode.OK, Setup.Responses.AccountInfos);
        using var httpClient = new HttpClient(handler);
        httpClient.DefaultRequestHeaders.Add("X-Caller", "kept");
        var client = new DebridLinkFrNETClient(Setup.FakeApiKey, httpClient);

        await client.Account.Infos();

        Assert.False(httpClient.DefaultRequestHeaders.Contains("Authorization"));
        Assert.True(httpClient.DefaultRequestHeaders.Contains("X-Caller"));
    }

    [Fact]
    public async Task Authorization_ConcurrentClientsSharingHttpClient_UseTheirOwnKey()
    {
        var handler = new MockHttpMessageHandler();
        handler.AddMockResponse("account/infos", HttpStatusCode.OK, Setup.Responses.AccountInfos);
        using var httpClient = new HttpClient(handler);
        var clientA = new DebridLinkFrNETClient("key-a", httpClient);
        var clientB = new DebridLinkFrNETClient("key-b", httpClient);

        var tasks = Enumerable.Range(0, 100)
                              .Select(i => i % 2 == 0 ? clientA.Account.Infos() : clientB.Account.Infos())
                              .ToList();
        await Task.WhenAll(tasks);

        var requests = handler.Requests;
        Assert.Equal(100, requests.Count);
        Assert.Equal(50, requests.Count(r => r.Authorization == "Bearer key-a"));
        Assert.Equal(50, requests.Count(r => r.Authorization == "Bearer key-b"));
    }
}
