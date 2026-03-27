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
}
