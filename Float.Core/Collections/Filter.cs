// <copyright file="Filter.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.ComponentModel;

namespace Float.Core.Collections
{
    /// <summary>
    /// Base filter definition.
    /// Provides default behaviors for handling property changes.
    /// </summary>
    /// <typeparam name="T">The type of object being filtered.</typeparam>
    public abstract class Filter<T> : IFilter<T>
    {
        string name;
        bool enabled = true;

        /// <summary>
        /// Occurs when a property changes in the filter.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public event EventHandler FilterChanged;

        /// <inheritdoc />
        public string Name
        {
            get => name;

            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }

        /// <inheritdoc />
        public bool IsEnabled
        {
            get => enabled;

            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    NotifyPropertyChanged(nameof(IsEnabled));
                    NotifyFilterChanged();
                }
            }
        }

        /// <inheritdoc />
        public virtual void Toggle()
        {
            IsEnabled = !IsEnabled;
        }

        /// <inheritdoc />
        public abstract bool Matches(T value);

        /// <summary>
        /// Notifies observers that a property changed.
        /// </summary>
        /// <param name="propertyName">Property name of the changed property.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifies observers that the filter status changed.
        /// </summary>
        protected void NotifyFilterChanged()
        {
            FilterChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
