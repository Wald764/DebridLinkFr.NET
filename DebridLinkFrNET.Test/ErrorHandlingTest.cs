using System.Net;
using Xunit;

namespace DebridLinkFrNET.Test;

public class ErrorHandlingTest
{
    [Fact]
    public async Task ApiError_ThrowsDebridLinkFrException()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("account/infos", HttpStatusCode.Unauthorized, @"{""success"":false,""error"":""badToken""}");

        var ex = await Assert.ThrowsAsync<DebridLinkFrException>(() => client.Account.Infos());

        Assert.Equal("badToken", ex.ErrorCode);
        Assert.Equal("badToken", ex.ServerError);
        Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
    }

    [Fact]
    public async Task ApiError_WithHttp200_ThrowsDebridLinkFrException()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("account/infos", HttpStatusCode.OK, @"{""success"":false,""error"":""maintenance""}");

        var ex = await Assert.ThrowsAsync<DebridLinkFrException>(() => client.Account.Infos());

        Assert.Equal("maintenance", ex.ErrorCode);
        Assert.Equal(HttpStatusCode.OK, ex.StatusCode);
    }

    [Fact]
    public async Task ApiError_IsStillCatchableAsException()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("account/infos", HttpStatusCode.OK, @"{""success"":false,""error"":""notDebrid""}");

        // Existing callers using catch (Exception) keep working.
        Exception ex = await Assert.ThrowsAnyAsync<Exception>(() => client.Account.Infos());

        Assert.IsType<DebridLinkFrException>(ex);
    }

    [Fact]
    public async Task HtmlErrorPage_ThrowsDebridLinkFrExceptionWithStatusCode()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("account/infos", HttpStatusCode.ServiceUnavailable, "<html><body>Maintenance</body></html>");

        var ex = await Assert.ThrowsAsync<DebridLinkFrException>(() => client.Account.Infos());

        Assert.Equal("httpError", ex.ErrorCode);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, ex.StatusCode);
    }

    [Fact]
    public async Task EmptyErrorResponse_ThrowsDebridLinkFrExceptionWithStatusCode()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("account/infos", HttpStatusCode.BadGateway, "");

        var ex = await Assert.ThrowsAsync<DebridLinkFrException>(() => client.Account.Infos());

        Assert.Equal(HttpStatusCode.BadGateway, ex.StatusCode);
    }

    [Fact]
    public async Task InvalidJsonOnSuccess_ThrowsGenericException()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("account/infos", HttpStatusCode.OK, "not json");

        var ex = await Assert.ThrowsAsync<Exception>(() => client.Account.Infos());

        Assert.StartsWith("Unable to deserialize DebridLinkFr API response", ex.Message);
    }

    [Fact]
    public async Task NoContent_ReturnsEmptyValue()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("seedbox/abc/remove", HttpStatusCode.NoContent, "");

        await client.Seedbox.DeleteAsync("abc");
    }
}
