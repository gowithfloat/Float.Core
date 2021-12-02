// <copyright file="SelectableViewModel.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

namespace Float.Core.ViewModels
{
    /// <summary>
    /// Selectable view model.
    /// </summary>
    /// <typeparam name="T">Type of backing model.</typeparam>
    public abstract class SelectableViewModel<T> : ViewModel<T>
    {
        bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableViewModel{T}"/> class.
        /// </summary>
        /// <param name="model">The backing model.</param>
        public SelectableViewModel(T model) : base(model)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SelectableViewModel{T}"/> is selected.
        /// </summary>
        /// <value><c>true</c> if is selected; otherwise, <c>false</c>.</value>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                SetField(ref isSelected, value);
            }
        }
    }
}
