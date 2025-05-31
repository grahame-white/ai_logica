using AiLogica.Core.ViewModels;

namespace AiLogica.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private string _welcomeMessage = "Logic Gate Design Canvas";
        private string? _selectedGate;
        private bool _isDragging;
        private double _mouseX;
        private double _mouseY;
        private List<PlacedGate> _placedGates = new();
        private List<Wire> _wires = new();
        private Guid? _wireStartConnectionId;
        private bool _isWiring;

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

        public Guid? SelectedConnectionId => _wireStartConnectionId;

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
                PlacedGates.Add(new PlacedGate
                {
                    Type = SelectedGate,
                    X = x - 30, // Same offset as dragging-gate positioning
                    Y = y - 15, // Same offset as dragging-gate positioning
                    Id = Guid.NewGuid()
                });
                OnPropertyChanged(nameof(PlacedGates));

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

        public void StartWiring(Guid connectionId)
        {
            _wireStartConnectionId = connectionId;
            IsWiring = true;
            OnPropertyChanged(nameof(SelectedConnectionId));
        }

        public void CompleteWiring(Guid endConnectionId)
        {
            if (_wireStartConnectionId.HasValue && _wireStartConnectionId != endConnectionId)
            {
                var wire = new Wire
                {
                    Id = Guid.NewGuid(),
                    StartConnectionId = _wireStartConnectionId.Value,
                    EndConnectionId = endConnectionId
                };

                // Calculate path using orthogonal routing
                wire.Path = CalculateWirePath(_wireStartConnectionId.Value, endConnectionId);

                Wires.Add(wire);
                OnPropertyChanged(nameof(Wires));
            }

            _wireStartConnectionId = null;
            IsWiring = false;
            OnPropertyChanged(nameof(SelectedConnectionId));
        }

        public void CancelWiring()
        {
            _wireStartConnectionId = null;
            IsWiring = false;
            OnPropertyChanged(nameof(SelectedConnectionId));
        }

        public List<ConnectionPoint> GetAllConnectionPoints()
        {
            var allPoints = new List<ConnectionPoint>();
            foreach (var gate in PlacedGates)
            {
                allPoints.AddRange(gate.GetConnectionPoints());
            }
            return allPoints;
        }

        public List<ConnectionPoint> GetUnconnectedConnectionPoints()
        {
            var allPoints = GetAllConnectionPoints();
            var connectedIds = new HashSet<Guid>();

            foreach (var wire in Wires)
            {
                connectedIds.Add(wire.StartConnectionId);
                connectedIds.Add(wire.EndConnectionId);
            }

            return allPoints.Where(p => !connectedIds.Contains(p.Id)).ToList();
        }

        private List<Point> CalculateWirePath(Guid startId, Guid endId)
        {
            var allPoints = GetAllConnectionPoints();
            var start = allPoints.FirstOrDefault(p => p.Id == startId);
            var end = allPoints.FirstOrDefault(p => p.Id == endId);

            if (start == null || end == null) return new List<Point>();

            var path = new List<Point>();
            path.Add(new Point { X = start.X, Y = start.Y });

            // Improved orthogonal routing that avoids gate collisions
            var startGate = PlacedGates.FirstOrDefault(g => g.Id == start.GateId);
            var endGate = PlacedGates.FirstOrDefault(g => g.Id == end.GateId);

            if (startGate != null && endGate != null)
            {
                // Calculate safe routing points that avoid gate bodies
                var routingPoints = CalculateSafeRoutingPoints(start, end, startGate, endGate);
                path.AddRange(routingPoints);
            }
            else
            {
                // Fallback to simple routing if gate information is missing
                if (Math.Abs(start.X - end.X) > Math.Abs(start.Y - end.Y))
                {
                    var midX = start.X + (end.X - start.X) * 0.5;
                    path.Add(new Point { X = midX, Y = start.Y });
                    path.Add(new Point { X = midX, Y = end.Y });
                }
                else
                {
                    var midY = start.Y + (end.Y - start.Y) * 0.5;
                    path.Add(new Point { X = start.X, Y = midY });
                    path.Add(new Point { X = end.X, Y = midY });
                }
            }

            path.Add(new Point { X = end.X, Y = end.Y });
            return path;
        }

        private List<Point> CalculateSafeRoutingPoints(ConnectionPoint start, ConnectionPoint end, PlacedGate startGate, PlacedGate endGate)
        {
            var points = new List<Point>();

            // Define gate boundaries (using larger 50% scaled gate size: 72x54)
            var startBounds = new { Left = startGate.X, Right = startGate.X + 72, Top = startGate.Y, Bottom = startGate.Y + 54 };
            var endBounds = new { Left = endGate.X, Right = endGate.X + 72, Top = endGate.Y, Bottom = endGate.Y + 54 };

            // Add some margin around gates for cleaner routing
            var margin = 10;

            if (start.Type == ConnectionType.Output)
            {
                // Route from output: go right from gate, then route to input
                var clearanceX = startBounds.Right + margin;
                points.Add(new Point { X = clearanceX, Y = start.Y });

                if (end.Type == ConnectionType.Input)
                {
                    // Output to Input: route around end gate if necessary
                    var targetX = endBounds.Left - margin;

                    if (Math.Abs(start.Y - end.Y) < 20) // Similar Y levels
                    {
                        points.Add(new Point { X = targetX, Y = start.Y });
                        points.Add(new Point { X = targetX, Y = end.Y });
                    }
                    else
                    {
                        // Route around gates vertically
                        var routeY = start.Y < end.Y ? Math.Max(startBounds.Bottom, endBounds.Bottom) + margin : Math.Min(startBounds.Top, endBounds.Top) - margin;
                        points.Add(new Point { X = clearanceX, Y = routeY });
                        points.Add(new Point { X = targetX, Y = routeY });
                        points.Add(new Point { X = targetX, Y = end.Y });
                    }
                }
            }
            else
            {
                // Route from input: this shouldn't normally happen in proper logic design, but handle it
                var clearanceX = startBounds.Left - margin;
                points.Add(new Point { X = clearanceX, Y = start.Y });

                if (end.Type == ConnectionType.Input)
                {
                    // Input to Input: route to the left, then to target
                    var targetX = endBounds.Left - margin;
                    var routeY = Math.Min(start.Y, end.Y) - margin;

                    points.Add(new Point { X = clearanceX, Y = routeY });
                    points.Add(new Point { X = targetX, Y = routeY });
                    points.Add(new Point { X = targetX, Y = end.Y });
                }
            }

            return points;
        }
    }

    public class PlacedGate
    {
        public string Type { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public Guid Id { get; set; }

        private List<ConnectionPoint>? _connectionPoints;

        public List<ConnectionPoint> GetConnectionPoints()
        {
            if (_connectionPoints == null)
            {
                _connectionPoints = new List<ConnectionPoint>();

                // OR gate connection points based on updated larger SVG coordinates
                if (Type == "OR")
                {
                    // Input connections (left side) - scaled for 50% larger gate
                    _connectionPoints.Add(new ConnectionPoint
                    {
                        Id = Guid.NewGuid(),
                        Type = ConnectionType.Input,
                        X = X + 3,  // Scaled from 2 to 3
                        Y = Y + 21, // Scaled from 14 to 21  
                        GateId = Id
                    });
                    _connectionPoints.Add(new ConnectionPoint
                    {
                        Id = Guid.NewGuid(),
                        Type = ConnectionType.Input,
                        X = X + 3,  // Scaled from 2 to 3
                        Y = Y + 33, // Scaled from 22 to 33
                        GateId = Id
                    });

                    // Output connection (right side) - scaled for 50% larger gate
                    _connectionPoints.Add(new ConnectionPoint
                    {
                        Id = Guid.NewGuid(),
                        Type = ConnectionType.Output,
                        X = X + 66, // Scaled from 44 to 66
                        Y = Y + 27, // Scaled from 18 to 27
                        GateId = Id
                    });
                }
                // AND gate connection points based on SVG coordinates
                else if (Type == "AND")
                {
                    // Input connections (left side)
                    _connectionPoints.Add(new ConnectionPoint
                    {
                        Id = Guid.NewGuid(),
                        Type = ConnectionType.Input,
                        X = X + 2,
                        Y = Y + 14,
                        GateId = Id
                    });
                    _connectionPoints.Add(new ConnectionPoint
                    {
                        Id = Guid.NewGuid(),
                        Type = ConnectionType.Input,
                        X = X + 2,
                        Y = Y + 22,
                        GateId = Id
                    });

                    // Output connection (right side) - AND gate output where line starts at x=34 in SVG
                    // But connection point should be at gate body edge, not line end
                    _connectionPoints.Add(new ConnectionPoint
                    {
                        Id = Guid.NewGuid(),
                        Type = ConnectionType.Output,
                        X = X + 34,
                        Y = Y + 18,
                        GateId = Id
                    });
                }
            }

            return _connectionPoints;
        }
    }

    public class ConnectionPoint
    {
        public Guid Id { get; set; }
        public ConnectionType Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Guid GateId { get; set; }
    }

    public enum ConnectionType
    {
        Input,
        Output
    }

    public class Wire
    {
        public Guid Id { get; set; }
        public Guid StartConnectionId { get; set; }
        public Guid EndConnectionId { get; set; }
        public List<Point> Path { get; set; } = new();
    }

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
