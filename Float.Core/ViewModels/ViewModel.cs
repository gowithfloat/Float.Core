// <copyright file="ViewModel.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.ComponentModel;

namespace Float.Core.ViewModels
{
    /// <summary>
    /// Convenience class for backing a view model with a single model.
    /// </summary>
    /// <typeparam name="T">Type of backing model.</typeparam>
    public abstract class ViewModel<T> : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel{T}"/> class.
        /// </summary>
        /// <param name="model">The backing model.</param>
        protected ViewModel(T model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        /// <summary>
        /// Gets the underlying model for the view model.
        /// </summary>
        /// <remarks>
        /// This is for internal use in Float.Core.
        /// </remarks>
        internal T UnderlyingModel => Model;

        /// <summary>
        /// Gets the underlying model.
        /// </summary>
        /// <value>The underlying model.</value>
        protected T Model { get; }

        /// <inheritdoc />
        protected override void OnObservingBegan()
        {
            base.OnObservingBegan();
            if (Model is INotifyPropertyChanged model)
            {
                model.PropertyChanged += OnModelPropertyChanged;
            }
        }

        /// <inheritdoc />
        protected override void OnObservingEnded()
        {
            base.OnObservingEnded();
            if (Model is INotifyPropertyChanged model)
            {
                model.PropertyChanged -= OnModelPropertyChanged;
            }
        }
    }
}
