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
    /// <param name="agent">
    ///     You'll also need to identify your software or script by a meaningful agent parameter (your software user-agent).
    ///     Try to make it explicit, like the name of your software, script or library.
    /// </param>
    /// <param name="apiKey">
    ///     The DebridLinkFr API uses API keys to authenticate requests. You can view and manage your API keys in your Apikey dashboard,
    ///     (https://DebridLinkFr.com/apikeys) or generate them remotely (with user action) through the PIN flow.
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