// <copyright file="NetException.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;

namespace Float.Core.Net
{
    /// <summary>
    /// Shared base class representing that something went wrong when making a network request using RequestClient.
    /// </summary>
    public abstract class NetException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetException"/> class.
        /// </summary>
        public NetException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public NetException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        protected NetException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
