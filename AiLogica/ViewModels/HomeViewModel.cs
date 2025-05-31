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

    public string WelcomeMessage
    {
<<<<<<< HEAD
        private string _welcomeMessage = "Logic Gate Design Canvas";
        private string? _selectedGate;
        private bool _isDragging;
        private double _mouseX;
        private double _mouseY;
        private List<PlacedGate> _placedGates = new();
        private List<Wire> _wires = new();
        private Guid? _wireStartConnectionId;
        private bool _isWiring;
=======
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }
>>>>>>> main

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

    public void SelectGate(string gateType)
    {
        SelectedGate = gateType;
        IsDragging = true;
        // Reset mouse position to avoid showing gate at stale coordinates
        MouseX = 0;
        MouseY = 0;
    }

<<<<<<< HEAD
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
=======
    public void UpdateMousePosition(double x, double y)
    {
        MouseX = x;
        MouseY = y;
    }
>>>>>>> main

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
            var margin = 20; // Increased safety margin around gates

            // Get all gate boundaries to avoid
            var gateBounds = PlacedGates.Select(g => new GateBound
            {
                Left = g.X - margin,
                Right = g.X + 72 + margin, // 72 is the scaled gate width
                Top = g.Y - margin,
                Bottom = g.Y + 54 + margin, // 54 is the scaled gate height
                Gate = g
            }).ToList();

            // Special handling for same-gate connections - always route around the gate
            if (start.GateId == end.GateId)
            {
                return RouteSameGateConnection(start, end, gateBounds);
            }

            // Try multiple routing strategies and pick collision-free one
            var routes = new List<List<Point>>
            {
                TryHorizontalFirstRoute(start, end, gateBounds),
                TryVerticalFirstRoute(start, end, gateBounds),
                TryAdvancedRoute(start, end, gateBounds)
            };

            // Return the first collision-free route
            foreach (var route in routes)
            {
                if (!HasCollisions(start, end, route, gateBounds))
                {
                    return route;
                }
            }

            // If all routes have collisions, force a safe route around all gates
            return ForceCollisionFreeRoute(start, end, gateBounds);
        }

        private List<Point> TryHorizontalFirstRoute(ConnectionPoint start, ConnectionPoint end, List<GateBound> gateBounds)
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

        private List<Point> TryVerticalFirstRoute(ConnectionPoint start, ConnectionPoint end, List<GateBound> gateBounds)
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

        private bool HasCollisions(ConnectionPoint start, ConnectionPoint end, List<Point> routePoints, List<GateBound> gateBounds)
        {
            if (routePoints.Count == 0) return false;

            // Create full path with actual start and end points
            var fullPath = new List<Point> { new Point { X = start.X, Y = start.Y } };
            fullPath.AddRange(routePoints);
            fullPath.Add(new Point { X = end.X, Y = end.Y });

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

        private List<Point> RouteSameGateConnection(ConnectionPoint start, ConnectionPoint end, List<GateBound> gateBounds)
        {
            var points = new List<Point>();
            var gateId = start.GateId;
            var gate = PlacedGates.FirstOrDefault(g => g.Id == gateId);
            if (gate == null) return points;

            // Find the gate bounds
            var gateBound = gateBounds.FirstOrDefault(b => b.Gate.Id == gateId);
            if (gateBound == null) return points;

            // Route around the gate - always go below the gate for same-gate connections
            var routeY = gateBound.Bottom + 15;

            points.Add(new Point { X = start.X, Y = routeY });
            points.Add(new Point { X = end.X, Y = routeY });

            return points;
        }

        private List<Point> TryAdvancedRoute(ConnectionPoint start, ConnectionPoint end, List<GateBound> gateBounds)
        {
            var points = new List<Point>();

            // Calculate a path that goes around all gates by finding clear corridors
            var midX = (start.X + end.X) / 2;
            var midY = (start.Y + end.Y) / 2;

            // Check if a straight path through the middle would work
            var straightPath = new List<Point> { new Point { X = midX, Y = start.Y }, new Point { X = midX, Y = end.Y } };
            if (!HasCollisions(start, end, straightPath, gateBounds))
            {
                return straightPath;
            }

            // Find a clear vertical corridor
            var clearX = FindClearVerticalCorridor(start.X, end.X, gateBounds);
            points.Add(new Point { X = clearX, Y = start.Y });
            points.Add(new Point { X = clearX, Y = end.Y });

            return points;
        }

        private double FindClearVerticalCorridor(double startX, double endX, List<GateBound> gateBounds)
        {
            var minX = Math.Min(startX, endX);
            var maxX = Math.Max(startX, endX);

            // Try positions between start and end
            for (var x = minX; x <= maxX; x += 10)
            {
                var isClear = true;
                foreach (var bounds in gateBounds)
                {
                    if (x >= bounds.Left && x <= bounds.Right)
                    {
                        isClear = false;
                        break;
                    }
                }
                if (isClear) return x;
            }

            // Try positions to the left of the leftmost point
            for (var x = minX - 20; x >= 0; x -= 10)
            {
                var isClear = true;
                foreach (var bounds in gateBounds)
                {
                    if (x >= bounds.Left && x <= bounds.Right)
                    {
                        isClear = false;
                        break;
                    }
                }
                if (isClear) return x;
            }

            // Try positions to the right of the rightmost point
            for (var x = maxX + 20; x <= 1000; x += 10)
            {
                var isClear = true;
                foreach (var bounds in gateBounds)
                {
                    if (x >= bounds.Left && x <= bounds.Right)
                    {
                        isClear = false;
                        break;
                    }
                }
                if (isClear) return x;
            }

            // Fallback - return the midpoint
            return (startX + endX) / 2;
        }

        private List<Point> ForceCollisionFreeRoute(ConnectionPoint start, ConnectionPoint end, List<GateBound> gateBounds)
        {
            var points = new List<Point>();

            // Find the bounds of all gates
            var minLeft = gateBounds.Min(b => b.Left);
            var maxRight = gateBounds.Max(b => b.Right);
            var minTop = gateBounds.Min(b => b.Top);
            var maxBottom = gateBounds.Max(b => b.Bottom);

            // Route around all gates by going to a clear area
            var safeX = maxRight + 30; // Go to the right of all gates
            var safeY = minTop - 30;   // Go above all gates

            // Choose the routing strategy based on relative positions
            if (start.X > end.X)
            {
                // Going from right to left - route above
                points.Add(new Point { X = start.X, Y = safeY });
                points.Add(new Point { X = end.X, Y = safeY });
            }
            else
            {
                // Going from left to right - route to the right
                points.Add(new Point { X = safeX, Y = start.Y });
                points.Add(new Point { X = safeX, Y = end.Y });
            }

            return points;
        }

        private bool LineIntersectsRectangle(Point p1, Point p2, GateBound rect)
        {
            // Proper line-rectangle intersection test
            var left = rect.Left;
            var right = rect.Right;
            var top = rect.Top;
            var bottom = rect.Bottom;

            // Check if either endpoint is inside the rectangle
            if (IsPointInRectangle(p1, left, right, top, bottom) ||
                IsPointInRectangle(p2, left, right, top, bottom))
            {
                return true;
            }

            // Check if line intersects any of the four rectangle edges
            return LineIntersectsLine(p1, p2, new Point { X = left, Y = top }, new Point { X = right, Y = top }) ||     // Top edge
                   LineIntersectsLine(p1, p2, new Point { X = right, Y = top }, new Point { X = right, Y = bottom }) || // Right edge
                   LineIntersectsLine(p1, p2, new Point { X = right, Y = bottom }, new Point { X = left, Y = bottom }) || // Bottom edge
                   LineIntersectsLine(p1, p2, new Point { X = left, Y = bottom }, new Point { X = left, Y = top });      // Left edge
        }

        private bool IsPointInRectangle(Point point, double left, double right, double top, double bottom)
        {
            return point.X >= left && point.X <= right && point.Y >= top && point.Y <= bottom;
        }

        private bool LineIntersectsLine(Point p1, Point p2, Point p3, Point p4)
        {
            // Line segment intersection using cross products
            var d1 = CrossProduct(p3, p4, p1);
            var d2 = CrossProduct(p3, p4, p2);
            var d3 = CrossProduct(p1, p2, p3);
            var d4 = CrossProduct(p1, p2, p4);

            if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
                ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
            {
                return true;
            }

            // Check for collinear points on segment
            if (d1 == 0 && OnSegment(p3, p1, p4)) return true;
            if (d2 == 0 && OnSegment(p3, p2, p4)) return true;
            if (d3 == 0 && OnSegment(p1, p3, p2)) return true;
            if (d4 == 0 && OnSegment(p1, p4, p2)) return true;

            return false;
        }

        private double CrossProduct(Point a, Point b, Point c)
        {
            return (c.Y - a.Y) * (b.X - a.X) - (c.X - a.X) * (b.Y - a.Y);
        }

        private bool OnSegment(Point p, Point q, Point r)
        {
            return q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                   q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y);
        }
    }

    public void CancelDrag()
    {
<<<<<<< HEAD
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

    public class GateBound
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }
        public PlacedGate Gate { get; set; } = null!;
=======
        SelectedGate = null;
        IsDragging = false;
>>>>>>> main
    }
}
