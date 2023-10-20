using System;
using System.Threading.Tasks;
using Xunit;

namespace DebridLinkFrNET.Test;

public class AccountTest
{
    [Fact]
    public async Task Infos()
    {
        var client = new DebridLinkFrNETClient(Setup.ApiKey);

        var result = await client.Account.Infos();

        Assert.NotNull(result.Username);
    }
}