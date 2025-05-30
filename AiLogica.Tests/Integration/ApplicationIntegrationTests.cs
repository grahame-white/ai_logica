using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Threading.Tasks;

namespace AiLogica.Tests.Integration
{
    public class ApplicationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApplicationIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Application_ShouldStart_Successfully()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            
            // Check that the main components are rendered
            Assert.Contains("Gate Palette", content);
            Assert.Contains("OR", content);
            Assert.Contains("Logic Gate Design Canvas", content);
        }

        [Fact]
        public void ViewModel_ShouldBeRegistered_AsSingleton()
        {
            // Arrange & Act
            var serviceProvider = _factory.Services;
            var viewModel1 = serviceProvider.GetRequiredService<AiLogica.ViewModels.HomeViewModel>();
            var viewModel2 = serviceProvider.GetRequiredService<AiLogica.ViewModels.HomeViewModel>();

            // Assert
            Assert.Same(viewModel1, viewModel2); // Should be the same instance
        }
    }
}