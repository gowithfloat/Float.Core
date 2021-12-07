using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Float.Core.Exceptions;
using Float.Core.Persistence;

namespace Float.Core.Net
{
    /// <summary>
    /// OAuth strategy.
    /// </summary>
    public class OAuth2Strategy : OAuth2StrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Strategy"/> class.
        /// </summary>
        /// <param name="baseUri">Base URI.</param>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="clientSecret">Client secret.</param>
        /// <param name="secureStore">Secure store.</param>
        /// <param name="loginEndpoint">Login endpoint.</param>
        /// <param name="revokeEndpoint">Revoke/logout endpoint.</param>
        /// <param name="grantType">The grant type.</param>
        public OAuth2Strategy(Uri baseUri, string clientId, string clientSecret, ISecureStore secureStore, string loginEndpoint = DefaultLoginEndpoint, string revokeEndpoint = DefaultRevokeEndpoint, OAuth2GrantType grantType = OAuth2GrantType.Password) : base(baseUri, clientId, clientSecret, secureStore, loginEndpoint, revokeEndpoint, grantType)
        {
        }

        /// <inheritdoc />
        public override async Task<bool> Login(string username = null, string password = null)
        {
            switch (CurrentGrantType)
            {
                case OAuth2GrantType.Password:
                    PutTokens(await GetTokensByLogin(username, password).ConfigureAwait(false));
                    break;
                case OAuth2GrantType.ClientCredentials:
                    PutTokens(await GetTokensByKeys().ConfigureAwait(false));
                    break;
                default:
                    // Other grant types are not handled by this Authorization Class
                    throw new WebException("Request could not be authenticated: wrong authorization Type");
            }

            return await IsAuthenticatedAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Login the specified tid.
        /// </summary>
        /// <returns>The login.</returns>
        /// <param name="tid">The Tid.</param>
        public async Task<bool> Login(string tid)
        {
            if (string.IsNullOrWhiteSpace(tid))
            {
                throw new InvalidStringArgumentException(nameof(tid));
            }

            return PutTokens(await GetTokensByOneTimeToken(tid).ConfigureAwait(false)) && (await IsAuthenticatedAsync().ConfigureAwait(false));
        }

        /// <summary>
        /// Get the Login tokens from server using username and password.
        /// </summary>
        /// <returns>The tokens by login.</returns>
        /// <param name="username">The user's Username.</param>
        /// <param name="password">The user's Password.</param>
        Task<OAuthTokens> GetTokensByLogin(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidStringArgumentException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidStringArgumentException(nameof(password));
            }

            var body = new Dictionary<string, string>
            {
                { UsernameKey, username },
                { PasswordKey, password },
                { GrantKey, GrantPasswordValue },
                { ClientIdKey, ClientId },
                { ClientSecretKey, ClientSecret },
            };

            return GetNewTokens(body);
        }

        /// <summary>
        /// Gets the tokens by one time token.
        /// </summary>
        /// <returns>The tokens by one time token.</returns>
        /// <param name="tid">The One time token value being used as a login..</param>
        Task<OAuthTokens> GetTokensByOneTimeToken(string tid)
        {
            if (string.IsNullOrWhiteSpace(tid))
            {
                throw new InvalidStringArgumentException(nameof(tid));
            }

            var body = new Dictionary<string, string>
            {
                { GrantKey, GrantOneTimeToken },
                { ClientIdKey, ClientId },
                { ClientSecretKey, ClientSecret },
                { TokenIDKey, tid },
            };

            return GetNewTokens(body);
        }
    }
}
