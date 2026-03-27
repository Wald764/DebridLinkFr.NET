using System.Net;
using Xunit;

namespace DebridLinkFrNET.Test;

public class AccountTest
{
    [Fact]
    public async Task Infos()
    {
        var (client, handler) = Setup.CreateMockClient();
        handler.AddMockResponse("account/infos", HttpStatusCode.OK, Setup.Responses.AccountInfos);

        var result = await client.Account.Infos();

        Assert.NotNull(result.Username);
        Assert.Equal("TestUser", result.Username);
        Assert.Equal("test@example.com", result.Email);
        Assert.True(result.EmailVerified);
    }
}
