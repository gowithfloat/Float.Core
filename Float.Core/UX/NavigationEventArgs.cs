﻿#if NETSTANDARD
using Xamarin.Forms;
#else
using Microsoft.Maui;
using Microsoft.Maui.Controls;
#endif

namespace Float.Core.UX
{
    /// <summary>
    /// Navigation event arguments.
    /// </summary>
    public class NavigationEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationEventArgs"/> class.
        /// </summary>
        /// <param name="type">Type of navigation event.</param>
        /// <param name="page">The page that was closed or opened.</param>
        public NavigationEventArgs(NavigationType type, Page page)
        {
            Type = type;
            Page = page;
        }

        /// <summary>
        /// The type of navigation event.
        /// </summary>
        public enum NavigationType
        {
            /// <summary>
            /// A page was closed.
            /// </summary>
            Popped,

            /// <summary>
            /// A page was opened.
            /// </summary>
            Pushed,

            /// <summary>
            /// The context was reset to it's original state.
            /// </summary>
            Reset,
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public NavigationType Type { get; }

        /// <summary>
        /// Gets the page that was closed or opened.
        /// If the navigation context is reset, it returns the first page of the context.
        /// </summary>
        /// <value>The page.</value>
        public Page Page { get; }
    }
}
