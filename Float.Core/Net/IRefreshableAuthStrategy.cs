﻿using System.Net.Http;
using System.Threading.Tasks;

namespace Float.Core.Net
{
    /// <summary>
    /// Authentication strategy where the credentials can be automatically refreshed (e.g. OAuth).
    /// </summary>
    public interface IRefreshableAuthStrategy : IAuthStrategy
    {
        /// <summary>
        /// Tries to authenticate request again. This should be called if authentication has failed once.
        /// </summary>
        /// <returns>A HttpRequestMessage with Authentication added to it.</returns>
        /// <param name="request">The Request.</param>
        Task<HttpRequestMessage> RefreshCredentialsAndAuthenticateRequest(HttpRequestMessage request);
    }
}
