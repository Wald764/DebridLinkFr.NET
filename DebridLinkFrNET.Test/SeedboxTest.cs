using System.Net;
using Xunit;

namespace DebridLinkFrNET.Test;

public class SeedboxTest
{
    [Fact]
    public async Task List()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/list", HttpStatusCode.OK, Setup.Responses.TorrentList);

        var result = await client.Seedbox.ListAsync();

        Assert.NotEmpty(result);
        Assert.Equal("dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c", result.First().HashString);
    }

    [Fact]
    public async Task ListWithGivenId()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/list", HttpStatusCode.OK, Setup.Responses.TorrentList);

        var result = await client.Seedbox.ListAsync("53263ca8ae0eea13260");

        Assert.NotEmpty(result);
        Assert.Equal("dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c", result.First().HashString);
    }

    [Fact]
    public async Task AddTorrentFile()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/add", HttpStatusCode.OK, Setup.Responses.TorrentSingle);

        var file = await File.ReadAllBytesAsync("big-buck-bunny.torrent");

        var result = await client.Seedbox.AddTorrentByFileAsync(file);

        Assert.Equal("dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c", result.HashString);
    }

    [Fact]
    public async Task AddTorrent()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/add", HttpStatusCode.OK, Setup.Responses.TorrentSingle);

        const string url = "magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c";

        var result = await client.Seedbox.AddTorrentAsync(url);

        Assert.Equal("dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c", result.HashString);
        Assert.NotEmpty(result.Files);
        Assert.NotNull(result.Files.First().DownloadUrl);
    }

    [Fact]
    public async Task Cached()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/cached", HttpStatusCode.OK, Setup.Responses.CachedResult);

        const string hash = "dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c";

        var result = await client.Seedbox.CachedAsync(hash);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Start()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/add", HttpStatusCode.OK, Setup.Responses.TorrentSingle);
        handler.AddMockResponse("config", HttpStatusCode.OK, Setup.Responses.StartResult);

        var newTorrent = await client.Seedbox.AddTorrentAsync("magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c", true);

        var unwanted = new List<string> { newTorrent.Files.First().Id };

        var result = await client.Seedbox.StartAsync(newTorrent.Id, unwanted.ToArray());

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task Delete()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/add", HttpStatusCode.OK, Setup.Responses.TorrentSingle);
        handler.AddMockResponse("remove", HttpStatusCode.NoContent, "");

        var addResult = await client.Seedbox.AddTorrentAsync("magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c");

        await client.Seedbox.DeleteAsync(addResult.Id);
    }

    [Fact]
    public async Task List_WithoutArguments_SendsNoQueryParameters()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/list", HttpStatusCode.OK, Setup.Responses.TorrentList);

        await client.Seedbox.ListAsync();

        var request = Assert.Single(handler.Requests);
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Equal("/api/v2/seedbox/list", request.Uri.AbsolutePath);
        Assert.Equal("", request.Uri.Query);
    }

    [Fact]
    public async Task List_WithIdsOnly_SendsOnlyIds()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/list", HttpStatusCode.OK, Setup.Responses.TorrentList);

        await client.Seedbox.ListAsync("abc,def");

        var request = Assert.Single(handler.Requests);
        Assert.Equal("?ids=abc%2cdef", request.Uri.Query);
    }

    [Fact]
    public async Task List_WithPagination_SendsPageAndPerPage()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/list", HttpStatusCode.OK, Setup.Responses.TorrentList);

        await client.Seedbox.ListAsync(page: 0, perPage: 20);

        var request = Assert.Single(handler.Requests);
        Assert.Equal("?page=0&perPage=20", request.Uri.Query);
    }

    [Theory]
    [InlineData(false, false, "wait=false&async=false")]
    [InlineData(true, false, "wait=true&async=false")]
    [InlineData(false, true, "wait=false&async=true")]
    public async Task AddTorrent_SendsLowercaseBooleans(bool wait, bool async, string expected)
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/add", HttpStatusCode.OK, Setup.Responses.TorrentSingle);

        await client.Seedbox.AddTorrentAsync("magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c", wait, async);

        var request = Assert.Single(handler.Requests);
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.EndsWith(expected, request.Body);
    }
}
