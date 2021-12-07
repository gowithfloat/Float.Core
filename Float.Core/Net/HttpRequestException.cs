using System;

namespace Float.Core.Net
{
    /// <summary>
    /// Represents an error returned from an HTTP request.
    /// </summary>
#pragma warning disable CA1032 // Implement standard exception constructors
    public class HttpRequestException : NetException
#pragma warning restore CA1032 // Implement standard exception constructors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestException"/> class.
        /// </summary>
        /// <param name="response">The response from the HTTP request.</param>
        public HttpRequestException(Response response) : base(response.Content)
        {
            Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestException"/> class.
        /// </summary>
        /// <param name="response">The response from the HTTP request.</param>
        /// <param name="baseException">The underlying exception that occurred during the request.</param>
        public HttpRequestException(Response response, Exception baseException) : base(response.Content, baseException)
        {
            Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestException"/> class.
        /// </summary>
        /// <param name="response">The Response.</param>
        /// <param name="message">The Message to display.</param>
        public HttpRequestException(Response response, string message) : base(message)
        {
            Response = response;
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>The response.</value>
        public Response Response { get; }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public int Code => (int)Response.HttpResponse.StatusCode;
    }
}
