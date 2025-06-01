namespace AiLogica.ViewModels;

/// <summary>
/// Represents a wire connecting two connection points.
/// </summary>
public class Wire
{
    public Guid Id { get; set; }
    public Guid FromConnectionId { get; set; }
    public Guid ToConnectionId { get; set; }
    public List<WireSegment> Segments { get; set; } = new();
    public bool IsConnected { get; set; } = true; // False for disconnected/dangling wires
}

/// <summary>
/// Represents a straight line segment of a wire.
/// </summary>
public class WireSegment
{
    public double StartX { get; set; }
    public double StartY { get; set; }
    public double EndX { get; set; }
    public double EndY { get; set; }
    public WireOrientation Orientation { get; set; }
}

public enum WireOrientation
{
    Horizontal,
    Vertical
}
