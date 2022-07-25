using System;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Compatibility;
using Xamarin.Forms;

namespace Float.Core.UX
{
    /// <summary>
    /// Navigation context for a navigation page.
    /// </summary>
    public class NavigationPageNavigationContext : INavigationContext
    {
        readonly NavigationPage navigationPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationPageNavigationContext"/> class.
        /// </summary>
        /// <param name="navigationPage">Navigation page.</param>
        public NavigationPageNavigationContext(NavigationPage navigationPage)
        {
            this.navigationPage = navigationPage ?? throw new ArgumentNullException(nameof(navigationPage));
            navigationPage.Popped += HandlePopped;
            navigationPage.Pushed += HandlePushed;
            navigationPage.PoppedToRoot += HandlePoppedToRoot;
        }

        /// <inheritdoc />
        public event EventHandler<NavigationEventArgs> Navigated;

        /// <inheritdoc />
        public bool IsAtRootPage
        {
            get
            {
                return navigationPage.CurrentPage == navigationPage.RootPage && !HasModal;
            }
        }

        bool HasModal
        {
            get
            {
                return navigationPage.Navigation.ModalStack.Any();
            }
        }

        /// <inheritdoc />
        public async Task ShowOverviewPageAsync(Page page, bool animated = true)
        {
            var isDeeplyNavigated = navigationPage.Navigation.NavigationStack.Count > 1;
            if (isDeeplyNavigated)
            {
                Reset(false);
                await PushPageAsync(page, false);
            }
            else
            {
                await PushPageAsync(page, animated);
            }
        }

        /// <inheritdoc />
        public async Task ShowDetailPageAsync(Page page, bool animated = true)
        {
            await PushPageAsync(page, animated);
        }

        /// <inheritdoc />
        public async Task DismissPageAsync(bool animated = true)
        {
            await DeviceProxy.InvokeOnMainThreadAsync(async () =>
            {
                if (HasModal)
                {
                    await navigationPage.Navigation.PopModalAsync(animated);
                }
            });
        }

        /// <inheritdoc />
        public async Task PushPageAsync(Page page, bool animated = true)
        {
            await DeviceProxy.InvokeOnMainThreadAsync(async () =>
            {
                await navigationPage.PushAsync(page, animated);
            });
        }

        /// <inheritdoc />
        public async Task PopPageAsync(bool animated = true)
        {
            await DeviceProxy.InvokeOnMainThreadAsync(async () =>
            {
                await navigationPage.PopAsync(animated);
            });
        }

        /// <inheritdoc />
        public async Task PresentPageAsync(Page page, bool animated = true)
        {
            await DeviceProxy.InvokeOnMainThreadAsync(async () =>
            {
                await navigationPage.Navigation.PushModalAsync(page, animated);
            });
        }

        /// <inheritdoc />
        public void Reset(bool animated)
        {
            DeviceProxy.BeginInvokeOnMainThread(async () =>
            {
                await navigationPage.PopToRootAsync(animated).ConfigureAwait(false);

                while (HasModal)
                {
                    await navigationPage.Navigation.PopModalAsync(animated).ConfigureAwait(false);
                }
            });
        }

        void HandlePopped(object sender, Xamarin.Forms.NavigationEventArgs e)
        {
            Navigated?.Invoke(navigationPage, new NavigationEventArgs(NavigationEventArgs.NavigationType.Popped, e.Page));
        }

        void HandlePoppedToRoot(object sender, Xamarin.Forms.NavigationEventArgs e)
        {
            Navigated?.Invoke(navigationPage, new NavigationEventArgs(NavigationEventArgs.NavigationType.Reset, navigationPage.RootPage));
        }

        void HandlePushed(object sender, Xamarin.Forms.NavigationEventArgs e)
        {
            Navigated?.Invoke(navigationPage, new NavigationEventArgs(NavigationEventArgs.NavigationType.Pushed, e.Page));
        }
    }
}
