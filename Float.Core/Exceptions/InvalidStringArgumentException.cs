using System;

namespace Float.Core.Exceptions
{
    /// <summary>
    /// Exceptions raised by a failed <c>string.IsNullOrEmpty</c> or <c>string.IsNullOrWhitespace</c>.
    /// </summary>
    public class InvalidStringArgumentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStringArgumentException"/> class.
        /// </summary>
        public InvalidStringArgumentException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStringArgumentException"/> class.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        public InvalidStringArgumentException(string paramName) : base($"Invalid string argument {paramName}")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStringArgumentException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidStringArgumentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
