using AiLogica.Core.ViewModels;

namespace AiLogica.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private string _welcomeMessage = "Logic Gate Design Canvas";
    private string? _selectedGate;
    private bool _isDragging;
    private double _mouseX;
    private double _mouseY;
    private List<PlacedGate> _placedGates = new();
    private List<Wire> _wires = new();
    private bool _isWiring;
    private Connection? _activeConnection;

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
        if (SelectedGate != null)
        {
            // Apply the same offset used during dragging to center the gate on cursor
            // Updated offsets for larger gate size (96x72 instead of 48x36)
            var gate = new PlacedGate
            {
                Type = SelectedGate,
                X = x - 48, // Same offset as dragging-gate positioning (half of 96px width)
                Y = y - 36, // Same offset as dragging-gate positioning (half of 72px height)
                Id = Guid.NewGuid()
            };

            Console.WriteLine($"[DEBUG] PlaceGate called:");
            Console.WriteLine($"  Click position: ({x}, {y})");
            Console.WriteLine($"  Gate position after offset: ({gate.X}, {gate.Y})");
            Console.WriteLine($"  Gate type: {gate.Type}");

            // Create connection points for the gate
            CreateConnectionsForGate(gate);

            PlacedGates.Add(gate);
            OnPropertyChanged(nameof(PlacedGates));

            // Log connection positions for debugging
            foreach (var connection in gate.Connections)
            {
                Console.WriteLine($"  Connection: {connection.Type} {connection.Index} at ({connection.X}, {connection.Y})");
            }

            // Intentionally keep gate selected and dragging state active to allow 
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
    /// Starts a wiring operation from a connection point.
    /// </summary>
    public void StartWiring(Connection connection)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        Console.WriteLine($"[DEBUG] StartWiring called:");
        Console.WriteLine($"  Connection: {connection.Type} {connection.Index} at ({connection.X}, {connection.Y})");
        Console.WriteLine($"  Gate ID: {connection.GateId}");

        ActiveConnection = connection;
        IsWiring = true;
    }

    /// <summary>
    /// Completes a wiring operation by connecting to another connection point.
    /// </summary>
    public void CompleteWiring(Connection toConnection)
    {
        if (toConnection == null)
            throw new ArgumentNullException(nameof(toConnection));

        Console.WriteLine($"[DEBUG] CompleteWiring called:");
        Console.WriteLine($"  From: {ActiveConnection?.Type} {ActiveConnection?.Index} at ({ActiveConnection?.X}, {ActiveConnection?.Y})");
        Console.WriteLine($"  To: {toConnection.Type} {toConnection.Index} at ({toConnection.X}, {toConnection.Y})");

        if (ActiveConnection != null && CanConnect(ActiveConnection, toConnection))
        {
            Console.WriteLine($"  Connection allowed: true");

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

            // Debug: Log the final wire segments before adding to collection
            Console.WriteLine($"[DEBUG] Final wire segments before adding to collection:");
            for (int i = 0; i < wire.Segments.Count; i++)
            {
                var seg = wire.Segments[i];
                Console.WriteLine($"  Segment {i}: ({seg.StartX}, {seg.StartY}) -> ({seg.EndX}, {seg.EndY}) [{seg.Orientation}]");
            }

            Wires.Add(wire);

            // Debug: Log each segment being added to the wire collection
            Console.WriteLine($"[DEBUG] Wire added to collection. Logging stored segments:");
            for (int i = 0; i < wire.Segments.Count; i++)
            {
                var seg = wire.Segments[i];
                Console.WriteLine($"  Stored segment {i}: ({seg.StartX}, {seg.StartY}) -> ({seg.EndX}, {seg.EndY}) [{seg.Orientation}]");
            }

            // Debug: Verify segments in the Wires collection after adding
            Console.WriteLine($"[DEBUG] Verifying wire segments in collection after adding:");
            var addedWire = Wires[^1];
            for (int i = 0; i < addedWire.Segments.Count; i++)
            {
                var seg = addedWire.Segments[i];
                Console.WriteLine($"  Collection segment {i}: ({seg.StartX}, {seg.StartY}) -> ({seg.EndX}, {seg.EndY}) [{seg.Orientation}]");
            }

            OnPropertyChanged(nameof(Wires));

            Console.WriteLine($"  Wire created with {wire.Segments.Count} segments");
            Console.WriteLine($"  Total wires in collection: {Wires.Count}");
        }
        else
        {
            Console.WriteLine($"  Connection not allowed");
            CancelWiring();
        }
    }

    /// <summary>
    /// Cancels the current wiring operation.
    /// </summary>
    public void CancelWiring()
    {
        Console.WriteLine($"[DEBUG] CancelWiring called - clearing active connection and wiring state");
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
    /// </summary>
    private static bool CanConnect(Connection from, Connection to)
    {
        // Can connect output to input, input to output, or input to input (for fan-out)
        // Same-gate connections are explicitly allowed for feedback loops (FR-3.7)
        return (from.Type == ConnectionType.Output && to.Type == ConnectionType.Input) ||
               (from.Type == ConnectionType.Input && to.Type == ConnectionType.Output) ||
               (from.Type == ConnectionType.Input && to.Type == ConnectionType.Input);
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
        }

        // Update connection positions based on gate position
        gate.UpdateConnectionPositions();
    }

    /// <summary>
    /// Generates orthogonal wire segments between two connection points.
    /// </summary>
    private List<WireSegment> GenerateWireSegments(Connection from, Connection to)
    {
        var segments = new List<WireSegment>();

        // Improved orthogonal routing with collision avoidance
        double startX = from.X;
        double startY = from.Y;
        double endX = to.X;
        double endY = to.Y;

        Console.WriteLine($"[DEBUG] GenerateWireSegments called:");
        Console.WriteLine($"  From connection: ({startX}, {startY}) - Type: {from.Type}");
        Console.WriteLine($"  To connection: ({endX}, {endY}) - Type: {to.Type}");

        // Find a safe intermediate X position that avoids gates
        double midX = FindSafeXPosition(startX, endX, startY, endY);

        Console.WriteLine($"  Using midX: {midX} for routing");

        // Create three-segment route: horizontal -> vertical -> horizontal
        // Only add segment if there's meaningful distance
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
            Console.WriteLine($"  Added horizontal segment 1: ({horizontalSegment1.StartX}, {horizontalSegment1.StartY}) -> ({horizontalSegment1.EndX}, {horizontalSegment1.EndY})");
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
            Console.WriteLine($"  Added vertical segment: ({verticalSegment.StartX}, {verticalSegment.StartY}) -> ({verticalSegment.EndX}, {verticalSegment.EndY})");
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
            Console.WriteLine($"  Added horizontal segment 2: ({horizontalSegment2.StartX}, {horizontalSegment2.StartY}) -> ({horizontalSegment2.EndX}, {horizontalSegment2.EndY})");
        }

        Console.WriteLine($"  Total segments created: {segments.Count}");
        return segments;
    }

    /// <summary>
    /// Finds a safe X position for wire routing that avoids gates.
    /// </summary>
    private double FindSafeXPosition(double startX, double endX, double startY, double endY)
    {
        double midX = (startX + endX) / 2;

        Console.WriteLine($"[DEBUG] FindSafeXPosition called:");
        Console.WriteLine($"  Start: ({startX}, {startY})");
        Console.WriteLine($"  End: ({endX}, {endY})");
        Console.WriteLine($"  Initial midX: {midX}");
        Console.WriteLine($"  Checking {PlacedGates.Count} gates for collisions...");

        // Check if the simple midpoint route would intersect any gates
        for (int i = 0; i < PlacedGates.Count; i++)
        {
            var gate = PlacedGates[i];
            Console.WriteLine($"  Gate {i}: Type={gate.Type}, Position=({gate.X}, {gate.Y})");

            // Gate bounding box (with some padding for wire clearance)
            double gateLeft = gate.X - 10;
            double gateRight = gate.X + 96 + 10; // Gate width + padding
            double gateTop = gate.Y - 10;
            double gateBottom = gate.Y + 72 + 10; // Gate height + padding

            Console.WriteLine($"    Gate bounds: Left={gateLeft}, Right={gateRight}, Top={gateTop}, Bottom={gateBottom}");

            // Check if the vertical segment would pass through this gate
            // A vertical segment from startY to endY intersects the gate if:
            // 1. The X position is within gate bounds
            // 2. The Y range of the segment overlaps with the gate's Y range
            double segmentMinY = Math.Min(startY, endY);
            double segmentMaxY = Math.Max(startY, endY);

            Console.WriteLine($"    Segment Y range: {segmentMinY} to {segmentMaxY}");

            bool xInBounds = midX >= gateLeft && midX <= gateRight;
            bool yOverlaps = segmentMinY <= gateBottom && segmentMaxY >= gateTop;
            bool verticalSegmentIntersects = xInBounds && yOverlaps;

            Console.WriteLine($"    X in bounds ({midX} >= {gateLeft} && {midX} <= {gateRight}): {xInBounds}");
            Console.WriteLine($"    Y overlaps ({segmentMinY} <= {gateBottom} && {segmentMaxY} >= {gateTop}): {yOverlaps}");
            Console.WriteLine($"    Vertical segment intersects: {verticalSegmentIntersects}");

            if (verticalSegmentIntersects)
            {
                Console.WriteLine($"    COLLISION DETECTED! Rerouting around gate {i}");

                // Try routing around the gate
                double leftRoute = gateLeft - 15; // Route to the left of the gate
                double rightRoute = gateRight + 15; // Route to the right of the gate

                Console.WriteLine($"    Left route option: {leftRoute}");
                Console.WriteLine($"    Right route option: {rightRoute}");
                Console.WriteLine($"    Distance to left: {Math.Abs(leftRoute - midX)}");
                Console.WriteLine($"    Distance to right: {Math.Abs(rightRoute - midX)}");

                // Choose the route that's closer to the original midpoint
                if (Math.Abs(leftRoute - midX) <= Math.Abs(rightRoute - midX))
                {
                    midX = leftRoute;
                    Console.WriteLine($"    Chose LEFT route: {midX}");
                }
                else
                {
                    midX = rightRoute;
                    Console.WriteLine($"    Chose RIGHT route: {midX}");
                }

                break; // Only avoid the first conflicting gate for simplicity
            }
            else
            {
                Console.WriteLine($"    No collision with gate {i}");
            }
        }

        Console.WriteLine($"  Final midX: {midX}");
        return midX;
    }
}
