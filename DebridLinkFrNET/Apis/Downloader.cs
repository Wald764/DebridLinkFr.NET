using DebridLinkFrNET.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DebridLinkFrNET.Apis
{
    /// <summary>
    /// Provides methods for interacting with the DebridLink.fr downloader API.
    /// </summary>
    public interface IDownloaderApi
    {
        /// <summary>
        /// Gets a list of hosted files from the downloader.
        /// </summary>
        /// <param name="page">The page number (starts at 0).</param>
        /// <param name="perPage">Number of items per page (minimum: 20, maximum: 50).</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A list of hosted files.</returns>
        Task<List<HostedFile>> ListAsync(int page = 0, int perPage = 50, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a hosted file by its ID.
        /// </summary>
        /// <param name="idLink">The ID of the hosted file to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The hosted file with the specified ID, or null if not found.</returns>
        Task<HostedFile> GetByIdAsync(string idLink, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a hosted file to the downloader.
        /// </summary>
        /// <param name="url">The URL of the file to add.</param>
        /// <param name="password">The optional password for the file.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A list of hosted files, including the newly added file.</returns>
        Task<List<HostedFile>> AddAsync(string url, string? password = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes one or more hosted files from the downloader.
        /// </summary>
        /// <param name="idLinks">The IDs of the hosted files to delete (comma-delimited or JSON).</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        Task DeleteAsync(string idLinks, CancellationToken cancellationToken = default);
    }

   /// <inheritdoc />
    public class DownloaderApi : IDownloaderApi
    {
        private readonly Store _store;
        private readonly Requests _requests;

        public DownloaderApi(HttpClient httpClient, Store store)
        {
            _store = store;
            _requests = new Requests(httpClient, store);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<HostedFile> GetByIdAsync(string idLink, CancellationToken cancellationToken = default)
        {
            const int perPage = 50;
            var page = 0;
            var visitedPages = new HashSet<int>();
            string? previousFirstId = null;

            // Walks downloader/list page by page, following pagination.next so we stop on the last page
            // instead of looping when the API keeps returning items for out-of-range pages.
            while (visitedPages.Add(page))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var parameters = new Dictionary<string, string>
                {
                    { "page", page.ToString(CultureInfo.InvariantCulture) },
                    { "perPage", perPage.ToString(CultureInfo.InvariantCulture) },
                };

                var (files, pagination) = await _requests.GetPagedRequestAsync<List<HostedFile>>("downloader/list", true, parameters, cancellationToken);

                var found = files.FirstOrDefault(hostedFile => hostedFile.Id == idLink);
                if (found != null)
                {
                    return found;
                }

                if (pagination != null)
                {
                    if (pagination.Next <= page)
                    {
                        break;
                    }

                    page = pagination.Next;
                }
                else
                {
                    // No pagination block: a short page, or the same page served again, is the last one.
                    var firstId = files.FirstOrDefault()?.Id;
                    if (files.Count < perPage || firstId == previousFirstId)
                    {
                        break;
                    }

                    previousFirstId = firstId;
                    page++;
                }
            }

            return null!;
        }

        /// <inheritdoc />
        public async Task<List<HostedFile>> AddAsync(string url, string? password = null, CancellationToken cancellationToken = default)
        {
            var data = new[]
            {
                new KeyValuePair<string, string>("url", url),
                new KeyValuePair<string, string>("password", password ?? string.Empty),
            };

            var list = new List<HostedFile>();
            try
            {
                var result = await _requests.PostRequestAsync<HostedFile>("downloader/add", data, true, cancellationToken);
                list.Add(result);
            }
            catch 
            {
                list.AddRange(await _requests.PostRequestAsync<List<HostedFile>>("downloader/add", data, true, cancellationToken));
            }
            
            return list;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string idLinks, CancellationToken cancellationToken = default)
        {
            await _requests.DeleteRequestAsync<List<string>>($"downloader/{idLinks}/remove", true, null, cancellationToken);
        }
    }
}
