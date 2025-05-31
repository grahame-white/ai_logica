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
        // Can't connect to the same gate
        if (from.GateId == to.GateId) return false;

        // Can connect output to input, or input to input (for fan-out)
        return (from.Type == ConnectionType.Output && to.Type == ConnectionType.Input) ||
               (from.Type == ConnectionType.Input && to.Type == ConnectionType.Input);
    }

    /// <summary>
    /// Generates orthogonal wire segments between two connection points
    /// </summary>
    private List<WireSegment> GenerateWireSegments(Connection from, Connection to)
    {
        var segments = new List<WireSegment>();

        // Simple orthogonal routing: horizontal then vertical
        // TODO: Implement collision avoidance with gates

        double midX = (from.X + to.X) / 2;

        // First segment: horizontal from start to midpoint
        segments.Add(new WireSegment
        {
            StartX = from.X,
            StartY = from.Y,
            EndX = midX,
            EndY = from.Y,
            Orientation = WireOrientation.Horizontal
        });

        // Second segment: vertical from midpoint height change
        segments.Add(new WireSegment
        {
            StartX = midX,
            StartY = from.Y,
            EndX = midX,
            EndY = to.Y,
            Orientation = WireOrientation.Vertical
        });

        // Third segment: horizontal to target
        segments.Add(new WireSegment
        {
            StartX = midX,
            StartY = to.Y,
            EndX = to.X,
            EndY = to.Y,
            Orientation = WireOrientation.Horizontal
        });

        return segments;
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
