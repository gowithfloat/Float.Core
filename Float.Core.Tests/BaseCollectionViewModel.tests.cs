using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Float.Core.ViewModels;
using Xunit;
using System.Collections.ObjectModel;
using Xunit.Abstractions;
using System;

namespace Float.Core.Tests
{
    public class BaseCollectionViewModelTests : XunitContextBase
    {
        class CollectionViewModel : BaseCollectionViewModel<TestModel, TestViewModel>
        {
            internal CollectionViewModel(IEnumerable<TestModel> modelCollection) : base(modelCollection)
            {
            }
        }

        public BaseCollectionViewModelTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void TestViewModelConstructionOrder()
        {
            var coll = new ObservableCollection<TestModel>
            {
                new TestModel(0),
                new TestModel(1)
            };

            var vm = new CollectionViewModel(coll);

            Assert.Equal(2, vm.Count());
            Assert.Equal(0, vm.ElementAt(0).Vmid);
            Assert.Equal(1, vm.ElementAt(1).Vmid);
        }

        [Fact]
        public void TestViewModelAddOrder()
        {
            var coll = new ObservableCollection<TestModel>
            {
                new TestModel(0),
                new TestModel(1)
            };

            var vm = new CollectionViewModel(coll);

            coll.Add(new TestModel(2));

            Assert.Equal(3, vm.Count());
            Assert.Equal(0, vm.ElementAt(0).Vmid);
            Assert.Equal(1, vm.ElementAt(1).Vmid);
            Assert.Equal(2, vm.ElementAt(2).Vmid);
        }

        [Fact]
        public void TestViewModelInsertOrder()
        {
            var coll = new ObservableCollection<TestModel>
            {
                new TestModel(0),
                new TestModel(1)
            };

            var vm = new CollectionViewModel(coll);

            coll.Insert(0, new TestModel(-1));

            Assert.Equal(3, vm.Count());
            Assert.Equal(0, vm.ElementAt(0).Vmid);
            Assert.Equal(1, vm.ElementAt(1).Vmid);
            Assert.Equal(-1, vm.ElementAt(2).Vmid);
        }

        [Fact]
        public void TestViewModelMultipleInsertOrder()
        {
            var m0 = new TestModel(0);
            var m1 = new TestModel(1);
            var m2 = new TestModel(2);
            var m3 = new TestModel(3);

            var coll = new ObservableCollection<TestModel>
            {
                m2,
            };

            var vm = new CollectionViewModel(coll);

            coll.Insert(m0.Mid, m0);
            Assert.Equal(m0, vm.ElementAt(1).UnderlyingModel);

            coll.Insert(m1.Mid, m1);
            Assert.Equal(m1, vm.ElementAt(2).UnderlyingModel);

            coll.Insert(m3.Mid, m3);
            Assert.Equal(m3, vm.ElementAt(3).UnderlyingModel);
        }
    }
}
