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

            // Create connection points for the gate
            CreateConnectionsForGate(gate);

            PlacedGates.Add(gate);
            OnPropertyChanged(nameof(PlacedGates));

            // Intentionally keep gate selected and dragging state active to allow 
            // users to place multiple gates of the same type without re-selecting.
            // This implements the requirement for continuous gate placement workflow.
            // To reset selection, users can click elsewhere or select a different gate.
        }
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

    public void CancelDrag()
    {
        SelectedGate = null;
        IsDragging = false;
    }

    /// <summary>
    /// Starts a wiring operation from a connection point
    /// </summary>
    public void StartWiring(Connection connection)
    {
        ActiveConnection = connection;
        IsWiring = true;
    }

    /// <summary>
    /// Completes a wiring operation by connecting to another connection point
    /// </summary>
    public void CompleteWiring(Connection toConnection)
    {
        if (ActiveConnection != null && CanConnect(ActiveConnection, toConnection))
        {
            var wire = new Wire
            {
                Id = Guid.NewGuid(),
                FromConnectionId = ActiveConnection.Id,
                ToConnectionId = toConnection.Id,
                IsConnected = true
            };

            // Generate wire segments using orthogonal routing
            wire.Segments = GenerateWireSegments(ActiveConnection, toConnection);

            Wires.Add(wire);
            OnPropertyChanged(nameof(Wires));
        }

        CancelWiring();
    }

    /// <summary>
    /// Cancels the current wiring operation
    /// </summary>
    public void CancelWiring()
    {
        ActiveConnection = null;
        IsWiring = false;
    }

    /// <summary>
    /// Determines if two connections can be wired together
    /// </summary>
    private bool CanConnect(Connection from, Connection to)
    {
        // Can connect output to input, input to output, or input to input (for fan-out)
        // Same-gate connections are allowed for feedback loops
        return (from.Type == ConnectionType.Output && to.Type == ConnectionType.Input) ||
               (from.Type == ConnectionType.Input && to.Type == ConnectionType.Output) ||
               (from.Type == ConnectionType.Input && to.Type == ConnectionType.Input);
    }

    /// <summary>
    /// Generates orthogonal wire segments between two connection points
    /// </summary>
    private List<WireSegment> GenerateWireSegments(Connection from, Connection to)
    {
        var segments = new List<WireSegment>();

        // Improved orthogonal routing with collision avoidance
        double startX = from.X;
        double startY = from.Y;
        double endX = to.X;
        double endY = to.Y;

        // Find a safe intermediate X position that avoids gates
        double midX = FindSafeXPosition(startX, endX, startY, endY);

        // Create three-segment route: horizontal -> vertical -> horizontal
        if (Math.Abs(midX - startX) > 1) // Only add segment if there's meaningful distance
        {
            segments.Add(new WireSegment
            {
                StartX = startX,
                StartY = startY,
                EndX = midX,
                EndY = startY,
                Orientation = WireOrientation.Horizontal
            });
        }

        if (Math.Abs(endY - startY) > 1) // Only add segment if there's meaningful distance
        {
            segments.Add(new WireSegment
            {
                StartX = midX,
                StartY = startY,
                EndX = midX,
                EndY = endY,
                Orientation = WireOrientation.Vertical
            });
        }

        if (Math.Abs(endX - midX) > 1) // Only add segment if there's meaningful distance
        {
            segments.Add(new WireSegment
            {
                StartX = midX,
                StartY = endY,
                EndX = endX,
                EndY = endY,
                Orientation = WireOrientation.Horizontal
            });
        }

        return segments;
    }

    /// <summary>
    /// Finds a safe X position for wire routing that avoids gates
    /// </summary>
    private double FindSafeXPosition(double startX, double endX, double startY, double endY)
    {
        double midX = (startX + endX) / 2;

        // Check if the simple midpoint route would intersect any gates
        foreach (var gate in PlacedGates)
        {
            // Gate bounding box (with some padding for wire clearance)
            double gateLeft = gate.X - 10;
            double gateRight = gate.X + 96 + 10; // Gate width + padding
            double gateTop = gate.Y - 10;
            double gateBottom = gate.Y + 72 + 10; // Gate height + padding

            // Check if the vertical segment would pass through this gate
            // A vertical segment from startY to endY intersects the gate if:
            // 1. The X position is within gate bounds
            // 2. The Y range of the segment overlaps with the gate's Y range
            double segmentMinY = Math.Min(startY, endY);
            double segmentMaxY = Math.Max(startY, endY);
            bool verticalSegmentIntersects = midX >= gateLeft && midX <= gateRight &&
                                           segmentMinY <= gateBottom && segmentMaxY >= gateTop;

            if (verticalSegmentIntersects)
            {
                // Try routing around the gate
                double leftRoute = gateLeft - 15; // Route to the left of the gate
                double rightRoute = gateRight + 15; // Route to the right of the gate

                // Choose the route that's closer to the original midpoint
                if (Math.Abs(leftRoute - midX) <= Math.Abs(rightRoute - midX))
                {
                    midX = leftRoute;
                }
                else
                {
                    midX = rightRoute;
                }
                break; // Only avoid the first conflicting gate for simplicity
            }
        }

        return midX;
    }

    /// <summary>
    /// Finds a connection point near the given coordinates
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
}
