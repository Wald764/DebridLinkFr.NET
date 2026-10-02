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
    public async Task Add_SingleObject_SendsOneRequest()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("downloader/add", HttpStatusCode.OK, Setup.Responses.DownloaderGetById);

        var result = await client.Downloader.AddAsync("https://example.com/file");

        Assert.Single(result);
        Assert.Equal("hosted123", result[0].Id);
        var request = Assert.Single(handler.Requests);
        Assert.Equal(HttpMethod.Post, request.Method);
    }

    [Fact]
    public async Task Add_Array_SendsOneRequest()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("downloader/add", HttpStatusCode.OK, Setup.Responses.DownloaderAddResult);

        var result = await client.Downloader.AddAsync("https://example.com/folder");

        Assert.Single(result);
        Assert.Equal("test-file.zip", result[0].Name);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task Add_ApiError_DoesNotResendRequest()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("downloader/add", HttpStatusCode.OK, @"{""success"":false,""error"":""floodDetected""}");

        var ex = await Assert.ThrowsAsync<DebridLinkFrException>(() => client.Downloader.AddAsync("https://example.com/file"));

        Assert.Equal("floodDetected", ex.ErrorCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task Add_NetworkError_DoesNotResendRequest()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.ExceptionToThrow = new HttpRequestException("connection reset");

        await Assert.ThrowsAsync<HttpRequestException>(() => client.Downloader.AddAsync("https://example.com/file"));

        Assert.Single(handler.Requests);
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
