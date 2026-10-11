using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class LineSegment2DTests
{
    private static readonly LineSegment2D Diagonal = new(new Point2D(0, 0), new Point2D(4, 4));

    [Fact]
    public void BasicProperties()
    {
        Assert.Equal(new Point2D(0, 0), Diagonal.Start);
        Assert.Equal(new Point2D(4, 4), Diagonal.End);
        Assert.Equal(new Vector2(4, 4), Diagonal.Direction);
        Assert.Equal(Math.Sqrt(32), Diagonal.Length, 12);
        Assert.Equal(new Point2D(2, 2), Diagonal.Midpoint);
        Assert.True(Diagonal.ToLine().ApproximatelyEquals(Line2D.FromSlopeIntercept(1, 0)));
        Assert.Throws<InvalidOperationException>(() => new LineSegment2D(Point2D.Origin, Point2D.Origin).ToLine());
    }

    [Fact]
    public void ContainsDistanceAndClosestPoint()
    {
        Assert.True(Diagonal.Contains(new Point2D(1, 1)));
        Assert.True(Diagonal.Contains(new Point2D(4, 4)));
        Assert.False(Diagonal.Contains(new Point2D(5, 5)));
        Assert.False(Diagonal.Contains(new Point2D(1, 2)));
        Assert.Equal(Math.Sqrt(2), Diagonal.DistanceTo(new Point2D(5, 5)), 12);
        Assert.Equal(new Point2D(0, 0), Diagonal.ClosestPoint(new Point2D(-3, -1)));
        Assert.True(Diagonal.ClosestPoint(new Point2D(0, 2)).ApproximatelyEquals(new Point2D(1, 1)));
        var point = new LineSegment2D(new Point2D(1, 1), new Point2D(1, 1));
        Assert.Equal(new Point2D(1, 1), point.ClosestPoint(new Point2D(5, 5)));
    }

    [Fact]
    public void Intersect_Segments()
    {
        var crossing = Diagonal.Intersect(new LineSegment2D(new Point2D(0, 4), new Point2D(4, 0)));
        Assert.Equal(IntersectionKind.Point, crossing.Kind);
        Assert.True(crossing.Point!.Value.ApproximatelyEquals(new Point2D(2, 2)));
        Assert.Null(crossing.Segment);

        var touching = Diagonal.Intersect(new LineSegment2D(new Point2D(4, 4), new Point2D(6, 0)));
        Assert.True(touching.Point!.Value.ApproximatelyEquals(new Point2D(4, 4)));

        Assert.Equal(IntersectionKind.None,
            Diagonal.Intersect(new LineSegment2D(new Point2D(0, 4), new Point2D(1, 3))).Kind);
        Assert.Equal(IntersectionKind.None,
            Diagonal.Intersect(new LineSegment2D(new Point2D(0, 1), new Point2D(4, 5))).Kind);
        Assert.Equal(IntersectionKind.None,
            Diagonal.Intersect(new LineSegment2D(new Point2D(5, 5), new Point2D(6, 6))).Kind);
    }

    [Fact]
    public void Intersect_CollinearSegments()
    {
        var overlap = Diagonal.Intersect(new LineSegment2D(new Point2D(6, 6), new Point2D(2, 2)));
        Assert.Equal(IntersectionKind.Overlap, overlap.Kind);
        Assert.True(overlap.Segment!.Value.ApproximatelyEquals(new LineSegment2D(new Point2D(2, 2), new Point2D(4, 4))));

        var endToEnd = Diagonal.Intersect(new LineSegment2D(new Point2D(4, 4), new Point2D(6, 6)));
        Assert.Equal(IntersectionKind.Point, endToEnd.Kind);
        Assert.True(endToEnd.Point!.Value.ApproximatelyEquals(new Point2D(4, 4)));
    }

    [Fact]
    public void Intersect_DegenerateSegments()
    {
        var point = new LineSegment2D(new Point2D(1, 1), new Point2D(1, 1));
        Assert.Equal(IntersectionKind.Point, point.Intersect(Diagonal).Kind);
        Assert.Equal(IntersectionKind.Point, Diagonal.Intersect(point).Kind);
        Assert.Equal(IntersectionKind.None, new LineSegment2D(new Point2D(1, 2), new Point2D(1, 2)).Intersect(Diagonal).Kind);
        Assert.Equal(IntersectionKind.None, Diagonal.Intersect(new LineSegment2D(new Point2D(1, 2), new Point2D(1, 2))).Kind);
    }

    [Fact]
    public void Intersect_Lines()
    {
        var crossing = Diagonal.Intersect(Line2D.FromGeneralForm(1, 0, -3));
        Assert.True(crossing.Point!.Value.ApproximatelyEquals(new Point2D(3, 3)));
        Assert.Equal(IntersectionKind.None, Diagonal.Intersect(Line2D.FromGeneralForm(1, 0, -5)).Kind);
        Assert.Equal(IntersectionKind.None, Diagonal.Intersect(Line2D.FromSlopeIntercept(1, 1)).Kind);
        var coincident = Diagonal.Intersect(Line2D.FromSlopeIntercept(1, 0));
        Assert.Equal(IntersectionKind.Overlap, coincident.Kind);
        Assert.Equal(Diagonal, coincident.Segment);
        var point = new LineSegment2D(new Point2D(1, 1), new Point2D(1, 1));
        Assert.Equal(IntersectionKind.Point, point.Intersect(Line2D.FromSlopeIntercept(1, 0)).Kind);
        Assert.Equal(IntersectionKind.None, point.Intersect(Line2D.FromSlopeIntercept(1, 1)).Kind);
    }

    [Fact]
    public void EqualityAndStrings()
    {
        Assert.True(Diagonal == new LineSegment2D(new Point2D(0, 0), new Point2D(4, 4)));
        Assert.True(Diagonal != new LineSegment2D(new Point2D(4, 4), new Point2D(0, 0)));
        Assert.True(Diagonal.Equals((object) new LineSegment2D(new Point2D(0, 0), new Point2D(4, 4))));
        Assert.Equal(Diagonal.GetHashCode(), new LineSegment2D(new Point2D(0, 0), new Point2D(4, 4)).GetHashCode());
        Assert.Equal("LineSegment2D [(0, 0) → (4, 4)]", Diagonal.ToString());
    }
}
