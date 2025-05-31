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

            // Calculate routing points that absolutely avoid all gate collisions
            var routingPoints = CalculateCollisionFreeRoute(start, end);
            path.AddRange(routingPoints);

            path.Add(new Point { X = end.X, Y = end.Y });
            return path;
        }

        private List<Point> CalculateCollisionFreeRoute(ConnectionPoint start, ConnectionPoint end)
        {
            var points = new List<Point>();
            var margin = 15; // Safety margin around gates

            // Get all gate boundaries to avoid
            var gateBounds = PlacedGates.Select(g => new
            {
                Left = g.X - margin,
                Right = g.X + 72 + margin, // 72 is the scaled gate width
                Top = g.Y - margin,
                Bottom = g.Y + 54 + margin, // 54 is the scaled gate height
                Gate = g
            }).ToList();

            // Strategy: Route orthogonally with collision avoidance
            // Try horizontal-first, then vertical-first routing and pick the best

            var route1 = TryHorizontalFirstRoute(start, end, gateBounds);
            var route2 = TryVerticalFirstRoute(start, end, gateBounds);

            // Choose the route with fewer collision issues (prefer horizontal-first)
            var chosenRoute = !HasCollisions(route1, gateBounds) ? route1 :
                             !HasCollisions(route2, gateBounds) ? route2 :
                             route1; // Fallback to route1 if both have issues

            return chosenRoute;
        }

        private List<Point> TryHorizontalFirstRoute(ConnectionPoint start, ConnectionPoint end, dynamic gateBounds)
        {
            var points = new List<Point>();

            // Go horizontal first, then vertical
            var intermediateX = end.X;
            var intermediateY = start.Y;

            // Check if horizontal segment would collide with gates
            var horizontalSegment = new { X1 = Math.Min(start.X, intermediateX), X2 = Math.Max(start.X, intermediateX), Y = start.Y };

            // If horizontal route collides, route around obstacles
            foreach (var bounds in gateBounds)
            {
                if (horizontalSegment.Y >= bounds.Top && horizontalSegment.Y <= bounds.Bottom &&
                    horizontalSegment.X1 <= bounds.Right && horizontalSegment.X2 >= bounds.Left)
                {
                    // Collision detected - route around
                    if (start.Y < end.Y)
                    {
                        // Route above the gate
                        intermediateY = bounds.Top - 10;
                    }
                    else
                    {
                        // Route below the gate  
                        intermediateY = bounds.Bottom + 10;
                    }
                    break;
                }
            }

            if (Math.Abs(intermediateY - start.Y) > 5) // Only add intermediate point if significantly different
            {
                points.Add(new Point { X = start.X, Y = intermediateY });
                points.Add(new Point { X = intermediateX, Y = intermediateY });
            }
            else
            {
                points.Add(new Point { X = intermediateX, Y = start.Y });
            }

            return points;
        }

        private List<Point> TryVerticalFirstRoute(ConnectionPoint start, ConnectionPoint end, dynamic gateBounds)
        {
            var points = new List<Point>();

            // Go vertical first, then horizontal
            var intermediateX = start.X;
            var intermediateY = end.Y;

            // Check if vertical segment would collide with gates
            var verticalSegment = new { X = start.X, Y1 = Math.Min(start.Y, intermediateY), Y2 = Math.Max(start.Y, intermediateY) };

            // If vertical route collides, route around obstacles
            foreach (var bounds in gateBounds)
            {
                if (verticalSegment.X >= bounds.Left && verticalSegment.X <= bounds.Right &&
                    verticalSegment.Y1 <= bounds.Bottom && verticalSegment.Y2 >= bounds.Top)
                {
                    // Collision detected - route around
                    if (start.X < end.X)
                    {
                        // Route to the left of the gate
                        intermediateX = bounds.Left - 10;
                    }
                    else
                    {
                        // Route to the right of the gate
                        intermediateX = bounds.Right + 10;
                    }
                    break;
                }
            }

            if (Math.Abs(intermediateX - start.X) > 5) // Only add intermediate point if significantly different
            {
                points.Add(new Point { X = intermediateX, Y = start.Y });
                points.Add(new Point { X = intermediateX, Y = intermediateY });
            }
            else
            {
                points.Add(new Point { X = start.X, Y = intermediateY });
            }

            return points;
        }

        private bool HasCollisions(List<Point> routePoints, dynamic gateBounds)
        {
            if (routePoints.Count == 0) return false;

            // Create full path including start and end points for collision checking
            var fullPath = new List<Point> { new Point { X = 0, Y = 0 } }; // Will be replaced with actual start
            fullPath.AddRange(routePoints);
            fullPath.Add(new Point { X = 0, Y = 0 }); // Will be replaced with actual end

            // Check each line segment for collisions
            for (int i = 0; i < fullPath.Count - 1; i++)
            {
                var p1 = fullPath[i];
                var p2 = fullPath[i + 1];

                foreach (var bounds in gateBounds)
                {
                    if (LineIntersectsRectangle(p1, p2, bounds))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool LineIntersectsRectangle(Point p1, Point p2, dynamic rect)
        {
            // Check if line segment from p1 to p2 intersects with rectangle
            var minX = Math.Min(p1.X, p2.X);
            var maxX = Math.Max(p1.X, p2.X);
            var minY = Math.Min(p1.Y, p2.Y);
            var maxY = Math.Max(p1.Y, p2.Y);

            // Check if line bounding box intersects with rectangle
            return !(maxX < rect.Left || minX > rect.Right || maxY < rect.Top || minY > rect.Bottom);
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
