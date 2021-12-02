// <copyright file="ViewModelSelectedEventArgs.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using Float.Core.ViewModels;

namespace Float.Core.Events
{
    /// <summary>
    /// View model selected event arguments.
    /// </summary>
    /// <typeparam name="T">The type of model that this event is representing.</typeparam>
    public class ViewModelSelectedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelSelectedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="model">The Model.</param>
        public ViewModelSelectedEventArgs(ViewModel<T> model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Model = model.UnderlyingModel;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model that was selected.</value>
        public T Model { get; }
    }
}
