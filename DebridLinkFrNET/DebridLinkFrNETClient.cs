using DebridLinkFrNET.Apis;

namespace DebridLinkFrNET;

public interface IDebridLinkFrNETClient
{
    IAccountApi Account { get; }
    ISeedboxApi Seedbox { get; }
    IDownloaderApi Downloader { get; }
}

/// <summary>
///     The DebridLinkFrNET consumed the DebridLinkFr.com API.
///     Documentation about the API can be found here: https://docs.DebridLinkFr.com/
/// </summary>
public class DebridLinkFrNETClient : IDebridLinkFrNETClient
{
    private readonly Store _store = new();

    public IAccountApi Account { get; }
    public ISeedboxApi Seedbox { get; }
    public IDownloaderApi Downloader { get; }

    /// <summary>
    ///     Initialize the DebridLinkFrNET API.
    ///     To use authentication provide the key for your user.
    /// </summary>
    /// <param name="apiKey">
    ///     The Debrid-Link API key used to authenticate requests. It can be generated from your Debrid-Link account.
    /// </param>
    /// <param name="httpClient">
    ///     Optional HttpClient if you want to use your own HttpClient.
    /// </param>
    public DebridLinkFrNETClient(String apiKey, HttpClient? httpClient = null)
    {
        var client = httpClient ?? new HttpClient();

        _store.ApiKey = apiKey;

        Account = new AccountApi(client, _store);
        Seedbox = new SeedboxApi(client, _store);
        Downloader = new DownloaderApi(client, _store);
    }
}