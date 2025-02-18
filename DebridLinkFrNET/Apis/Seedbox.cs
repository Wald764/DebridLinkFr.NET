using DebridLinkFrNET.Models;
using System.Threading;

namespace DebridLinkFrNET.Apis
{
    /// <summary>
    /// Provides methods for interacting with the DebridLink.fr seedbox API.
    /// </summary>
    public interface ISeedboxApi
    {
        /// <summary>
        /// Retrieves a list of torrents from the seedbox.
        /// </summary>
        /// <param name="ids">Comma-delimited or JSON torrent IDs (maximum: 50).</param>
        /// <param name="page">The page number (starts at 0).</param>
        /// <param name="perPage">Number of items per page (minimum: 20, maximum: 50).</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A list of torrents from the seedbox.</returns>
        Task<List<Torrent>> ListAsync(string? ids = null, int page = -1, int perPage = -1, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a torrent to the seedbox by uploading a torrent file.
        /// </summary>
        /// <param name="file">The torrent file as a byte array.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The added torrent.</returns>
        Task<Torrent> AddTorrentByFileAsync(byte[] file, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a torrent to the seedbox by URL.
        /// </summary>
        /// <param name="url">The URL of the torrent to add.</param>
        /// <param name="wait">A value indicating whether to wait for the torrent to be ready for download.</param>
        /// <param name="async">A value indicating whether to add the torrent asynchronously.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The added torrent.</returns>
        Task<Torrent> AddTorrentAsync(string url, bool wait = false, bool async = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Only return torrent(s) that are fully downloaded and ready.
        /// </summary>
        /// <param name="url">The Torrent URL (urlencoded), Magnet (urlencoded) or hash list (comma-delimited or json). (200 max.) for which to retrieve cached torrents.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A dictionary of cached torrents keyed by the given identifier (Torrent URL (urlencoded), Magnet (urlencoded) or hash list (comma-delimited or json). (200 max.)).</returns>
        Task<Dictionary<string, Torrent>> CachedAsync(string url, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts the configuration of a torrent on the seedbox.
        /// </summary>
        /// <param name="idTorrent">The ID of the torrent to configure.</param>
        /// <param name="unwantedFileIds">An array of file IDs to mark as unwanted.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A list of file IDs marked as unwanted.</returns>
        Task<List<string>> StartAsync(string idTorrent, string[] unwantedFileIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes one or more hosted torrents from the seedbox.
        /// </summary>
        /// <param name="idTorrents">The IDs of the torrents to delete (comma-delimited or JSON).</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        Task DeleteAsync(string idTorrents, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc />
    public class SeedboxApi : ISeedboxApi
    {
        private readonly Store _store;
        private readonly Requests _requests;

        public SeedboxApi(HttpClient httpClient, Store store)
        {
            _store = store;
            _requests = new Requests(httpClient, store);
        }

        /// <inheritdoc />
        public async Task<List<Torrent>> ListAsync(string? ids = null, int page = -1, int perPage = -1, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>
            {
                { "ids", ids ?? "" },
                { "page", page.ToString() },
                { "perPage", perPage.ToString() },
            };

            var response = await _requests.GetRequestAsync<List<Torrent>>("seedbox/list", true, parameters, cancellationToken);

            return response ?? new List<Torrent>();
        }

        /// <inheritdoc />
        public async Task<Torrent> AddTorrentByFileAsync(byte[] file, CancellationToken cancellationToken = default)
        {
            var result = await _requests.PostFileRequestAsync<Torrent>("seedbox/add", file, true, cancellationToken);

            return result;
        }

        /// <inheritdoc />
        public async Task<Torrent> AddTorrentAsync(string url, bool wait = false, bool async = false, CancellationToken cancellationToken = default)
        {
            var data = new[]
            {
                new KeyValuePair<string, string>("url", url),
                new KeyValuePair<string, string>("wait", wait.ToString()),
                new KeyValuePair<string, string>("async", async.ToString())
            };

            var result = await _requests.PostRequestAsync<Torrent>("seedbox/add", data, true, cancellationToken);

            return result;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, Torrent>> CachedAsync(string url, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("url", url);

            var response = await _requests.GetRequestAsync<Dictionary<string, Torrent>>("seedbox/cached", true, parameters, cancellationToken);

            return response;
        }

        /// <inheritdoc />
        public async Task<List<string>> StartAsync(string idTorrent, string[] unwantedFileIds, CancellationToken cancellationToken = default)
        {
            var files = string.Join(",", unwantedFileIds);

            var data = new[]
            {
                new KeyValuePair<string, string>("files-unwanted", files)
            };

            var response = await _requests.PostRequestAsync<List<string>>($"seedbox/{idTorrent}/config", data, true, cancellationToken);

            return response;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string idTorrents, CancellationToken cancellationToken = default)
        {
            await _requests.DeleteRequestAsync<List<string>>($"seedbox/{idTorrents}/remove", true, null, cancellationToken);
        }
    }
}
