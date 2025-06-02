namespace AiLogica.ViewModels;

public class PlacedGate
{
    public string Type { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public Guid Id { get; set; }
    public List<Connection> Connections { get; set; } = new();

    /// <summary>
    /// Gets or sets value property for constant gates (0 or 1). Null for non-constant gates.
    /// </summary>
    public int? Value { get; set; }

    /// <summary>
    /// Updates the absolute positions of all connection points based on gate position.
    /// </summary>
    public void UpdateConnectionPositions()
    {
        foreach (var connection in Connections)
        {
            var (relativeX, relativeY) = GetGateSpecificConnectionPosition(connection.Type, connection.Index);
            connection.X = X + relativeX;
            connection.Y = Y + relativeY;
        }
    }

    /// <summary>
    /// Defines the fixed connection coordinates for OR gate template (96x72 pixels).
    /// These are the standard positions within the OR gate SVG shape.
    /// </summary>
    private static (double X, double Y) GetOrGateTemplateCoordinates(ConnectionType type, int index)
    {
        return type switch
        {
            ConnectionType.Input => index switch
            {
                0 => (4, 28), // Top input (matching SVG coordinates)
                1 => (4, 44), // Bottom input
                _ => (4, 36) // Default to middle if index out of range
            },
            ConnectionType.Output => (88, 36), // Output (matching SVG coordinates)
            _ => (0, 0)
        };
    }

    /// <summary>
    /// Defines the fixed connection coordinates for constant gate template (32x16 pixels).
    /// Constants have only one output connection at the end of the connector line.
    /// 16x16 grid square + 16px connector line for proper grid alignment.
    /// Connection point positioned at right end of connector line.
    /// SVG connector line ends at x=32, connection point at x=32 to align exactly.
    /// </summary>
#pragma warning disable S1172 // Remove this unused method parameter 'index'
    private static (double X, double Y) GetConstantGateTemplateCoordinates(ConnectionType type, int index)
#pragma warning restore S1172
    {
        return type switch
        {
            ConnectionType.Output => (32, 8), // Output coordinates aligned with SVG line end
            _ => (0, 0) // Constants have no inputs
        };
    }

    /// <summary>
    /// Resolves connection position for the specific gate type by delegating to the appropriate template method.
    /// </summary>
    private (double X, double Y) GetGateSpecificConnectionPosition(ConnectionType type, int index)
    {
        return Type switch
        {
            "OR" => GetOrGateTemplateCoordinates(type, index),
            "CONSTANT0" => GetConstantGateTemplateCoordinates(type, index),
            "CONSTANT1" => GetConstantGateTemplateCoordinates(type, index),
            _ => (0, 0) // Default for unsupported gate types
        };
    }
}
