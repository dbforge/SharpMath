using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class Line3DTests
{
    [Fact]
    public void Factories_StoreCanonicalForm()
    {
        var line = Line3D.FromPoints(Point3D.Origin, new Point3D(1, 1, 1));
        Assert.Equal(Point3D.Origin, line.Point);
        Assert.True(line.Direction.ApproximatelyEquals(Vector3.One.Normalize()));

        var reversed = Line3D.FromPoints(new Point3D(2, 2, 2), new Point3D(-1, -1, -1));
        Assert.True(line.ApproximatelyEquals(reversed));

        var offset = Line3D.FromPointAndDirection(new Point3D(5, 1, 0), new Vector3(-3, 0, 0));
        Assert.Equal(new Point3D(0, 1, 0), offset.Point);
        Assert.Equal(Vector3.UnitX, offset.Direction);

        Assert.Throws<ArgumentException>(() => Line3D.FromPoints(Point3D.Origin, Point3D.Origin));
        Assert.Throws<ArgumentException>(() => Line3D.FromPointAndDirection(Point3D.Origin, Vector3.Zero));
    }

    [Fact]
    public void Contains_WorksForAxisAlignedDirections()
    {
        var line = Line3D.FromPointAndDirection(new Point3D(2, 3, 1), new Vector3(2, 1, 2));
        Assert.True(line.Contains(new Point3D(6, 5, 5)));
        Assert.False(line.Contains(new Point3D(2, 3.5, 2)));

        // Regression: zero direction components used to break the check.
        Assert.True(Line3D.FromPointAndDirection(Point3D.Origin, Vector3.UnitX).Contains(new Point3D(5, 0, 0)));
        Assert.True(Line3D.FromPointAndDirection(Point3D.Origin, new Vector3(0, 1, 1)).Contains(new Point3D(0, 2, 2)));
    }

    [Fact]
    public void DistancesAndClosestPoints()
    {
        var xAxis = Line3D.FromPointAndDirection(Point3D.Origin, Vector3.UnitX);
        Assert.Equal(5, xAxis.DistanceTo(new Point3D(7, 3, 4)), 12);
        Assert.Equal(new Point3D(7, 0, 0), xAxis.ClosestPoint(new Point3D(7, 3, 4)));
        Assert.Equal(new Point3D(2, 0, 0), xAxis.PointAt(2));

        var skew = Line3D.FromPointAndDirection(new Point3D(0, 0, 3), Vector3.UnitY);
        Assert.Equal(3, xAxis.DistanceTo(skew), 12);
        var (onX, onSkew) = xAxis.ClosestPoints(skew);
        Assert.True(onX.ApproximatelyEquals(Point3D.Origin));
        Assert.True(onSkew.ApproximatelyEquals(new Point3D(0, 0, 3)));

        var parallel = Line3D.FromPointAndDirection(new Point3D(0, 2, 0), Vector3.UnitX);
        Assert.Equal(2, xAxis.DistanceTo(parallel), 12);
        Assert.True(xAxis.IsParallelTo(parallel));
        Assert.False(xAxis.IsParallelTo(skew));
    }

    [Fact]
    public void Intersect_ReportsAllKinds()
    {
        var xAxis = Line3D.FromPointAndDirection(Point3D.Origin, Vector3.UnitX);
        var crossing = Line3D.FromPoints(new Point3D(1, -1, 0), new Point3D(1, 1, 0));
        var intersection = xAxis.Intersect(crossing);
        Assert.Equal(IntersectionKind.Point, intersection.Kind);
        Assert.True(intersection.Point!.Value.ApproximatelyEquals(new Point3D(1, 0, 0)));

        Assert.Equal(IntersectionKind.Skew,
            xAxis.Intersect(Line3D.FromPointAndDirection(new Point3D(0, 0, 3), Vector3.UnitY)).Kind);
        Assert.Equal(IntersectionKind.None,
            xAxis.Intersect(Line3D.FromPointAndDirection(new Point3D(0, 2, 0), Vector3.UnitX)).Kind);
        Assert.Equal(IntersectionKind.Coincident,
            xAxis.Intersect(Line3D.FromPoints(new Point3D(3, 0, 0), new Point3D(-1, 0, 0))).Kind);
    }

    [Fact]
    public void EqualityAndStrings()
    {
        var line = Line3D.FromPointAndDirection(new Point3D(4, 1, 0), Vector3.UnitX);
        Assert.True(line == Line3D.FromPointAndDirection(new Point3D(-2, 1, 0), -Vector3.UnitX));
        Assert.True(line != Line3D.FromPointAndDirection(new Point3D(0, 2, 0), Vector3.UnitX));
        Assert.True(line.Equals((object) Line3D.FromPointAndDirection(new Point3D(9, 1, 0), Vector3.UnitX)));
        Assert.Equal(line.GetHashCode(), Line3D.FromPointAndDirection(new Point3D(1, 1, 0), Vector3.UnitX).GetHashCode());
        Assert.Equal("Line3D [(0, 1, 0) + t·(1, 0, 0)]", line.ToString());
    }
}
