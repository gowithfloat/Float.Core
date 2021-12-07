using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Float.Core.Collections
{
    /// <summary>
    /// A collection of objects which each implement INotifyPropertyChanged.
    /// When element properties change, ChildPropertyChangeEvent is sent.
    /// Subclasses can also override OnChildPropertyChanged to update based on new values.
    /// </summary>
    /// <typeparam name="T">The type of elements in this collection. Must be able to notify of property changes.</typeparam>
    public class ObservableElementCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableElementCollection{T}"/> class.
        /// </summary>
        public ObservableElementCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableElementCollection{T}"/> class.
        /// </summary>
        /// <param name="enumerable">Enumerable to use to populate this collection.</param>
        public ObservableElementCollection(IEnumerable<T> enumerable)
        {
            if (enumerable != null)
            {
                AddRange(enumerable);
            }
        }

        /// <summary>
        /// Occurs when a child property changes.
        /// </summary>
        public event PropertyChangedEventHandler ChildPropertyChanged;

        /// <summary>
        /// Gets a range of elements as a new observable element property collection.
        /// </summary>
        /// <returns>The range of elements.</returns>
        /// <param name="index">Index of first element to retrieve.</param>
        /// <param name="count">Number of elements to retrieve.</param>
        public ObservableElementCollection<T> GetRange(int index, int count)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count < 0 || count > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var list = new List<T>();
            var end = index + count;

            if (end > Count)
            {
                throw new ArgumentException("Desired end of range exceeds collection size.");
            }

            for (var i = index; i < end; i++)
            {
                list.Add(this[i]);
            }

            return new ObservableElementCollection<T>(list);
        }

        /// <summary>
        /// Adds all elements of a collection to this collection.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null || !collection.Any())
            {
                return;
            }

            foreach (var element in collection)
            {
                Items.Add(element);
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <inheritdoc />
        protected override void ClearItems()
        {
            foreach (T item in Items)
            {
                if (item != null)
                {
                    item.PropertyChanged -= HandleChildPropertyChanged;
                }
            }

            base.ClearItems();
        }

        /// <summary>
        /// Subscribes to property change events on all new children.
        /// Unsubscribes from events on removed children.
        /// </summary>
        /// <param name="e">Arguments related to the change in the collection.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in Items)
                {
                    if (item != null)
                    {
                        // Remove the handler before adding it in case this item
                        // was already being observed
                        item.PropertyChanged -= HandleChildPropertyChanged;
                        item.PropertyChanged += HandleChildPropertyChanged;
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (T item in e.NewItems)
                {
                    if (item != null)
                    {
                        item.PropertyChanged += HandleChildPropertyChanged;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (T item in e.OldItems)
                {
                    if (item != null)
                    {
                        item.PropertyChanged -= HandleChildPropertyChanged;
                    }
                }
            }

            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Handles property changed events on child elements.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">Arguments related to the event.</param>
        protected virtual void HandleChildPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            ChildPropertyChanged?.Invoke(sender, args);
        }
    }
}
