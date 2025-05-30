using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AiLogica.ViewModels;
using AiLogica.Components.Layout;

namespace AiLogica.Tests.Components
{
    public class MainLayoutTests : TestContext
    {
        [Fact]
        public void MainLayout_ClickOrGate_ShouldSelectGate()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            Services.AddSingleton(viewModel);

            // Act
            var component = RenderComponent<MainLayout>();
            var orGateButton = component.Find(".gate-item:contains('OR')");
            orGateButton.Click();

            // Assert
            Assert.Equal("OR", viewModel.SelectedGate);
            Assert.True(viewModel.IsDragging);

            // Check if the selected class is applied
            var orGateElement = component.Find(".gate-item:contains('OR')");
            Assert.Contains("selected", orGateElement.GetAttribute("class"));
        }

        [Fact]
        public void MainLayout_OrGateNotSelected_ShouldNotHaveSelectedClass()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            Services.AddSingleton(viewModel);

            // Act
            var component = RenderComponent<MainLayout>();

            // Assert
            var orGateElement = component.Find(".gate-item:contains('OR')");
            Assert.DoesNotContain("selected", orGateElement.GetAttribute("class") ?? "");
        }
    }
}