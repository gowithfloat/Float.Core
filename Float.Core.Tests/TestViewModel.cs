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
    public class TestViewModel : ViewModel<TestModel>
    {
        public TestViewModel(TestModel model) : base(model)
        {
        }

        internal int Vmid => Model.Mid;

        public override string ToString() => $"<{GetType()}: {nameof(Vmid)}={Vmid}>";
    }
}
