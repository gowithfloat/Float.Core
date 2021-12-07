using System;

namespace Float.Core.Commands
{
    /// <summary>
    /// Exceptions caused by <see cref="CommandFactoryRegistry"/>.
    /// </summary>
    public class CommandFactoryRegistryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFactoryRegistryException"/> class.
        /// </summary>
        public CommandFactoryRegistryException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFactoryRegistryException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public CommandFactoryRegistryException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFactoryRegistryException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CommandFactoryRegistryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
