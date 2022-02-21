using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Collections;

namespace Float.Core.ViewModels
{
    /// <summary>
    /// Extends <see cref="BaseCollectionViewModel{TModel, TViewModel}"/> by inserting new view models at the same index as the underlying model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model object associated with the viewmodel in this collection.</typeparam>
    /// <typeparam name="TViewModel">The type of the viewmodel in this collection.</typeparam>
    public class OrderedBaseCollectionViewModel<TModel, TViewModel> : BaseCollectionViewModel<TModel, TViewModel> where TModel : INotifyPropertyChanged where TViewModel : ViewModel<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedBaseCollectionViewModel{TModel, TViewModel}"/> class.
        /// </summary>
        /// <param name="modelCollectionTask">The task which resolves to a model collection.</param>
        protected OrderedBaseCollectionViewModel(Task<IEnumerable<TModel>> modelCollectionTask) : base(modelCollectionTask)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedBaseCollectionViewModel{TModel, TViewModel}"/> class.
        /// </summary>
        /// <param name="modelCollection">Model collection.</param>
        /// <param name="filter">An optional filter to apply to the model collection.</param>
        protected OrderedBaseCollectionViewModel(IEnumerable<TModel> modelCollection, IFilter<TModel> filter = null) : base(modelCollection, filter)
        {
        }

        /// <inheritdoc />
        protected override void AddViewModels(IEnumerable modelCollection)
        {
            if (modelCollection == null)
            {
                return;
            }

            foreach (var model in modelCollection.OfType<TModel>())
            {
                if (ConvertModelToViewModel(model) is TViewModel viewModel)
                {
                    if (Models is IList list && list.IndexOf(model) is int idx && idx > -1)
                    {
                        ViewModels.Insert(idx, viewModel);
                    }
                    else
                    {
                        ViewModels.Add(viewModel);
                    }

                    if (HasSubscribers)
                    {
                        model.PropertyChanged += OnModelPropertyChanged;
                    }
                }
            }
        }
    }
}
