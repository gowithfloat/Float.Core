using System;
using Xamarin.Forms;

namespace Float.Core.UX
{
    /// <summary>
    /// Default implementation of ICoordinator.
    /// This can not be used directly; instead, it should be subclassed.
    /// </summary>
    public abstract class Coordinator : ICoordinator
    {
        /// <summary>
        /// Internal flag tracking whether or not this coordinator has been started.
        /// </summary>
        bool isStarted;

        /// <summary>
        /// Internal flag tracking whether or not this coordinator has been finished.
        /// </summary>
        bool isFinished;

        /// <summary>
        /// The managed page.
        /// When this page is closed, the coordinator will automatically finish.
        /// </summary>
        Page managedPage;

        /// <summary>
        /// Occurs when this coordinator is started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Occurs when this coordinator is finished.
        /// </summary>
        public event EventHandler<EventArgs> Finished;

        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        protected INavigationContext NavigationContext { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Coordinator"/> is started.
        /// </summary>
        /// <value><c>true</c> if is started; otherwise, <c>false</c>.</value>
        protected bool IsStarted => isStarted;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Coordinator"/> is finished.
        /// </summary>
        /// <value><c>true</c> if is finished; otherwise, <c>false</c>.</value>
        protected bool IsFinished => isFinished;

        /// <inheritdoc />
        public virtual void Start(INavigationContext context)
        {
            NavigationContext = context;

            Start();
        }

        /// <summary>
        /// Start this coordinator.
        /// By default, this just sends the started event.
        /// </summary>
        public virtual void Start()
        {
            if (isStarted)
            {
                throw new CoordinatorException("You cannot start a coordinator that has already been started.");
            }

            if (isFinished)
            {
                throw new CoordinatorException("You cannot start a finished coordinator.");
            }

            // If this is a managed coordinator, present the initial page
            // and begin observing for changes in status
            var page = PresentInitialPage();
            BeginObservingManagedPage(page);

            var handler = Started;

            if (handler != null)
            {
                handler.Invoke(this, null);
            }

            isStarted = true;
        }

        /// <summary>
        /// Returning a page here will allow the coordinator to automatically
        /// manage itself based on the state of the UI.
        /// The coordinator will automatically finish when the initial page
        /// is removed from the navigation stack.
        /// </summary>
        /// <returns>The initial page.</returns>
        protected virtual Page PresentInitialPage()
        {
            // This is an opt-in behavior (for now)
            return null;
        }

        /// <summary>
        /// Internal method to finish this coordinator.
        /// Implementing subclasses should override Finish to add any additional logic and call Finish when done.
        /// By default, this will create OnFinishedEvent and flip the IsFinished backing field to true.
        /// If you override this, be aware that some coordinators return null values.
        /// </summary>
        /// <param name="args">Arguments related to this event.</param>
        protected virtual void Finish(EventArgs args)
        {
            if (!isStarted)
            {
                throw new CoordinatorException("You cannot finish a coordinator that has not been started.");
            }

            if (isFinished)
            {
                throw new CoordinatorException("You cannot finish a coordinator that has already been finished.");
            }

            managedPage = null;

            if (NavigationContext != null)
            {
                NavigationContext.Navigated -= HandleNavigation;
            }

            var handler = Finished;

            if (handler != null)
            {
                handler.Invoke(this, args);
            }

            isFinished = true;
        }

        /// <summary>
        /// Convenience method to push a page onto the navigation stack.
        /// This should only be used once the app's main page is a MasterDetailPage.
        /// </summary>
        /// <param name="page">The page to add to the stack.</param>
        protected void PushPageAsync(Page page)
        {
            NavigationContext.PushPage(page);
        }

        /// <summary>
        /// Convenience method to pop the top page off the navigation stack.
        /// </summary>
        protected void PopPageAsync()
        {
            NavigationContext.PopPage();
        }

        /// <summary>
        /// Convenience method to push a modal onto the navigation stack.
        /// </summary>
        /// <param name="page">The page to add to the stack as a modal.</param>
        protected void PushModalAsync(Page page)
        {
            NavigationContext.PresentPage(page);
        }

        /// <summary>
        /// Convenience method to pop the current modal off of the navigation stack.
        /// Will only pop the modal if the modal stack has at least one page.
        /// </summary>
        protected void PopModalAsync()
        {
            NavigationContext.DismissPage();
        }

        /// <summary>
        /// This method will show the page in a modal if the targeted device is a phone, while using the detail portion of the screen for tablets.
        /// </summary>
        /// <param name="page">The page to be presented.</param>
        [Obsolete("Use navigationContext.Present instead")]
        protected void ShowTemporary(Page page)
        {
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                ShowPageInDetail(page);
            }
            else
            {
                PushModalAsync(page);
            }
        }

        /// <summary>
        /// Shows the page in a manner that will suit phone or tablet idioms.
        /// </summary>
        /// <param name="page">The page to be shown.</param>
        /// <param name="hideMaster">If true, the master will hide using the isPresented boolean. This only applies to phone. Defaults to false.</param>
        /// <param name="pushToMaster">If true, this will push the page onto the master side. This only applies to tablet.</param>
        [Obsolete("Use navigationContext.ShowPage instead")]
        protected void ShowPage(Page page, bool hideMaster = false, bool pushToMaster = false)
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                PushPageAsync(page);

                if (hideMaster)
                {
                    var main = Application.Current.MainPage as MasterDetailPage;
                    main.IsPresented = false;
                }
            }
            else
            {
                if (pushToMaster)
                {
                    var main = Application.Current.MainPage as MasterDetailPage;
                    var mainNav = main.Master as NavigationPage;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        mainNav.PushAsync(page);
                    });
                }
                else
                {
                    ShowPageInDetail(page);
                }
            }
        }

        /// <summary>
        /// Shows the page in the detail portion of the app.
        /// </summary>
        /// <param name="page">The page to show.</param>
        [Obsolete("Use navigationContext.ShowDetailPage instead")]
        protected void ShowPageInDetail(Page page)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var main = Application.Current.MainPage as MasterDetailPage;
                if (page is NavigationPage)
                {
                    main.Detail = page;
                }
                else
                {
                    main.Detail = new NavigationPage(page);
                }
            });
        }

        void BeginObservingManagedPage(Page page)
        {
            if (managedPage != null)
            {
                throw new CoordinatorException("Already managing a page");
            }

            managedPage = page;
            if (managedPage == null || NavigationContext == null)
            {
                return;
            }

            NavigationContext.Navigated += HandleNavigation;
        }

        void HandleNavigation(object sender, NavigationEventArgs args)
        {
            switch (args.Type)
            {
                case NavigationEventArgs.NavigationType.Popped:
                    if (args.Page == managedPage && !IsFinished)
                    {
                        Finish(EventArgs.Empty);
                    }

                    break;

                case NavigationEventArgs.NavigationType.Reset:
                    if (args.Page != managedPage && !IsFinished)
                    {
                        Finish(EventArgs.Empty);
                    }

                    break;
            }
        }
    }
}
