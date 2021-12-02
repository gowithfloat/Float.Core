// <copyright file="BasicHTTPAuthStrategy.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Float.Core.Net
{
    /// <summary>
    /// Basic HTTP Auth strategy.
    /// </summary>
    public class BasicHTTPAuthStrategy : IAuthStrategy
    {
        string username;
        string password;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicHTTPAuthStrategy"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public BasicHTTPAuthStrategy(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        /// <inheritdoc />
        public bool IsAuthenticated => true;

        /// <inheritdoc />
        public Task<bool> IsAuthenticatedAsync() => Task.FromResult(IsAuthenticated);

        /// <inheritdoc />
        public Task<HttpRequestMessage> AuthenticateRequest(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
            return Task.FromResult(request);
        }

        /// <inheritdoc />
        public Task<bool> Logout()
        {
            username = null;
            password = null;

            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<bool> Login(string username = null, string password = null)
        {
            this.username = username;
            this.password = password;

            return Task.FromResult(true);
        }
    }
}
