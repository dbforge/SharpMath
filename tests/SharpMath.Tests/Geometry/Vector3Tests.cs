using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class Vector3Tests
{
    [Fact]
    public void Constructors_SetComponents()
    {
        Assert.Equal(new Vector3(1, 2, 3), new Vector3(new Vector2(1, 2), 3));
        Assert.Equal(new Vector3(1, 2, 3), new Vector3(new Point3D(1, 2, 3)));
        Assert.Equal(3, Vector3.One.Dimension);
    }

    [Fact]
    public void StaticVectors_FollowRightHandedConvention()
    {
        Assert.Equal(new Vector3(0, 0, -1), Vector3.Forward);
        Assert.Equal(new Vector3(0, 0, 1), Vector3.Back);
        Assert.Equal(new Vector3(0, 1, 0), Vector3.Up);
        Assert.Equal(new Vector3(0, -1, 0), Vector3.Down);
        Assert.Equal(new Vector3(-1, 0, 0), Vector3.Left);
        Assert.Equal(new Vector3(1, 0, 0), Vector3.Right);
        Assert.Equal(Vector3.UnitZ, Vector3.UnitX.Cross(Vector3.UnitY));
        Assert.Equal(Vector3.Zero, default);
        Assert.Equal(new Vector3(1, 1, 1), Vector3.One);
        Assert.Equal(new Vector3(0, 1, 0), Vector3.UnitY);
    }

    [Fact]
    public void Indexer_ReturnsComponentsAndValidatesIndex()
    {
        var vector = new Vector3(10, 5, 3);
        Assert.Equal(10, vector[0]);
        Assert.Equal(5, vector[1]);
        Assert.Equal(3, vector[2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => vector[3]);
    }

    [Fact]
    public void LengthAndDistance()
    {
        Assert.Equal(13, new Vector3(13, 0, 0).Length);
        Assert.Equal(3, new Vector3(1, 2, 2).Length);
        Assert.Equal(Math.Sqrt(30), new Vector3(10, 5, 3).Distance(new Vector3(5, 6, 1)));
        Assert.Equal(Math.Sqrt(30), Vector3.Distance(new Vector3(10, 5, 3), new Vector3(5, 6, 1)));
        Assert.Equal(30, Vector3.DistanceSquared(new Vector3(10, 5, 3), new Vector3(5, 6, 1)));
        Assert.Equal(30, new Vector3(10, 5, 3).DistanceSquared(new Vector3(5, 6, 1)));
    }

    [Fact]
    public void Products()
    {
        Assert.Equal(8, new Vector3(2, 3, 1).Dot(new Vector3(-1, 2, 4)));
        Assert.Equal(0, Vector3.Dot(Vector3.Forward, Vector3.Up));
        Assert.Equal(new Vector3(-15, 1, 10), new Vector3(1, -5, 2).Cross(new Vector3(2, 0, 3)));
        Assert.Equal(new Vector3(-15, 1, 10), Vector3.Cross(new Vector3(1, -5, 2), new Vector3(2, 0, 3)));
        // Behaviour change: the old implementation returned the absolute value; a·(b×c) = det([a; b; c]) is signed.
        Assert.Equal(-31, Vector3.ScalarTripleProduct(new Vector3(2, 0, 5), new Vector3(-1, 5, -2), new Vector3(2, 1, 2)));
        Assert.Equal(Math.Sqrt(525), new Vector3(3, 4, 4).ParallelogramArea(new Vector3(1, -2, 3)), 12);
    }

    [Fact]
    public void AngleNormalizeAndPredicates()
    {
        Assert.Equal(Math.PI / 2, Vector3.Angle(Vector3.UnitX, Vector3.UnitZ), 12);
        Assert.True(new Vector3(1, 2, 2).Normalize().ApproximatelyEquals(new Vector3(1 / 3.0, 2 / 3.0, 2 / 3.0)));
        Assert.Throws<InvalidOperationException>(() => Vector3.Zero.Normalize());
        Assert.Throws<InvalidOperationException>(() => Vector3.Zero.Angle(Vector3.UnitX));

        Assert.True(new Vector3(2, 3, 3).IsParallelTo(new Vector3(4, 6, 6)));
        // Regression: zero components produced NaN and "not parallel".
        Assert.True(new Vector3(0, 1, 3).IsParallelTo(new Vector3(0, 2, 6)));
        Assert.True(new Vector3(1, 0, 0).IsParallelTo(new Vector3(-4, 0, 0)));
        Assert.False(new Vector3(0, 1, 3).IsParallelTo(new Vector3(0, 3, 2)));

        Assert.True(Vector3.Forward.IsOrthogonalTo(Vector3.Up));
        Assert.False(Vector3.Forward.IsOrthogonalTo(Vector3.Back));
        Assert.True(Vector3.Zero.IsOrthogonalTo(Vector3.UnitX));
        Assert.True(Vector3.Back.IsOrthonormalTo(Vector3.Down));
        Assert.False(Vector3.Forward.IsOrthonormalTo(new Vector3(2, 3, 2)));
        Assert.True(new Vector3(0, 0.6, 0.8).IsNormalized);
        Assert.True(new Vector3(0, 0, 1e-13).IsZero);
    }

    [Fact]
    public void InterpolationAndMovement()
    {
        Assert.Equal(new Vector3(4, 8, 9), Vector3.Lerp(new Vector3(2, 6, 8), new Vector3(6, 10, 10), 0.5));
        Assert.Equal(new Vector3(10.5, 4, 8), Vector3.Lerp(new Vector3(13, 2, 9), new Vector3(3, 10, 5), 0.25));
        Assert.Equal(new Vector3(2, 0, 0), Vector3.LerpUnclamped(Vector3.Zero, Vector3.UnitX, 2));
        Assert.Equal(new Vector3(1, 0, 0), Vector3.MoveTowards(Vector3.Zero, new Vector3(5, 0, 0), 1));
        Assert.Equal(Vector3.One, Vector3.MoveTowards(Vector3.One, Vector3.One, 1));
        Assert.Equal(new Vector3(5, 0, 0), Vector3.MoveTowards(Vector3.Zero, new Vector3(5, 0, 0), 9));
    }

    [Fact]
    public void Transform_AppliesMatricesAndSinglePerspectiveDivide()
    {
        Assert.Equal(new Vector3(2, 4, 6), Vector3.Transform(new Vector3(1, 2, 3), Matrix3x3.Identity * 2));
        Assert.Equal(new Vector3(2, 3, 4), Vector3.Transform(new Vector3(1, 2, 3), Matrix4x4.Translation(1, 1, 1)));

        // Regression: Z used to be divided by W twice.
        var halve = new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 2);
        Assert.Equal(new Vector3(1, 1, 1), Vector3.Transform(new Vector3(2, 2, 2), halve));
    }

    [Fact]
    public void With_CreatesModifiedCopies()
    {
        var vector = new Vector3(1, 2, 3);
        Assert.Equal(new Vector3(1, 2, 0), vector with { Z = 0 });
        Assert.Equal(new Vector3(1, 2, 3), vector);
        Assert.Equal(new Point3D(0, 2, 3), new Point3D(1, 2, 3) with { X = 0 });
        Assert.Equal(new Vector4(1, 2, 3, 1), new Vector4(1, 2, 3, 4) with { W = 1 });
        Assert.Equal(new Matrix4x4(1, 0, 0, 5, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1), Matrix4x4.Identity with { M14 = 5 });
        Assert.Equal(new Matrix3x3(9, 0, 0, 0, 1, 0, 0, 0, 1), Matrix3x3.Identity with { M11 = 9 });
        Assert.Equal(new Matrix2x2(1, 2, 0, 1), Matrix2x2.Identity with { M12 = 2 });
        Assert.Equal(new Vector2(0, 1), Vector2.One with { X = 0 });
        Assert.Equal(new Point2D(1, 0), new Point2D(1, 1) with { Y = 0 });
        var segment = new LineSegment2D(Point2D.Origin, new Point2D(1, 1)) with { End = new Point2D(2, 2) };
        Assert.Equal(new Point2D(2, 2), segment.End);
    }

    [Fact]
    public void FromHomogeneous_DividesByW()
    {
        Assert.Equal(new Vector3(1, 2, 3), Vector3.FromHomogeneous(new Vector4(2, 4, 6, 2)));
        Assert.Throws<ArgumentException>(() => Vector3.FromHomogeneous(new Vector4(1, 2, 3, 0)));
    }

    [Fact]
    public void OperatorsEqualityAndStrings()
    {
        var a = new Vector3(1, 2, 3);
        Assert.Equal(new Vector3(2, 4, 6), a + a);
        Assert.Equal(Vector3.Zero, a - a);
        Assert.Equal(new Vector3(-1, -2, -3), -a);
        Assert.Equal(new Vector3(2, 4, 6), 2 * a);
        Assert.Equal(new Vector3(2, 4, 6), a * 2);
        Assert.Equal(new Vector3(0.5, 1, 1.5), a / 2);
        Assert.True(a == new Vector3(1, 2, 3));
        Assert.True(a != new Vector3(2, 2, 3));
        Assert.True(a.Equals((object) new Vector3(1, 2, 3)));
        Assert.False(a.Equals((object) "a"));
        Assert.Equal(a.GetHashCode(), new Vector3(1, 2, 3).GetHashCode());
        Assert.True(a.ApproximatelyEquals(new Vector3(1, 2, 3 + 1e-12)));
        Assert.False(a.ApproximatelyEquals(new Vector3(1, 2, 3.1), 0.01, 0));
        Assert.Equal(new Point3D(1, 2, 3), a.ToPoint());
        Assert.Equal("Vector3 {X: 1, Y: 2, Z: 3}", a.ToString());
        Assert.Equal(@"\left( \begin{array}{c} 1 \\ 6 \\ 8 \end{array} \right)", new Vector3(1, 6, 8).ToLaTeXString());
    }
}
