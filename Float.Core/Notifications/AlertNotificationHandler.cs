using System;
using Float.Core.Compatibility;
using Float.Core.Extensions;
using Float.Core.L10n;
using Float.Core.Resources;
#if NETSTANDARD
using Xamarin.Forms;
#else
using Microsoft.Maui;
using Microsoft.Maui.Controls;
#endif

namespace Float.Core.Notifications
{
    /// <summary>
    /// Displays alerts to the user in response to notifications.
    /// </summary>
    public class AlertNotificationHandler : INotificationHandler
    {
        readonly Page context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertNotificationHandler"/> class.
        /// </summary>
        /// <param name="context">Optional context for the alerts.</param>
        public AlertNotificationHandler(Page context = null)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public virtual void NotifyError(string localizedErrorDescription, string suggestion = null)
        {
            DisplayAlert(localizedErrorDescription, suggestion);
        }

        /// <inheritdoc />
        public virtual void NotifyException(Exception e, string implication = null)
        {
            DisplayAlert(implication, FormatException(e));
        }

        /// <inheritdoc />
        public virtual string FormatException(Exception e)
        {
            if (e == null)
            {
                return null;
            }

            if (e is AggregateException aggregateException)
            {
                return FormatException(aggregateException.InnerException);
            }

            if (e.IsOfflineException())
            {
                return FloatStrings.NoInternetMessage;
            }

            return e.Message;
        }

        /// <summary>
        /// Displays an alert message to the user with the specified title and message.
        /// </summary>
        /// <param name="title">The title of the alert.</param>
        /// <param name="message">The body of the alert.</param>
        protected void DisplayAlert(string title, string message)
        {
            DeviceProxy.BeginInvokeOnMainThread(() =>
            {
                #if NET8_0_OR_GREATER
                var page = context ?? Application.Current.Windows[0].Page;
                #else
                var page = context ?? Application.Current.MainPage;
                #endif
                page.DisplayAlert(title, message, Localize.String("OK"));
            });
        }
    }
}
