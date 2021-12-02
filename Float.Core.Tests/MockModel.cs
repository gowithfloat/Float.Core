// <copyright file="MockModel.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Float.Core.Tests
{
    /// <summary>
    /// Base implementation of a mock model that allows for easy
    /// notification of property changes.
    /// </summary>
    public abstract class MockModel : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the field.
        /// </summary>
        /// <param name="field">Field reference.</param>
        /// <param name="value">New value.</param>
        /// <param name="propertyName">Property name.</param>
        /// <typeparam name="T">Type of the field.</typeparam>
        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
