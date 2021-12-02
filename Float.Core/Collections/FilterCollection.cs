// <copyright file="FilterCollection.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Float.Core.Collections
{
    /// <summary>
    /// A collection of filters.
    /// </summary>
    /// <typeparam name="T">The type of object being filtered.</typeparam>
    public class FilterCollection<T> : Filter<T>, IEnumerable, IEnumerable<IFilter<T>>
    {
        readonly ObservableElementCollection<IFilter<T>> options;
        FilterOperation operation = FilterOperation.AnyIfActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCollection{T}"/> class.
        /// </summary>
        /// <param name="filterOptions">Filter options.</param>
        public FilterCollection(IEnumerable<IFilter<T>> filterOptions)
        {
            if (filterOptions == null)
            {
                throw new ArgumentNullException(nameof(filterOptions));
            }

            options = new ObservableElementCollection<IFilter<T>>(filterOptions);
            options.ChildPropertyChanged += HandleOptionChanged;
            options.CollectionChanged += HandleOptionChanged;
        }

        /// <summary>
        /// Define the behavior of how many options a value needs to pass
        /// in order for a value to pass the filter.
        /// </summary>
        public enum FilterOperation
        {
            /// <summary>
            /// Values must match all options to pass the filter.
            /// </summary>
            All,

            /// <summary>
            /// Values must match at least one option to pass the filter.
            /// If no filters are enabled, then no values will pass the filter.
            /// </summary>
            Any,

            /// <summary>
            /// Values must match at least one enabled option to pass the filter.
            /// If no filters are enabled, then all values will pass the filter.
            /// </summary>
            AnyIfActive,
        }

        /// <summary>
        /// Gets the options in this filter group.
        /// </summary>
        /// <value>The options.</value>
        public IEnumerable<IFilter<T>> Options => options;

        /// <summary>
        /// Gets or sets the operation used by the filter group to determine
        /// if a given value passes the filter.
        /// </summary>
        /// <value>The operation.</value>
        public FilterOperation Operation
        {
            get => operation;

            set
            {
                if (value != operation)
                {
                    operation = value;
                    NotifyPropertyChanged(nameof(Operation));
                    NotifyFilterChanged();
                }
            }
        }

        /// <inheritdoc />
        public override bool Matches(T value)
        {
            switch (operation)
            {
                case FilterOperation.All:
                    return options.Where(opt => opt.IsEnabled).All(opt => opt.Matches(value));
                case FilterOperation.Any:
                    return options.Where(opt => opt.IsEnabled).Any(opt => opt.Matches(value));
                default:
                    var enabledFilters = options.Where(opt => opt.IsEnabled);

                    // If no filter options are enabled, then all values pass the filter
                    return enabledFilters.Any() == false || enabledFilters.Any(opt => opt.Matches(value));
            }
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            return options.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator<IFilter<T>> IEnumerable<IFilter<T>>.GetEnumerator()
        {
            return options.GetEnumerator();
        }

        /// <summary>
        /// Invoked when the filter options have changed.
        /// </summary>
        /// <param name="sender">The initiator of the change.</param>
        /// <param name="args">Contains the proeprty that was changed.</param>
        protected virtual void HandleOptionChanged(object sender, EventArgs args)
        {
            NotifyPropertyChanged(nameof(Options));
            NotifyFilterChanged();
        }
    }
}
