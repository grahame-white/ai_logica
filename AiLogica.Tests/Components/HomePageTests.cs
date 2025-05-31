using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AiLogica.ViewModels;
using AiLogica.Components.Pages;

namespace AiLogica.Tests.Components
{
    public class HomePageTests : TestContext
    {
        [Fact]
        public void HomePage_SelectOrGate_ShouldUpdateViewModel()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            Services.AddSingleton(viewModel);

            // Act - Test the ViewModel directly since interactive server mode doesn't work in unit tests
            viewModel.SelectGate("OR");

            // Assert
            Assert.Equal("OR", viewModel.SelectedGate);
            Assert.True(viewModel.IsDragging);
        }

        [Fact]
        public void HomePage_OrGateNotSelected_ShouldRenderCorrectly()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            Services.AddSingleton(viewModel);

            // Act
            var component = RenderComponent<Home>();

            // Assert - Check that the component renders the gate palette
            var orGateElement = component.Find(".gate-item:contains('OR')");
            Assert.NotNull(orGateElement);
            Assert.DoesNotContain("selected", orGateElement.GetAttribute("class") ?? "");
        }

        [Fact]
        public void HomePage_WithSelectedGate_ShouldRenderWithSelectedClass()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            viewModel.SelectGate("OR"); // Pre-select the gate
            Services.AddSingleton(viewModel);

            // Act
            var component = RenderComponent<Home>();

            // Assert - Check that the selected class is applied
            var orGateElement = component.Find(".gate-item:contains('OR')");
            Assert.Contains("selected", orGateElement.GetAttribute("class"));
        }
    }
}
