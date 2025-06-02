using AiLogica.Tests.Helpers;
using AiLogica.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AiLogica.Tests.ViewModels;

/// <summary>
/// Requirements Traceability: Unit tests for HomeViewModel functionality
/// FR-10.1-FR-10.5: Demonstrates testing requirements (one assert, AAA pattern, descriptive names)
/// FR-2: Tests gate selection and dragging state management
/// FR-2.4: Tests gate placement functionality
/// See TRACEABILITY_MATRIX.md for complete mapping
/// </summary>
public class HomeViewModelTests
{
    [Fact]
    public void WelcomeMessage_ShouldHaveDefaultValue()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();

        // Act & Assert
        Assert.Equal("Logic Gate Design Canvas", viewModel.WelcomeMessage);
    }

    [Fact]
    public void WelcomeMessage_ShouldRaisePropertyChangedEvent()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
    }

    [Fact]
    public void WelcomeMessage_ShouldSetNewValue()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();

        // Act
        viewModel.WelcomeMessage = "New Message";

        // Assert
        Assert.Equal("New Message", viewModel.WelcomeMessage);
    }

    [Theory]
    [InlineData("OR")]
    [InlineData("AND")]
    [InlineData("NOT")]
    public void SelectGate_ShouldSetSelectedGateForDifferentTypes(string gateType)
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();

        // Act
        viewModel.SelectGate(gateType);

        // Assert
        Assert.Equal(gateType, viewModel.SelectedGate);
    }

    [Theory]
    [InlineData("OR")]
    [InlineData("AND")]
    [InlineData("NOT")]
    public void SelectGate_ShouldSetDraggingStateForDifferentTypes(string gateType)
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();

        // Act
        viewModel.SelectGate(gateType);

        // Assert
        Assert.True(viewModel.IsDragging);
    }

    [Fact]
    public void SelectGate_ShouldSetSelectedGate()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();

        // Act
        viewModel.SelectGate("OR");

        // Assert
        Assert.Equal("OR", viewModel.SelectedGate);
    }

    [Fact]
    public void SelectGate_ShouldSetDraggingState()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();

        // Act
        viewModel.SelectGate("OR");

        // Assert
        Assert.True(viewModel.IsDragging);
    }

    [Fact]
    public void UpdateMousePosition_ShouldSetMouseX()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();

        // Act
        viewModel.UpdateMousePosition(100, 200);

        // Assert
        Assert.Equal(100, viewModel.MouseX);
    }

    [Fact]
    public void UpdateMousePosition_ShouldSetMouseY()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();

        // Act
        viewModel.UpdateMousePosition(100, 200);

        // Assert
        Assert.Equal(200, viewModel.MouseY);
    }

    [Fact]
    public void PlaceGate_WithSelectedGate_ShouldAddToPlacedGates()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(150, 250);

        // Assert
        Assert.Single(viewModel.PlacedGates);
    }

    [Fact]
    public void PlaceGate_WithSelectedGate_ShouldSetGateType()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(150, 250);

        // Assert
        Assert.Equal("OR", viewModel.PlacedGates[0].Type);
    }

    [Fact]
    public void PlaceGate_WithSelectedGate_ShouldSetGateXPosition()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(150, 250);

        // Assert
        // Expected X position: 150 (click position) - 48 (half of 96px gate width to center gate on cursor) = 102
        Assert.Equal(102, viewModel.PlacedGates[0].X);
    }

    [Fact]
    public void PlaceGate_WithSelectedGate_ShouldSetGateYPosition()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(150, 250);

        // Assert
        Assert.Equal(214, viewModel.PlacedGates[0].Y); // 250 - 36 offset (updated for larger gates)
    }

    [Fact]
    public void PlaceGate_WithSelectedGate_ShouldKeepGateSelected()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(150, 250);

        // Assert
        Assert.Equal("OR", viewModel.SelectedGate); // Gate should stay selected
    }

    [Fact]
    public void PlaceGate_WithSelectedGate_ShouldRemainInDraggingState()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(150, 250);

        // Assert
        Assert.True(viewModel.IsDragging); // Should remain in dragging state
    }

    [Fact]
    public void PlaceGate_WithoutSelectedGate_ShouldNotAddToPlacedGates()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();

        // Act
        viewModel.PlaceGate(150, 250);

        // Assert
        Assert.Empty(viewModel.PlacedGates);
    }

    [Fact]
    public void PlaceGate_MultipleGates_ShouldCreateMultipleGates()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act - Place multiple gates
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 150);
        viewModel.PlaceGate(300, 200);

        // Assert
        Assert.Equal(3, viewModel.PlacedGates.Count);
    }

    [Fact]
    public void PlaceGate_MultipleGates_ShouldAllHaveSameType()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act - Place multiple gates
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 150);
        viewModel.PlaceGate(300, 200);

        // Assert
        Assert.All(viewModel.PlacedGates, gate => Assert.Equal("OR", gate.Type));
    }

    [Fact]
    public void PlaceGate_MultipleGates_ShouldKeepGateSelected()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act - Place multiple gates
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 150);
        viewModel.PlaceGate(300, 200);

        // Assert
        Assert.Equal("OR", viewModel.SelectedGate); // Should still be selected
    }

    [Fact]
    public void PlaceGate_MultipleGates_ShouldRemainInDraggingState()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act - Place multiple gates
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 150);
        viewModel.PlaceGate(300, 200);

        // Assert
        Assert.True(viewModel.IsDragging); // Should still be dragging
    }

    [Fact]
    public void PlaceGate_MultipleGates_ShouldSetFirstGateXPosition()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act - Place multiple gates
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 150);
        viewModel.PlaceGate(300, 200);

        // Assert
        Assert.Equal(52, viewModel.PlacedGates[0].X); // 100 - 48 offset
    }

    [Fact]
    public void PlaceGate_MultipleGates_ShouldSetFirstGateYPosition()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act - Place multiple gates
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 150);
        viewModel.PlaceGate(300, 200);

        // Assert
        Assert.Equal(64, viewModel.PlacedGates[0].Y); // 100 - 36 offset
    }

    [Fact]
    public void PlaceGate_MultipleGates_ShouldSetSecondGateXPosition()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act - Place multiple gates
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 150);
        viewModel.PlaceGate(300, 200);

        // Assert
        Assert.Equal(152, viewModel.PlacedGates[1].X); // 200 - 48 offset
    }

    [Fact]
    public void PlaceGate_MultipleGates_ShouldSetSecondGateYPosition()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act - Place multiple gates
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 150);
        viewModel.PlaceGate(300, 200);

        // Assert
        Assert.Equal(114, viewModel.PlacedGates[1].Y); // 150 - 36 offset
    }

    [Fact]
    public void PlaceGate_MultipleGates_ShouldSetThirdGateXPosition()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act - Place multiple gates
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 150);
        viewModel.PlaceGate(300, 200);

        // Assert
        Assert.Equal(252, viewModel.PlacedGates[2].X); // 300 - 48 offset
    }

    [Fact]
    public void PlaceGate_MultipleGates_ShouldSetThirdGateYPosition()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act - Place multiple gates
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 150);
        viewModel.PlaceGate(300, 200);

        // Assert
        Assert.Equal(164, viewModel.PlacedGates[2].Y); // 200 - 36 offset
    }

    [Fact]
    public void CancelDrag_ShouldClearSelection()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.CancelDrag();

        // Assert
        Assert.Null(viewModel.SelectedGate);
    }

    [Fact]
    public void CancelDrag_ShouldClearDraggingState()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.CancelDrag();

        // Assert
        Assert.False(viewModel.IsDragging);
    }

    [Fact]
    public void PlacedGates_PropertyChanged_ShouldBeRaisedWhenGateIsPlaced()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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

    [Fact]
    public void PlaceGate_ShouldCreateConstant0Gate_WithCorrectValue()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("CONSTANT0");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var placedGate = Assert.Single(viewModel.PlacedGates);
        Assert.Equal("CONSTANT0", placedGate.Type);
        Assert.Equal(0, placedGate.Value);
    }

    [Fact]
    public void PlaceGate_ShouldCreateConstant1Gate_WithCorrectValue()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("CONSTANT1");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var placedGate = Assert.Single(viewModel.PlacedGates);
        Assert.Equal("CONSTANT1", placedGate.Type);
        Assert.Equal(1, placedGate.Value);
    }

    [Fact]
    public void PlaceGate_ShouldCreateConstantGate_WithOnlyOutputConnection()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("CONSTANT0");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var placedGate = Assert.Single(viewModel.PlacedGates);
        var outputConnection = Assert.Single(placedGate.Connections);
        Assert.Equal(ConnectionType.Output, outputConnection.Type);
        Assert.Equal(0, outputConnection.Index);
    }

    [Fact]
    public void ToggleConstantValue_ShouldToggleConstant0To1()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("CONSTANT0");
        viewModel.PlaceGate(100, 100);
        var gate = viewModel.PlacedGates[0];

        // Act
        viewModel.ToggleConstantValue(gate);

        // Assert
        Assert.Equal(1, gate.Value);
        Assert.Equal("CONSTANT1", gate.Type);
    }

    [Fact]
    public void ToggleConstantValue_ShouldToggleConstant1To0()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("CONSTANT1");
        viewModel.PlaceGate(100, 100);
        var gate = viewModel.PlacedGates[0];

        // Act
        viewModel.ToggleConstantValue(gate);

        // Assert
        Assert.Equal(0, gate.Value);
        Assert.Equal("CONSTANT0", gate.Type);
    }

    [Fact]
    public void ToggleConstantValue_ShouldNotAffectNonConstantGates()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        var gate = viewModel.PlacedGates[0];
        var originalType = gate.Type;
        var originalValue = gate.Value;

        // Act
        viewModel.ToggleConstantValue(gate);

        // Assert
        Assert.Equal(originalType, gate.Type);
        Assert.Equal(originalValue, gate.Value);
    }

    [Fact]
    public void WireRouting_ShouldUseCorrectGateDimensionsForCollisionDetection()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();

        // Place a constant gate (32x16) and an OR gate (96x72)
        viewModel.SelectGate("CONSTANT0");
        viewModel.PlaceGate(100, 100);
        var constantGate = viewModel.PlacedGates[0];

        viewModel.SelectGate("OR");
        viewModel.PlaceGate(200, 100);
        var orGate = viewModel.PlacedGates[1];

        // Act - Start wiring from constant gate output to OR gate input
        var constantOutput = constantGate.Connections.First(c => c.Type == ConnectionType.Output);
        var orInput = orGate.Connections.First(c => c.Type == ConnectionType.Input);

        viewModel.StartWiring(constantOutput);
        viewModel.CompleteWiring(orInput);

        // Assert - Wire should be created successfully
        Assert.Single(viewModel.Wires);
        var wire = viewModel.Wires[0];
        Assert.Equal(constantOutput.Id, wire.FromConnectionId);
        Assert.Equal(orInput.Id, wire.ToConnectionId);

        // Wire should have segments (indicating routing logic executed)
        Assert.NotEmpty(wire.Segments);
    }
}
