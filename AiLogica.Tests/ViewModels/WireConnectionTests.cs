using AiLogica.ViewModels;
using Xunit;

namespace AiLogica.Tests.ViewModels;

public class WireConnectionTests
{
    [Fact]
    public void PlaceGate_ShouldCreateConnectionPoints()
    {
        // Arrange
        var viewModel = new HomeViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var gate = viewModel.PlacedGates[0];
        Assert.Equal(3, gate.Connections.Count); // OR gate should have 3 connections

        // Check input connections
        var inputs = gate.Connections.Where(c => c.Type == ConnectionType.Input).ToList();
        Assert.Equal(2, inputs.Count);
        Assert.Equal(0, inputs[0].Index);
        Assert.Equal(1, inputs[1].Index);

        // Check output connection
        var output = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        Assert.Equal(0, output.Index);
    }

    [Fact]
    public void StartWiring_ShouldSetActiveConnectionAndWiringMode()
    {
        // Arrange
        var viewModel = new HomeViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        var connection = viewModel.PlacedGates[0].Connections[0];

        // Act
        viewModel.StartWiring(connection);

        // Assert
        Assert.True(viewModel.IsWiring);
        Assert.Equal(connection, viewModel.ActiveConnection);
    }

    [Fact]
    public void CompleteWiring_ValidConnection_ShouldCreateWire()
    {
        // Arrange
        var viewModel = new HomeViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 100);

        var gate1 = viewModel.PlacedGates[0];
        var gate2 = viewModel.PlacedGates[1];
        var outputConnection = gate1.Connections.Single(c => c.Type == ConnectionType.Output);
        var inputConnection = gate2.Connections.First(c => c.Type == ConnectionType.Input);

        // Act
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(inputConnection);

        // Assert
        Assert.Single(viewModel.Wires);
        var wire = viewModel.Wires[0];
        Assert.Equal(outputConnection.Id, wire.FromConnectionId);
        Assert.Equal(inputConnection.Id, wire.ToConnectionId);
        Assert.True(wire.IsConnected);
        Assert.False(viewModel.IsWiring);
        Assert.Null(viewModel.ActiveConnection);
    }

    [Fact]
    public void CompleteWiring_SameGate_ShouldCreateWire()
    {
        // Arrange
        var viewModel = new HomeViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        var inputConnection = gate.Connections.First(c => c.Type == ConnectionType.Input);

        // Act
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(inputConnection);

        // Assert
        Assert.Single(viewModel.Wires); // Wire should be created for feedback loops
        var wire = viewModel.Wires[0];
        Assert.Equal(outputConnection.Id, wire.FromConnectionId);
        Assert.Equal(inputConnection.Id, wire.ToConnectionId);
        Assert.True(wire.IsConnected);
        Assert.False(viewModel.IsWiring);
        Assert.Null(viewModel.ActiveConnection);
    }

    [Fact]
    public void CompleteWiring_InputToInput_ShouldCreateWire()
    {
        // Arrange
        var viewModel = new HomeViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 100);

        var gate1 = viewModel.PlacedGates[0];
        var gate2 = viewModel.PlacedGates[1];
        var input1 = gate1.Connections.First(c => c.Type == ConnectionType.Input);
        var input2 = gate2.Connections.First(c => c.Type == ConnectionType.Input);

        // Act
        viewModel.StartWiring(input1);
        viewModel.CompleteWiring(input2);

        // Assert
        Assert.Single(viewModel.Wires); // Should allow input-to-input connections
        var wire = viewModel.Wires[0];
        Assert.Equal(input1.Id, wire.FromConnectionId);
        Assert.Equal(input2.Id, wire.ToConnectionId);
        Assert.True(wire.IsConnected);
    }

    [Fact]
    public void CancelWiring_ShouldClearWiringState()
    {
        // Arrange
        var viewModel = new HomeViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        var connection = viewModel.PlacedGates[0].Connections[0];
        viewModel.StartWiring(connection);

        // Act
        viewModel.CancelWiring();

        // Assert
        Assert.False(viewModel.IsWiring);
        Assert.Null(viewModel.ActiveConnection);
    }

    [Fact]
    public void UpdateConnectionPositions_ShouldSetAbsoluteCoordinates()
    {
        // Arrange
        var viewModel = new HomeViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);

        // Output should be at gate position + relative position (52, 64) + (88, 36) = (140, 100)
        Assert.Equal(140, outputConnection.X); // 52 + 88
        Assert.Equal(100, outputConnection.Y); // 64 + 36
    }

    [Fact]
    public void GenerateWireSegments_ShouldAvoidGateCollisions()
    {
        // Arrange
        var viewModel = new HomeViewModel();
        viewModel.SelectGate("OR");

        // Place gates that could cause wire collision
        viewModel.PlaceGate(100, 100); // Gate 1 (left)
        viewModel.PlaceGate(300, 100); // Gate 2 (right)
        viewModel.PlaceGate(200, 100); // Gate 3 (middle - potential collision)

        var gate1 = viewModel.PlacedGates[0];
        var gate2 = viewModel.PlacedGates[1];
        var outputConnection = gate1.Connections.Single(c => c.Type == ConnectionType.Output);
        var inputConnection = gate2.Connections.First(c => c.Type == ConnectionType.Input);

        // Act
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(inputConnection);

        // Assert
        Assert.Single(viewModel.Wires);
        var wire = viewModel.Wires[0];

        // Wire should have been created with collision avoidance
        Assert.True(wire.Segments.Count >= 1); // Should have at least one segment

        // Verify the wire doesn't pass through the middle gate (approximate check)
        var middleGate = viewModel.PlacedGates[2]; // Gate 3
        bool passesThrough = wire.Segments.Any(segment =>
            segment.Orientation == WireOrientation.Vertical &&
            segment.StartX >= middleGate.X - 10 && segment.StartX <= middleGate.X + 106 &&
            ((segment.StartY <= middleGate.Y + 82 && segment.EndY >= middleGate.Y - 10) ||
             (segment.EndY <= middleGate.Y + 82 && segment.StartY >= middleGate.Y - 10)));

        Assert.False(passesThrough); // Wire should avoid passing through the middle gate
    }
}
