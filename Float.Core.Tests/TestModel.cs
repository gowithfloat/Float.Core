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
    public class TestModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged { add { } remove { } }

        internal TestModel(int id) => this.Mid = id;

        internal int Mid { get; }

        public override string ToString() => $"<{GetType()}: {nameof(Mid)}={Mid}>";
    }
}
