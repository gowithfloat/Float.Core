using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Float.Core.ViewModels;
using Xunit;
using System.Collections.ObjectModel;
using Xunit.Abstractions;

namespace Float.Core.Tests
{
    public class BaseCollectionViewModelTests : XunitContextBase
    {
        class SmallModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            internal SmallModel(int id) => this.Mid = id;

            internal int Mid { get; }

            public override string ToString() => $"<{GetType()}: {nameof(Mid)}={Mid}>";
        }

        class SmallViewModel : ViewModel<SmallModel>
        {
            public SmallViewModel(SmallModel model) : base(model)
            {
            }

            internal int Vmid => Model.Mid;

            public override string ToString() => $"<{GetType()}: {nameof(Vmid)}={Vmid}>";
        }

        class SmallCollectionViewModel : BaseCollectionViewModel<SmallModel, SmallViewModel>
        {
            internal SmallCollectionViewModel(IEnumerable<SmallModel> modelCollection) : base(modelCollection)
            {
            }
        }

        public BaseCollectionViewModelTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void TestViewModelConstructionOrder()
        {
            var coll = new ObservableCollection<SmallModel>
            {
                new SmallModel(0),
                new SmallModel(1)
            };

            var vm = new SmallCollectionViewModel(coll);

            Assert.Equal(2, vm.Count());
            Assert.Equal(0, vm.ElementAt(0).Vmid);
            Assert.Equal(1, vm.ElementAt(1).Vmid);
        }

        [Fact]
        public void TestViewModelAddOrder()
        {
            var coll = new ObservableCollection<SmallModel>
            {
                new SmallModel(0),
                new SmallModel(1)
            };

            var vm = new SmallCollectionViewModel(coll);

            coll.Add(new SmallModel(2));

            Assert.Equal(3, vm.Count());
            Assert.Equal(0, vm.ElementAt(0).Vmid);
            Assert.Equal(1, vm.ElementAt(1).Vmid);
            Assert.Equal(2, vm.ElementAt(2).Vmid);
        }

        [Fact]
        public void TestViewModelInsertOrder()
        {
            var coll = new ObservableCollection<SmallModel>
            {
                new SmallModel(0),
                new SmallModel(1)
            };

            var vm = new SmallCollectionViewModel(coll);

            coll.Insert(0, new SmallModel(-1));

            Assert.Equal(3, vm.Count());
            Assert.Equal(-1, vm.ElementAt(0).Vmid);
            Assert.Equal(0, vm.ElementAt(1).Vmid);
            Assert.Equal(1, vm.ElementAt(2).Vmid);
        }
    }
}
