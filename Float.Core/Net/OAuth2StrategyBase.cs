using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Float.Core.Exceptions;
using Float.Core.Extensions;
using Float.Core.Persistence;
using Float.Core.Resources;
using Float.Core.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Float.Core.Net
{
    /// <summary>
    /// This class contains shared logic for various OAuth2 Strategies.
    /// </summary>
    public abstract class OAuth2StrategyBase : IRefreshableAuthStrategy
    {
        internal const string TokenKey = "tokens";
        internal const string GrantKey = "grant_type";
        internal const string RefreshTokenKey = "refresh_token";
        internal const string AccessTokenKey = "access_token";
        internal const string ExpiresInKey = "expires_in";
        internal const string ClientIdKey = "client_id";
        internal const string ClientSecretKey = "client_secret";
        internal const string KeyToken = "token";
        internal const string KeyTokenType = "token_type_hint";
        internal const string UsernameKey = "username";
        internal const string PasswordKey = "password";
        internal const string RedirectURIKey = "redirect_uri";
        internal const string AuthorizationCodeKey = "code";
        internal const string GrantRefreshValue = "refresh_token";
        internal const string GrantPasswordValue = "password";
        internal const string GrantCredentialsValue = "client_credentials";
        internal const string GrantAuthorizationCodeValue = "authorization_code";
        internal const string AuthHeaderKey = "Authorization";
        internal const string GrantOneTimeToken = "one_time_token";
        internal const string TokenIDKey = "tid";
        internal const string DefaultLoginEndpoint = "oauth/token";
        internal const string DefaultRevokeEndpoint = "oauth/revoke";

        // container used to make sure multiple refreshes don't happen at once
        readonly RepeatableTaskContainer refreshTokensContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2StrategyBase"/> class.
        /// </summary>
        /// <param name="baseUri">Base URI.</param>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="clientSecret">Client secret.</param>
        /// <param name="secureStore">Secure store.</param>
        /// <param name="loginEndpoint">Login endpoint.</param>
        /// <param name="revokeEndpoint">Revoke/Logout endpoint.</param>
        /// <param name="grantType">The Grant Type of this AuthStrategy.</param>
        internal OAuth2StrategyBase(Uri baseUri, string clientId, string clientSecret, ISecureStore secureStore, string loginEndpoint = DefaultLoginEndpoint, string revokeEndpoint = DefaultRevokeEndpoint, OAuth2GrantType grantType = OAuth2GrantType.Password)
        {
            this.BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            this.ClientId = clientId.NullIfWhiteSpace() ?? throw new InvalidStringArgumentException(nameof(clientId));
            this.ClientSecret = clientSecret.NullIfWhiteSpace() ?? throw new InvalidStringArgumentException(nameof(clientId));
            this.SecureStore = secureStore ?? throw new ArgumentNullException(nameof(secureStore));
            this.LoginEndpoint = loginEndpoint.NullIfWhiteSpace() ?? throw new InvalidStringArgumentException(nameof(clientId));
            this.RevokeEndpoint = revokeEndpoint.NullIfWhiteSpace() ?? throw new InvalidStringArgumentException(nameof(clientId));
            this.CurrentGrantType = grantType;
            this.RequestClient = new RequestClient(baseUri, null, null);

            refreshTokensContainer = new RepeatableTaskContainer(async () =>
            {
                switch (CurrentGrantType)
                {
                    case OAuth2GrantType.ClientCredentials:
                        PutTokens(await GetTokensByKeys().ConfigureAwait(false));
                        break;
                    default:
                        PutTokens(await GetTokensByRefresh().ConfigureAwait(false));
                        break;
                }
            });
        }

        /// <inheritdoc />
        public bool IsAuthenticated => GetTokens()?.Result?.AccessToken != null;

        /// <summary>
        /// Gets the secure store.
        /// </summary>
        /// <value>The secure store.</value>
        protected ISecureStore SecureStore { get; }

        /// <summary>
        /// Gets the base URI.
        /// </summary>
        /// <value>The base URI.</value>
        protected Uri BaseUri { get; }

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        protected string ClientId { get; }

        /// <summary>
        /// Gets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        protected string ClientSecret { get; }

        /// <summary>
        /// Gets the request client.
        /// </summary>
        /// <value>The request client.</value>
        protected RequestClient RequestClient { get; }

        /// <summary>
        /// Gets the login endpoint.
        /// </summary>
        /// <value>The login endpoint.</value>
        protected string LoginEndpoint { get; }

        /// <summary>
        /// Gets the revoke endpoint.
        /// </summary>
        /// <value>The revoke endpoint.</value>
        protected string RevokeEndpoint { get; }

        /// <summary>
        /// Gets the current OAuth grant type.
        /// </summary>
        /// <value>The current OAuth grant type.</value>
        protected OAuth2GrantType CurrentGrantType { get; }

        /// <inheritdoc />
        public async Task<bool> IsAuthenticatedAsync() => (await GetTokens().ConfigureAwait(false))?.AccessToken != null;

        /// <inheritdoc />
        public abstract Task<bool> Login(string username = null, string password = null);

        /// <summary>
        /// Revokes the access and refresh tokens.
        /// </summary>
        /// <returns>Whether the tokens were revoked.</returns>
        /// <param name="tokens">The tokens to revoke.</param>
        public async Task<bool> RevokeTokens(OAuthTokens tokens)
        {
            if (tokens == null)
            {
                return false;
            }

            var response = await Revoke(RefreshTokenKey, tokens.RefreshToken, tokens.AccessToken).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        /// <inheritdoc />
        public async Task<HttpRequestMessage> AuthenticateRequest(HttpRequestMessage request)
        {
            var accessToken = await GetAccessToken().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new WebException(FloatStrings.UnableToGetTokenErrorMessage);
            }

            if (request?.Headers?.Contains(AuthHeaderKey) == true)
            {
                request.Headers.Remove(AuthHeaderKey);
            }

            request?.Headers?.Add(AuthHeaderKey, $"Bearer {accessToken}");
            return request;
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <returns>The access token.</returns>
        public async Task<string> GetAccessToken()
        {
            if ((await GetTokens().ConfigureAwait(false)) == null)
            {
                switch (CurrentGrantType)
                {
                    case OAuth2GrantType.Password:
                    case OAuth2GrantType.AuthorizationCode:
                        throw new WebException(FloatStrings.UnableToAuthenticateErrorMessage);
                    case OAuth2GrantType.ClientCredentials:
                        await Login().ConfigureAwait(false);
                        break;
                    default:
                        throw new WebException(FloatStrings.UnableToGetTokenErrorMessage);
                }
            }

            var tokens = await GetTokens().ConfigureAwait(false);

            if (tokens?.ShouldRefresh == true)
            {
                await refreshTokensContainer.Run().ConfigureAwait(false);
            }

            return (await GetTokens().ConfigureAwait(false))?.AccessToken;
        }

        /// <inheritdoc />
        public async Task<HttpRequestMessage> RefreshCredentialsAndAuthenticateRequest(HttpRequestMessage request)
        {
            await refreshTokensContainer.Run().ConfigureAwait(false);
            return await AuthenticateRequest(request).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> Logout()
        {
            var tokens = await GetTokens().ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            RevokeTokens(tokens).OnFailure(task =>
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Failed to log user out: {task.Exception}");
#endif
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return SecureStore.Delete(TokenKey);
        }

        // allows setting arbitrary token data for testing
        internal async Task StoreTokens(string access = null, string refresh = null, int? duration = null)
        {
            var tokens = await GetTokens().ConfigureAwait(false);
            PutTokens(new OAuthTokens(access ?? tokens?.AccessToken, refresh ?? tokens?.RefreshToken, duration ?? tokens.DurationSeconds));
        }

        internal async Task<string> RawAccessToken()
        {
            var tokens = await GetTokens().ConfigureAwait(false);
            return tokens.AccessToken;
        }

        internal async Task<string> RawRefreshToken()
        {
            var tokens = await GetTokens().ConfigureAwait(false);
            return tokens.RefreshToken;
        }

        /// <summary>
        /// Gets the saved tokens from the device.
        /// </summary>
        /// <returns>The tokens needed to login.</returns>
        internal async Task<OAuthTokens> GetTokens()
        {
            try
            {
                var json = await SecureStore.GetAsync(TokenKey).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<OAuthTokens>(json);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                SecureStore.Delete(TokenKey);
                return null;
            }
        }

        /// <summary>
        /// Create a new OAuthToken object from json object containing token information.
        /// </summary>
        /// <returns>The tokens.</returns>
        /// <param name="json">A json string containing the token data.</param>
        protected static OAuthTokens ConstructTokens(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                var tokenWrapper = JObject.Parse(json);
                return new OAuthTokens(
                    tokenWrapper.Value<string>(AccessTokenKey),
                    tokenWrapper.Value<string>(RefreshTokenKey),
                    tokenWrapper.Value<int>(ExpiresInKey));
            }
            catch (Exception e)
            {
                throw new WebException(FloatStrings.ServerErrorMessage, e);
            }
        }

        /// <summary>
        /// Gets the login tokens by refresh.
        /// </summary>
        /// <returns>The tokens obtained using refresh token.</returns>
        protected async Task<OAuthTokens> GetTokensByRefresh()
        {
            if (await GetTokens().ConfigureAwait(false) is not OAuthTokens tokens
                || tokens.RefreshToken is not string refresh
                || string.IsNullOrWhiteSpace(refresh))
            {
                throw new WebException("cannot refresh without refresh token");
            }

            var body = new Dictionary<string, string>
            {
                { GrantKey, GrantRefreshValue },
                { RefreshTokenKey, refresh },
                { ClientIdKey, ClientId },
                { ClientSecretKey, ClientSecret },
            };

            return await GetNewTokens(body).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the tokens by keys.
        /// </summary>
        /// <returns>The tokens by keys.</returns>
        protected Task<OAuthTokens> GetTokensByKeys()
        {
            var body = new Dictionary<string, string>
            {
                { ClientIdKey, ClientId },
                { ClientSecretKey, ClientSecret },
                { GrantKey, GrantCredentialsValue },
            };

            return GetNewTokens(body);
        }

        /// <summary>
        /// Use the given parameters to get new set of Tokens from server.
        /// </summary>
        /// <returns>The new tokens.</returns>
        /// <param name="body">The login parameters to get tokens.</param>
        protected async Task<OAuthTokens> GetNewTokens(IReadOnlyDictionary<string, string> body)
        {
            using var content = new FormUrlEncodedContent(body);
            var response = await RequestClient.Send(HttpMethod.Post, LoginEndpoint, body: content).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(response.Content))
            {
                throw new WebException("Authorization tokens not found.");
            }

            return ConstructTokens(response.Content);
        }

        /// <summary>
        /// Save the Login tokens to the device for future use.
        /// </summary>
        /// <returns><c>true</c>, if tokens was saved, <c>false</c> otherwise.</returns>
        /// <param name="tokens">Tokens to be saved.</param>
        protected bool PutTokens(OAuthTokens tokens)
        {
            if (tokens == null)
            {
                return false;
            }

            return SecureStore.Put(TokenKey, JsonConvert.SerializeObject(tokens));
        }

        async Task<Response> Revoke(string tokenType, string token, string authToken)
        {
            var revokeParams = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                { KeyTokenType, tokenType },
                { KeyToken, token },
                { ClientIdKey, ClientId },
                { ClientSecretKey, ClientSecret },
            });

            var revokeHeaders = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {authToken}" },
            };

            using var content = new FormUrlEncodedContent(revokeParams);
            return await RequestClient.Post(RevokeEndpoint, null, revokeHeaders, content).ConfigureAwait(false);
        }
    }
}
