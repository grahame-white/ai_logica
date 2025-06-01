namespace AiLogica.ViewModels;

/// <summary>
/// Represents a connection point on a gate (input or output).
/// </summary>
public class Connection
{
    public Guid Id { get; set; }
    public Guid GateId { get; set; }
    public ConnectionType Type { get; set; }
    public int Index { get; set; } // 0-based index for multiple inputs/outputs
    public double X { get; set; } // Absolute X coordinate of connection point
    public double Y { get; set; } // Absolute Y coordinate of connection point
}

public enum ConnectionType
{
    Input,
    Output
}
