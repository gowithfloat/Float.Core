using System;
using System.ComponentModel;

namespace Float.Core.Collections
{
    /// <summary>
    /// Defines behavior for a filter.
    /// </summary>
    /// <typeparam name="T">The type of object being filtered.</typeparam>
    public interface IFilter<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when filter changed.
        /// </summary>
        event EventHandler FilterChanged;

        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IFilter{T}"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; }

        /// <summary>
        /// Toggle whether this filter is enabled or disabled.
        /// </summary>
        void Toggle();

        /// <summary>
        /// Determines if the filter matches the specified value.
        /// </summary>
        /// <returns>True if the filter matches the specified value, false otherwise.</returns>
        /// <param name="value">The value.</param>
        bool Matches(T value);
    }
}
