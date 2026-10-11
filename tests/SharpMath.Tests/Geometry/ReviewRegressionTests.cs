using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

/// <summary>
///     Regression tests for numerical problems found in the review of 2.0: scale dependence, cancellation and
///     degenerate inputs.
/// </summary>
public class ReviewRegressionTests
{
    [Fact]
    public void Transformations_WithLargeTranslationsOrSmallScales_AreInvertible()
    {
        Matrix4x4[] transformations =
        [
            Matrix4x4.Translation(178, 0, 0),
            Matrix4x4.Translation(1e6, -2e6, 3e6),
            Matrix4x4.LookAt(new Vector3(1e4, 2e3, -5e3), Vector3.Zero, Vector3.Up),
            Matrix4x4.Scaling(0.001),
            Matrix4x4.Translation(500, 0, 0) * Matrix4x4.Rotation(0.3, 0.2, 0.1) * Matrix4x4.Scaling(1e-3)
        ];
        foreach (var transformation in transformations)
        {
            Assert.False(transformation.IsSingular);
            Assert.True(transformation.TryInvert(out var inverse));
            Assert.True((transformation * inverse).ApproximatelyEquals(Matrix4x4.Identity, 1e-9, 1e-9));
        }

        Assert.False(Matrix3x3.Translation(1000, 0).IsSingular);
        Assert.Equal(new Vector2(0, 5), Matrix3x3.Translation(1000, 0).Inverse() * new Vector2(1000, 5));
        var shear = new Matrix2x2(1, 1e5, 0, 1);
        Assert.False(shear.IsSingular);
        Assert.Equal(new Matrix2x2(1, -1e5, 0, 1), shear.Inverse());
    }

    [Fact]
    public void SingularMatrices_AreStillDetected()
    {
        Assert.True(new Matrix4x4(1, 2, 3, 4, 2, 4, 6, 8, 1, 1, 1, 1, 0, 1, 0, 1).IsSingular);
        Assert.True(new Matrix3x3(1, 2, 3, 4, 5, 6, 7, 8, 9).IsSingular);
        Assert.True(new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0).IsSingular);
        Assert.True(new Matrix2x2(double.NaN, 0, 0, 1).IsSingular);
    }

    [Fact]
    public void StructuralPredicates_DoNotDependOnTheScale()
    {
        Assert.False(new Matrix2x2(1e-15, 1e-15, 1e-15, 1e-15).IsDiagonal);
        Assert.True(new Matrix2x2(1e-15, 0, 0, 2e-15).IsDiagonal);
        Assert.False(new Matrix2x2(1e-15, 2e-15, 0, 1e-15).IsSymmetric);
        Assert.True(new Matrix3x3(1e12, 1, 0, 0, 1e12, 0, 0, 0, 1e12).IsDiagonal);
    }

    [Fact]
    public void MatrixMxN_Determinant_IsTheProductOfPivots()
    {
        var illConditioned = new MatrixMxN(2, 2, [1, 0, 0, 1e-17]);
        Assert.Equal(1e-17, illConditioned.Determinant());
        Assert.False(illConditioned.TryInvert(out _));
        Assert.Equal(0, new MatrixMxN(2, 2, [1, 2, 2, 4]).Determinant());
        Assert.Equal(-2, new MatrixMxN(2, 2, [0, 1, 2, 0]).Determinant());
    }

    [Fact]
    public void VectorPredicates_DoNotDependOnTheScale()
    {
        var tinyX = new Vector2(1e-6, 0);
        var tinyY = new Vector2(0, 1e-6);
        Assert.False(tinyX.IsParallelTo(tinyY));
        Assert.True(tinyX.IsOrthogonalTo(tinyY));
        Assert.False(tinyX.IsOrthogonalTo(tinyX));
        Assert.True(tinyX.IsParallelTo(tinyX * -3));
        Assert.False(new Vector3(1e-7, 0, 0).IsParallelTo(new Vector3(1e-7, 1e-9, 0)));
        Assert.False(new Vector4(1e-7, 0, 0, 0).IsParallelTo(new Vector4(0, 1e-7, 0, 0)));
        Assert.True(new Vector3(1e200, 1e200, 0).IsParallelTo(new Vector3(2e200, 2e200, 0)));
    }

    [Fact]
    public void Angle_HandlesTinyAndHugeVectors()
    {
        Assert.Equal(0, new Vector2(1e-200, 0).Angle(new Vector2(3e-200, 0)));
        Assert.Equal(0, new Vector3(1e100, 1e100, 0).Angle(new Vector3(2e100, 2e100, 0)), 12);
        Assert.Equal(Math.PI / 2, new Vector4(1e300, 0, 0, 0).Angle(new Vector4(0, 0, 0, 1e300)), 12);
        Assert.True(Tolerance.AreEqual(1e-12, new Vector3(1, 0, 0).Angle(new Vector3(1, 1e-12, 0)), 0, 1e-6));
        Assert.Throws<InvalidOperationException>(() => new Vector4(0, 0, 0, 0).Angle(Vector4.UnitX));
    }

    [Fact]
    public void NearlyParallel3DLines_HaveFiniteClosestPoints()
    {
        // Regression: 1 − (d·e)² cancelled to 0 for angles around 1e-8 rad, giving NaN.
        var first = Line3D.FromPointAndDirection(Point3D.Origin, Vector3.UnitX);
        var second = Line3D.FromPointAndDirection(Point3D.Origin, new Vector3(1, 5e-9, 0));
        var (onFirst, onSecond) = first.ClosestPoints(second);
        Assert.True(double.IsFinite(onFirst.X) && double.IsFinite(onSecond.Y));
        Assert.Equal(0, first.DistanceTo(second), 9);
        Assert.Equal(IntersectionKind.Point, first.Intersect(second).Kind);

        var offset = Line3D.FromPointAndDirection(new Point3D(0, 0, 1), new Vector3(1, 1e-8, 0));
        Assert.Equal(1, first.DistanceTo(offset), 6);
        Assert.Equal(IntersectionKind.Skew, first.Intersect(offset).Kind);
    }

    [Fact]
    public void TinySegments_BehaveLikeLargeOnes()
    {
        // Regression: disjoint segments of length 1e-6 were reported to intersect.
        var horizontal = new LineSegment2D(new Point2D(0, 0), new Point2D(1e-6, 0));
        var vertical = new LineSegment2D(new Point2D(5e-7, 1e-7), new Point2D(5e-7, 2e-7));
        Assert.Equal(IntersectionKind.None, horizontal.Intersect(vertical).Kind);
        var crossing = new LineSegment2D(new Point2D(5e-7, -1e-7), new Point2D(5e-7, 1e-7));
        Assert.True(horizontal.Intersect(crossing).Point!.Value.ApproximatelyEquals(new Point2D(5e-7, 0), 1e-15, 1e-9));

        // Regression: segments shorter than about 1e-12 threw from ToLine and Intersect(Line2D).
        var minute = new LineSegment2D(new Point2D(0, 0), new Point2D(1e-13, 1e-13));
        Assert.True(minute.ToLine().IsParallelTo(Line2D.FromSlopeIntercept(1, 0)));
        Assert.Equal(IntersectionKind.Overlap, minute.Intersect(Line2D.FromSlopeIntercept(1, 0)).Kind);
    }

    [Fact]
    public void PolygonContainment_ChecksEveryPieceOfAnEdge()
    {
        // A U shape; the edge from (1, 1.5) to (4, 6) leaves through the notch corner (2, 3) and ends at the corner (4, 6).
        var u = new Polygon(new Point2D(0, 0), new Point2D(6, 0), new Point2D(6, 6), new Point2D(4, 6), new Point2D(4, 3),
            new Point2D(2, 3), new Point2D(2, 6), new Point2D(0, 6));
        Assert.False(u.Contains(new Polygon(new Point2D(1, 1.5), new Point2D(4, 6), new Point2D(5, 1))));

        // Touching the border from inside, along the bottom of the notch, is allowed.
        Assert.True(u.Contains(new Polygon(new Point2D(1, 1), new Point2D(5, 1), new Point2D(5, 3), new Point2D(1, 3))));
        Assert.True(u.Contains(u));
    }

    [Fact]
    public void ApproximateEqualityOverloads()
    {
        Assert.True(Line2D.FromSlopeIntercept(1, 0).ApproximatelyEquals(Line2D.FromSlopeIntercept(1.001, 0), 0.01, 0));
        Assert.True(Line3D.FromPointAndDirection(Point3D.Origin, Vector3.UnitX)
            .ApproximatelyEquals(Line3D.FromPointAndDirection(new Point3D(0, 0.001, 0), Vector3.UnitX), 0.01, 0));
        var segment = new LineSegment2D(Point2D.Origin, new Point2D(1, 1));
        Assert.True(segment.ApproximatelyEquals(new LineSegment2D(Point2D.Origin, new Point2D(1.001, 1)), 0.01, 0));
        Assert.False(segment.ApproximatelyEquals(new LineSegment2D(Point2D.Origin, new Point2D(1.001, 1))));
        var box = new BoundingBox2D(0, 0, 1, 1);
        Assert.True(box.ApproximatelyEquals(new BoundingBox2D(0, 0, 1 + 1e-12, 1)));
        Assert.True(box.ApproximatelyEquals(new BoundingBox2D(0, 0, 1.05, 1), 0.1, 0));
        var triangle = new Polygon(Point2D.Origin, new Point2D(1, 0), new Point2D(0, 1));
        Assert.True(triangle.ApproximatelyEquals(new Polygon(Point2D.Origin, new Point2D(1 + 1e-12, 0), new Point2D(0, 1))));
        Assert.False(triangle.ApproximatelyEquals(new Polygon(Point2D.Origin, new Point2D(1, 0), new Point2D(0, 1), new Point2D(-1, 0))));
        Assert.True(triangle.ApproximatelyEquals(new Polygon(Point2D.Origin, new Point2D(1.05, 0), new Point2D(0, 1)), 0.1, 0));
        Assert.Throws<ArgumentNullException>(() => triangle.ApproximatelyEquals(null!));
    }

    [Fact]
    public void EqualityOperators_FollowIeee_WhileEqualsTreatsNaNAsEqual()
    {
        var nanLine = Line2D.FromGeneralForm(1, 0, 0) with { };
        Assert.True(nanLine == Line2D.FromGeneralForm(1, 0, 0));
        var nanBox = new BoundingBox2D(0, 0, 1, 1);
        Assert.True(nanBox == new BoundingBox2D(0, 0, 1, 1));
        var nanMatrix = new MatrixMxN(1, 1, [double.NaN]);
        var otherNanMatrix = new MatrixMxN(1, 1, [double.NaN]);
        Assert.False(nanMatrix == otherNanMatrix);
        Assert.True(nanMatrix.Equals(otherNanMatrix));
        var nanPolygon = new Polygon(new Point2D(double.NaN, 0), new Point2D(1, 0), new Point2D(0, 1));
        var otherNanPolygon = new Polygon(new Point2D(double.NaN, 0), new Point2D(1, 0), new Point2D(0, 1));
        Assert.False(nanPolygon == otherNanPolygon);
        Assert.True(nanPolygon.Equals(otherNanPolygon));
        Assert.False(nanPolygon == new Polygon(new Point2D(double.NaN, 0), new Point2D(1, 0), new Point2D(0, 1), new Point2D(1, 1)));
        var segment = new LineSegment2D(new Point2D(double.NaN, 0), Point2D.Origin);
        Assert.False(segment == new LineSegment2D(new Point2D(double.NaN, 0), Point2D.Origin));
        Assert.True(ProjectionData.Default == ProjectionData.Default);
    }
}
