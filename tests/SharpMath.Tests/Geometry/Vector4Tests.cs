using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class Vector4Tests
{
    [Fact]
    public void Constructors_StaticVectorsAndIndexer()
    {
        var vector = new Vector4(new Vector3(1, 2, 3), 4);
        Assert.Equal(new Vector4(1, 2, 3, 4), vector);
        Assert.Equal(4, vector.Dimension);
        Assert.Equal(4, vector[3]);
        Assert.Equal(3, vector[2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => vector[4]);

        // Regression: UnitW used to be (0, 0, 1, 0).
        Assert.Equal(new Vector4(0, 0, 0, 1), Vector4.UnitW);
        Assert.Equal(new Vector4(1, 0, 0, 0), Vector4.UnitX);
        Assert.Equal(new Vector4(0, 1, 0, 0), Vector4.UnitY);
        Assert.Equal(new Vector4(0, 0, 1, 0), Vector4.UnitZ);
        Assert.Equal(new Vector4(1, 1, 1, 1), Vector4.One);
        Assert.Equal(new Vector4(0, 0, 0, 0), Vector4.Zero);
    }

    [Fact]
    public void ArithmeticOperators_Exist()
    {
        var a = new Vector4(1, 2, 3, 4);
        Assert.Equal(new Vector4(2, 4, 6, 8), a + a);
        Assert.Equal(Vector4.Zero, a - a);
        Assert.Equal(new Vector4(-1, -2, -3, -4), -a);
        Assert.Equal(new Vector4(2, 4, 6, 8), a * 2);
        Assert.Equal(new Vector4(2, 4, 6, 8), 2 * a);
        Assert.Equal(new Vector4(0.5, 1, 1.5, 2), a / 2);
    }

    [Fact]
    public void MetricOperations()
    {
        var a = new Vector4(1, 1, 1, 1);
        Assert.Equal(2, a.Length);
        Assert.Equal(4, a.LengthSquared);
        Assert.Equal(4, a.Dot(Vector4.One));
        Assert.Equal(4, Vector4.Dot(a, Vector4.One));
        Assert.True(a.Normalize().ApproximatelyEquals(new Vector4(0.5, 0.5, 0.5, 0.5)));
        Assert.Throws<InvalidOperationException>(() => Vector4.Zero.Normalize());
        Assert.Equal(Math.PI / 2, Vector4.Angle(Vector4.UnitX, Vector4.UnitW), 12);
        Assert.Equal(Math.PI / 2, Vector4.UnitX.Angle(Vector4.UnitW), 12);
        Assert.Equal(2, Vector4.Distance(Vector4.Zero, a));
        Assert.Equal(2, a.Distance(Vector4.Zero));
        Assert.Equal(4, Vector4.DistanceSquared(Vector4.Zero, a));
        Assert.Equal(4, a.DistanceSquared(Vector4.Zero));
        Assert.True(new Vector4(0, 0, 0, 1).IsNormalized);
        Assert.True(new Vector4(0, 0, 0, 1e-13).IsZero);
    }

    [Fact]
    public void Predicates()
    {
        Assert.True(new Vector4(1, 0, 2, 0).IsParallelTo(new Vector4(-2, 0, -4, 0)));
        Assert.False(new Vector4(1, 0, 2, 0).IsParallelTo(new Vector4(1, 1, 2, 0)));
        Assert.True(Vector4.UnitX.IsOrthogonalTo(Vector4.UnitW));
        Assert.True(Vector4.UnitX.IsOrthonormalTo(Vector4.UnitW));
        Assert.False(Vector4.UnitX.IsOrthonormalTo(Vector4.UnitX));
    }

    [Fact]
    public void InterpolationTransformEqualityStrings()
    {
        Assert.Equal(new Vector4(1, 1, 1, 1), Vector4.Lerp(Vector4.Zero, new Vector4(2, 2, 2, 2), 0.5));
        Assert.Equal(new Vector4(2, 2, 2, 2), Vector4.Lerp(Vector4.Zero, new Vector4(2, 2, 2, 2), 3));
        Assert.Equal(new Vector4(4, 4, 4, 4), Vector4.LerpUnclamped(Vector4.Zero, new Vector4(2, 2, 2, 2), 2));
        Assert.Equal(new Vector4(1, 0, 0, 0), Vector4.MoveTowards(Vector4.Zero, new Vector4(3, 0, 0, 0), 1));
        Assert.Equal(new Vector4(3, 0, 0, 0), Vector4.MoveTowards(Vector4.Zero, new Vector4(3, 0, 0, 0), 5));
        Assert.Equal(new Vector4(2, 3, 4, 1), Vector4.Transform(new Vector4(1, 2, 3, 1), Matrix4x4.Translation(1, 1, 1)));
        Assert.Equal(new Vector4(1, 2, 3, 0), Vector4.Transform(new Vector4(1, 2, 3, 0), Matrix4x4.Translation(1, 1, 1)));

        var a = new Vector4(1, 2, 3, 4);
        Assert.True(a == new Vector4(1, 2, 3, 4));
        Assert.True(a != new Vector4(1, 2, 3, 5));
        Assert.True(a.Equals((object) new Vector4(1, 2, 3, 4)));
        Assert.Equal(a.GetHashCode(), new Vector4(1, 2, 3, 4).GetHashCode());
        Assert.True(a.ApproximatelyEquals(new Vector4(1, 2, 3, 4 + 1e-12)));
        Assert.Equal("Vector4 {X: 1, Y: 2, Z: 3, W: 4}", a.ToString());
        Assert.Equal(@"\left( \begin{array}{c} 1 \\ 2 \\ 3 \\ 4 \end{array} \right)", a.ToLaTeXString());
    }
}
