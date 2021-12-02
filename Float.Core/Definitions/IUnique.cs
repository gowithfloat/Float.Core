// <copyright file="IUnique.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;

namespace Float.Core.Definitions
{
    /// <summary>
    /// Defines an object with a unique identifier.
    /// </summary>
    public interface IUnique
    {
        /// <summary>
        /// Gets the unique identifier for this object.
        /// </summary>
        /// <value>This object's unique identifier.</value>
        Guid UUID { get; }
    }
}
