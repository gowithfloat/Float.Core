// <copyright file="CoordinatorException.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;

namespace Float.Core.UX
{
    /// <summary>
    /// Container for exceptions thrown by coordinator classes.
    /// </summary>
    public class CoordinatorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinatorException"/> class.
        /// </summary>
        public CoordinatorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinatorException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public CoordinatorException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinatorException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CoordinatorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
