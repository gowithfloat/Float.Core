// <copyright file="UserRequestCommandFactory.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using Float.Core.Notifications;

namespace Float.Core.Commands
{
    /// <summary>
    /// Extends from a normal <see cref="CommandFactory{T}"/> but with the implication
    /// that this command is the direct result of a user request.
    /// Granted, most commands will always be the result of a user request,
    /// but the distinction here is that these types of commands must provide
    /// a route for notifying the user if something goes wrong (e.g. display an alert).
    /// </summary>
    /// <typeparam name="T">The subject of the request.</typeparam>
    public abstract class UserRequestCommandFactory<T> : CommandFactory<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRequestCommandFactory{T}"/> class.
        /// </summary>
        /// <param name="notificationHandler">Notification handler.</param>
        protected UserRequestCommandFactory(INotificationHandler notificationHandler)
        {
            NotificationHandler = notificationHandler ?? throw new ArgumentNullException(nameof(notificationHandler));
        }

        /// <summary>
        /// Gets the notification handler.
        /// </summary>
        /// <value>The notification handler.</value>
        protected INotificationHandler NotificationHandler { get; }

        /// <summary>
        /// Gets the localized user implication of an exception occuring.
        /// Basically: why the user should care.
        /// </summary>
        /// <remarks>
        /// This should be a very brief, but human-friendly explanation.
        /// </remarks>
        /// <value>The exception implication.</value>
        protected abstract string ExceptionImplication { get; }

        /// <inheritdoc />
        protected override void HandleException(Exception e)
        {
            NotificationHandler.NotifyException(e, ExceptionImplication);
        }
    }
}
