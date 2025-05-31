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
            Assert.Equal("Logic Gate Design Canvas", viewModel.WelcomeMessage);
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

        [Fact]
        public void SelectGate_ShouldSetSelectedGateAndDraggingState()
        {
            // Arrange
            var viewModel = new HomeViewModel();

            // Act
            viewModel.SelectGate("OR");

            // Assert
            Assert.Equal("OR", viewModel.SelectedGate);
            Assert.True(viewModel.IsDragging);
        }

        [Fact]
        public void UpdateMousePosition_ShouldSetMouseCoordinates()
        {
            // Arrange
            var viewModel = new HomeViewModel();

            // Act
            viewModel.UpdateMousePosition(100, 200);

            // Assert
            Assert.Equal(100, viewModel.MouseX);
            Assert.Equal(200, viewModel.MouseY);
        }

        [Fact]
        public void PlaceGate_WithSelectedGate_ShouldAddToPlacedGatesAndKeepSelection()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            viewModel.SelectGate("OR");

            // Act
            viewModel.PlaceGate(150, 250);

            // Assert
            Assert.Single(viewModel.PlacedGates);
            Assert.Equal("OR", viewModel.PlacedGates[0].Type);
            Assert.Equal(120, viewModel.PlacedGates[0].X); // 150 - 30 offset
            Assert.Equal(235, viewModel.PlacedGates[0].Y); // 250 - 15 offset
            Assert.Equal("OR", viewModel.SelectedGate); // Gate should stay selected
            Assert.True(viewModel.IsDragging); // Should remain in dragging state
        }

        [Fact]
        public void PlaceGate_WithoutSelectedGate_ShouldNotAddToPlacedGates()
        {
            // Arrange
            var viewModel = new HomeViewModel();

            // Act
            viewModel.PlaceGate(150, 250);

            // Assert
            Assert.Empty(viewModel.PlacedGates);
        }

        [Fact]
        public void PlaceGate_MultipleGates_ShouldAllowPlacingMultipleGatesWithoutReselection()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            viewModel.SelectGate("OR");

            // Act - Place multiple gates
            viewModel.PlaceGate(100, 100);
            viewModel.PlaceGate(200, 150);
            viewModel.PlaceGate(300, 200);

            // Assert
            Assert.Equal(3, viewModel.PlacedGates.Count);
            Assert.All(viewModel.PlacedGates, gate => Assert.Equal("OR", gate.Type));
            Assert.Equal("OR", viewModel.SelectedGate); // Should still be selected
            Assert.True(viewModel.IsDragging); // Should still be dragging

            // Verify positions
            Assert.Equal(70, viewModel.PlacedGates[0].X); // 100 - 30 offset
            Assert.Equal(85, viewModel.PlacedGates[0].Y); // 100 - 15 offset
            Assert.Equal(170, viewModel.PlacedGates[1].X); // 200 - 30 offset
            Assert.Equal(135, viewModel.PlacedGates[1].Y); // 150 - 15 offset
            Assert.Equal(270, viewModel.PlacedGates[2].X); // 300 - 30 offset
            Assert.Equal(185, viewModel.PlacedGates[2].Y); // 200 - 15 offset
        }

        [Fact]
        public void CancelDrag_ShouldClearSelectionAndDraggingState()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            viewModel.SelectGate("OR");

            // Act
            viewModel.CancelDrag();

            // Assert
            Assert.Null(viewModel.SelectedGate);
            Assert.False(viewModel.IsDragging);
        }

        [Fact]
        public void PlacedGates_PropertyChanged_ShouldBeRaisedWhenGateIsPlaced()
        {
            // Arrange
            var viewModel = new HomeViewModel();
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
        public void StartWiring_ShouldSetWiringState()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            var connectionId = Guid.NewGuid();

            // Act
            viewModel.StartWiring(connectionId);

            // Assert
            Assert.True(viewModel.IsWiring);
        }

        [Fact]
        public void CompleteWiring_WithValidConnections_ShouldCreateWire()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            var startConnectionId = Guid.NewGuid();
            var endConnectionId = Guid.NewGuid();

            // Act
            viewModel.StartWiring(startConnectionId);
            viewModel.CompleteWiring(endConnectionId);

            // Assert
            Assert.Single(viewModel.Wires);
            Assert.Equal(startConnectionId, viewModel.Wires[0].StartConnectionId);
            Assert.Equal(endConnectionId, viewModel.Wires[0].EndConnectionId);
            Assert.False(viewModel.IsWiring);
        }

        [Fact]
        public void CompleteWiring_WithSameConnection_ShouldNotCreateWire()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            var connectionId = Guid.NewGuid();

            // Act
            viewModel.StartWiring(connectionId);
            viewModel.CompleteWiring(connectionId);

            // Assert
            Assert.Empty(viewModel.Wires);
            Assert.False(viewModel.IsWiring);
        }

        [Fact]
        public void GetConnectionPoints_OrGate_ShouldReturnCorrectPoints()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            viewModel.SelectGate("OR");
            viewModel.PlaceGate(100, 100);
            var gate = viewModel.PlacedGates[0];

            // Act
            var connectionPoints = gate.GetConnectionPoints();

            // Assert
            Assert.Equal(3, connectionPoints.Count);
            Assert.Equal(2, connectionPoints.Count(p => p.Type == ConnectionType.Input));
            Assert.Equal(1, connectionPoints.Count(p => p.Type == ConnectionType.Output));
        }

        [Fact]
        public void GetUnconnectedConnectionPoints_WithoutWires_ShouldReturnAllPoints()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            viewModel.SelectGate("OR");
            viewModel.PlaceGate(100, 100);

            // Act
            var unconnectedPoints = viewModel.GetUnconnectedConnectionPoints();

            // Assert
            Assert.Equal(3, unconnectedPoints.Count);
        }

        [Fact]
        public void GetUnconnectedConnectionPoints_WithWires_ShouldReturnOnlyUnconnectedPoints()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            viewModel.SelectGate("OR");
            viewModel.PlaceGate(100, 100);
            viewModel.PlaceGate(200, 100);

            var allPoints = viewModel.GetAllConnectionPoints();
            var startConnection = allPoints.First(p => p.Type == ConnectionType.Output);
            var endConnection = allPoints.First(p => p.Type == ConnectionType.Input && p.GateId != startConnection.GateId);

            // Act
            viewModel.StartWiring(startConnection.Id);
            viewModel.CompleteWiring(endConnection.Id);
            var unconnectedPoints = viewModel.GetUnconnectedConnectionPoints();

            // Assert
            Assert.Equal(4, unconnectedPoints.Count); // 6 total - 2 connected = 4 unconnected
        }

        [Fact]
        public void PlaceAndGate_ShouldHaveCorrectConnectionPoints()
        {
            // Arrange
            var viewModel = new HomeViewModel();

            // Act
            viewModel.SelectGate("AND");
            viewModel.PlaceGate(100, 100);

            // Assert
            var gate = viewModel.PlacedGates.First();
            var connectionPoints = gate.GetConnectionPoints();

            Assert.Equal(3, connectionPoints.Count);
            Assert.Equal(2, connectionPoints.Count(p => p.Type == ConnectionType.Input));
            Assert.Equal(1, connectionPoints.Count(p => p.Type == ConnectionType.Output));

            // Check input positions (relative to gate at 70, 85 after PlaceGate(100, 100) applies -30,-15 offset)
            var inputs = connectionPoints.Where(p => p.Type == ConnectionType.Input).ToList();
            Assert.Equal(72, inputs[0].X); // X + 2 = 70 + 2
            Assert.Equal(99, inputs[0].Y); // Y + 14 = 85 + 14
            Assert.Equal(72, inputs[1].X); // X + 2 = 70 + 2
            Assert.Equal(107, inputs[1].Y); // Y + 22 = 85 + 22

            // Check output position
            var output = connectionPoints.Single(p => p.Type == ConnectionType.Output);
            Assert.Equal(104, output.X); // X + 34 = 70 + 34
            Assert.Equal(103, output.Y); // Y + 18 = 85 + 18
        }

        [Fact]
        public void AndGate_ConnectionPoints_ShouldSupportWiring()
        {
            // Arrange
            var viewModel = new HomeViewModel();
            viewModel.SelectGate("AND");
            viewModel.PlaceGate(100, 100);
            viewModel.PlaceGate(200, 100);

            var allPoints = viewModel.GetAllConnectionPoints();
            var startConnection = allPoints.First(p => p.Type == ConnectionType.Output);
            var endConnection = allPoints.First(p => p.Type == ConnectionType.Input && p.GateId != startConnection.GateId);

            // Act
            viewModel.StartWiring(startConnection.Id);
            viewModel.CompleteWiring(endConnection.Id);

            // Assert
            Assert.Single(viewModel.Wires);
            var wire = viewModel.Wires.First();
            Assert.Equal(startConnection.Id, wire.StartConnectionId);
            Assert.Equal(endConnection.Id, wire.EndConnectionId);
            Assert.True(wire.Path.Count > 1);
        }

        [Fact]
        public void MixedGates_OrAndAnd_ShouldAllHaveConnectionPoints()
        {
            // Arrange
            var viewModel = new HomeViewModel();

            // Act - Place different gate types
            viewModel.SelectGate("OR");
            viewModel.PlaceGate(100, 100);
            viewModel.SelectGate("AND");
            viewModel.PlaceGate(200, 100);

            // Assert
            var allPoints = viewModel.GetAllConnectionPoints();
            Assert.Equal(6, allPoints.Count); // 3 per gate × 2 gates

            var orGate = viewModel.PlacedGates.First(g => g.Type == "OR");
            var andGate = viewModel.PlacedGates.First(g => g.Type == "AND");

            Assert.Equal(3, orGate.GetConnectionPoints().Count);
            Assert.Equal(3, andGate.GetConnectionPoints().Count);
        }
    }
}
