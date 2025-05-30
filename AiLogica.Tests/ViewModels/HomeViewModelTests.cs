using AiLogica.ViewModels;
using Xunit;

namespace AiLogica.Tests.ViewModels
{
    public class HomeViewModelTests
    {
        [Fact]
        public void WelcomeMessage_ShouldHaveDefaultValue()
        {
            // Arrange
            var viewModel = new HomeViewModel();

            // Act & Assert
            Assert.Equal("Logic Gate Design Canvas", viewModel.WelcomeMessage);
        }

        [Fact]
        public void WelcomeMessage_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            var propertyChangedRaised = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(HomeViewModel.WelcomeMessage))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.WelcomeMessage = "New Message";

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal("New Message", viewModel.WelcomeMessage);
        }

        [Fact]
        public void SelectGate_ShouldSetSelectedGateAndDraggingState()
        {
            // Arrange
            var viewModel = new HomeViewModel();

            // Act
            viewModel.SelectGate("OR");

            // Assert
            Assert.Equal("OR", viewModel.SelectedGate);
            Assert.True(viewModel.IsDragging);
        }

        [Fact]
        public void UpdateMousePosition_ShouldSetMouseCoordinates()
        {
            // Arrange
            var viewModel = new HomeViewModel();

            // Act
            viewModel.UpdateMousePosition(100, 200);

            // Assert
            Assert.Equal(100, viewModel.MouseX);
            Assert.Equal(200, viewModel.MouseY);
        }

        [Fact]
        public void PlaceGate_WithSelectedGate_ShouldAddToPlacedGatesAndClearSelection()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            viewModel.SelectGate("OR");

            // Act
            viewModel.PlaceGate(150, 250);

            // Assert
            Assert.Single(viewModel.PlacedGates);
            Assert.Equal("OR", viewModel.PlacedGates[0].Type);
            Assert.Equal(150, viewModel.PlacedGates[0].X);
            Assert.Equal(250, viewModel.PlacedGates[0].Y);
            Assert.Null(viewModel.SelectedGate);
            Assert.False(viewModel.IsDragging);
        }

        [Fact]
        public void PlaceGate_WithoutSelectedGate_ShouldNotAddToPlacedGates()
        {
            // Arrange
            var viewModel = new HomeViewModel();

            // Act
            viewModel.PlaceGate(150, 250);

            // Assert
            Assert.Empty(viewModel.PlacedGates);
        }

        [Fact]
        public void CancelDrag_ShouldClearSelectionAndDraggingState()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            viewModel.SelectGate("OR");

            // Act
            viewModel.CancelDrag();

            // Assert
            Assert.Null(viewModel.SelectedGate);
            Assert.False(viewModel.IsDragging);
        }

        [Fact]
        public void PlacedGates_PropertyChanged_ShouldBeRaisedWhenGateIsPlaced()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            viewModel.SelectGate("OR");
            var propertyChangedRaised = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(HomeViewModel.PlacedGates))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.PlaceGate(150, 250);

            // Assert
            Assert.True(propertyChangedRaised);
        }
    }
}