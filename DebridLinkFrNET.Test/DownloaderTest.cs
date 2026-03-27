using System.Net;
using Xunit;

namespace DebridLinkFrNET.Test;

public class DownloaderTest
{
    [Fact]
    public async Task Add()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("downloader/add", HttpStatusCode.OK, Setup.Responses.DownloaderGetById);

        var result = await client.Downloader.AddAsync("https://example.com/file");

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal("test-file.zip", result.First().Name);
    }

    [Fact]
    public async Task GetById()
    {
        var (client, handler) = Setup.CreateMockClient();
        // GetByIdAsync iterates over ListAsync to find the file
        handler.AddMockResponse("downloader/list", HttpStatusCode.OK, Setup.Responses.DownloaderAddResult);

        var result = await client.Downloader.GetByIdAsync("hosted123");

        Assert.NotNull(result);
        Assert.Equal("test-file.zip", result!.Name);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("downloader/list", HttpStatusCode.OK, Setup.Responses.EmptyList);

        var result = await client.Downloader.GetByIdAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("downloader/add", HttpStatusCode.OK, Setup.Responses.DownloaderGetById);
        handler.AddMockResponse("remove", HttpStatusCode.NoContent, "");

        var addResult = await client.Downloader.AddAsync("https://example.com/file");
        var ids = string.Join(",", addResult.Select(link => link.Id));

        await client.Downloader.DeleteAsync(ids);
    }
}
