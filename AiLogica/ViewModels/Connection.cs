namespace AiLogica.ViewModels;

/// <summary>
/// Represents a Connection Point on a gate (input or output).
/// </summary>
public class Connection
{
    public Guid Id { get; set; }
    public Guid GateId { get; set; }
    public ConnectionType Type { get; set; }
    public int Index { get; set; } // 0-based index for multiple inputs/outputs
    public double X { get; set; } // Absolute X coordinate of Connection Point
    public double Y { get; set; } // Absolute Y coordinate of Connection Point
}

public enum ConnectionType
{
    Input,
    Output
}
