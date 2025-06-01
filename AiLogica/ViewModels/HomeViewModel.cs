using AiLogica.Core.ViewModels;
using Microsoft.Extensions.Logging;

namespace AiLogica.ViewModels;

/// <summary>
/// Requirements Traceability: Core business logic for gate layout and wiring functionality.
/// FR-2.2, FR-2.5: Gate selection and dragging state management.
/// FR-2.4: Gate placement with coordinate calculations.  
/// FR-3: Complete wiring system implementation.
/// FR-3.5-3.7: Connection validation and routing logic.
/// FR-3.8-3.9: Orthogonal wire routing with collision avoidance.
/// See TRACEABILITY_MATRIX.md for complete mapping.
/// </summary>
public class HomeViewModel : ViewModelBase
{
    private readonly ILogger<HomeViewModel> _logger;
    private string _welcomeMessage = "Logic Gate Design Canvas";
    private string? _selectedGate;
    private bool _isDragging;
    private double _mouseX;
    private double _mouseY;
    private List<PlacedGate> _placedGates = new();
    private List<Wire> _wires = new();
    private bool _isWiring;
    private Connection? _activeConnection;

    public HomeViewModel(ILogger<HomeViewModel> logger)
    {
        _logger = logger;
    }

    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }

    public string? SelectedGate
    {
        get => _selectedGate;
        set => SetProperty(ref _selectedGate, value);
    }

    public bool IsDragging
    {
        get => _isDragging;
        set => SetProperty(ref _isDragging, value);
    }

    public double MouseX
    {
        get => _mouseX;
        set => SetProperty(ref _mouseX, value);
    }

    public double MouseY
    {
        get => _mouseY;
        set => SetProperty(ref _mouseY, value);
    }

    public List<PlacedGate> PlacedGates
    {
        get => _placedGates;
        set => SetProperty(ref _placedGates, value);
    }

    public List<Wire> Wires
    {
        get => _wires;
        set => SetProperty(ref _wires, value);
    }

    public bool IsWiring
    {
        get => _isWiring;
        set => SetProperty(ref _isWiring, value);
    }

    public Connection? ActiveConnection
    {
        get => _activeConnection;
        set => SetProperty(ref _activeConnection, value);
    }

    public void SelectGate(string gateType)
    {
        // FR-2.2: Previous selection clearing handled by SetProperty
        SelectedGate = gateType;
        IsDragging = true;
        // Reset mouse position to avoid showing gate at stale coordinates
        MouseX = 0;
        MouseY = 0;
    }

    public void UpdateMousePosition(double x, double y)
    {
        MouseX = x;
        MouseY = y;
    }

    public void PlaceGate(double x, double y)
    {
        // FR-2.4: Gate placement by clicking on canvas
        if (SelectedGate != null)
        {
            // Apply the same offset used during dragging to center the gate on cursor
            // Different gate types have different sizes
            var (offsetX, offsetY) = GetGateOffsets(SelectedGate);
            var gate = new PlacedGate
            {
                Type = SelectedGate,
                X = x - offsetX,
                Y = y - offsetY,
                Id = Guid.NewGuid()
            };

            _logger.LogDebug(
                "PlaceGate called: Click position: ({X}, {Y}), Gate position after offset: ({GateX}, {GateY}), Gate type: {GateType}",
                x,
                y,
                gate.X,
                gate.Y,
                gate.Type);

            // Create connection points for the gate
            CreateConnectionsForGate(gate);

            PlacedGates.Add(gate);
            OnPropertyChanged(nameof(PlacedGates));

            // Log connection positions for debugging
            foreach (var connection in gate.Connections)
            {
                _logger.LogDebug(
                    "Connection created: {Type} {Index} at ({X}, {Y})",
                    connection.Type,
                    connection.Index,
                    connection.X,
                    connection.Y);
            }

            // FR-2.5: Intentionally keep gate selected and dragging state active to allow 
            // users to place multiple gates of the same type without re-selecting.
            // This implements the requirement for continuous gate placement workflow.
            // To reset selection, users can click elsewhere or select a different gate.
        }
    }

    public void CancelDrag()
    {
        SelectedGate = null;
        IsDragging = false;
    }

    /// <summary>
    /// Toggles the value of a constant gate between 0 and 1.
    /// Also updates the gate type to match the new value.
    /// </summary>
    public void ToggleConstantValue(PlacedGate gate)
    {
        if (gate == null) return;

        // Only toggle constant gates
        if (gate.Type == "CONSTANT0" || gate.Type == "CONSTANT1")
        {
            // Toggle the value
            gate.Value = gate.Value == 0 ? 1 : 0;

            // Update the gate type to match the new value
            gate.Type = gate.Value == 0 ? "CONSTANT0" : "CONSTANT1";

            _logger.LogDebug(
                "Toggled constant gate {GateId} to value {Value}, type {Type}",
                gate.Id,
                gate.Value,
                gate.Type);

            // Notify that the gates collection has changed
            OnPropertyChanged(nameof(PlacedGates));
        }
    }

    /// <summary>
    /// FR-3.4: Starts a wiring operation from a connection point.
    /// </summary>
    public void StartWiring(Connection connection)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        _logger.LogDebug(
            "StartWiring called: Connection: {Type} {Index} at ({X}, {Y}), Gate ID: {GateId}",
            connection.Type,
            connection.Index,
            connection.X,
            connection.Y,
            connection.GateId);

        ActiveConnection = connection;
        IsWiring = true;
    }

    /// <summary>
    /// FR-3.4: Completes a wiring operation by connecting to another connection point.
    /// </summary>
    public void CompleteWiring(Connection toConnection)
    {
        if (toConnection == null)
            throw new ArgumentNullException(nameof(toConnection));

        _logger.LogDebug(
            "CompleteWiring called: From: {FromType} {FromIndex} at ({FromX}, {FromY}), To: {ToType} {ToIndex} at ({ToX}, {ToY})",
            ActiveConnection?.Type,
            ActiveConnection?.Index,
            ActiveConnection?.X,
            ActiveConnection?.Y,
            toConnection.Type,
            toConnection.Index,
            toConnection.X,
            toConnection.Y);

        if (ActiveConnection != null && CanConnect(ActiveConnection, toConnection))
        {
            _logger.LogDebug("Connection allowed: true");

            // Store reference to active connection before clearing wiring state
            var fromConnection = ActiveConnection;

            // Clear wiring state IMMEDIATELY to hide preview wire before adding routed wire
            CancelWiring();

            var wire = new Wire
            {
                Id = Guid.NewGuid(),
                FromConnectionId = fromConnection.Id,
                ToConnectionId = toConnection.Id,
                IsConnected = true
            };

            // Generate wire segments using orthogonal routing
            wire.Segments = GenerateWireSegments(fromConnection, toConnection);

            _logger.LogDebug("Final wire segments before adding to collection: {SegmentCount} segments", wire.Segments.Count);
            for (int i = 0; i < wire.Segments.Count; i++)
            {
                var seg = wire.Segments[i];
                _logger.LogTrace(
                    "Segment {Index}: ({StartX}, {StartY}) -> ({EndX}, {EndY}) [{Orientation}]",
                    i,
                    seg.StartX,
                    seg.StartY,
                    seg.EndX,
                    seg.EndY,
                    seg.Orientation);
            }

            Wires.Add(wire);

            // Debug: Log each segment being added to the wire collection
            _logger.LogTrace("Wire added to collection. Logging stored segments:");
            for (int i = 0; i < wire.Segments.Count; i++)
            {
                var seg = wire.Segments[i];
                _logger.LogTrace(
                    "Stored segment {Index}: ({StartX}, {StartY}) -> ({EndX}, {EndY}) [{Orientation}]",
                    i,
                    seg.StartX,
                    seg.StartY,
                    seg.EndX,
                    seg.EndY,
                    seg.Orientation);
            }

            _logger.LogTrace("Wire added to collection. Verifying segments in collection after adding:");
            var addedWire = Wires[Wires.Count - 1];
            for (int i = 0; i < addedWire.Segments.Count; i++)
            {
                var seg = addedWire.Segments[i];
                _logger.LogTrace(
                    "Collection segment {Index}: ({StartX}, {StartY}) -> ({EndX}, {EndY}) [{Orientation}]",
                    i,
                    seg.StartX,
                    seg.StartY,
                    seg.EndX,
                    seg.EndY,
                    seg.Orientation);
            }

            OnPropertyChanged(nameof(Wires));

            _logger.LogDebug(
                "Wire created with {SegmentCount} segments. Total wires in collection: {TotalWires}",
                wire.Segments.Count,
                Wires.Count);
        }
        else
        {
            _logger.LogDebug("Connection not allowed");
            CancelWiring();
        }
    }

    /// <summary>
    /// Cancels the current wiring operation.
    /// </summary>
    public void CancelWiring()
    {
        _logger.LogDebug("CancelWiring called - clearing active connection and wiring state");
        ActiveConnection = null;
        IsWiring = false;
        OnPropertyChanged(nameof(IsWiring));
        OnPropertyChanged(nameof(ActiveConnection));
    }

    /// <summary>
    /// Finds a connection point near the given coordinates.
    /// </summary>
    public Connection? FindConnectionAt(double x, double y, double tolerance = 10)
    {
        foreach (var gate in PlacedGates)
        {
            foreach (var connection in gate.Connections)
            {
                double distance = Math.Sqrt(Math.Pow(connection.X - x, 2) + Math.Pow(connection.Y - y, 2));
                if (distance <= tolerance)
                {
                    return connection;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Determines if two connections can be wired together.
    /// FR-3.5: Output to input connections.
    /// FR-3.6: Input to input connections (fan-out capability).  
    /// FR-3.7: Same-gate connections for feedback loops.
    /// </summary>
    private static bool CanConnect(Connection from, Connection to)
    {
        // Can connect output to input, input to output, or input to input (for fan-out)
        // Same-gate connections are explicitly allowed for feedback loops (FR-3.7)
        return (from.Type == ConnectionType.Output && to.Type == ConnectionType.Input) ||
               (from.Type == ConnectionType.Input && to.Type == ConnectionType.Output) ||
               (from.Type == ConnectionType.Input && to.Type == ConnectionType.Input);
    }

    /// <summary>
    /// Gets the offset values for centering gates during dragging and placement.
    /// </summary>
    private static (double offsetX, double offsetY) GetGateOffsets(string gateType)
    {
        return gateType switch
        {
            "CONSTANT0" or "CONSTANT1" => (16, 8), // Half of 32x16
            "OR" => (48, 36), // Half of 96x72
            _ => (24, 18) // Default fallback
        };
    }

    private void CreateConnectionsForGate(PlacedGate gate)
    {
        switch (gate.Type)
        {
            case "OR":
                // OR gate has 2 inputs and 1 output
                gate.Connections.Add(new Connection
                {
                    Id = Guid.NewGuid(),
                    GateId = gate.Id,
                    Type = ConnectionType.Input,
                    Index = 0
                });
                gate.Connections.Add(new Connection
                {
                    Id = Guid.NewGuid(),
                    GateId = gate.Id,
                    Type = ConnectionType.Input,
                    Index = 1
                });
                gate.Connections.Add(new Connection
                {
                    Id = Guid.NewGuid(),
                    GateId = gate.Id,
                    Type = ConnectionType.Output,
                    Index = 0
                });
                break;

            case "CONSTANT0":
                // Constant 0 gate has 0 inputs and 1 output
                gate.Value = 0;
                gate.Connections.Add(new Connection
                {
                    Id = Guid.NewGuid(),
                    GateId = gate.Id,
                    Type = ConnectionType.Output,
                    Index = 0
                });
                break;

            case "CONSTANT1":
                // Constant 1 gate has 0 inputs and 1 output
                gate.Value = 1;
                gate.Connections.Add(new Connection
                {
                    Id = Guid.NewGuid(),
                    GateId = gate.Id,
                    Type = ConnectionType.Output,
                    Index = 0
                });
                break;
        }

        // Update connection positions based on gate position
        gate.UpdateConnectionPositions();
    }

    /// <summary>
    /// FR-3.8: Generates orthogonal wire segments between two connection points.
    /// FR-3.16: Only creates segments when there is meaningful distance between points.
    /// </summary>
    private List<WireSegment> GenerateWireSegments(Connection from, Connection to)
    {
        var segments = new List<WireSegment>();

        // Improved orthogonal routing with collision avoidance
        double startX = from.X;
        double startY = from.Y;
        double endX = to.X;
        double endY = to.Y;

        _logger.LogDebug(
            "GenerateWireSegments called: From connection: ({StartX}, {StartY}) - Type: {FromType}, To connection: ({EndX}, {EndY}) - Type: {ToType}",
            startX,
            startY,
            from.Type,
            endX,
            endY,
            to.Type);

        // FR-3.9: Find a safe intermediate X position that avoids gates
        double midX = FindSafeXPosition(startX, endX, startY, endY);

        _logger.LogDebug("Using midX: {MidX} for routing", midX);

        // FR-3.8: Create three-segment route: horizontal -> vertical -> horizontal
        // FR-3.16: Only add segment if there's meaningful distance
        if (Math.Abs(midX - startX) > 1)
        {
            var horizontalSegment1 = new WireSegment
            {
                StartX = startX,
                StartY = startY,
                EndX = midX,
                EndY = startY,
                Orientation = WireOrientation.Horizontal
            };
            segments.Add(horizontalSegment1);
            _logger.LogTrace(
                "Added horizontal segment 1: ({StartX}, {StartY}) -> ({EndX}, {EndY})",
                horizontalSegment1.StartX,
                horizontalSegment1.StartY,
                horizontalSegment1.EndX,
                horizontalSegment1.EndY);
        }

        // Only add segment if there's meaningful distance
        if (Math.Abs(endY - startY) > 1)
        {
            var verticalSegment = new WireSegment
            {
                StartX = midX,
                StartY = startY,
                EndX = midX,
                EndY = endY,
                Orientation = WireOrientation.Vertical
            };
            segments.Add(verticalSegment);
            _logger.LogTrace(
                "Added vertical segment: ({StartX}, {StartY}) -> ({EndX}, {EndY})",
                verticalSegment.StartX,
                verticalSegment.StartY,
                verticalSegment.EndX,
                verticalSegment.EndY);
        }

        // Only add segment if there's meaningful distance
        if (Math.Abs(endX - midX) > 1)
        {
            var horizontalSegment2 = new WireSegment
            {
                StartX = midX,
                StartY = endY,
                EndX = endX,
                EndY = endY,
                Orientation = WireOrientation.Horizontal
            };
            segments.Add(horizontalSegment2);
            _logger.LogTrace(
                "Added horizontal segment 2: ({StartX}, {StartY}) -> ({EndX}, {EndY})",
                horizontalSegment2.StartX,
                horizontalSegment2.StartY,
                horizontalSegment2.EndX,
                horizontalSegment2.EndY);
        }

        _logger.LogDebug("Total segments created: {SegmentCount}", segments.Count);
        return segments;
    }

    /// <summary>
    /// FR-3.9: Finds a safe X position for wire routing that avoids gates.
    /// </summary>
    private double FindSafeXPosition(double startX, double endX, double startY, double endY)
    {
        double midX = (startX + endX) / 2;

        _logger.LogDebug(
            "FindSafeXPosition called: Start: ({StartX}, {StartY}), End: ({EndX}, {EndY}), Initial midX: {MidX}, Checking {GateCount} gates for collisions",
            startX,
            startY,
            endX,
            endY,
            midX,
            PlacedGates.Count);

        // Check if the simple midpoint route would intersect any gates
        for (int i = 0; i < PlacedGates.Count; i++)
        {
            var gate = PlacedGates[i];
            _logger.LogTrace(
                "Gate {Index}: Type={Type}, Position=({X}, {Y})",
                i,
                gate.Type,
                gate.X,
                gate.Y);

            // Gate bounding box (with some padding for wire clearance)
            // Use explicit left/right edge calculations to avoid confusion
            double gateLeft = CoordinateHelper.GetPositionToTheLeftOf(gate.X, 10);  // Left edge with padding (LOWER X)
            double gateRight = CoordinateHelper.GetPositionToTheRightOf(gate.X, 96, 10); // Right edge with padding (HIGHER X)
            double gateTop = gate.Y - 10;
            double gateBottom = gate.Y + 72 + 10; // Gate height + padding

            _logger.LogTrace(
                "Gate bounds: Left={Left}, Right={Right}, Top={Top}, Bottom={Bottom}",
                gateLeft,
                gateRight,
                gateTop,
                gateBottom);

            // Check if the vertical segment would pass through this gate
            // A vertical segment from startY to endY intersects the gate if:
            // 1. The X position is within gate bounds
            // 2. The Y range of the segment overlaps with the gate's Y range
            double segmentMinY = Math.Min(startY, endY);
            double segmentMaxY = Math.Max(startY, endY);

            _logger.LogTrace("Segment Y range: {MinY} to {MaxY}", segmentMinY, segmentMaxY);

            bool xInBounds = midX >= gateLeft && midX <= gateRight;
            bool yOverlaps = segmentMinY <= gateBottom && segmentMaxY >= gateTop;
            bool verticalSegmentIntersects = xInBounds && yOverlaps;

            _logger.LogTrace(
                "X in bounds ({MidX1} >= {Left} && {MidX2} <= {Right}): {XInBounds}, Y overlaps ({MinY} <= {Bottom} && {MaxY} >= {Top}): {YOverlaps}, Vertical segment intersects: {Intersects}",
                midX,
                gateLeft,
                midX,
                gateRight,
                xInBounds,
                segmentMinY,
                gateBottom,
                segmentMaxY,
                gateTop,
                yOverlaps,
                verticalSegmentIntersects);

            if (verticalSegmentIntersects)
            {
                _logger.LogDebug("COLLISION DETECTED! Rerouting around gate {Index}", i);

                // Try routing around the gate using explicit left/right coordinate calculations
                // LEFT route = position to the LEFT of gate (LOWER X coordinate)
                // RIGHT route = position to the RIGHT of gate (HIGHER X coordinate)
                double leftRoute = CoordinateHelper.GetPositionToTheLeftOf(gate.X, 15);
                double rightRoute = CoordinateHelper.GetPositionToTheRightOf(gate.X, 96, 15);

                _logger.LogTrace(
                    "Left route option: {LeftRoute}, Right route option: {RightRoute}, Distance to left: {LeftDistance}, Distance to right: {RightDistance}",
                    leftRoute,
                    rightRoute,
                    Math.Abs(leftRoute - midX),
                    Math.Abs(rightRoute - midX));

                // Choose the route that's closer to the original midpoint
                if (Math.Abs(leftRoute - midX) <= Math.Abs(rightRoute - midX))
                {
                    midX = leftRoute;
                    _logger.LogTrace("Chose LEFT route (lower X): {MidX}", midX);
                }
                else
                {
                    midX = rightRoute;
                    _logger.LogTrace("Chose RIGHT route (higher X): {MidX}", midX);
                }

                break; // Only avoid the first conflicting gate for simplicity
            }
            else
            {
                _logger.LogTrace("No collision with gate {Index}", i);
            }
        }

        _logger.LogDebug("Final midX: {MidX}", midX);
        return midX;
    }
}
