using DebridLinkFrNET.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebridLinkFrNET.Apis
{
    /// <summary>
    /// Provides methods for interacting with the DebridLink.fr downloader API.
    /// </summary>
    public class DownloaderApi
    {
        private readonly Store _store;
        private readonly Requests _requests;

        public DownloaderApi(HttpClient httpClient, Store store)
        {
            _store = store;
            _requests = new Requests(httpClient, store);
        }

        /// <summary>
        /// Gets a list of hosted files from the downloader.
        /// </summary>
        /// <param name="page">The page number (starts at 0).</param>
        /// <param name="perPage">Number of items per page (minimum: 20, maximum: 50).</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A list of hosted files.</returns>
        public async Task<List<HostedFile>> ListAsync(int page = 0, int perPage = 50, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>
            {
                { "page", page.ToString() },
                { "perPage", perPage.ToString() },
            };

            var response = await _requests.GetRequestAsync<List<HostedFile>>("downloader/list", true, parameters, cancellationToken);

            return response ?? new List<HostedFile>();
        }

        /// <summary>
        /// Gets a hosted file by its ID.
        /// </summary>
        /// <param name="idLink">The ID of the hosted file to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The hosted file with the specified ID, or null if not found.</returns>
        public async Task<HostedFile> GetByIdAsync(string idLink, CancellationToken cancellationToken = default)
        {
            List<HostedFile> response = new List<HostedFile>();
            HostedFile? foundHostedFile = null;
            int page = 0; 

            do
            {
                response = await ListAsync(page);
                foundHostedFile = response.FirstOrDefault(hostedFile => hostedFile.Id == idLink);
                page++;
            } while (response.Count > 0 && foundHostedFile == null);

            return foundHostedFile;
        }

        /// <summary>
        /// Adds a hosted file to the downloader.
        /// </summary>
        /// <param name="url">The URL of the file to add.</param>
        /// <param name="password">The optional password for the file.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A list of hosted files, including the newly added file.</returns>
        public async Task<List<HostedFile>> AddAsync(string url, string? password = null, CancellationToken cancellationToken = default)
        {
            var data = new[]
            {
                new KeyValuePair<string, string>("url", url),
                new KeyValuePair<string, string>("password", password ?? string.Empty),
            };

            var result = await _requests.PostRequestAsync<List<HostedFile>>("downloader/add", data, true, cancellationToken);

            return result;
        }

        /// <summary>
        /// Deletes one or more hosted files from the downloader.
        /// </summary>
        /// <param name="idLinks">The IDs of the hosted files to delete (comma-delimited or JSON).</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        public async Task DeleteAsync(string idLinks, CancellationToken cancellationToken = default)
        {
            await _requests.DeleteRequestAsync<List<string>>($"downloader/{idLinks}/remove", true, null, cancellationToken);
        }
    }
}
