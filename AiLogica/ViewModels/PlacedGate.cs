namespace AiLogica.ViewModels;

public class PlacedGate
{
    public string Type { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public Guid Id { get; set; }
    public List<Connection> Connections { get; set; } = new();

    /// <summary>
    /// Updates the absolute positions of all connection points based on gate position.
    /// </summary>
    public void UpdateConnectionPositions()
    {
        foreach (var connection in Connections)
        {
            var (relativeX, relativeY) = GetRelativeConnectionPosition(connection.Type, connection.Index);
            connection.X = X + relativeX;
            connection.Y = Y + relativeY;
        }
    }

    /// <summary>
    /// Gets connection positions for OR gates (96x72 pixels).
    /// </summary>
    private static (double X, double Y) GetOrGateConnectionPosition(ConnectionType type, int index)
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
    /// Gets the relative position of a connection point within the gate.
    /// </summary>
    private (double X, double Y) GetRelativeConnectionPosition(ConnectionType type, int index)
    {
        return Type switch
        {
            "OR" => GetOrGateConnectionPosition(type, index),
            _ => (0, 0) // Default for unsupported gate types
        };
    }
}
