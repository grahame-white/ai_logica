using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using AiLogica.ViewModels;
using AiLogica.Components.Pages;

namespace AiLogica.Tests.Components;
public class HomePageTests : TestContext
{
    private static HomeViewModel CreateTestViewModel() => new(NullLogger<HomeViewModel>.Instance);
    [Fact]
    public void HomePage_SelectOrGate_ShouldUpdateViewModel()
    {
        // Arrange
        var viewModel = CreateTestViewModel();
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
        var viewModel = CreateTestViewModel();
        Services.AddSingleton(viewModel);

        // Act
        var component = RenderComponent<Home>();

        // Assert - Check that the component renders the gate palette
        // Find the OR gate using its specific data attribute
        var orGateElement = component.Find("[data-gate-type='OR']");
        Assert.NotNull(orGateElement);
        Assert.DoesNotContain("selected", orGateElement.GetAttribute("class") ?? "");
    }

    [Fact]
    public void HomePage_WithSelectedGate_ShouldRenderWithSelectedClass()
    {
        // Arrange
        var viewModel = CreateTestViewModel();
        viewModel.SelectGate("OR"); // Pre-select the gate
        Services.AddSingleton(viewModel);

        // Act
        var component = RenderComponent<Home>();

        // Assert - Check that the selected class is applied
        // Find the OR gate using its specific data attribute
        var orGateElement = component.Find("[data-gate-type='OR']");
        Assert.Contains("selected", orGateElement.GetAttribute("class"));
    }

    [Fact]
    public void HomePage_OrGate_ShouldDisplaySvgSymbol()
    {
        // Arrange
        var viewModel = CreateTestViewModel();
        Services.AddSingleton(viewModel);

        // Act
        var component = RenderComponent<Home>();

        // Assert - Check that the OR gate contains an SVG symbol instead of text
        var orGateElement = component.Find("[data-gate-type='OR']");
        var svgElement = orGateElement.QuerySelector("svg");
        Assert.NotNull(svgElement);
        Assert.Equal("32", svgElement.GetAttribute("width"));
        Assert.Equal("24", svgElement.GetAttribute("height"));

        // Verify that it doesn't contain the raw text "OR"
        Assert.DoesNotContain("OR", orGateElement.TextContent.Trim());
    }

    [Fact]
    public void HomePage_PlacedOrGate_ShouldDisplaySvgSymbol()
    {
        // Arrange
        var viewModel = CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        Services.AddSingleton(viewModel);

        // Act
        var component = RenderComponent<Home>();

        // Assert - Check that placed OR gate contains an SVG symbol
        var placedGateElement = component.Find(".placed-gate");
        var svgElement = placedGateElement.QuerySelector("svg");
        Assert.NotNull(svgElement);

        // Verify that it doesn't contain the raw text "OR"
        Assert.DoesNotContain("OR", placedGateElement.TextContent.Trim());
    }

    [Fact]
    public void HomePage_NonOrGates_ShouldDisplayAsText()
    {
        // Test that the fallback behavior for non-OR gates still displays text
        // (This is tested by manually creating a gate through ViewModel since other gates aren't functional in UI)

        // Arrange
        var viewModel = CreateTestViewModel();
        viewModel.PlacedGates.Add(new PlacedGate { Type = "AND", X = 50, Y = 50, Id = Guid.NewGuid() });
        Services.AddSingleton(viewModel);

        // Act
        var component = RenderComponent<Home>();

        // Assert - Check that non-OR gate displays as text
        var placedGateElement = component.Find(".placed-gate");
        Assert.Contains("AND", placedGateElement.TextContent);

        // Should not contain SVG for non-OR gates
        var svgElement = placedGateElement.QuerySelector("svg");
        Assert.Null(svgElement);
    }
}
