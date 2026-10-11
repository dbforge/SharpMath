using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class Line2DTests
{
    [Fact]
    public void FromPoints_ProvidesSlopeAndIntercept()
    {
        var line = Line2D.FromPoints(new Point2D(0, 2), new Point2D(3, 1));
        Assert.Equal(-1d / 3d, line.Slope, 12);
        Assert.Equal(2, line.YIntercept, 12);

        var second = Line2D.FromPoints(new Point2D(4, 5), new Point2D(1, 3));
        Assert.Equal(2d / 3d, second.Slope, 12);
        Assert.Equal(7d / 3d, second.YIntercept, 12);
    }

    [Fact]
    public void FromPoints_RejectsIdenticalPoints() =>
        Assert.Throws<ArgumentException>(() => Line2D.FromPoints(new Point2D(1, 1), new Point2D(1, 1)));

    [Fact]
    public void VerticalLines_AreSupported()
    {
        // Regression: vertical lines used to have an infinite slope and contain none of their points.
        var line = Line2D.FromPoints(new Point2D(1, 0), new Point2D(1, 5));
        Assert.True(line.IsVertical);
        Assert.False(line.IsHorizontal);
        Assert.True(double.IsNaN(line.Slope));
        Assert.True(double.IsNaN(line.YIntercept));
        Assert.True(double.IsNaN(line.SolveForY(1)));
        Assert.Equal(1, line.SolveForX(42), 12);
        Assert.True(line.Contains(new Point2D(1, 0)));
        Assert.True(line.Contains(new Point2D(1, 5)));
        Assert.False(line.Contains(new Point2D(2, 5)));
    }

    [Fact]
    public void GeneralForm_IsNormalized()
    {
        // Issue #9: 4/3·x − y + 1/3 = 0
        var line = Line2D.FromGeneralForm(1.3333333333333333, -1, 0.3333333333333335);
        Assert.Equal(1, line.A * line.A + line.B * line.B, 12);
        Assert.True(line.A > 0);
        Assert.Equal((-0.3333333333333335 - 1.3333333333333333 * 2) / -1, line.SolveForY(2), 12);
        Assert.True(line.ApproximatelyEquals(Line2D.FromGeneralForm(-2.6666666666666666, 2, -0.666666666666667)));
        Assert.True(Line2D.FromGeneralForm(0, -2, 4).ApproximatelyEquals(Line2D.FromGeneralForm(0, 1, -2)));
        Assert.True(Line2D.FromGeneralForm(0, 1, -2).IsHorizontal);
        Assert.True(double.IsNaN(Line2D.FromGeneralForm(0, 1, -2).SolveForX(1)));
        Assert.Throws<ArgumentException>(() => Line2D.FromGeneralForm(0, 0, 1));
        Assert.Throws<ArgumentException>(() => Line2D.FromGeneralForm(double.NaN, 1, 1));
    }

    [Fact]
    public void FactoriesProduceTheSameLine()
    {
        var fromSlope = Line2D.FromSlopeIntercept(2, 3);
        var fromPoints = Line2D.FromPoints(new Point2D(0, 3), new Point2D(1, 5));
        var fromDirection = Line2D.FromPointAndDirection(new Point2D(-1, 1), new Vector2(-2, -4));
        Assert.True(fromSlope.ApproximatelyEquals(fromPoints));
        Assert.True(fromSlope.ApproximatelyEquals(fromDirection));
        Assert.Throws<ArgumentException>(() => Line2D.FromPointAndDirection(Point2D.Origin, Vector2.Zero));
    }

    [Fact]
    public void Contains_DistanceAndProjection()
    {
        var line = Line2D.FromSlopeIntercept(1, 0);
        Assert.True(line.Contains(new Point2D(1, 1)));
        Assert.False(line.Contains(new Point2D(2, 1)));
        Assert.True(Line2D.FromSlopeIntercept(2, 3).Contains(new Point2D(4, 11)));
        Assert.Equal(Math.Sqrt(2), line.DistanceTo(new Point2D(2, 0)), 12);
        Assert.Equal(-Math.Sqrt(2), line.SignedDistanceTo(new Point2D(0, 2)), 12);
        Assert.True(line.ClosestPoint(new Point2D(2, 0)).ApproximatelyEquals(new Point2D(1, 1)));
        Assert.Equal(new Point2D(1, 4), new Point2D(1, Line2D.FromSlopeIntercept(3, 1).SolveForY(1)));
        Assert.True(line.Contains(line.PointAt(3.5)));
        Assert.True(line.Direction.IsNormalized);
        Assert.True(line.Normal.IsOrthogonalTo(line.Direction));
    }

    [Fact]
    public void Parallelism()
    {
        Assert.True(Line2D.FromSlopeIntercept(2, 0).IsParallelTo(Line2D.FromSlopeIntercept(2, 3)));
        Assert.False(Line2D.FromSlopeIntercept(2, 3).IsParallelTo(Line2D.FromSlopeIntercept(3, 1)));
    }

    [Fact]
    public void Intersect_ReportsPointNoneAndCoincident()
    {
        var intersection = Line2D.FromSlopeIntercept(0.5, -1).Intersect(Line2D.FromSlopeIntercept(-2, -6));
        Assert.Equal(IntersectionKind.Point, intersection.Kind);
        Assert.True(intersection.Point!.Value.ApproximatelyEquals(new Point2D(-2, -2)));

        var second = Line2D.FromSlopeIntercept(2, 0).Intersect(Line2D.FromSlopeIntercept(3, 1));
        Assert.True(second.Point!.Value.ApproximatelyEquals(new Point2D(-1, -2)));

        var parallel = Line2D.FromSlopeIntercept(2, 0).Intersect(Line2D.FromSlopeIntercept(2, 3));
        Assert.Equal(new LineIntersection2D(IntersectionKind.None, null), parallel);

        // Regression: coincident lines used to report "no intersection" with a NaN point.
        var line = Line2D.FromSlopeIntercept(2, 1);
        Assert.Equal(IntersectionKind.Coincident, line.Intersect(Line2D.FromPoints(new Point2D(0, 1), new Point2D(1, 3))).Kind);

        var vertical = Line2D.FromGeneralForm(1, 0, -2).Intersect(Line2D.FromSlopeIntercept(1, 0));
        Assert.True(vertical.Point!.Value.ApproximatelyEquals(new Point2D(2, 2)));
    }

    [Fact]
    public void EqualityAndStrings()
    {
        var line = Line2D.FromGeneralForm(3, 4, 5);
        Assert.Equal(0.6, line.A, 12);
        Assert.Equal(0.8, line.B, 12);
        Assert.Equal(1, line.C, 12);
        Assert.True(line == Line2D.FromGeneralForm(3, 4, 5));
        Assert.True(line != Line2D.FromGeneralForm(3, 4, 6));
        Assert.True(line.Equals((object) Line2D.FromGeneralForm(6, 8, 10)));
        Assert.False(line.Equals((object) 3));
        Assert.Equal(line.GetHashCode(), Line2D.FromGeneralForm(3, 4, 5).GetHashCode());
        Assert.Equal("Line2D [0.6·x + 0.8·y + 1 = 0]", line.ToString());
    }
}
