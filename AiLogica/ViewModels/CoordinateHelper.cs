namespace AiLogica.ViewModels;

/// <summary>
/// Utility class that provides explicit left/right coordinate calculations to avoid confusion.
/// Addresses the bug: "Copilot is easily confused by the concept of left and right"
/// 
/// Coordinate System Rules:
/// - LEFT means LOWER X coordinates (moving left decreases X)
/// - RIGHT means HIGHER X coordinates (moving right increases X)
/// - Element A is "to the right of" element B when A.X is greater than B.X.
/// - Element A is "to the left of" element B when A.X is less than B.X.
/// </summary>
public static class CoordinateHelper
{
    /// <summary>
    /// Returns true if positionA is to the left of positionB.
    /// Left means LOWER X coordinate.
    /// </summary>
    public static bool IsToTheLeftOf(double positionAX, double positionBX)
    {
        return positionAX < positionBX;
    }

    /// <summary>
    /// Returns true if positionA is to the right of positionB.
    /// Right means HIGHER X coordinate.
    /// </summary>
    public static bool IsToTheRightOf(double positionAX, double positionBX)
    {
        return positionAX > positionBX;
    }

    /// <summary>
    /// Moves a position to the right by increasing the X coordinate.
    /// Right means INCREASING X coordinate.
    /// </summary>
    public static double MoveToTheRight(double currentX, double distance)
    {
        return currentX + Math.Abs(distance);
    }

    /// <summary>
    /// Moves a position to the left by decreasing the X coordinate.
    /// Left means DECREASING X coordinate.
    /// </summary>
    public static double MoveToTheLeft(double currentX, double distance)
    {
        return currentX - Math.Abs(distance);
    }

    /// <summary>
    /// Gets the left edge X coordinate of a rectangular object.
    /// Left edge has the LOWEST X coordinate.
    /// </summary>
    public static double GetLeftEdge(double objectX, double objectWidth = 0)
    {
        return objectX;
    }

    /// <summary>
    /// Gets the right edge X coordinate of a rectangular object.
    /// Right edge has the HIGHEST X coordinate.
    /// </summary>
    public static double GetRightEdge(double objectX, double objectWidth)
    {
        return objectX + objectWidth;
    }

    /// <summary>
    /// Calculates a position to the right of an object's right edge.
    /// Result will have HIGHER X coordinate than the object's right edge.
    /// </summary>
    public static double GetPositionToTheRightOf(double objectX, double objectWidth, double clearance = 0)
    {
        return GetRightEdge(objectX, objectWidth) + Math.Abs(clearance);
    }

    /// <summary>
    /// Calculates a position to the left of an object's left edge.
    /// Result will have LOWER X coordinate than the object's left edge.
    /// </summary>
    public static double GetPositionToTheLeftOf(double objectX, double clearance = 0)
    {
        return GetLeftEdge(objectX) - Math.Abs(clearance);
    }

    /// <summary>
    /// Gets the leftmost X coordinate from a collection of X coordinates.
    /// Leftmost means the SMALLEST X value.
    /// </summary>
    public static double GetLeftmostPosition(params double[] xCoordinates)
    {
        if (xCoordinates == null || xCoordinates.Length == 0)
            throw new ArgumentException("At least one coordinate must be provided", nameof(xCoordinates));

        return xCoordinates.Min();
    }

    /// <summary>
    /// Gets the rightmost X coordinate from a collection of X coordinates.
    /// Rightmost means the LARGEST X value.
    /// </summary>
    public static double GetRightmostPosition(params double[] xCoordinates)
    {
        if (xCoordinates == null || xCoordinates.Length == 0)
            throw new ArgumentException("At least one coordinate must be provided", nameof(xCoordinates));

        return xCoordinates.Max();
    }
}
