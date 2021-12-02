// <copyright file="BaseViewModel.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Float.Core.Commands;

namespace Float.Core.ViewModels
{
    /// <summary>
    /// A base implementation of a view model.
    /// Types inheriting from this class (or the generic specified below)
    /// can specify custom mappings between it's own properties and properties in the
    /// related model(s) to conveniently bubble up property changed notifications to the UI.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        PropertyChangedEventHandler propertyChangedHandler;

        /// <summary>
        /// Occurs when a property changes on the view model.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                var alreadyHadSubscribers = HasSubscribers;
                propertyChangedHandler += value;

                if (!alreadyHadSubscribers)
                {
                    OnObservingBegan();
                }
            }

            remove
            {
                propertyChangedHandler -= value;

                if (!HasSubscribers)
                {
                    OnObservingEnded();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this view model has subscribers listening for changes.
        /// </summary>
        /// <value><c>true</c> if there are active event listeners on this view model.</value>
        /// <remarks>
        /// Subclasses should extend this if they offer their own events.
        /// </remarks>
        protected virtual bool HasSubscribers => propertyChangedHandler?.GetInvocationList().Length > 0;

        /// <summary>
        /// Gets a registered command factory from the <see cref="CommandFactoryRegistry"/>.
        /// </summary>
        /// <returns>The command factory.</returns>
        /// <typeparam name="TCommandFactory">The type of command factory to retrieve.</typeparam>
        protected static TCommandFactory GetCommandFactory<TCommandFactory>() where TCommandFactory : class, ICommandFactory
        {
            return CommandFactoryRegistry.Get<TCommandFactory>();
        }

        /// <summary>
        /// Invoke when a property changes in the backing model(s).
        /// </summary>
        /// <remarks>
        /// The default implementation of this method attempts to raise property changed
        /// events on the view model in response to the backing model(s) changing.
        /// </remarks>
        /// <param name="sender">The model that changed.</param>
        /// <param name="e">Event arguments containing the name of the property that changed.</param>
        protected virtual void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            // Iterate through all the properties on this view model to determine
            // which properties should raise a property change event on the view model
            var properties = GetType().GetProperties();

            foreach (var propertyInfo in properties)
            {
                // Check for any view model property that is mapped to this model property
                EvaluatePropertyMapping(propertyInfo, e.PropertyName);

                // Additionally, automatically trigger an event on a matching view model property name
                // unless that property has a NotifyWhenPropertyChanges attribute
                if (propertyInfo.Name == e.PropertyName)
                {
                    var attributes = propertyInfo.GetCustomAttributes<NotifyWhenPropertyChangesAttribute>();
                    var hasCustomMapping = attributes.Any(a => a.TargetName == e.PropertyName);
                    if (!hasCustomMapping)
                    {
                        OnPropertyChanged(propertyInfo.Name);
                    }
                }
            }

            // Finally, also check the class info to see if there are any further
            // instructions for raising a property changed event to the model changing
            var typeInfo = GetType().GetTypeInfo();
            EvaluatePropertyMapping(typeInfo, e.PropertyName);
        }

        /// <summary>
        /// Invoke when a property has changed on the view model.
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The property name that has changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            propertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Convenience method to set a private field with a new value and invoke a property change event.
        /// Will return early if the new value is the same as the old value.
        /// Will raise a PropertyChanged event if the new value is different from the old value.
        /// </summary>
        /// <remarks>
        /// This method borrows and is inspired from the XLabs ViewModel code licensed under Apache 2.0.
        /// https://github.com/XLabs/Xamarin-Forms-Labs/wiki/ViewModel
        /// https://github.com/XLabs/Xamarin-Forms-Labs/blob/master/samples/XLabs.Samples/XLabs.Samples/ViewModel/BaseViewModel.cs.
        /// </remarks>
        /// <param name="field">The private field backing the property that is changing.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">The property name (determined automatically).</param>
        /// <typeparam name="T">The type of the field that is changing.</typeparam>
        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Invoked when the first subscriber has been added to this view model.
        /// </summary>
        /// <remarks>
        /// This is a good opportunity to begin listening for changes in the underlying model.
        /// </remarks>
        protected virtual void OnObservingBegan()
        {
        }

        /// <summary>
        /// Invoked when there are no more subscribers on this view model.
        /// </summary>
        /// <remarks>
        /// This is a good opportunity to remove event listeners since there are no
        /// objects watching for changes in the view model.
        /// </remarks>
        protected virtual void OnObservingEnded()
        {
        }

        /// <summary>
        /// Given a TypeInfo (for a class) or PropertyInfo (for a property), retrieve attributes and determine
        /// if a PropertyChanged event should be raised based on NotifyWhenPropertyChanges attribute.
        /// </summary>
        /// <param name="memberInfo">Member info.</param>
        /// <param name="attributePropertyName">Attribute property name.</param>
        void EvaluatePropertyMapping(MemberInfo memberInfo, string attributePropertyName)
        {
            // Scan the class attributes to see if any properties derive their value
            // from the value that changed; if so, notify receivers of the change in value
            var attributes = memberInfo.GetCustomAttributes<NotifyWhenPropertyChangesAttribute>();

            foreach (var attribute in attributes)
            {
                if (attribute.ModelPropertyName == attributePropertyName)
                {
                    OnPropertyChanged(attribute.TargetName);
                }
            }
        }

        /// <summary>
        /// Attach to a view model attribute to trigger a property changed notification
        /// when the specified property changes in the backing model.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
        protected sealed class NotifyWhenPropertyChangesAttribute : Attribute
        {
            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="NotifyWhenPropertyChangesAttribute"/> class.
            /// </summary>
            /// <param name="modelPropertyName">The name of the property on the model.</param>
            /// <param name="targetName">The name of the property on the view model.</param>
            public NotifyWhenPropertyChangesAttribute(string modelPropertyName, [CallerMemberName] string targetName = null)
            {
                TargetName = targetName;
                ModelPropertyName = modelPropertyName;
            }

            /// <summary>
            /// Gets the name property on the view model that should be considered
            /// changed when the backing model property changes.
            /// </summary>
            /// <value>The name of the property on the view model.</value>
            public string TargetName { get; }

            /// <summary>
            /// Gets the name of the model property.
            /// When a PropertyChanged event is raised containing this property name,
            /// an additional PropertyChanged event will be raised on the target view model property.
            /// </summary>
            /// <value>The name of the model property.</value>
            public string ModelPropertyName { get; }
        }
    }
}
