using SharpMath.Expressions;
using SharpMath.Geometry;

namespace SharpMath.Tests;

/// <summary>
///     Guards the hot paths against regressions that reintroduce heap allocations (e.g. boxing, iterators or LINQ).
/// </summary>
public class AllocationTests
{
    private static double _sink;

    private static long AllocatedBytes(Action action)
    {
        // Warm up so that JIT compilation and static initialization are not counted.
        for (var i = 0; i < 3; i++)
            action();

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 100; i++)
            action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    [Fact]
    public void VectorOperations_DoNotAllocate()
    {
        var a = new Vector3(1, 2, 3);
        var b = new Vector3(-2, 0.5, 4);
        Assert.Equal(0, AllocatedBytes(() =>
        {
            var c = a + b * 2 - a.Cross(b) / 3;
            _sink = c.Dot(a) + c.Normalize().Length + (a == b ? 1 : 0) + a.GetHashCode() + (a.IsZero ? 1 : 0) +
                    a.Angle(b) + (a.IsParallelTo(b) ? 1 : 0) + (a.ApproximatelyEquals(b) ? 1 : 0);
        }));
        var p = new Vector4(1, 2, 3, 4);
        Assert.Equal(0, AllocatedBytes(() => _sink = (p * 2 + p).Normalize().Length + (p.IsParallelTo(p) ? 1 : 0)));
    }

    [Fact]
    public void MatrixOperations_DoNotAllocate()
    {
        var m = Matrix4x4.Rotation(0.3, 0.5, 0.7) * Matrix4x4.Translation(1, 2, 3) * Matrix4x4.Scaling(2);
        var v = new Vector3(1, 2, 3);
        Assert.Equal(0, AllocatedBytes(() =>
        {
            var product = m * m.Inverse() * m.Transpose();
            _sink = product.Determinant + (m * v).X + (m == product ? 1 : 0) + m.GetHashCode() +
                    (m.IsOrthogonal ? 1 : 0) + (m.IsSingular ? 1 : 0) + m.Adjugate().Trace +
                    (m.ApproximatelyEquals(product) ? 1 : 0) + (m.IsDiagonal ? 1 : 0) + (m.IsSymmetric ? 1 : 0);
        }));

        var small = new Matrix3x3(2, 5, 2, 3, -3, 1, 1, 4, -4);
        Assert.Equal(0, AllocatedBytes(() => _sink = small.Inverse().Determinant + (small * Vector2.One).X +
                                                     small.Cofactors().Trace + (small.IsTriangular ? 1 : 0)));
        var tiny = new Matrix2x2(8, 3, 4, 2);
        Assert.Equal(0, AllocatedBytes(() => _sink = tiny.Inverse().Determinant + (tiny * tiny).Trace));
    }

    [Fact]
    public void GeometryQueries_DoNotAllocate()
    {
        var polygon = new Polygon(new Point2D(0, 0), new Point2D(4, 0), new Point2D(4, 4), new Point2D(2, 1),
            new Point2D(0, 4));
        var line = Line2D.FromPoints(new Point2D(0, 1), new Point2D(3, 2));
        var other = Line2D.FromSlopeIntercept(-1, 5);
        var segment = new LineSegment2D(new Point2D(0, 0), new Point2D(5, 5));
        var line3 = Line3D.FromPointAndDirection(new Point3D(1, 2, 3), new Vector3(1, 1, 0));
        var skew = Line3D.FromPointAndDirection(new Point3D(0, 0, 5), new Vector3(0, 1, 1));
        Assert.Equal(0, AllocatedBytes(() =>
            _sink = (polygon.Contains(new Point2D(1, 1)) ? 1 : 0) + polygon.Area + polygon.Perimeter +
                    polygon.Centroid.X + polygon.BoundingBox.Width + line.Intersect(other).Point!.Value.X +
                    segment.Intersect(new LineSegment2D(new Point2D(0, 5), new Point2D(5, 0))).Point!.Value.Y +
                    line3.DistanceTo(skew) + (line3.Intersect(skew).Kind == IntersectionKind.Skew ? 1 : 0)));
    }

    [Fact]
    public void EvaluatingParsedExpressions_DoesNotAllocate()
    {
        var expression = MathExpression.Parse("2x^2 - 3x + sin(x) * max(x, 1, 2) + 5!");
        var function = expression.ToFunc("x");
        var x = 0.0;
        Assert.Equal(0, AllocatedBytes(() => _sink = function(x += 0.1)));
        Assert.Equal(0, AllocatedBytes(() => _sink = expression.Evaluate("x", x += 0.1)));
        var constant = MathExpression.Parse("2 + 3 * 4");
        Assert.Equal(0, AllocatedBytes(() => _sink = constant.Evaluate()));
    }
}
