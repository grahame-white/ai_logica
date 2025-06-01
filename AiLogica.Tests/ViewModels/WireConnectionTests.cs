using AiLogica.Tests.Helpers;
using AiLogica.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AiLogica.Tests.ViewModels;

public class WireConnectionTests
{
    [Fact]
    public void PlaceGate_ShouldCreateCorrectNumberOfConnections()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var gate = viewModel.PlacedGates[0];
        Assert.Equal(3, gate.Connections.Count); // OR gate should have 3 connections
    }

    [Fact]
    public void PlaceGate_ShouldCreateCorrectNumberOfInputConnections()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var gate = viewModel.PlacedGates[0];
        var inputs = gate.Connections.Where(c => c.Type == ConnectionType.Input).ToList();
        Assert.Equal(2, inputs.Count);
    }

    [Fact]
    public void PlaceGate_ShouldCreateFirstInputConnectionWithCorrectIndex()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var gate = viewModel.PlacedGates[0];
        var inputs = gate.Connections.Where(c => c.Type == ConnectionType.Input).ToList();
        Assert.Equal(0, inputs[0].Index);
    }

    [Fact]
    public void PlaceGate_ShouldCreateSecondInputConnectionWithCorrectIndex()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var gate = viewModel.PlacedGates[0];
        var inputs = gate.Connections.Where(c => c.Type == ConnectionType.Input).ToList();
        Assert.Equal(1, inputs[1].Index);
    }

    [Fact]
    public void PlaceGate_ShouldCreateOutputConnectionWithCorrectIndex()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var gate = viewModel.PlacedGates[0];
        var output = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        Assert.Equal(0, output.Index);
    }

    [Theory]
    [InlineData(ConnectionType.Input)]
    [InlineData(ConnectionType.Output)]
    public void StartWiring_ShouldSetWiringModeForDifferentConnectionTypes(ConnectionType connectionType)
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        var connection = viewModel.PlacedGates[0].Connections.First(c => c.Type == connectionType);

        // Act
        viewModel.StartWiring(connection);

        // Assert
        Assert.True(viewModel.IsWiring);
    }

    [Theory]
    [InlineData(ConnectionType.Input)]
    [InlineData(ConnectionType.Output)]
    public void StartWiring_ShouldSetActiveConnectionForDifferentConnectionTypes(ConnectionType connectionType)
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        var connection = viewModel.PlacedGates[0].Connections.First(c => c.Type == connectionType);

        // Act
        viewModel.StartWiring(connection);

        // Assert
        Assert.Equal(connection, viewModel.ActiveConnection);
    }

    [Fact]
    public void StartWiring_ShouldSetWiringMode()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        var connection = viewModel.PlacedGates[0].Connections[0];

        // Act
        viewModel.StartWiring(connection);

        // Assert
        Assert.True(viewModel.IsWiring);
    }

    [Fact]
    public void StartWiring_ShouldSetActiveConnection()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        var connection = viewModel.PlacedGates[0].Connections[0];

        // Act
        viewModel.StartWiring(connection);

        // Assert
        Assert.Equal(connection, viewModel.ActiveConnection);
    }

    [Fact]
    public void CompleteWiring_ValidConnection_ShouldCreateSingleWire()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
    }

    [Fact]
    public void CompleteWiring_ValidConnection_ShouldSetFromConnectionId()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
        var wire = viewModel.Wires[0];
        Assert.Equal(outputConnection.Id, wire.FromConnectionId);
    }

    [Fact]
    public void CompleteWiring_ValidConnection_ShouldSetToConnectionId()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
        var wire = viewModel.Wires[0];
        Assert.Equal(inputConnection.Id, wire.ToConnectionId);
    }

    [Fact]
    public void CompleteWiring_ValidConnection_ShouldSetWireAsConnected()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
        var wire = viewModel.Wires[0];
        Assert.True(wire.IsConnected);
    }

    [Fact]
    public void CompleteWiring_ValidConnection_ShouldClearWiringMode()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
        Assert.False(viewModel.IsWiring);
    }

    [Fact]
    public void CompleteWiring_ValidConnection_ShouldClearActiveConnection()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
        Assert.Null(viewModel.ActiveConnection);
    }

    [Fact]
    public void CompleteWiring_SameGate_ShouldCreateSingleWire()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
    }

    [Fact]
    public void CompleteWiring_SameGate_ShouldSetFromConnectionId()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        var inputConnection = gate.Connections.First(c => c.Type == ConnectionType.Input);

        // Act
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(inputConnection);

        // Assert
        var wire = viewModel.Wires[0];
        Assert.Equal(outputConnection.Id, wire.FromConnectionId);
    }

    [Fact]
    public void CompleteWiring_SameGate_ShouldSetToConnectionId()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        var inputConnection = gate.Connections.First(c => c.Type == ConnectionType.Input);

        // Act
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(inputConnection);

        // Assert
        var wire = viewModel.Wires[0];
        Assert.Equal(inputConnection.Id, wire.ToConnectionId);
    }

    [Fact]
    public void CompleteWiring_SameGate_ShouldSetWireAsConnected()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        var inputConnection = gate.Connections.First(c => c.Type == ConnectionType.Input);

        // Act
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(inputConnection);

        // Assert
        var wire = viewModel.Wires[0];
        Assert.True(wire.IsConnected);
    }

    [Fact]
    public void CompleteWiring_SameGate_ShouldClearWiringMode()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        var inputConnection = gate.Connections.First(c => c.Type == ConnectionType.Input);

        // Act
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(inputConnection);

        // Assert
        Assert.False(viewModel.IsWiring);
    }

    [Fact]
    public void CompleteWiring_SameGate_ShouldClearActiveConnection()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        var inputConnection = gate.Connections.First(c => c.Type == ConnectionType.Input);

        // Act
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(inputConnection);

        // Assert
        Assert.Null(viewModel.ActiveConnection);
    }

    [Fact]
    public void CompleteWiring_InputToInput_ShouldCreateSingleWire()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
    }

    [Fact]
    public void CompleteWiring_InputToInput_ShouldSetFromConnectionId()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
        var wire = viewModel.Wires[0];
        Assert.Equal(input1.Id, wire.FromConnectionId);
    }

    [Fact]
    public void CompleteWiring_InputToInput_ShouldSetToConnectionId()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
        var wire = viewModel.Wires[0];
        Assert.Equal(input2.Id, wire.ToConnectionId);
    }

    [Fact]
    public void CompleteWiring_InputToInput_ShouldSetWireAsConnected()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
        var wire = viewModel.Wires[0];
        Assert.True(wire.IsConnected);
    }

    [Fact]
    public void CompleteWiring_InputToOutput_ShouldCreateSingleWire()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 100);

        var gate1 = viewModel.PlacedGates[0];
        var gate2 = viewModel.PlacedGates[1];
        var inputConnection = gate1.Connections.First(c => c.Type == ConnectionType.Input);
        var outputConnection = gate2.Connections.Single(c => c.Type == ConnectionType.Output);

        // Act - Test input-to-output connection (reverse direction)
        viewModel.StartWiring(inputConnection);
        viewModel.CompleteWiring(outputConnection);

        // Assert
        Assert.Single(viewModel.Wires); // Should allow input-to-output connections
    }

    [Fact]
    public void CompleteWiring_InputToOutput_ShouldSetFromConnectionId()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 100);

        var gate1 = viewModel.PlacedGates[0];
        var gate2 = viewModel.PlacedGates[1];
        var inputConnection = gate1.Connections.First(c => c.Type == ConnectionType.Input);
        var outputConnection = gate2.Connections.Single(c => c.Type == ConnectionType.Output);

        // Act - Test input-to-output connection (reverse direction)
        viewModel.StartWiring(inputConnection);
        viewModel.CompleteWiring(outputConnection);

        // Assert
        var wire = viewModel.Wires[0];
        Assert.Equal(inputConnection.Id, wire.FromConnectionId);
    }

    [Fact]
    public void CompleteWiring_InputToOutput_ShouldSetToConnectionId()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 100);

        var gate1 = viewModel.PlacedGates[0];
        var gate2 = viewModel.PlacedGates[1];
        var inputConnection = gate1.Connections.First(c => c.Type == ConnectionType.Input);
        var outputConnection = gate2.Connections.Single(c => c.Type == ConnectionType.Output);

        // Act - Test input-to-output connection (reverse direction)
        viewModel.StartWiring(inputConnection);
        viewModel.CompleteWiring(outputConnection);

        // Assert
        var wire = viewModel.Wires[0];
        Assert.Equal(outputConnection.Id, wire.ToConnectionId);
    }

    [Fact]
    public void CompleteWiring_InputToOutput_ShouldSetWireAsConnected()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        viewModel.PlaceGate(200, 100);

        var gate1 = viewModel.PlacedGates[0];
        var gate2 = viewModel.PlacedGates[1];
        var inputConnection = gate1.Connections.First(c => c.Type == ConnectionType.Input);
        var outputConnection = gate2.Connections.Single(c => c.Type == ConnectionType.Output);

        // Act - Test input-to-output connection (reverse direction)
        viewModel.StartWiring(inputConnection);
        viewModel.CompleteWiring(outputConnection);

        // Assert
        var wire = viewModel.Wires[0];
        Assert.True(wire.IsConnected);
    }

    [Fact]
    public void CancelWiring_ShouldClearWiringState()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        var connection = viewModel.PlacedGates[0].Connections[0];
        viewModel.StartWiring(connection);

        // Act
        viewModel.CancelWiring();

        // Assert
        Assert.False(viewModel.IsWiring);
    }

    [Fact]
    public void CancelWiring_ShouldClearActiveConnection()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);
        var connection = viewModel.PlacedGates[0].Connections[0];
        viewModel.StartWiring(connection);

        // Act
        viewModel.CancelWiring();

        // Assert
        Assert.Null(viewModel.ActiveConnection);
    }

    [Fact]
    public void UpdateConnectionPositions_ShouldSetAbsoluteXCoordinate()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);

        // Output should be at gate position + relative position (52, 64) + (88, 36) = (140, 100)
        Assert.Equal(140, outputConnection.X); // 52 + 88
    }

    [Fact]
    public void UpdateConnectionPositions_ShouldSetAbsoluteYCoordinate()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);

        // Output should be at gate position + relative position (52, 64) + (88, 36) = (140, 100)
        Assert.Equal(100, outputConnection.Y); // 64 + 36
    }

    [Fact]
    public void GenerateWireSegments_ShouldAvoidGateCollisions()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
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
        bool passesThrough = wire.Segments.Exists(segment =>
            segment.Orientation == WireOrientation.Vertical &&
            segment.StartX >= middleGate.X - 10 && segment.StartX <= middleGate.X + 106 &&
            ((segment.StartY <= middleGate.Y + 82 && segment.EndY >= middleGate.Y - 10) ||
             (segment.EndY <= middleGate.Y + 82 && segment.StartY >= middleGate.Y - 10)));

        Assert.False(passesThrough); // Wire should avoid passing through the middle gate
    }

    [Fact]
    public void GenerateWireSegments_SameGateConnection_ShouldNotPassThroughGate()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        var bottomInputConnection = gate.Connections.Where(c => c.Type == ConnectionType.Input).Skip(1).First(); // Bottom input

        // Act - Start from output and connect to bottom input (matching successful test pattern)
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(bottomInputConnection);

        // Assert
        var wire = viewModel.Wires[0];

        // Wire should route around the gate, not through it
        // More specific check: look for vertical segments that pass through the visual center of the gate
        // The OR gate visual center is approximately at gate.X + 48, so check for segments near there
        bool passesThrough = wire.Segments.Exists(segment =>
            segment.Orientation == WireOrientation.Vertical &&
            segment.StartX >= gate.X + 20 && segment.StartX <= gate.X + 76 && // Middle portion of gate width
            ((segment.StartY <= gate.Y + 60 && segment.EndY >= gate.Y + 12) || // Intersects middle of gate height
             (segment.EndY <= gate.Y + 60 && segment.StartY >= gate.Y + 12)));

        Assert.False(passesThrough); // Wire should not pass through the visual center of the gate
    }

    [Fact]
    public void GenerateWireSegments_SameGateConnection_ShouldCreateMultipleSegments()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        var bottomInputConnection = gate.Connections.Where(c => c.Type == ConnectionType.Input).Skip(1).First(); // Bottom input

        // Act - Start from output and connect to bottom input (matching successful test pattern)
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(bottomInputConnection);

        // Assert
        var wire = viewModel.Wires[0];
        Assert.True(wire.Segments.Count >= 2); // Should have multiple segments for routing around
    }

    [Fact]
    public void GenerateWireSegments_SameGateConnection_SpecificScenario_ShouldRouteAroundGate()
    {
        // Arrange - Create exact scenario that might cause the bug
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100); // Place gate in middle

        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        var bottomInputConnection = gate.Connections.Where(c => c.Type == ConnectionType.Input).Skip(1).First(); // Bottom input

        // Debug the exact coordinates and midpoint calculation
#if DEBUG
        Console.WriteLine($"Gate position: ({gate.X}, {gate.Y})");
        Console.WriteLine($"Bottom input: ({bottomInputConnection.X}, {bottomInputConnection.Y})");
        Console.WriteLine($"Output: ({outputConnection.X}, {outputConnection.Y})");

        double expectedMidX = (bottomInputConnection.X + outputConnection.X) / 2.0;
        Console.WriteLine($"Expected midpoint X: {expectedMidX}");
        Console.WriteLine($"Gate center X: {gate.X + 48}");
        Console.WriteLine($"Gate left boundary: {gate.X - 10}");
        Console.WriteLine($"Gate right boundary: {gate.X + 96 + 10}");

        // Test if midpoint would be inside gate
        bool midpointInsideGate = expectedMidX >= (gate.X - 10) && expectedMidX <= (gate.X + 96 + 10);
        Console.WriteLine($"Midpoint inside gate boundaries: {midpointInsideGate}");
#endif

        // Act - Create wire from bottom input to output
        viewModel.StartWiring(bottomInputConnection);
        viewModel.CompleteWiring(outputConnection);

        // Assert
        Assert.Single(viewModel.Wires);
        var wire = viewModel.Wires[0];

#if DEBUG
        Console.WriteLine($"Wire segments count: {wire.Segments.Count}");
        foreach (var segment in wire.Segments)
        {
            Console.WriteLine($"Segment: ({segment.StartX}, {segment.StartY}) -> ({segment.EndX}, {segment.EndY}) [{segment.Orientation}]");
        }
#endif

        // Check if any vertical segment passes through the gate center area
        bool passesThrough = wire.Segments.Exists(segment =>
            segment.Orientation == WireOrientation.Vertical &&
            segment.StartX >= gate.X + 30 && segment.StartX <= gate.X + 66 && // Through visual center of gate
            Math.Min(segment.StartY, segment.EndY) <= gate.Y + 50 &&
            Math.Max(segment.StartY, segment.EndY) >= gate.Y + 20);

        if (passesThrough)
        {
#if DEBUG
            Console.WriteLine("ERROR: Wire passes through gate center!");
#endif
        }

        Assert.False(passesThrough); // Wire should not pass through the gate center
    }

    [Fact]
    public void GenerateWireSegments_OutputToTopInput_UserScenario_ShouldNotPassThroughGate()
    {
        // Arrange - Reproduce the exact user scenario: Output -> Top Input
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        var topInputConnection = gate.Connections.First(c => c.Type == ConnectionType.Input); // Top input (index 0)

        // Act - Create wire from output to top input (user's exact scenario)
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(topInputConnection);

        // Assert
        var wire = viewModel.Wires[0];

        // Verify collision avoidance: wire should route around the gate, not through it
        bool passesThrough = wire.Segments.Exists(segment =>
            segment.Orientation == WireOrientation.Vertical &&
            segment.StartX >= gate.X + 20 && segment.StartX <= gate.X + 76 && // Through main body of gate
            Math.Min(segment.StartY, segment.EndY) <= gate.Y + 60 &&
            Math.Max(segment.StartY, segment.EndY) >= gate.Y + 12);

        Assert.False(passesThrough); // Wire should not pass through the gate
    }

    [Fact]
    public void GenerateWireSegments_OutputToTopInput_UserScenario_ShouldCreateMultipleSegments()
    {
        // Arrange - Reproduce the exact user scenario: Output -> Top Input
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);
        var topInputConnection = gate.Connections.First(c => c.Type == ConnectionType.Input); // Top input (index 0)

        // Act - Create wire from output to top input (user's exact scenario)
        viewModel.StartWiring(outputConnection);
        viewModel.CompleteWiring(topInputConnection);

        // Assert
        var wire = viewModel.Wires[0];
        Assert.True(wire.Segments.Count >= 2); // Should have multiple segments for proper routing
    }
}
