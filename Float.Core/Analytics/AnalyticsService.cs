using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using Float.Core.Messages;
#if NETSTANDARD
using Xamarin.Forms;
#else
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
#endif

namespace Float.Core.Analytics
{
    /// <summary>
    /// Base class for an analytics service to track analytics within the application.
    /// </summary>
    public abstract class AnalyticsService
    {
        const string SendUsageDataKey = "kSendUsageData";

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticsService"/> class.
        /// Also subscribes to the messaging center for page view events automatically.
        /// </summary>
        protected AnalyticsService()
        {
            WeakReferenceMessenger.Default.Register<PageViewMessage>(this, HandlePageView);
        }

        /// <summary>
        /// Enables the tracking.
        /// By default, this will enable tracking across _all_ analytics services.
        /// </summary>
        public static void EnableTracking()
        {
#if NETSTANDARD
            Application.Current.Properties[SendUsageDataKey] = true;
#else
            Preferences.Default.Set(SendUsageDataKey, true);
#endif
        }

        /// <summary>
        /// Disables the tracking.
        /// By default, this will disable tracking across _all_ analytics services.
        /// </summary>
        public static void DisableTracking()
        {
#if NETSTANDARD
            Application.Current.Properties[SendUsageDataKey] = false;
#else
            Preferences.Default.Set(SendUsageDataKey, false);
#endif
        }

        /// <summary>
        /// Determines if tracking is currently enabled.
        /// </summary>
        /// <returns><c>true</c>, if tracking is enabled, <c>false</c> otherwise.</returns>
        public static bool IsTrackingEnabled()
        {
            try
            {
#if NETSTANDARD
                if (Application.Current?.Properties?.ContainsKey(SendUsageDataKey) == true)
                {
                    return (bool?)Application.Current.Properties[SendUsageDataKey] != false;
                }
#else
                if (Preferences.Default?.ContainsKey(SendUsageDataKey) == true)
                {
                    return (bool?)Preferences.Default.Get(SendUsageDataKey, true) != false;
                }
#endif

                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // If anything goes wrong when checking for enabled tracking,
                // it is likely because the properties file is inaccessible (probably due to file system protection).
                // We'll enable tracking in this case because the application may need to report exceptions.
                return true;
            }
        }

        /// <summary>
        /// Tracks a page view.
        /// </summary>
        /// <param name="name">The screen name.</param>
        /// <param name="page">The page that was viewed.</param>
        public virtual void TrackPageView(string name, Page page = null)
        {
            if (ShouldTrackUsage())
            {
                OnPageView(name, page);
            }
        }

        /// <summary>
        /// Tracks an exception.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        public virtual void TrackException(Exception exception)
        {
            if (ShouldTrackUsage())
            {
                OnException(exception);
            }
        }

        /// <summary>
        /// Tracks an event.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="parameters">The parameters associated with the event.</param>
        public virtual void TrackEvent(string eventName, Dictionary<string, string> parameters)
        {
            if (ShouldTrackUsage())
            {
                OnEvent(eventName, parameters);
            }
        }

        /// <summary>
        /// Invoked when a page view occurred.
        /// The analytics service should implement the logic required for reporting this
        /// event to the analytics service.
        /// </summary>
        /// <param name="name">The screen name.</param>
        /// <param name="page">The page that was viewed (could be null).</param>
        protected abstract void OnPageView(string name, Page page);

        /// <summary>
        /// Invoked when an exception occurs.
        /// The analytics service should implement the logic required for reporting this
        /// event to the analytics service.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        protected abstract void OnException(Exception exception);

        /// <summary>
        /// Invoked when an analytics event occured.
        /// The analytics service should implement the logic required for reporting this
        /// event to the analytics service.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="parameters">The parameters associated with that event.</param>
        protected abstract void OnEvent(string eventName, Dictionary<string, string> parameters);

        /// <summary>
        /// Returns true if tracking usage is allowed.
        /// </summary>
        /// <returns><c>true</c>, if we can track usage, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldTrackUsage()
        {
            return IsTrackingEnabled();
        }

        /// <summary>
        /// Handler for when a page view occurs.
        /// </summary>
        /// <param name="sender">The page that appeared.</param>
        /// <param name="message">The page name.</param>
        void HandlePageView(object sender, PageViewMessage message)
        {
            TrackPageView(message.Value, message.Page);
        }
    }
}
