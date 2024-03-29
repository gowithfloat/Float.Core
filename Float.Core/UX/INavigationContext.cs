﻿using System;
using System.Threading.Tasks;
#if NETSTANDARD
using Xamarin.Forms;
#else
using Microsoft.Maui;
using Microsoft.Maui.Controls;
#endif

namespace Float.Core.UX
{
    /// <summary>
    /// Defines the navigation context for a coordinator.
    /// </summary>
    public interface INavigationContext
    {
        /// <summary>
        /// Occurs when the user has navigated to a new page.
        /// </summary>
        event EventHandler<NavigationEventArgs> Navigated;

        /// <summary>
        /// Gets a value indicating whether this <see cref="INavigationContext"/> is currently on the root page.
        /// </summary>
        /// <value><c>true</c> if root page is current; otherwise, <c>false</c>.</value>
        bool IsAtRootPage { get; }

        /// <summary>
        /// Show a page representing an overview of information.
        /// This is frequently a list of data.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="animated">Whether to animate the transition.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ShowOverviewPageAsync(Page page, bool animated = true);

        /// <summary>
        /// Show a detail page.
        /// This is usually in response to selecting an item from an overview page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="animated">Whether to animate the transition.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ShowDetailPageAsync(Page page, bool animated = true);

        /// <summary>
        /// Pushs a page onto the existing navigation stack.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="animated">Whether to animate the transition.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PushPageAsync(Page page, bool animated = true);

        /// <summary>
        /// Pop the top-most page off the existing navigation stack.
        /// </summary>
        /// <param name="animated">Whether to animate the transition.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopPageAsync(bool animated = true);

        /// <summary>
        /// Modally present the specified page in the current context.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="animated">Whether to animate the transition.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PresentPageAsync(Page page, bool animated = true);

        /// <summary>
        /// Dismisses a modally-presented page.
        /// </summary>
        /// <param name="animated">Whether to animate the transition.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DismissPageAsync(bool animated = true);

        /// <summary>
        /// Reset the navigation context back to the beginning.
        /// </summary>
        /// <param name="animated">Whether to animate the transition.</param>
        void Reset(bool animated = true);
    }
}
