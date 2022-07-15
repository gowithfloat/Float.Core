using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Collections;
using Float.Core.Compatibility;
using Float.Core.Extensions;

namespace Float.Core.ViewModels
{
    /// <summary>
    /// Base collection view model.
    /// Note that in order for this to work, TViewModel has to have a constructor that takes only TModel as a parameter.
    /// </summary>
    /// <typeparam name="TModel">The type of the model object associated with the viewmodel in this collection.</typeparam>
    /// <typeparam name="TViewModel">The type of the viewmodel in this collection.</typeparam>
    public abstract class BaseCollectionViewModel<TModel, TViewModel> : BaseViewModel, IEnumerable<TViewModel>, INotifyCollectionChanged where TModel : INotifyPropertyChanged where TViewModel : ViewModel<TModel>
    {
        NotifyCollectionChangedEventHandler collectionChangedHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCollectionViewModel{TModel, TViewModel}"/> class.
        /// </summary>
        /// <param name="modelCollectionTask">The task which resolves to a model collection.</param>
        protected BaseCollectionViewModel(Task<IEnumerable<TModel>> modelCollectionTask) : this(new ObservableCollection<TModel>())
        {
            modelCollectionTask.OnSuccess((task) =>
            {
                if (Models is ObservableCollection<TModel> collection)
                {
                    foreach (TModel model in task.Result)
                    {
                        collection.Add(model);
                    }
                }
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCollectionViewModel{TModel, TViewModel}"/> class.
        /// </summary>
        /// <param name="modelCollection">Model collection.</param>
        /// <param name="filter">An optional filter to apply to the model collection.</param>
        protected BaseCollectionViewModel(IEnumerable<TModel> modelCollection, IFilter<TModel> filter = null)
        {
            Models = modelCollection ?? throw new ArgumentNullException(nameof(modelCollection));

            // When using Realm, the CollectionChanged event seems to be called
            // less reliably if the collection is iterated over before the event
            // handler is added...so we add the handler first before iterating.
            // ¯\_(ツ)_/¯
            if (modelCollection is INotifyCollectionChanged bindableCollection)
            {
                ModelCollectionChangeProxy.Subscribe(bindableCollection, this);
            }

            AddViewModels(modelCollection);

            if (filter != null)
            {
                Filter = filter;
                filter.FilterChanged += OnFilterChanged;
            }
        }

        /// <summary>
        /// Occurs the list of filtered elements changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                var alreadyHadSubscribers = HasSubscribers;
                collectionChangedHandler += value;
                if (!alreadyHadSubscribers)
                {
                    OnObservingBegan();
                }
            }

            remove
            {
                collectionChangedHandler -= value;
                if (!HasSubscribers)
                {
                    OnObservingEnded();
                }
            }
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public IFilter<TModel> Filter { get; private set; }

        /// <summary>
        /// Gets all view models, regardless if they're impacted by the current filter.
        /// </summary>
        /// <value>All elements.</value>
        public IEnumerable<TViewModel> AllElements => ViewModels;

        /// <summary>
        /// Gets only the filtered view models.
        /// If no filter is defined, then it returns all elements.
        /// </summary>
        /// <value>The filtered elements.</value>
        public IEnumerable<TViewModel> Elements
        {
            get
            {
                if (Filter == null)
                {
                    return ViewModels;
                }

                return ViewModels.Where(vm => Filter.Matches(vm.UnderlyingModel));
            }
        }

        /// <summary>
        /// Gets the original collection of models.
        /// </summary>
        /// <value>The underlying collection of models that are represented by this view model collection.</value>
        protected IEnumerable<TModel> Models { get; }

        /// <summary>
        /// Gets the view model representations of the models.
        /// </summary>
        /// <value>The view models.</value>
        protected List<TViewModel> ViewModels { get; } = new ();

        /// <inheritdoc />
        protected override bool HasSubscribers => base.HasSubscribers || collectionChangedHandler?.GetInvocationList().Length > 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCollectionViewModel{TModel, TViewModel}"/> class.
        /// Gets the view model for model.
        /// </summary>
        /// <returns>The view model.</returns>
        /// <param name="model">The Model.</param>
        public TViewModel GetViewModelForModel(TModel model)
        {
            if (model == null)
            {
                return null;
            }

            return AllElements.FirstOrDefault((arg) => arg.UnderlyingModel.Equals(model));
        }

        /// <inheritdoc />
        public IEnumerator<TViewModel> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        /// <summary>
        /// Clears all the view models.
        /// </summary>
        public void Clear()
        {
            if (Models is ICollection<TModel> collection)
            {
                collection.Clear();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        /// <summary>
        /// Create a viewmodel for the given model.
        /// </summary>
        /// <returns>The viewmodel for the given model.</returns>
        /// <param name="model">The model for which to create a viewmodel.</param>
        protected virtual TViewModel ConvertModelToViewModel(TModel model)
        {
            // get a reference to the type of the view model
            var type = typeof(TViewModel);

            // create constructor parameters that take only the model object
            var args = new object[] { model };

            // attempt to invoke a constructor on the view model that takes only the model
            // if that constructor doesn't exist, this will throw an exception
            return Activator.CreateInstance(type, args) as TViewModel;
        }

        /// <inheritdoc />
        protected override void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnModelPropertyChanged(sender, e);
            //// TODO: Filter results may have changed
        }

        /// <summary>
        /// Invoked when the underlying collection of models changes.
        /// Specifically, when a model is added/removed.
        /// The collection of view models is updated to reflect these changes.
        /// </summary>
        /// <param name="sender">The underlying collection of models.</param>
        /// <param name="args">Collection change arguments.</param>
        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args?.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    // It's very tempting to cast these...but it won't work.
                    AddViewModels(args.NewItems);
                    RemoveViewModels(args.OldItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                // In Realm, args.OldItems contains null objects on remove which is completely useless.
                // So, we have to treat removing a model as reseting the models.
                case NotifyCollectionChangedAction.Remove:
                    ClearViewModels();
                    AddViewModels(sender as IEnumerable<TModel>);
                    break;
                default:
                    // Do nothing
                    return;
            }

            HandleViewModelsChanged();
        }

        /// <summary>
        /// Refreshes the view model order based on the order of the models.
        /// </summary>
        protected virtual void RefreshViewModelOrder()
        {
            ClearViewModels();
            AddViewModels(Models);
        }

        /// <summary>
        /// Handles a change in the filter settings.
        /// </summary>
        /// <param name="sender">The filter that changed.</param>
        /// <param name="args">Not used.</param>
        protected virtual void OnFilterChanged(object sender, EventArgs args)
        {
            OnElementsChanged();
        }

        /// <summary>
        /// Responds to a change in the elements in this collection.
        /// Invokes a property change on Elements so that any bound ListViews automatically update.
        /// </summary>
        protected virtual void OnElementsChanged()
        {
            DeviceProxy.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(Elements));
                collectionChangedHandler?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }

        /// <summary>
        /// Adds view models for the specified models.
        /// </summary>
        /// <remarks>
        /// The caller is responsible for triggering a change event on Elements and AllElements.
        /// </remarks>
        /// <param name="modelCollection">Model collection.</param>
        protected virtual void AddViewModels(IEnumerable modelCollection)
        {
            if (modelCollection == null)
            {
                return;
            }

            foreach (var model in modelCollection.OfType<TModel>())
            {
                if (ConvertModelToViewModel(model) is TViewModel viewModel)
                {
                    ViewModels.Add(viewModel);

                    if (HasSubscribers)
                    {
                        model.PropertyChanged += OnModelPropertyChanged;
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnObservingBegan()
        {
            base.OnObservingBegan();

            foreach (var model in Models)
            {
                model.PropertyChanged += OnModelPropertyChanged;
            }
        }

        /// <inheritdoc />
        protected override void OnObservingEnded()
        {
            base.OnObservingEnded();

            foreach (var model in Models)
            {
                model.PropertyChanged -= OnModelPropertyChanged;
            }
        }

        /// <summary>
        /// Handles when the view models changed (add/removed/replaced).
        /// </summary>
        void HandleViewModelsChanged()
        {
            OnElementsChanged();
            OnPropertyChanged(nameof(AllElements));
        }

        /// <summary>
        /// Removes the view models.
        /// </summary>
        /// <remarks>
        /// The caller is responsible for triggering a change event on Elements and AllElements.
        /// </remarks>
        /// <param name="modelCollection">Model collection.</param>
        void RemoveViewModels(IEnumerable modelCollection)
        {
            if (modelCollection == null)
            {
                return;
            }

            foreach (var model in modelCollection.OfType<TModel>())
            {
                var vms = ViewModels.Where(vm => model.Equals(vm.UnderlyingModel));

                foreach (var vm in vms)
                {
                    ViewModels.Remove(vm);
                }

                model.PropertyChanged -= OnModelPropertyChanged;
            }
        }

        /// <summary>
        /// Clears all the view models.
        /// </summary>
        /// <remarks>
        /// The caller is responsible for triggering a change event on Elements and AllElements.
        /// </remarks>
        void ClearViewModels()
        {
            foreach (var viewModel in ViewModels)
            {
                viewModel.UnderlyingModel.PropertyChanged -= OnModelPropertyChanged;
            }

            ViewModels.Clear();
        }

        /// <summary>
        /// Event proxy for observing change events on the model collection.
        /// This helps ensure that there is no retain cycle betewen the collection view model
        /// and the orignal model collection being observed by adding a weak event listener.
        /// </summary>
        /// <remarks>
        /// When using Realm, we were having issues where it seemed like Realm held a reference
        /// to any results collection, which in turn caused all event listeners on that
        /// collection to be retained. Using this proxy allows the collection view model to be
        /// collected by garbage collector. However, the ModelCollectionChangeProxy instance
        /// may get left behind--potentially indefintely. We attempt to handle that by
        /// having the proxy remove itself from listening for changes on the collection if
        /// the original listener is no longer alive, but there's no guarentee that will happen.
        /// </remarks>
        class ModelCollectionChangeProxy
        {
            /// <summary>
            /// The source collection.
            /// </summary>
            readonly WeakReference<INotifyCollectionChanged> source;

            /// <summary>
            /// The original listener for change events.
            /// </summary>
            readonly WeakReference<BaseCollectionViewModel<TModel, TViewModel>> listener;

            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="ModelCollectionChangeProxy"/> class.
            /// </summary>
            /// <param name="source">The source collection.</param>
            /// <param name="listener">The listener for change events.</param>
            ModelCollectionChangeProxy(INotifyCollectionChanged source, BaseCollectionViewModel<TModel, TViewModel> listener)
            {
                this.source = new WeakReference<INotifyCollectionChanged>(source);
                this.listener = new WeakReference<BaseCollectionViewModel<TModel, TViewModel>>(listener);

                source.CollectionChanged += HandleEvent;
            }

            /// <summary>
            /// Subscribe the specified source and listener.
            /// </summary>
            /// <returns>The proxy instance.</returns>
            /// <param name="source">The source collection.</param>
            /// <param name="listener">The listener.</param>
            internal static ModelCollectionChangeProxy Subscribe(INotifyCollectionChanged source, BaseCollectionViewModel<TModel, TViewModel> listener)
            {
                return new ModelCollectionChangeProxy(source, listener);
            }

            /// <summary>
            /// Handles the change event on the collection.
            /// </summary>
            /// <param name="sender">The sender of the event.</param>
            /// <param name="args">Event arguments.</param>
            void HandleEvent(object sender, NotifyCollectionChangedEventArgs args)
            {
                if (!source.TryGetTarget(out INotifyCollectionChanged sourceTarget))
                {
                    return;
                }

                if (!listener.TryGetTarget(out BaseCollectionViewModel<TModel, TViewModel> listenerTarget) || listenerTarget == null)
                {
                    // If the listener is no longer around,
                    // then there's no point in continuing to
                    // observe events on the original collection.
                    sourceTarget.CollectionChanged -= HandleEvent;
                    return;
                }

                listenerTarget.OnCollectionChanged(sender, args);
            }
        }
    }
}
