using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using AiLogica.ViewModels;
using AiLogica.Components.Pages;
using AiLogica.Tests.Helpers;

namespace AiLogica.Tests.Components;
public class HomePageTests : TestContext
{
    [Theory]
    [InlineData("OR")]
    [InlineData("AND")]
    [InlineData("NOT")]
    public void HomePage_SelectGate_ShouldUpdateSelectedGateForDifferentTypes(string gateType)
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        Services.AddSingleton(viewModel);

        // Act - Test the ViewModel directly since interactive server mode doesn't work in unit tests
        viewModel.SelectGate(gateType);

        // Assert
        Assert.Equal(gateType, viewModel.SelectedGate);
    }

    [Theory]
    [InlineData("OR")]
    [InlineData("AND")]
    [InlineData("NOT")]
    public void HomePage_SelectGate_ShouldUpdateDraggingStateForDifferentTypes(string gateType)
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        Services.AddSingleton(viewModel);

        // Act - Test the ViewModel directly since interactive server mode doesn't work in unit tests
        viewModel.SelectGate(gateType);

        // Assert
        Assert.True(viewModel.IsDragging);
    }

    [Fact]
    public void HomePage_SelectOrGate_ShouldUpdateSelectedGate()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        Services.AddSingleton(viewModel);

        // Act - Test the ViewModel directly since interactive server mode doesn't work in unit tests
        viewModel.SelectGate("OR");

        // Assert
        Assert.Equal("OR", viewModel.SelectedGate);
    }

    [Fact]
    public void HomePage_SelectOrGate_ShouldUpdateDraggingState()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        Services.AddSingleton(viewModel);

        // Act - Test the ViewModel directly since interactive server mode doesn't work in unit tests
        viewModel.SelectGate("OR");

        // Assert
        Assert.True(viewModel.IsDragging);
    }

    [Fact]
    public void HomePage_OrGateNotSelected_ShouldRenderCorrectly()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
        var viewModel = TestHelper.CreateTestViewModel();
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
    public void HomePage_OrGate_ShouldDisplaySvgElement()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        Services.AddSingleton(viewModel);

        // Act
        var component = RenderComponent<Home>();

        // Assert - Check that the OR gate contains an SVG symbol instead of text
        var orGateElement = component.Find("[data-gate-type='OR']");
        var svgElement = orGateElement.QuerySelector("svg");
        Assert.NotNull(svgElement);
    }

    [Fact]
    public void HomePage_OrGate_ShouldDisplaySvgWithCorrectWidth()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        Services.AddSingleton(viewModel);

        // Act
        var component = RenderComponent<Home>();

        // Assert - Check that the OR gate contains an SVG symbol instead of text
        var orGateElement = component.Find("[data-gate-type='OR']");
        var svgElement = orGateElement.QuerySelector("svg");
        Assert.NotNull(svgElement);
        Assert.Equal("32", svgElement!.GetAttribute("width"));
    }

    [Fact]
    public void HomePage_OrGate_ShouldDisplaySvgWithCorrectHeight()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        Services.AddSingleton(viewModel);

        // Act
        var component = RenderComponent<Home>();

        // Assert - Check that the OR gate contains an SVG symbol instead of text
        var orGateElement = component.Find("[data-gate-type='OR']");
        var svgElement = orGateElement.QuerySelector("svg");
        Assert.NotNull(svgElement);
        Assert.Equal("24", svgElement!.GetAttribute("height"));
    }

    [Fact]
    public void HomePage_OrGate_ShouldNotDisplayRawText()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        Services.AddSingleton(viewModel);

        // Act
        var component = RenderComponent<Home>();

        // Assert - Check that the OR gate contains an SVG symbol instead of text
        var orGateElement = component.Find("[data-gate-type='OR']");

        // Verify that it doesn't contain the raw text "OR"
        Assert.DoesNotContain("OR", orGateElement.TextContent.Trim());
    }

    [Fact]
    public void HomePage_PlacedOrGate_ShouldDisplaySvgSymbol()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
        var viewModel = TestHelper.CreateTestViewModel();
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
