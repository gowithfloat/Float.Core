using System;

namespace Float.Core.Net
{
    /// <summary>
    /// Represents an error occurred establishing a connection for an HTTP request (e.g. the device is offline).
    /// </summary>
    public class HttpConnectionException : NetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpConnectionException"/> class.
        /// </summary>
        public HttpConnectionException() : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpConnectionException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public HttpConnectionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpConnectionException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public HttpConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
