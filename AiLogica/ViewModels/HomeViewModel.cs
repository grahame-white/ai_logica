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
        }

        public void CancelWiring()
        {
            _wireStartConnectionId = null;
            IsWiring = false;
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

            // Simple orthogonal routing
            if (Math.Abs(start.X - end.X) > Math.Abs(start.Y - end.Y))
            {
                // Route horizontally first
                var midX = start.X + (end.X - start.X) * 0.5;
                path.Add(new Point { X = midX, Y = start.Y });
                path.Add(new Point { X = midX, Y = end.Y });
            }
            else
            {
                // Route vertically first
                var midY = start.Y + (end.Y - start.Y) * 0.5;
                path.Add(new Point { X = start.X, Y = midY });
                path.Add(new Point { X = end.X, Y = midY });
            }

            path.Add(new Point { X = end.X, Y = end.Y });
            return path;
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

                // OR gate connection points based on SVG coordinates from Home.razor
                if (Type == "OR")
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

                    // Output connection (right side)
                    _connectionPoints.Add(new ConnectionPoint
                    {
                        Id = Guid.NewGuid(),
                        Type = ConnectionType.Output,
                        X = X + 44,
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
