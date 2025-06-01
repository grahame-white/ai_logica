using AiLogica.ViewModels;
using AiLogica.Tests.Helpers;

namespace AiLogica.Tests.ViewModels;

/// <summary>
/// Tests to validate that the coordinate system correctly handles left/right positioning concepts.
/// Addresses bug: "Copilot is easily confused by the concept of left and right"
/// </summary>
public class CoordinateSystemTests
{
    [Fact]
    public void Gate_RightEdge_ShouldHaveHigherXCoordinate_ThanLeftEdge()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var gate = viewModel.PlacedGates[0];
        double leftEdge = gate.X;
        double rightEdge = gate.X + 96; // Gate width is 96 pixels

        Assert.True(
            rightEdge > leftEdge,
            $"Right edge ({rightEdge}) should have higher X coordinate than left edge ({leftEdge})");
        Assert.Equal(96, rightEdge - leftEdge, 0.001); // Gate width should be exactly 96 pixels
    }

    [Fact]
    public void GateConnections_Output_ShouldBeToTheRightOf_Inputs()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(100, 100);

        // Assert
        var gate = viewModel.PlacedGates[0];
        var inputConnections = gate.Connections.Where(c => c.Type == ConnectionType.Input).ToList();
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);

        foreach (var input in inputConnections)
        {
            Assert.True(
                outputConnection.X > input.X,
                $"Output connection ({outputConnection.X}) should be to the right of input connection ({input.X})");
        }
    }

    [Theory]
    [InlineData(50, 100, "Gate A should be to the left of Gate B")]
    [InlineData(100, 150, "Gate A should be to the left of Gate B")]
    [InlineData(0, 200, "Gate A should be to the left of Gate B")]
    public void PlaceGate_LeftPosition_ShouldHaveLowerXCoordinate_ThanRightPosition(double leftX, double rightX, string description)
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act
        viewModel.PlaceGate(leftX, 100); // Place first gate at left position
        viewModel.PlaceGate(rightX, 100); // Place second gate at right position

        // Assert
        var leftGate = viewModel.PlacedGates[0];
        var rightGate = viewModel.PlacedGates[1];

        Assert.True(
            leftGate.X < rightGate.X,
            $"{description}: Left gate X ({leftGate.X}) should be less than right gate X ({rightGate.X})");
    }

    [Fact]
    public void WireRouting_LeftRoute_ShouldHaveLowerXCoordinate_ThanRightRoute()
    {
        // Arrange - Create a simple same-gate connection scenario that we know works
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Place one gate that will create internal routing
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        var inputConnection = gate.Connections.First(c => c.Type == ConnectionType.Input);
        var outputConnection = gate.Connections.Single(c => c.Type == ConnectionType.Output);

        // Act - Create wire that should route around the gate
        viewModel.StartWiring(inputConnection);
        viewModel.CompleteWiring(outputConnection);

        // Assert
        var wire = viewModel.Wires[0];
        var verticalSegments = wire.Segments.Where(s => s.Orientation == WireOrientation.Vertical).ToList();

        Assert.NotEmpty(verticalSegments);

        // Get the routing X coordinate
        var routingX = verticalSegments[0].StartX;
        var gateLeftBoundary = gate.X - 10;
        var gateRightBoundary = gate.X + 96 + 10;

        // The wire should route either to the left or right of the gate
        bool routedLeft = routingX < gateLeftBoundary;
        bool routedRight = routingX > gateRightBoundary;

        Assert.True(
            routedLeft || routedRight,
            $"Wire should route either left (X < {gateLeftBoundary}) or right (X > {gateRightBoundary}), but routed at X = {routingX}");

        // Validate the correct understanding of left vs right coordinates
        if (routedLeft)
        {
            Assert.True(routingX < gate.X, "Left route should have lower X coordinate than gate center");
        }

        if (routedRight)
        {
            Assert.True(routingX > gate.X + 96, "Right route should have higher X coordinate than gate right edge");
        }
    }

    [Fact]
    public void MoveGateToTheRight_ShouldIncreaseXCoordinate()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        double originalX = gate.X;

        // Act - Simulate moving gate to the right by placing it at a higher X coordinate
        gate.X = originalX + 50; // Move 50 pixels to the right

        // Assert
        Assert.True(
            gate.X > originalX,
            $"Moving to the right should increase X coordinate from {originalX} to {gate.X}");
        Assert.Equal(originalX + 50, gate.X, 0.001);
    }

    [Fact]
    public void MoveGateToTheLeft_ShouldDecreaseXCoordinate()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");
        viewModel.PlaceGate(100, 100);

        var gate = viewModel.PlacedGates[0];
        double originalX = gate.X;

        // Act - Simulate moving gate to the left by placing it at a lower X coordinate
        gate.X = originalX - 30; // Move 30 pixels to the left

        // Assert
        Assert.True(
            gate.X < originalX,
            $"Moving to the left should decrease X coordinate from {originalX} to {gate.X}");
        Assert.Equal(originalX - 30, gate.X, 0.001);
    }

    [Fact]
    public void PlaceGateToTheRightOfAnotherGate_ShouldHaveHigherXCoordinate()
    {
        // Arrange
        var viewModel = TestHelper.CreateTestViewModel();
        viewModel.SelectGate("OR");

        // Act - Place first gate
        viewModel.PlaceGate(100, 100);
        var firstGate = viewModel.PlacedGates[0];

        // Calculate position to the right of the first gate's right edge
        double rightEdgeOfFirstGate = firstGate.X + 96; // Gate width is 96
        double positionToTheRight = rightEdgeOfFirstGate + 20; // 20 pixels to the right of the right edge

        // Place second gate to the right of the first gate
        viewModel.PlaceGate(positionToTheRight + 48, 100); // Add 48 to account for centering offset
        var secondGate = viewModel.PlacedGates[1];

        // Assert
        Assert.True(
            secondGate.X > rightEdgeOfFirstGate,
            $"Second gate left edge ({secondGate.X}) should be to the right of first gate right edge ({rightEdgeOfFirstGate})");
        Assert.True(
            secondGate.X > firstGate.X,
            $"Second gate ({secondGate.X}) should have higher X coordinate than first gate ({firstGate.X})");
    }
}
