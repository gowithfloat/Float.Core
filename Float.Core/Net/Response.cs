using System;
using System.Net;
using System.Net.Http;

namespace Float.Core.Net
{
    /// <summary>
    /// An response (and body) from an HTTP request.
    /// </summary>
    public struct Response : IEquatable<Response>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> struct.
        /// </summary>
        /// <param name="httpResponse">The HTTP response.</param>
        /// <param name="content">The response body.</param>
        public Response(HttpResponseMessage httpResponse, string content)
        {
            HttpResponse = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));
            Content = content;
        }

        /// <summary>
        /// Gets the HTTP response.
        /// </summary>
        /// <value>The response.</value>
        public HttpResponseMessage HttpResponse { get; }

        /// <summary>
        /// Gets the body content of the response formatted as a string.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Response"/> contains a successful status code.
        /// </summary>
        /// <value><c>true</c> if is success status code; otherwise, <c>false</c>.</value>
        public bool IsSuccessStatusCode => HttpResponse.IsSuccessStatusCode;

        /// <summary>
        /// Gets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode => HttpResponse.StatusCode;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Response"/> is indicating that authentication is required.
        /// This would probably mean the user needs to log in again.
        /// </summary>
        /// <value><c>true</c> if authentication is required; otherwise, <c>false</c>.</value>
        public bool IsAuthenticationRequired => HttpResponse.StatusCode == HttpStatusCode.Unauthorized;

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if the objects are considered equal, <c>false</c> otherwise.</returns>
        public static bool operator ==(Response left, Response right) => left.Equals(right);

        /// <summary>
        /// Determines whether two object instances are inequal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if the objects are considered inequal, <c>false</c> otherwise.</returns>
        public static bool operator !=(Response left, Response right) => !(left == right);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Response response && Equals(response);

        /// <inheritdoc />
        public bool Equals(Response other)
        {
            return other.HttpResponse == HttpResponse && other.Content == Content;
        }

        /// <inheritdoc />
        public override int GetHashCode() => (HttpResponse, Content).GetHashCode();
    }
}
