namespace AiLogica.ViewModels;

/// <summary>
/// Requirements Traceability: Wire representation for logic gate connections.
/// FR-3: Core wire model for connecting gate inputs and outputs.
/// FR-3.8: Supports orthogonal wire routing via segments.
/// FR-3.10: IsConnected property supports visual distinction.
/// See TRACEABILITY_MATRIX.md for complete mapping.
/// </summary>
/// <summary>
/// Represents a wire connecting two connection points.
/// </summary>
public class Wire
{
    public Guid Id { get; set; }
    public Guid FromConnectionId { get; set; }
    public Guid ToConnectionId { get; set; }
    public List<WireSegment> Segments { get; set; } = new(); // FR-3.8: Orthogonal segments
    public bool IsConnected { get; set; } = true; // FR-3.10: False for disconnected/dangling wires
}

/// <summary>
/// FR-3.8: Represents a straight line segment of a wire for orthogonal routing.
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
