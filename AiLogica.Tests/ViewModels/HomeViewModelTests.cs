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
            Assert.Equal("Hello World", viewModel.WelcomeMessage);
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
    }
}