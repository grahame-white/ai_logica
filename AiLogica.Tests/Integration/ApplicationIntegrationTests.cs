using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Threading.Tasks;

namespace AiLogica.Tests.Integration;

/// <summary>
/// Requirements Traceability: Integration tests covering application startup and core functionality
/// FR-10.1-FR-10.5: Implements testing requirements (one assert, descriptive names, AAA pattern)
/// FR-1, FR-2: Validates core UI components are rendered properly
/// See TRACEABILITY_MATRIX.md for complete mapping
/// </summary>
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
    public void ViewModel_ShouldBeRegistered_AsScoped()
    {
        // Arrange & Act
        var serviceProvider = _factory.Services;

        // Create separate scopes to verify scoped registration
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();

        var viewModel1 = scope1.ServiceProvider.GetRequiredService<AiLogica.ViewModels.HomeViewModel>();
        var viewModel2 = scope2.ServiceProvider.GetRequiredService<AiLogica.ViewModels.HomeViewModel>();

        // Assert
        Assert.NotSame(viewModel1, viewModel2); // Should be different instances across scopes
    }
}
