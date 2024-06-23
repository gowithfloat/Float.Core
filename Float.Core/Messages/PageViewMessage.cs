using CommunityToolkit.Mvvm.Messaging.Messages;
#if NETSTANDARD
using Xamarin.Forms;
#else
using Microsoft.Maui.Controls;
#endif

namespace Float.Core.Messages
{
    /// <summary>
    /// The page view message class definition.
    /// </summary>
    public class PageViewMessage : ValueChangedMessage<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageViewMessage"/> class.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="page">The page displayed.</param>
        public PageViewMessage(string eventName, Page page) : base(eventName)
        {
            Page = page;
        }

        /// <summary>
        /// Gets the page displayed.
        /// </summary>
        /// <value>
        /// The page displayed.
        /// </value>
        public Page Page { get; }
    }
}
