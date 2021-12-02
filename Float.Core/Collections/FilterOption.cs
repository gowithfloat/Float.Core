// <copyright file="FilterOption.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;

namespace Float.Core.Collections
{
    /// <summary>
    /// A single filter option.
    /// </summary>
    /// <typeparam name="T">The type of object being filtered.</typeparam>
    public class FilterOption<T> : Filter<T>
    {
        /// <summary>
        /// Gets or sets the delegate.
        /// Used to determine whether the value matches this filter option.
        /// </summary>
        /// <value>The delegate.</value>
        public Func<T, bool> Delegate { get; set; }

        /// <inheritdoc />
        public override bool Matches(T value)
        {
            if (Delegate == null)
            {
                throw new InvalidOperationException("No delegate to match");
            }

            return Delegate(value);
        }
    }
}
