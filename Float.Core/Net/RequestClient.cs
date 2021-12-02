// <copyright file="RequestClient.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Float.Core.Extensions;
using Float.Core.Resources;
using Newtonsoft.Json.Linq;

namespace Float.Core.Net
{
    /// <summary>
    /// Provides a convenient HTTP request client for requesting string data from a server.
    /// </summary>
    public sealed class RequestClient
    {
        readonly Uri baseUri;
        readonly IAuthStrategy authStrategy;
        readonly Dictionary<string, string> defaultHeaders;
        readonly HttpMessageHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestClient"/> class.
        /// </summary>
        /// <param name="baseUri">The base URI for all HTTP requests (required).</param>
        /// <param name="authStrategy">Authentication strategy to use to authorize requests before they are made.</param>
        /// <param name="defaultHeaders">Default request headers to add to each request (can be overridden when the request is made).</param>
        /// <param name="handler">Allows the ability to configure HTTP message handler for the HTTP client.</param>
        public RequestClient(Uri baseUri, IAuthStrategy authStrategy = null, Dictionary<string, string> defaultHeaders = null, HttpMessageHandler handler = null)
        {
            this.baseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            this.authStrategy = authStrategy;
            this.defaultHeaders = defaultHeaders;
            this.handler = handler;
        }

        /// <summary>
        /// Performs a GET request.
        /// </summary>
        /// <returns>The HTTP response and response body.</returns>
        /// <param name="path">The request path to append to the base URI provided when the client was created.</param>
        /// <param name="query">Optional query parameters to add to the URL.</param>
        /// <param name="headers">Optional HTTP headers to add to the request.</param>
        public Task<Response> Get(string path, Dictionary<string, string> query = null, Dictionary<string, string> headers = null)
        {
            return Send(HttpMethod.Get, path, query, headers);
        }

        /// <summary>
        /// Performs a POST request.
        /// </summary>
        /// <returns>The HTTP response and response body.</returns>
        /// <param name="path">The request path to append to the base URI provided when the client was created.</param>
        /// <param name="body">Content to send with the request (be sure to set the Content-Type of the request).</param>
        public Task<Response> Post(string path, HttpContent body)
        {
            return Post(path, null, null, body);
        }

        /// <summary>
        /// Performs a POST request.
        /// </summary>
        /// <returns>The HTTP response and response body.</returns>
        /// <param name="path">The request path to append to the base URI provided when the client was created.</param>
        /// <param name="query">Optional query parameters to add to the URL.</param>
        /// <param name="headers">Optional HTTP headers to add to the request.</param>
        /// <param name="body">Content to send with the request (be sure to set the Content-Type of the request).</param>
        public Task<Response> Post(string path, Dictionary<string, string> query = null, Dictionary<string, string> headers = null, HttpContent body = null)
        {
            return Send(HttpMethod.Post, path, query, headers, body);
        }

        /// <summary>
        /// Performs a PUT request.
        /// </summary>
        /// <returns>The HTTP response and response body.</returns>
        /// <param name="path">The request path to append to the base URI provided when the client was created.</param>
        /// <param name="body">Content to send with the request (be sure to set the Content-Type of the request).</param>
        public Task<Response> Put(string path, HttpContent body)
        {
            return Put(path, null, null, body);
        }

        /// <summary>
        /// Performs a PUT request.
        /// </summary>
        /// <returns>The HTTP response and response body.</returns>
        /// <param name="path">The request path to append to the base URI provided when the client was created.</param>
        /// <param name="query">Optional query parameters to add to the URL.</param>
        /// <param name="headers">Optional HTTP headers to add to the request.</param>
        /// <param name="body">Content to send with the request (be sure to set the Content-Type of the request).</param>
        public Task<Response> Put(string path, Dictionary<string, string> query = null, Dictionary<string, string> headers = null, HttpContent body = null)
        {
            return Send(HttpMethod.Put, path, query, headers, body);
        }

        /// <summary>
        /// Performs a patch request.
        /// </summary>
        /// <returns>The patch.</returns>
        /// <param name="path">The Path.</param>
        /// <param name="body">The Body.</param>
        public Task<Response> Patch(string path, HttpContent body)
        {
           return Send(new HttpMethod("PATCH"), path, null, null, body);
        }

        /// <summary>
        /// Sends an HTTP request with the specified method.
        /// </summary>
        /// <returns>The HTTP response and response body.</returns>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="path">The request path to append to the base URI provided when the client was created.</param>
        /// <param name="query">Optional query parameters to add to the URL.</param>
        /// <param name="headers">Optional HTTP headers to add to the request.</param>
        /// <param name="body">Content to send with the request (be sure to set the Content-Type of the request).</param>
        public async Task<Response> Send(HttpMethod method, string path, Dictionary<string, string> query = null, Dictionary<string, string> headers = null, HttpContent body = null)
        {
            if (body != null && authStrategy is IRefreshableAuthStrategy)
            {
                // This call stops the HttpClient from disposing of this content
                // in case we need it again. Fix for QIC #1851.
                await body.LoadIntoBufferAsync().ConfigureAwait(false);
            }

            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }

            if (!headers.ContainsKey("Accept-Language"))
            {
                headers.Add("Accept-Language", System.Globalization.CultureInfo.CurrentCulture.Name);
            }

            // Prepare the URL for the request by combining the base uri supplied when the client was created
            // with the path and query parameters for this request
            var url = new Uri(baseUri, path);

            if (query != null)
            {
                var queryString = string.Join("&", query.Select(kvp => Uri.EscapeUriString(kvp.Key) + "=" + Uri.EscapeUriString(kvp.Value)));
                if (url.Query.Contains("?"))
                {
                    url = new Uri(baseUri, $"{path}&{queryString}");
                }
                else
                {
                    url = new Uri(baseUri, $"{path}?{queryString}");
                }
            }

            // deferred for now as the change is non-trivial
#pragma warning disable CA2000 // Dispose objects before losing scope
            var request = PrepareRequestMessage(method, url, headers, body);
#pragma warning restore CA2000 // Dispose objects before losing scope

            // If an authentication strategy was specified, authenticate the request
            // This may replace the Authorization header if one was already set
            if (authStrategy != null)
            {
                // Send the first authenticated request
                request = await authStrategy.AuthenticateRequest(request).ConfigureAwait(false);
            }

            var response = await Send(request).ConfigureAwait(false);

            // If we get unauthorized response, try to authenticate a second time.
            // This is common in OAuth when an access token is expired.
            if ((response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden) && authStrategy is IRefreshableAuthStrategy refreshableAuthStrategy)
            {
                // we need to build a new request here; requests can only be sent once
                request.Dispose();
#pragma warning disable CA2000 // Dispose objects before losing scope
                request = PrepareRequestMessage(method, url, headers, body);
#pragma warning restore CA2000 // Dispose objects before losing scope
                request = await refreshableAuthStrategy.RefreshCredentialsAndAuthenticateRequest(request).ConfigureAwait(false);
                response = await Send(request).ConfigureAwait(false);
            }

            // Bubble up all expections if the second auth attempt failed.
            if (!response.IsSuccessStatusCode)
            {
                HandleHttpException(response);
            }

            return response;
        }

        /// <summary>
        /// Adds request headers.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="requestHeaders">Request headers (can be null).</param>
        /// <param name="defaultHeaders">Default headers (can be null; will not replace any headers in requestHeaders).</param>
        static void AddRequestHeaders(HttpRequestMessage request, IEnumerable<KeyValuePair<string, string>> requestHeaders, IEnumerable<KeyValuePair<string, string>> defaultHeaders)
        {
            AddRequestHeaders(request, requestHeaders);
            AddRequestHeaders(request, defaultHeaders);
        }

        /// <summary>
        /// Adds request headers.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="headers">The headers to add. Headers will not be added if they are already present on the request.</param>
        static void AddRequestHeaders(HttpRequestMessage request, IEnumerable<KeyValuePair<string, string>> headers)
        {
            if (headers == null)
            {
                return;
            }

            foreach (var kvp in headers)
            {
                if (!request.Headers.Contains(kvp.Key))
                {
                    request.Headers.Add(kvp.Key, kvp.Value);
                }
            }
        }

        static void HandleHttpException(Response response)
        {
            if (string.IsNullOrWhiteSpace(response.Content))
            {
                throw new HttpRequestException(response, FloatStrings.ServerErrorMessage);
            }

            JToken error;

            try
            {
                error = JObject.Parse(response.Content).GetValue("message", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception e)
            {
                throw new HttpRequestException(response, e);
            }

            if (error != null)
            {
                throw new HttpRequestException(response, error.ToString());
            }
            else
            {
                throw new HttpRequestException(response);
            }
        }

        HttpRequestMessage PrepareRequestMessage(HttpMethod method, Uri url, Dictionary<string, string> headers = null, HttpContent body = null)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var request = new HttpRequestMessage(method, url);

            // Add the request-specific headers and then the default headers provided when the client was created
            // Request-specific headers will take precedence over any default headers
            AddRequestHeaders(request, headers, defaultHeaders);

            // Attach the request body, if one was supplied
            // Only applicable for POST and PUT requests
            request.Content = body;

            return request;
        }

        async Task<Response> Send(HttpRequestMessage request)
        {
            using var webClient = handler != null ? new HttpClient(handler, false) : new HttpClient();

            try
            {
                var httpResponse = await webClient.SendAsync(request).ConfigureAwait(false);
                var responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                return new Response(httpResponse, responseContent);
            }
            catch (Exception e)
            {
                if (e.IsOfflineException())
                {
                    throw new HttpConnectionException(FloatStrings.NoInternetMessage, e);
                }

                throw;
            }
        }
    }
}
