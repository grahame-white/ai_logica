using AiLogica.ViewModels;

namespace AiLogica.Tests.ViewModels;

/// <summary>
/// Tests for CoordinateHelper utility class to validate left/right coordinate calculations.
/// </summary>
public class CoordinateHelperTests
{
    [Theory]
    [InlineData(10, 20, true)] // 10 is to the left of 20
    [InlineData(100, 50, false)] // 100 is not to the left of 50
    [InlineData(0, 1, true)] // 0 is to the left of 1
    [InlineData(5, 5, false)] // 5 is not to the left of 5 (equal)
    public void IsToTheLeftOf_ShouldReturnCorrectResult(double positionA, double positionB, bool expected)
    {
        // Act
        var result = CoordinateHelper.IsToTheLeftOf(positionA, positionB);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(20, 10, true)] // 20 is to the right of 10
    [InlineData(50, 100, false)] // 50 is not to the right of 100
    [InlineData(1, 0, true)] // 1 is to the right of 0
    [InlineData(5, 5, false)] // 5 is not to the right of 5 (equal)
    public void IsToTheRightOf_ShouldReturnCorrectResult(double positionA, double positionB, bool expected)
    {
        // Act
        var result = CoordinateHelper.IsToTheRightOf(positionA, positionB);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(100, 10, 110)] // Move right by 10: 100 + 10 = 110
    [InlineData(50, 25, 75)] // Move right by 25: 50 + 25 = 75
    [InlineData(0, 5, 5)] // Move right by 5: 0 + 5 = 5
    [InlineData(100, -10, 110)] // Move right by negative value should still move right
    public void MoveToTheRight_ShouldIncreaseXCoordinate(double currentX, double distance, double expected)
    {
        // Act
        var result = CoordinateHelper.MoveToTheRight(currentX, distance);

        // Assert
        Assert.Equal(expected, result, 0.001);
        Assert.True(result > currentX, $"Moving right should increase X from {currentX} to {result}");
    }

    [Theory]
    [InlineData(100, 10, 90)] // Move left by 10: 100 - 10 = 90
    [InlineData(50, 25, 25)] // Move left by 25: 50 - 25 = 25
    [InlineData(10, 5, 5)] // Move left by 5: 10 - 5 = 5
    [InlineData(100, -10, 90)] // Move left by negative value should still move left
    public void MoveToTheLeft_ShouldDecreaseXCoordinate(double currentX, double distance, double expected)
    {
        // Act
        var result = CoordinateHelper.MoveToTheLeft(currentX, distance);

        // Assert
        Assert.Equal(expected, result, 0.001);
        Assert.True(result < currentX, $"Moving left should decrease X from {currentX} to {result}");
    }

    [Fact]
    public void GetLeftEdge_ShouldReturnObjectX()
    {
        // Arrange
        double objectX = 100;

        // Act
        var leftEdge = CoordinateHelper.GetLeftEdge(objectX);

        // Assert
        Assert.Equal(objectX, leftEdge);
    }

    [Theory]
    [InlineData(100, 50, 150)] // Object at X=100, width=50, right edge = 150
    [InlineData(0, 96, 96)] // Object at X=0, width=96, right edge = 96
    [InlineData(200, 10, 210)] // Object at X=200, width=10, right edge = 210
    public void GetRightEdge_ShouldReturnObjectXPlusWidth(double objectX, double width, double expected)
    {
        // Act
        var rightEdge = CoordinateHelper.GetRightEdge(objectX, width);

        // Assert
        Assert.Equal(expected, rightEdge, 0.001);
        Assert.True(rightEdge > objectX, $"Right edge ({rightEdge}) should be greater than left edge ({objectX})");
    }

    [Theory]
    [InlineData(100, 50, 10, 160)] // Object at X=100, width=50, clearance=10, result = 100+50+10 = 160
    [InlineData(0, 96, 15, 111)] // Object at X=0, width=96, clearance=15, result = 0+96+15 = 111
    [InlineData(50, 20, 0, 70)] // Object at X=50, width=20, no clearance, result = 50+20 = 70
    public void GetPositionToTheRightOf_ShouldReturnPositionRightOfObject(double objectX, double width, double clearance, double expected)
    {
        // Act
        var position = CoordinateHelper.GetPositionToTheRightOf(objectX, width, clearance);

        // Assert
        Assert.Equal(expected, position, 0.001);
        Assert.True(position >= objectX + width, $"Position to the right ({position}) should be greater than or equal to object's right edge ({objectX + width})");
    }

    [Theory]
    [InlineData(100, 10, 90)] // Object at X=100, clearance=10, result = 100-10 = 90
    [InlineData(50, 15, 35)] // Object at X=50, clearance=15, result = 50-15 = 35
    [InlineData(20, 0, 20)] // Object at X=20, no clearance, result = 20
    public void GetPositionToTheLeftOf_ShouldReturnPositionLeftOfObject(double objectX, double clearance, double expected)
    {
        // Act
        var position = CoordinateHelper.GetPositionToTheLeftOf(objectX, clearance);

        // Assert
        Assert.Equal(expected, position, 0.001);
        Assert.True(position <= objectX, $"Position to the left ({position}) should be less than or equal to object's left edge ({objectX})");
    }

    [Theory]
    [InlineData(new double[] { 10, 20, 5, 30 }, 5)] // Leftmost = 5
    [InlineData(new double[] { 100, 50, 200 }, 50)] // Leftmost = 50
    [InlineData(new double[] { 0, -10, 15 }, -10)] // Leftmost = -10
    [InlineData(new double[] { 42 }, 42)] // Single value = 42
    public void GetLeftmostPosition_ShouldReturnSmallestXValue(double[] xCoordinates, double expected)
    {
        // Act
        var leftmost = CoordinateHelper.GetLeftmostPosition(xCoordinates);

        // Assert
        Assert.Equal(expected, leftmost, 0.001);
    }

    [Theory]
    [InlineData(new double[] { 10, 20, 5, 30 }, 30)] // Rightmost = 30
    [InlineData(new double[] { 100, 50, 200 }, 200)] // Rightmost = 200
    [InlineData(new double[] { 0, -10, 15 }, 15)] // Rightmost = 15
    [InlineData(new double[] { 42 }, 42)] // Single value = 42
    public void GetRightmostPosition_ShouldReturnLargestXValue(double[] xCoordinates, double expected)
    {
        // Act
        var rightmost = CoordinateHelper.GetRightmostPosition(xCoordinates);

        // Assert
        Assert.Equal(expected, rightmost, 0.001);
    }

    [Fact]
    public void GetLeftmostPosition_EmptyArray_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CoordinateHelper.GetLeftmostPosition());
        Assert.Throws<ArgumentException>(() => CoordinateHelper.GetLeftmostPosition(new double[0]));
    }

    [Fact]
    public void GetRightmostPosition_EmptyArray_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CoordinateHelper.GetRightmostPosition());
        Assert.Throws<ArgumentException>(() => CoordinateHelper.GetRightmostPosition(new double[0]));
    }

    [Fact]
    public void CoordinateHelper_LeftRightConsistency_ShouldMaintainCorrectRelationships()
    {
        // Arrange
        double objectX = 100;
        double objectWidth = 50;
        double clearance = 10;

        // Act
        var leftEdge = CoordinateHelper.GetLeftEdge(objectX);
        var rightEdge = CoordinateHelper.GetRightEdge(objectX, objectWidth);
        var positionToLeft = CoordinateHelper.GetPositionToTheLeftOf(objectX, clearance);
        var positionToRight = CoordinateHelper.GetPositionToTheRightOf(objectX, objectWidth, clearance);

        // Assert - Verify the fundamental coordinate relationships
        Assert.True(leftEdge < rightEdge, "Left edge should have lower X than right edge");
        Assert.True(positionToLeft < leftEdge, "Position to the left should have lower X than left edge");
        Assert.True(positionToRight > rightEdge, "Position to the right should have higher X than right edge");

        // Verify relative positioning
        Assert.True(CoordinateHelper.IsToTheLeftOf(positionToLeft, leftEdge), "Position to left should be identified as left of object");
        Assert.True(CoordinateHelper.IsToTheRightOf(positionToRight, rightEdge), "Position to right should be identified as right of object");
    }
}
