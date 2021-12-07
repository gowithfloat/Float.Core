using System;

namespace Float.Core.ViewModels
{
    /// <summary>
    /// Interface representing a view model for a page.
    /// </summary>
    public interface IPageViewModel
    {
        /// <summary>
        /// Gets the page title.
        /// </summary>
        /// <value>A string representing the title for the page.</value>
        string Title { get; }

        /// <summary>
        /// Gets a value indicating whether the page is currently loading content.
        /// </summary>
        /// <value><c>true</c> if the page is currently busy loading data; <c>false</c> otherwise.</value>
        bool IsLoading { get; }

        /// <summary>
        /// Gets a value indicating whether the page is ready for user input.
        /// </summary>
        /// <value><c>true</c> if the page is ready for user input; <c>false</c> otherwise.</value>
        bool IsReady { get; }

        /// <summary>
        /// Gets the last user-generated error that occurred.
        /// </summary>
        /// <value>An exception that occurred in response to a user request; the user should be notified of this error.</value>
        Exception Error { get; }
    }
}
