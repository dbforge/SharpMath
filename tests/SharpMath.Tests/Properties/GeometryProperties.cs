using CsCheck;
using SharpMath.Geometry;

namespace SharpMath.Tests.Properties;

public class GeometryProperties
{
    private static readonly Gen<double> Coordinate = Gen.Double[-100, 100];
    private static readonly Gen<Point2D> Points = Gen.Select(Coordinate, Coordinate, (x, y) => new Point2D(x, y));
    private static readonly Gen<Vector3> Vectors3 = Gen.Select(Coordinate, Coordinate, Coordinate, (x, y, z) => new Vector3(x, y, z));

    [Fact]
    public void VectorProducts()
    {
        Gen.Select(Vectors3, Vectors3).Sample((a, b) =>
        {
            var cross = a.Cross(b);
            var scale = a.Length * b.Length;
            Assert.True(Tolerance.IsNegligible(cross.Dot(a), scale * a.Length));
            Assert.True(Tolerance.IsNegligible(cross.Dot(b), scale * b.Length));
            Assert.Equal(a.Dot(b), b.Dot(a));
            Assert.True(Tolerance.AreEqual(cross.LengthSquared + a.Dot(b) * a.Dot(b), scale * scale, 1e-6, 1e-9));
            if (a.LengthSquared > 0)
                Assert.True(a.Normalize().IsNormalized);
            if (a.LengthSquared > 0 && b.LengthSquared > 0)
                Assert.InRange(a.Angle(b), 0, Math.PI);
        });
    }

    [Fact]
    public void DirectionPredicates_DoNotDependOnTheScale()
    {
        Gen.Select(Vectors3, Vectors3, Gen.Int[-150, 150]).Sample((a, b, exponent) =>
        {
            var scale = Math.Pow(10, exponent);
            Assert.Equal(a.IsParallelTo(b), (a * scale).IsParallelTo(b * scale));
            Assert.Equal(a.IsOrthogonalTo(b), (a * scale).IsOrthogonalTo(b * scale));
            if (a.LengthSquared > 0)
                Assert.True((a * scale).IsParallelTo(a * -scale * 3));
        });
    }

    [Fact]
    public void PolygonArea_IsInvariantUnderTranslationAndVertexRotation()
    {
        Gen.Select(Points.Array[3, 12], Coordinate, Coordinate, Gen.Int[0, 11]).Sample((points, dx, dy, shift) =>
        {
            var polygon = new Polygon(points);
            var translated = new Polygon(points.Select(p => p + new Vector2(dx, dy)));
            var rotated = new Polygon(points.Skip(shift % points.Length).Concat(points.Take(shift % points.Length)));
            Assert.True(Tolerance.AreEqual(polygon.SignedArea, translated.SignedArea, 1e-6, 1e-9));
            Assert.True(Tolerance.AreEqual(polygon.SignedArea, rotated.SignedArea, 1e-6, 1e-9));
            Assert.True(Tolerance.AreEqual(polygon.Perimeter, rotated.Perimeter, 1e-6, 1e-9));
            var box = polygon.BoundingBox;
            Assert.All(points, p => Assert.True(box.Contains(p)));
        });
    }

    [Fact]
    public void LineIntersections_LieOnBothLines()
    {
        Gen.Select(Points, Points, Points, Points).Sample((a, b, c, d) =>
        {
            if (a.Distance(b) < 1e-3 || c.Distance(d) < 1e-3)
                return;
            var first = Line2D.FromPoints(a, b);
            var second = Line2D.FromPoints(c, d);
            Assert.True(first.Contains(a) && first.Contains(b));
            var intersection = first.Intersect(second);
            if (intersection.Kind != IntersectionKind.Point)
                return;
            var point = intersection.Point!.Value;
            var scale = Math.Max(1, Math.Max(Math.Abs(point.X), Math.Abs(point.Y)));
            Assert.True(first.DistanceTo(point) < 1e-6 * scale);
            Assert.True(second.DistanceTo(point) < 1e-6 * scale);
        });
    }

    [Fact]
    public void ClosestPointsOnSkewLines_AreConnectedByACommonPerpendicular()
    {
        Gen.Select(Vectors3, Vectors3, Vectors3, Vectors3).Sample((p, d, q, e) =>
        {
            if (d.Length < 1e-3 || e.Length < 1e-3 || d.Normalize().Cross(e.Normalize()).Length < 1e-3)
                return;
            var first = Line3D.FromPointAndDirection(p.ToPoint(), d);
            var second = Line3D.FromPointAndDirection(q.ToPoint(), e);
            var (onFirst, onSecond) = first.ClosestPoints(second);
            var connection = onSecond - onFirst;
            Assert.True(Tolerance.IsNegligible(connection.Dot(first.Direction), 1e3));
            Assert.True(Tolerance.IsNegligible(connection.Dot(second.Direction), 1e3));
            Assert.True(first.Contains(onFirst));
            Assert.True(second.Contains(onSecond));
        });
    }
}
