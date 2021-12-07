using System;
using Float.Core.ViewModels;
using Xunit;

namespace Float.Core.Tests
{
    public class FavoriteModel : MockModel
	{
        bool isFavorited;

		public bool IsFavorited
		{
			get => isFavorited;
            set => SetField(ref isFavorited, value);
		}
	}

	public class FavoriteViewModel : ViewModel<FavoriteModel>
	{
		public FavoriteViewModel(FavoriteModel model) : base(model)
		{
		}

        public bool IsFavorited => Model.IsFavorited;

        [NotifyWhenPropertyChanges(nameof(FavoriteModel.IsFavorited))]
        public string AlsoDerivedFromIsFavorited { get; }
	}

    public class OverlappingNameViewModelTest
    {
        [Fact]
        public void TestModelAndViewModelWithSamePropertyName()
        {
            var model = new FavoriteModel();
            var viewModel = new FavoriteViewModel(model);

            var isFavoritedPropertyChangeCount = 0;
            var deriviedPropertyChangeCount = 0;

            viewModel.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(FavoriteViewModel.IsFavorited):
                        isFavoritedPropertyChangeCount++;
                        break;
                    case nameof(FavoriteViewModel.AlsoDerivedFromIsFavorited):
                        deriviedPropertyChangeCount++;
                        break;
                    default:
                        throw new Exception($"{e.PropertyName} is not an expected property name to change");
                }
            };

            Assert.Equal(0, isFavoritedPropertyChangeCount);
            Assert.Equal(0, deriviedPropertyChangeCount);

            model.IsFavorited = true;

			Assert.Equal(1, isFavoritedPropertyChangeCount);
			Assert.Equal(1, deriviedPropertyChangeCount);

            model.IsFavorited = true;

			Assert.Equal(1, isFavoritedPropertyChangeCount);
			Assert.Equal(1, deriviedPropertyChangeCount);

            model.IsFavorited = false;

			Assert.Equal(2, isFavoritedPropertyChangeCount);
			Assert.Equal(2, deriviedPropertyChangeCount);

            model.IsFavorited = false;

			Assert.Equal(2, isFavoritedPropertyChangeCount);
			Assert.Equal(2, deriviedPropertyChangeCount);
        }
    }
}
