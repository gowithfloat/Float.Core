using System;
using CommunityToolkit.Mvvm.Messaging;
using Float.Core.Extensions;
using Float.Core.Messages;
#if NETSTANDARD
using Xamarin.Forms;
#else
using Microsoft.Maui;
using Microsoft.Maui.Controls;
#endif

namespace Float.Core.UI
{
    /// <summary>
    /// Base content page.
    /// </summary>
    public abstract class BaseContentPage : ContentPage
    {
        /// <summary>
        /// Binding for the Error property.
        /// </summary>
        public static readonly BindableProperty ErrorProperty = BindableProperty.Create(nameof(Error), typeof(Exception), typeof(ContentPage), null);

        /// <summary>
        /// Gets or sets the current error on the page.
        /// </summary>
        /// <value>The exception that has occurred which the user should be aware of.</value>
        protected Exception Error
        {
            get => GetValue(ErrorProperty) as Exception;
            set => SetValue(ErrorProperty, value);
        }

        /// <summary>
        /// Gets the name of the page to send to Google analytics.
        /// Defaults to the class name.
        /// </summary>
        /// <returns>The analytics page name.</returns>
        public virtual string GetAnalyticsPageName() => GetType().Name;

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();
            TrackPageView(GetAnalyticsPageName());
        }

        /// <inheritdoc />
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext is ViewModels.IPageViewModel viewModel)
            {
                this.SetBinding(TitleProperty, nameof(viewModel.Title));
                this.SetBinding(IsBusyProperty, nameof(viewModel.IsLoading));
                this.SetBinding(ErrorProperty, nameof(viewModel.Error), BindingMode.TwoWay);
            }
        }

        /// <summary>
        /// Tracks a page view.
        /// This is automatically invoked when the page appears (in response to OnAppearing).
        /// </summary>
        /// <param name="name">The page name.</param>
        protected void TrackPageView(string name)
        {
            WeakReferenceMessenger.Default.Send(new PageViewMessage(name, this));
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(IsBusy):
                    if (IsBusy)
                    {
                        OnStartLoading();
                    }
                    else
                    {
                        OnStopLoading();
                    }

                    break;
                case nameof(Error):
                    OnError(Error);
                    break;
            }
        }

        /// <summary>
        /// Invoked when the page starts to load content.
        /// </summary>
        protected virtual void OnStartLoading()
        {
        }

        /// <summary>
        /// Invoked when the page finishes loading content.
        /// </summary>
        protected virtual void OnStopLoading()
        {
        }

        /// <summary>
        /// Invoked when an error occurs--probably an error performing a user request.
        /// </summary>
        /// <param name="exception">The error that occurred.</param>
        protected virtual void OnError(Exception exception)
        {
            if (exception != null)
            {
                // TODO: Improve (e.g. localize, improve formatting, etc.)
                DisplayAlert("Error", exception.Message, "OK").OnSuccess((task) => Error = null);
            }
        }
    }
}
