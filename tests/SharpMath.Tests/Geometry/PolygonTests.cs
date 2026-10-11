using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class PolygonTests
{
    private static readonly Polygon Square = new(new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2));

    private static readonly Polygon Pentagon = new(new Point2D(-2, -2), new Point2D(2, -2), new Point2D(4, 1),
        new Point2D(0, 2), new Point2D(-4, 1));

    [Fact]
    public void Constructor_ValidatesAndCopies()
    {
        Assert.Throws<ArgumentException>(() => new Polygon(new Point2D(0, 1), new Point2D(3, 4)));
        Assert.Throws<ArgumentNullException>(() => new Polygon((IEnumerable<Point2D>) null!));
        var points = new List<Point2D> { new(0, 0), new(1, 0), new(0, 1) };
        var polygon = new Polygon(points);
        points.Add(new Point2D(5, 5));
        Assert.Equal(3, polygon.Points.Count);
    }

    [Fact]
    public void Area_UsesShoelaceFormula()
    {
        Assert.Equal(4, Square.Area);
        Assert.Equal(2, new Polygon(new Point2D(0, 0), new Point2D(2, 0), new Point2D(1, 2)).Area);
        Assert.Equal(22, Pentagon.Area);

        // Regression: polygons that do not contain the origin used to get wrong areas.
        Assert.Equal(1, new Polygon(new Point2D(1, 1), new Point2D(2, 1), new Point2D(2, 2), new Point2D(1, 2)).Area);
        Assert.Equal(0.5, new Polygon(new Point2D(5, 5), new Point2D(6, 5), new Point2D(5, 6)).Area);
        var concave = new Polygon(new Point2D(0, 0), new Point2D(4, 0), new Point2D(4, 4), new Point2D(2, 1),
            new Point2D(0, 4));
        Assert.Equal(10, concave.Area);

        Assert.Equal(4, Square.SignedArea);
        Assert.Equal(-4, new Polygon(Square.Points.Reverse()).SignedArea);
    }

    [Fact]
    public void Perimeter()
    {
        Assert.Equal(8, Square.Perimeter);
        Assert.Equal(4 + 2 * Math.Sqrt(13) + 2 * Math.Sqrt(17), Pentagon.Perimeter, 12);
    }

    [Fact]
    public void Centroid_IsCenterOfMass()
    {
        // Regression: the centroid of this square used to be (16, 16).
        Assert.True(Square.Centroid.ApproximatelyEquals(new Point2D(1, 1)));
        Assert.True(new Polygon(new Point2D(0, 0), new Point2D(3, 0), new Point2D(0, 3)).Centroid
            .ApproximatelyEquals(new Point2D(1, 1)));
        Assert.Equal(new Point2D(1, 1), new Polygon(new Point2D(0, 0), new Point2D(1, 1), new Point2D(2, 2)).Centroid);
    }

    [Fact]
    public void BoundingBox_DoesNotIncludeOrigin()
    {
        // Regression: the box used to always include the origin.
        var polygon = new Polygon(new Point2D(1, 1), new Point2D(3, 1), new Point2D(2, 4));
        Assert.Equal(new BoundingBox2D(1, 1, 3, 4), polygon.BoundingBox);
    }

    [Fact]
    public void Contains_Points()
    {
        Assert.True(Square.Contains(new Point2D(1, 1)));
        Assert.True(Square.Contains(new Point2D(2, 1)));
        Assert.True(Square.Contains(new Point2D(0, 0)));
        Assert.False(Square.Contains(new Point2D(3, 1)));
        Assert.False(Square.Contains(new Point2D(-0.1, 1)));
        var concave = new Polygon(new Point2D(0, 0), new Point2D(4, 0), new Point2D(4, 4), new Point2D(2, 1),
            new Point2D(0, 4));
        Assert.False(concave.Contains(new Point2D(2, 3)));
        Assert.True(concave.Contains(new Point2D(1, 1)));
    }

    [Fact]
    public void Contains_Polygons()
    {
        var inner = new Polygon(new Point2D(0.5, 0.5), new Point2D(1.5, 0.5), new Point2D(1, 1.5));
        Assert.True(Square.Contains(inner));
        Assert.True(Square.Contains(Square));
        Assert.False(inner.Contains(Square));

        var concave = new Polygon(new Point2D(0, 0), new Point2D(4, 0), new Point2D(4, 4), new Point2D(2, 1),
            new Point2D(0, 4));
        var bridge = new Polygon(new Point2D(0.5, 3), new Point2D(3.5, 3), new Point2D(3.5, 3.2), new Point2D(0.5, 3.2));
        Assert.False(concave.Contains(bridge));
        Assert.Throws<ArgumentNullException>(() => Square.Contains((Polygon) null!));
    }

    [Fact]
    public void Equality_DoesNotRecurse()
    {
        // Regression: comparing two distinct polygons with == used to overflow the stack.
        var other = new Polygon(new Point2D(0, 0), new Point2D(1, 0), new Point2D(0, 1));
        Assert.False(Square == other);
        Assert.True(Square != other);
        Assert.True(Square == new Polygon(Square.Points));
        Assert.False(Square == null);
        Assert.False(null == Square);
        Assert.True((Polygon?) null == null);
        Assert.True(Square.Equals((object) new Polygon(Square.Points)));
        Assert.False(Square.Equals((object?) null));

        // Regression: equal polygons used to have different hash codes.
        Assert.Equal(Square.GetHashCode(), new Polygon(Square.Points).GetHashCode());
        Assert.Equal("Polygon [(0, 0), (2, 0), (2, 2), (0, 2)]", Square.ToString());
    }
}
