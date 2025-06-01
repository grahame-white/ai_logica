# Coordinate System Documentation

## Overview
This document clarifies the coordinate system used in AI Logica to avoid confusion about left/right positioning, particularly in response to issue #51: "Copilot is easily confused by the concept of left and right".

## Coordinate System Rules

### X-Axis (Horizontal)
- **LEFT** means **LOWER** X coordinates
- **RIGHT** means **HIGHER** X coordinates
- Moving **to the right** **INCREASES** the X coordinate
- Moving **to the left** **DECREASES** the X coordinate

### Positioning Relationships
- Element A is "to the right of" element B when `A.X > B.X`
- Element A is "to the left of" element B when `A.X < B.X`
- The "right edge" of an object has a **HIGHER** X coordinate than the "left edge"
- The "left edge" of an object has a **LOWER** X coordinate than the "right edge"

## Code Implementation

### CoordinateHelper Utility Class
The `CoordinateHelper` class provides explicit methods to avoid confusion:

```csharp
// Check positioning relationships
bool isLeft = CoordinateHelper.IsToTheLeftOf(positionA, positionB);    // A.X < B.X
bool isRight = CoordinateHelper.IsToTheRightOf(positionA, positionB);  // A.X > B.X

// Move objects
double rightPosition = CoordinateHelper.MoveToTheRight(currentX, distance);  // currentX + distance
double leftPosition = CoordinateHelper.MoveToTheLeft(currentX, distance);    // currentX - distance

// Calculate edge positions
double leftEdge = CoordinateHelper.GetLeftEdge(objectX);                     // objectX
double rightEdge = CoordinateHelper.GetRightEdge(objectX, width);           // objectX + width

// Position objects relative to others
double positionToRight = CoordinateHelper.GetPositionToTheRightOf(objectX, width, clearance);
double positionToLeft = CoordinateHelper.GetPositionToTheLeftOf(objectX, clearance);
```

### Wire Routing Example
In the `FindSafeXPosition` method:

```csharp
// LEFT route = position to the LEFT of gate (LOWER X coordinate)
double leftRoute = CoordinateHelper.GetPositionToTheLeftOf(gate.X, 15);

// RIGHT route = position to the RIGHT of gate (HIGHER X coordinate)  
double rightRoute = CoordinateHelper.GetPositionToTheRightOf(gate.X, 96, 15);

if (Math.Abs(leftRoute - midX) <= Math.Abs(rightRoute - midX))
{
    midX = leftRoute;
    _logger.LogTrace("Chose LEFT route (lower X): {MidX}", midX);
}
else
{
    midX = rightRoute;
    _logger.LogTrace("Chose RIGHT route (higher X): {MidX}", midX);
}
```

## Visual Examples

```
  Left Side    |    Right Side
  (Lower X)    |    (Higher X)
               |
    0    50   100   150   200
    |     |    |     |     |
   Left Edge  Center   Right Edge
```

For a gate at X=100 with width=96:
- Left edge: X = 100 (lower X)
- Right edge: X = 196 (higher X) 
- Position to the left: X < 100
- Position to the right: X > 196

## Testing
The coordinate system is validated by:
- `CoordinateSystemTests` - Validates gate positioning and movement
- `CoordinateHelperTests` - Validates utility method calculations
- Wire routing tests confirm left/right collision avoidance

## Common Mistakes to Avoid
1. ❌ Thinking "left" means higher X values
2. ❌ Thinking "right" means lower X values  
3. ❌ Confusing screen coordinates with mathematical coordinates
4. ✅ Remember: LEFT = LOWER X, RIGHT = HIGHER X

## Related Code Files
- `AiLogica/ViewModels/CoordinateHelper.cs` - Utility methods
- `AiLogica/ViewModels/HomeViewModel.cs` - Wire routing implementation
- `AiLogica.Tests/ViewModels/CoordinateSystemTests.cs` - System validation
- `AiLogica.Tests/ViewModels/CoordinateHelperTests.cs` - Utility validation