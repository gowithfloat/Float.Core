using System;

namespace Float.Core.Net
{
    /// <summary>
    /// Expection occurs when attempting to authenticate with an AuthStrategy.
    /// </summary>
    public class AuthStrategyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthStrategyException"/> class.
        /// </summary>
        public AuthStrategyException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthStrategyException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public AuthStrategyException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthStrategyException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AuthStrategyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
