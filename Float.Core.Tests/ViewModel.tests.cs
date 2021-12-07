using System;
using System.Collections.Generic;
using System.Linq;
using Float.Core.Collections;
using Float.Core.ViewModels;
using Xunit;

namespace Float.Core.Tests
{
    public class ViewModelTests
    {
        /// <summary>
        /// A property change event test should be raised for a view model property
        /// if the underlying model raises a property changed event for a property
        /// with the exact same name.
        /// </summary>
        [Fact]
        public void RaisePropertyChangedEventWhenModelChanges()
        {
            var model = new SimpleModel
            {
                Name = "Test"
            };

            var viewModel = new SimpleViewModel(model);
            var changeCount = 0;

            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Name")
                {
                    changeCount++;
                }
            };

            Assert.Equal("Test", model.Name);
            Assert.Equal("Test", viewModel.Name);

            model.Name = "New Name";

            Assert.Equal("New Name", model.Name);
            Assert.NotEqual("Test", model.Name);
            Assert.Equal("New Name", viewModel.Name);
            Assert.NotEqual("Test", viewModel.Name);
            Assert.Equal(1, changeCount);
            Assert.NotEqual(2, changeCount);
        }

        /// <summary>
        /// If a view model property has a NotifyWhenPropertyChanges attribute,
        /// also raise property changed event args when the mapped property changes.
        /// </summary>
        [Fact]
        public void RaisePropertyChangedEventOnMappedProperty()
        {
            var model = new SimpleModel
            {
                Name = "Test"
            };

            var viewModel = new SimpleViewModel(model);
            var namePropertyChangedCount = 0;
            var fullNamePropertyChangedCount = 0;

            viewModel.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(SimpleViewModel.Name):
                        namePropertyChangedCount++;
                        break;
                    case nameof(SimpleViewModel.FullName):
                        fullNamePropertyChangedCount++;
                        break;
                    default:
                        throw new Exception($"{e.PropertyName} is not an expected property name to change");
                }
            };

            model.Name = "New value";

            // Both Name and FullName
            Assert.Equal(1, namePropertyChangedCount);
            Assert.Equal(1, fullNamePropertyChangedCount);
        }

        /// <summary>
        /// A property changed event should NOT be raised for a property
        /// on the view model when a matching property on the underlying model
        /// changes IF the view model property has a NotifyWhenPropertyChanged attribute.
        /// </summary>
        [Fact]
        public void RedirectPropertyChangedEventWhenAttributeSet()
        {
            var model = new SimpleModel
            {
                Name = "Test",
                Age = 17
            };

            var viewModel = new SimpleViewModel(model);
            var changeCount = 0;

            Assert.False(viewModel.IsAdult);

            viewModel.PropertyChanged += (sender, e) =>
            {
                changeCount++;
            };

            model.IsAdult = true;

            Assert.Equal(0, changeCount);

            model.Age = 18;
            Assert.Equal(2, changeCount);
            Assert.True(viewModel.IsAdult);
        }

        /// <summary>
        /// NotifyWhenPropertyChanges attribute should also be supported on collection view models.
        /// </summary>
        [Fact]
        public void CollectionSupportsUnderlyingModelPropertyChangedEvent()
        {
            var first = new SimpleModel
            {
                Name = "First"
            };

            var list = new List<SimpleModel>
            {
                first,
                new SimpleModel
                {
                    Name = "Second"
                },
                new SimpleModel
                {
                    Name = "Third"
                }
            };

            var viewModel = new SimpleCollectionViewModel(new ObservableElementCollection<SimpleModel>(list));
            var changeCount = 0;

            viewModel.PropertyChanged += (sender, e) =>
            {
                changeCount++;
                Assert.Equal("Names", e.PropertyName);
            };

            Assert.Equal(0, changeCount);
            Assert.Equal("First, Second, Third", viewModel.Names);

            first.Name = "Not First";

            Assert.Equal(1, changeCount);
            Assert.Equal("Not First, Second, Third", viewModel.Names);
        }

        [Fact]
        public void ObtainViewModelBasedOnModel()
        {
            var first = new SimpleModel
            {
                Name = "First"
            };

            var list = new List<SimpleModel>
            {
                first,
                new SimpleModel
                {
                    Name = "Second"
                },
                new SimpleModel
                {
                    Name = "Third"
                }
            };

            var viewModel = new SimpleCollectionViewModel(new ObservableElementCollection<SimpleModel>(list));

            Assert.Equal(viewModel.GetViewModelForModel(first), viewModel.AllElements.First());

            Assert.Null(viewModel.GetViewModelForModel(new SimpleModel
            {
                Name = "Not Me"
            }));
        }
    }

    class SimpleModel : MockModel
    {
        string name;
        int age;
        bool isAdult;

        public string Name
        {
            get => name;
            set => SetField(ref name, value);
        }

        public int Age
        {
            get => age;
            set => SetField(ref age, value);
        }

        public bool IsAdult
        {
            get => isAdult;
            set => SetField(ref isAdult, value);
        }
    }

    class SimpleViewModel : ViewModel<SimpleModel>
    {
        public SimpleViewModel(SimpleModel model) : base(model)
        {
        }

        public string Name => Model.Name;

        [NotifyWhenPropertyChanges(nameof(SimpleModel.Name))]
        public string FullName => Model.Name;

        public int Age => Model.Age;

        [NotifyWhenPropertyChanges(nameof(SimpleModel.Age))]
        public bool IsAdult => Model.Age >= 18;
    }

    class SimpleCollectionViewModel : BaseCollectionViewModel<SimpleModel, SimpleViewModel>
    {
        public SimpleCollectionViewModel(ObservableElementCollection<SimpleModel> model) : base(model)
        {
        }

        [NotifyWhenPropertyChanges(nameof(SimpleModel.Name))]
        public string Names
        {
            get
            {
                return string.Join(", ", Elements.Select(e => e.Name));
            }
        }
    }
}
