using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DebridLinkFrNET.Apis
{
    public interface IAccountApi
    {
        /// <summary>
        ///     Get user infos
        /// </summary>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of
        ///     cancellation.
        /// </param>
        /// <returns>
        ///     The informations about the connected user
        /// </returns>
        Task<UserInfos> Infos(CancellationToken cancellationToken = default);
    }

    public class AccountApi : IAccountApi
    {
        private readonly Store _store;
        private readonly Requests _requests;
        internal AccountApi(HttpClient httpClient, Store store)
        {
            _store = store;
            _requests = new Requests(httpClient, store);
        }

        /// <inheritdoc />
        public async Task<UserInfos> Infos(CancellationToken cancellationToken = default)
        {
            return await _requests.GetRequestAsync<UserInfos>("account/infos", true, null, cancellationToken);
        }
    }
}
