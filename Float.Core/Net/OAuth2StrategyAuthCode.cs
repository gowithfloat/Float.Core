// <copyright file="OAuth2StrategyAuthCode.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Float.Core.Events;
using Float.Core.Extensions;
using Float.Core.Persistence;
using Xamarin.Forms;

namespace Float.Core.Net
{
    /// <summary>
    /// OAuth strategy.
    /// </summary>
    public class OAuth2StrategyAuthCode : OAuth2StrategyBase
    {
        readonly Uri redirectURIValue;
        readonly string authorizationEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2StrategyAuthCode"/> class.
        /// </summary>
        /// <param name="baseUri">Base URI.</param>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="clientSecret">Client secret.</param>
        /// <param name="secureStore">Secure store.</param>
        /// <param name="redirectURI">The Redirect URI used in Authorization Code transaction.</param>
        /// <param name="authorizationEndpoint"> The Endpoint used to retrieve Authorization Code. </param>
        /// <param name="loginEndpoint">Login endpoint.</param>
        /// <param name="revokeEndpoint">Revoke/Logout endpoint.</param>
        public OAuth2StrategyAuthCode(Uri baseUri, string clientId, string clientSecret, ISecureStore secureStore, Uri redirectURI, string authorizationEndpoint, string loginEndpoint = DefaultLoginEndpoint, string revokeEndpoint = DefaultRevokeEndpoint) : base(baseUri, clientId, clientSecret, secureStore, loginEndpoint, revokeEndpoint, OAuth2GrantType.AuthorizationCode)
        {
            redirectURIValue = redirectURI ?? throw new ArgumentNullException(nameof(redirectURI));
            this.authorizationEndpoint = authorizationEndpoint;
        }

        /// <summary>
        /// An event that will be invoked when the authorization code is received.
        /// </summary>
        public event EventHandler<TypedEventArgs<string>> OnAuthCodeReceived;

        /// <inheritdoc />
        public override async Task<bool> Login(string code, string notUsed)
        {
            if (CurrentGrantType == OAuth2GrantType.AuthorizationCode)
            {
                PutTokens(await GetTokensByCode(code).ConfigureAwait(false));
            }
            else
            {
                // Other grant types are not handled by this Authorization Class
                throw new WebException("Request could not be authenticated: wrong authorization Type");
            }

            return (await GetTokens().ConfigureAwait(false)).AccessToken != null;
        }

        /// <summary>
        /// Gets the URL query needed to login into federated login server.
        /// </summary>
        /// <returns>The URL of the federated login server.</returns>
        public Uri GetAuthorizationCodeURL()
        {
            return new Uri($"{BaseUri}{authorizationEndpoint}?response_type={AuthorizationCodeKey}&{ClientIdKey}="
                + $"{ClientId}&{RedirectURIKey}={WebUtility.UrlEncode(redirectURIValue.AbsoluteUri)}");
        }

        /// <summary>
        /// The Event listener can be attached to the webviews.navigating event
        /// to monitor the pages for authorization code. When found the  onAuthCodeCallback is fired.
        /// </summary>
        /// <param name="sender">The webview object creating the events.</param>
        /// <param name="args">The webNavigatingEventArgs that contain the current page information.</param>
        public void CheckForAuthorizationCode(object sender, WebNavigatingEventArgs args)
        {
            if (args?.Url is not string uri)
            {
                return;
            }

            var regex = new Regex(redirectURIValue.PatternForMatchingEitherHost());

            if (!regex.IsMatch(uri))
            {
                return;
            }

            if (sender is WebView webView)
            {
                webView.Navigating -= CheckForAuthorizationCode;
            }

            var code = args.Url.Substring(args.Url.IndexOf("?", StringComparison.OrdinalIgnoreCase) + 6);
            args.Cancel = true;
            OnAuthCodeReceived?.Invoke(this, new TypedEventArgs<string>(code));
        }

        /// <summary>
        /// Sets up the parameters needed to get tokens by code.
        /// </summary>
        /// <returns>The Login Tokens.</returns>
        /// <param name="code">Authorization Code for federateed login.</param>
        Task<OAuthTokens> GetTokensByCode(string code)
        {
            var body = new Dictionary<string, string>
            {
                { AuthorizationCodeKey, code },
                { RedirectURIKey, redirectURIValue.AbsoluteUri },
                { GrantKey, GrantAuthorizationCodeValue },
                { ClientIdKey, ClientId },
                { ClientSecretKey, ClientSecret },
            };

            return GetNewTokens(body);
        }
    }
}
