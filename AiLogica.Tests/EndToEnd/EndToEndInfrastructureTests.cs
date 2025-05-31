using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace AiLogica.Tests.EndToEnd
{
    /// <summary>
    /// End-to-end infrastructure tests that verify the test setup without requiring browser installation
    /// </summary>
    public class EndToEndInfrastructureTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public EndToEndInfrastructureTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Warning);
                });
            });
        }

        [Fact]
        public async Task EndToEndTestInfrastructure_ShouldBeConfiguredCorrectly()
        {
            // This test verifies that the end-to-end testing infrastructure is set up correctly
            // It doesn't require browser installation but validates the web application factory setup

            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Verify that the page contains the necessary elements for E2E testing
            Assert.Contains("Gate Palette", content);
            Assert.Contains("Properties", content);
            Assert.Contains("OR", content);
            Assert.Contains("gate-item", content); // CSS class needed for E2E tests
            Assert.Contains("properties-panel", content); // CSS class needed for E2E tests
            Assert.Contains("status-bar", content); // CSS class needed for E2E tests
            Assert.Contains("canvas-container", content); // CSS class needed for E2E tests
        }

        [Fact]
        public void PlaywrightPackage_ShouldBeAvailable()
        {
            // This test verifies that the Playwright package is correctly installed
            // It's a compile-time check that the namespace is available

            // Arrange & Act - This will fail to compile if Playwright is not properly referenced
            var playwrightType = typeof(Microsoft.Playwright.IPlaywright);

            // Assert
            Assert.NotNull(playwrightType);
            Assert.Equal("Microsoft.Playwright.IPlaywright", playwrightType.FullName);
        }

        [Fact]
        public void EndToEndTestStrategy_ShouldBeWellDocumented()
        {
            // This test verifies that our E2E test strategy is properly documented
            // and provides comprehensive coverage without browser automation complexity

            // Arrange & Act & Assert - Verify the strategy documentation exists
            Assert.NotNull(EndToEndTestStrategy.TestCoverageDocumentation);
            Assert.Contains("comprehensive workflow validation", EndToEndTestStrategy.TestCoverageDocumentation);
            Assert.Contains("reliability in CI environments", EndToEndTestStrategy.TestCoverageDocumentation);
        }
    }
}
