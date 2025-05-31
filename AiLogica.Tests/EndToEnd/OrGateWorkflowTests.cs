using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace AiLogica.Tests.EndToEnd
{
    /// <summary>
    /// End-to-end tests for the core OR gate functionality
    /// These tests validate the complete user workflow from browser interaction to visual feedback
    /// </summary>
    [Trait("Category", "EndToEnd")]
    public class OrGateWorkflowTests : EndToEndTestBase
    {
        [Fact]
        public async Task ApplicationHomePage_ShouldLoadSuccessfully()
        {
            // Arrange & Act
            await NavigateToHomePageAsync();

            // Assert - Check that main components are present
            await AssertElementContainsTextAsync("h2", "Logic Gate Design Canvas");
            await AssertElementContainsTextAsync("body", "Gate Palette");
            await AssertElementContainsTextAsync("body", "Properties");
            await AssertElementContainsTextAsync("body", "OR");
        }

        [Fact]
        public async Task OrGateSelection_ShouldHighlightGateAndEnableDragging()
        {
            // Arrange
            await NavigateToHomePageAsync();

            // Act - Click the OR gate in the palette
            var orGateButton = await WaitForElementAsync(".gate-item:has-text('OR')");
            await orGateButton.ClickAsync();

            // Assert - Check that OR gate is selected and highlighted
            var selectedGate = Page.Locator(".gate-item.selected:has-text('OR')");
            await selectedGate.WaitForAsync(new LocatorWaitForOptions { Timeout = 2000 });

            // Check properties panel shows selection
            await AssertElementContainsTextAsync(".properties-panel", "Selected: OR");
            await AssertElementContainsTextAsync(".properties-panel", "Status: Dragging");
        }

        [Fact]
        public async Task OrGateDragging_ShouldFollowMouseMovement()
        {
            // Arrange
            await NavigateToHomePageAsync();
            var orGateButton = await WaitForElementAsync(".gate-item:has-text('OR')");
            await orGateButton.ClickAsync();

            // Act - Move mouse over the canvas area
            var canvas = await WaitForElementAsync(".canvas-container");
            await canvas.HoverAsync(new LocatorHoverOptions { Position = new Position { X = 200, Y = 150 } });

            // Wait a moment for the dragging gate to appear
            await Task.Delay(100);

            // Assert - Check that dragging gate is visible and positioned correctly
            var draggingGate = Page.Locator(".dragging-gate");
            await draggingGate.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 2000
            });

            // Verify the dragging gate contains OR text
            await AssertElementContainsTextAsync(".dragging-gate", "OR");
        }

        [Fact]
        public async Task OrGatePlacement_ShouldPlaceGateAndUpdateStatus()
        {
            // Arrange
            await NavigateToHomePageAsync();
            var orGateButton = await WaitForElementAsync(".gate-item:has-text('OR')");
            await orGateButton.ClickAsync();

            // Act - Click on the canvas to place the gate
            var canvas = await WaitForElementAsync(".canvas-container");
            await canvas.ClickAsync(new LocatorClickOptions { Position = new Position { X = 300, Y = 200 } });

            // Assert - Check that gate is placed
            var placedGate = Page.Locator(".placed-gate");
            await placedGate.WaitForAsync(new LocatorWaitForOptions { Timeout = 2000 });
            await AssertElementContainsTextAsync(".placed-gate", "OR");

            // Check that status bar shows gate count
            await AssertElementContainsTextAsync(".status-bar", "Gates: 1");

            // Note: Properties panel will still show OR selected because the gate remains selected for multiple placements
            await AssertElementContainsTextAsync(".properties-panel", "Selected: OR");
        }

        [Fact]
        public async Task OrGateSelection_CancelOnMouseLeave_ShouldClearSelection()
        {
            // Arrange
            await NavigateToHomePageAsync();
            var orGateButton = await WaitForElementAsync(".gate-item:has-text('OR')");
            await orGateButton.ClickAsync();

            // Verify gate is selected
            await AssertElementContainsTextAsync(".properties-panel", "Selected: OR");

            // Act - Move mouse outside canvas area to cancel drag (this triggers onmouseleave on canvas-container)
            var appLayout = Page.Locator(".app-layout");
            await appLayout.HoverAsync(new LocatorHoverOptions { Position = new Position { X = 10, Y = 10 } });

            // Wait a moment for the cancellation to process
            await Task.Delay(200);

            // Assert - Check that selection is cancelled
            await AssertElementContainsTextAsync(".properties-panel", "No gate selected");

            // Verify dragging gate is no longer visible (if it was visible)
            var draggingGates = await Page.Locator(".dragging-gate").CountAsync();
            Assert.Equal(0, draggingGates);
        }

        [Fact]
        public async Task MultipleOrGatePlacement_ShouldAllowPlacingMultipleGates()
        {
            // Arrange
            await NavigateToHomePageAsync();
            var orGateButton = await WaitForElementAsync(".gate-item:has-text('OR')");
            await orGateButton.ClickAsync();

            // Act - Place first gate
            var canvas = await WaitForElementAsync(".canvas-container");
            await canvas.ClickAsync(new LocatorClickOptions { Position = new Position { X = 200, Y = 150 } });

            // Place second gate (without reselecting OR button)
            await canvas.ClickAsync(new LocatorClickOptions { Position = new Position { X = 400, Y = 300 } });

            // Assert - Check that two gates are placed
            var placedGates = Page.Locator(".placed-gate");
            var gateCount = await placedGates.CountAsync();
            Assert.Equal(2, gateCount);

            // Check status bar shows correct count
            await AssertElementContainsTextAsync(".status-bar", "Gates: 2");
        }

        [Fact]
        public async Task EndToEndWorkflow_CompleteOrGateSelection_ShouldProvideConsistentUserExperience()
        {
            // This test validates the complete end-to-end workflow
            // and can serve as a high-level acceptance test

            // Arrange - Start with clean application state
            await NavigateToHomePageAsync();

            // Act & Assert - Complete workflow

            // 1. Verify initial state
            await AssertElementContainsTextAsync(".properties-panel", "No gate selected");

            // 2. Select OR gate
            var orGateButton = await WaitForElementAsync(".gate-item:has-text('OR')");
            await orGateButton.ClickAsync();
            await AssertElementContainsTextAsync(".properties-panel", "Selected: OR");

            // 3. Verify dragging state
            var canvas = await WaitForElementAsync(".canvas-container");
            await canvas.HoverAsync(new LocatorHoverOptions { Position = new Position { X = 250, Y = 175 } });
            await AssertElementContainsTextAsync(".dragging-gate", "OR");

            // 4. Place gate
            await canvas.ClickAsync(new LocatorClickOptions { Position = new Position { X = 250, Y = 175 } });
            await AssertElementContainsTextAsync(".placed-gate", "OR");
            await AssertElementContainsTextAsync(".status-bar", "Gates: 1");

            // 5. Verify selection persists for additional placements
            await canvas.ClickAsync(new LocatorClickOptions { Position = new Position { X = 350, Y = 275 } });
            await AssertElementContainsTextAsync(".status-bar", "Gates: 2");

            // Take a final screenshot for visual verification
            await TakeScreenshotAsync("complete_workflow");
        }
    }
}
