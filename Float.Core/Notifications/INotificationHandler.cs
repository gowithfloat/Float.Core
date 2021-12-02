// <copyright file="INotificationHandler.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;

namespace Float.Core.Notifications
{
    /// <summary>
    /// A notification handler takes responsibility for notifying the user of
    /// important information during the application lifecycle.
    /// This may be notifying the user of an exception if a
    /// user-requested action failed, or notifying the user of success
    /// of persisting user-created data.
    /// </summary>
    /// <remarks>
    /// Not every exception or successful action is worthy of the user's attention.
    /// While it is outside the scope of this documentation to provide specific
    /// guidance of when a user should be notified, keep in mind that how an
    /// application handles errors is just as much part of the design
    /// as are the colors and icons.
    /// Additionally, while this class is designed to handle situations requiring
    /// the user's attention, it can absolutely be used for logging of those same
    /// situations.
    /// </remarks>
    public interface INotificationHandler
    {
        /// <summary>
        /// Notify the user that an error occurred.
        /// This is different from an exception in that an "error" is an expected
        /// scenario that the user might encounter (e.g. due to user error).
        /// </summary>
        /// <param name="localizedErrorDescription">Localized error description.</param>
        /// <param name="suggestion">A brief, human-friendly suggestion of how to fix the error.</param>
        void NotifyError(string localizedErrorDescription, string suggestion = null);

        /// <summary>
        /// Notify the user that an exception occurred.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <param name="implication">A brief, human-friendly description of the implication of the exception.</param>
        /// <remarks>
        /// The implication of an exception is an explanation of
        /// why it matters to the user. For example, if the user requested to save
        /// a bookmark, but a lack of internet connection caused the request to fail,
        /// the exception would contain the technical reason for the failure,
        /// but the implication would be: "Unable to Save Bookmark".
        /// </remarks>
        void NotifyException(Exception e, string implication = null);

        /// <summary>
        /// Formats an exception in preparation for presenting to the user.
        /// </summary>
        /// <param name="e">The exception to make human-friendly.</param>
        /// <returns>The string to display.</returns>
        /// <remarks>
        /// To simplify the call site, it is possible that a null value could get passed here.
        /// In which case, this should return null to signal to the call to substitute a default value.
        /// </remarks>
        string FormatException(Exception e);
    }
}
