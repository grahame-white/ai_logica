namespace AiLogica.ViewModels;

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

        }

        return _connectionPoints;
    }
}
