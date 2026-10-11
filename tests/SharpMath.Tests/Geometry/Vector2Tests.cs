using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class Vector2Tests
{
    [Fact]
    public void Constructors_SetComponents()
    {
        var vector = new Vector2(3, 4);
        Assert.Equal(3, vector.X);
        Assert.Equal(4, vector.Y);
        Assert.Equal(new Vector2(1, 2), new Vector2(new Point2D(1, 2)));
        Assert.Equal(2, vector.Dimension);
    }

    [Fact]
    public void StaticVectors_HaveExpectedValues()
    {
        Assert.Equal(new Vector2(0, 0), Vector2.Zero);
        Assert.Equal(new Vector2(1, 1), Vector2.One);
        Assert.Equal(new Vector2(1, 0), Vector2.UnitX);
        Assert.Equal(new Vector2(0, 1), Vector2.UnitY);
        Assert.Equal(new Vector2(0, 1), Vector2.Up);
        Assert.Equal(new Vector2(0, -1), Vector2.Down);
        Assert.Equal(new Vector2(-1, 0), Vector2.Left);
        Assert.Equal(new Vector2(1, 0), Vector2.Right);
    }

    [Fact]
    public void Indexer_ReturnsComponentsAndValidatesIndex()
    {
        var vector = new Vector2(10, 5);
        Assert.Equal(10, vector[0]);
        Assert.Equal(5, vector[1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => vector[2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => vector[-1]);
    }

    [Fact]
    public void Length_IsEuclidean()
    {
        var vector = new Vector2(3, 4);
        Assert.Equal(5, vector.Length);
        Assert.Equal(25, vector.LengthSquared);
    }

    [Fact]
    public void IsZero_AndIsNormalized_UseTolerance()
    {
        Assert.True(new Vector2(1e-13, -1e-13).IsZero);
        Assert.False(new Vector2(1e-3, 0).IsZero);
        Assert.True(new Vector2(0.6, 0.8).IsNormalized);
        Assert.False(new Vector2(1, 1).IsNormalized);
    }

    [Fact]
    public void Normalize_ReturnsUnitVector()
    {
        Assert.True(new Vector2(3, 4).Normalize().ApproximatelyEquals(new Vector2(0.6, 0.8)));
        Assert.True(new Vector2(1e-200, 0).Normalize().ApproximatelyEquals(Vector2.UnitX));
        Assert.True(new Vector2(1e200, 1e200).Normalize().IsNormalized);
    }

    [Fact]
    public void Normalize_ThrowsForZeroVector() =>
        Assert.Throws<InvalidOperationException>(() => Vector2.Zero.Normalize());

    [Fact]
    public void DotAndCross_AreComputed()
    {
        Assert.Equal(0, Vector2.UnitX.Dot(Vector2.UnitY));
        Assert.Equal(20, Vector2.Dot(new Vector2(2, 4), new Vector2(4, 3)));
        Assert.Equal(1, Vector2.UnitX.Cross(Vector2.UnitY));
        Assert.Equal(-10, Vector2.Cross(new Vector2(2, 4), new Vector2(3, 1)));
    }

    [Fact]
    public void Angle_IsComputedAndClamped()
    {
        Assert.Equal(Math.PI / 2, Vector2.UnitX.Angle(Vector2.UnitY), 12);
        Assert.Equal(Math.PI, Vector2.Angle(Vector2.UnitX, -Vector2.UnitX), 12);

        // Regression: parallel vectors used to produce NaN because the cosine slightly exceeded 1.
        var vector = new Vector2(0.1, 0.7);
        Assert.Equal(0, vector.Angle(vector * 3), 6);
    }

    [Fact]
    public void Angle_ThrowsForZeroVector() =>
        Assert.Throws<InvalidOperationException>(() => Vector2.Zero.Angle(Vector2.UnitX));

    [Fact]
    public void Distance_IsComputed()
    {
        Assert.Equal(5, new Vector2(1, 1).Distance(new Vector2(4, 5)));
        Assert.Equal(5, Vector2.Distance(new Vector2(1, 1), new Vector2(4, 5)));
        Assert.Equal(25, new Vector2(1, 1).DistanceSquared(new Vector2(4, 5)));
        Assert.Equal(25, Vector2.DistanceSquared(new Vector2(1, 1), new Vector2(4, 5)));
    }

    [Fact]
    public void ParallelogramArea_IsAbsoluteCross() =>
        Assert.Equal(10, new Vector2(2, 4).ParallelogramArea(new Vector2(3, 1)));

    [Fact]
    public void Perpendicular_RotatesCounterclockwise() =>
        Assert.Equal(new Vector2(-2, 1), new Vector2(1, 2).Perpendicular);

    [Fact]
    public void Lerp_ClampsAndLerpUnclamped_Extrapolates()
    {
        var a = new Vector2(0, 0);
        var b = new Vector2(10, 20);
        Assert.Equal(new Vector2(5, 10), Vector2.Lerp(a, b, 0.5));
        Assert.Equal(b, Vector2.Lerp(a, b, 2));
        Assert.Equal(a, Vector2.Lerp(a, b, -1));
        Assert.Equal(new Vector2(20, 40), Vector2.LerpUnclamped(a, b, 2));
    }

    [Fact]
    public void MoveTowards_ClampsAtTargetAndHandlesEqualPoints()
    {
        var source = new Vector2(0, 0);
        var target = new Vector2(3, 4);
        Assert.Equal(new Vector2(0.6, 0.8), Vector2.MoveTowards(source, target, 1));
        Assert.Equal(target, Vector2.MoveTowards(source, target, 10));

        // Regression: equal points produced NaN.
        Assert.Equal(target, Vector2.MoveTowards(target, target, 1));
    }

    [Fact]
    public void ParallelismAndOrthogonality_DoNotDivideByComponents()
    {
        // Regression: zero components produced NaN and "not parallel".
        Assert.True(new Vector2(1, 0).IsParallelTo(new Vector2(2, 0)));
        Assert.True(new Vector2(1, 0).IsParallelTo(new Vector2(-4, 0)));
        Assert.False(new Vector2(1, 0).IsParallelTo(new Vector2(1, 1)));
        Assert.True(Vector2.Zero.IsParallelTo(Vector2.UnitX));

        Assert.True(Vector2.UnitX.IsOrthogonalTo(Vector2.UnitY));
        Assert.False(Vector2.UnitX.IsOrthogonalTo(Vector2.One));
        Assert.True(Vector2.Zero.IsOrthogonalTo(Vector2.UnitX));

        Assert.True(Vector2.UnitX.IsOrthonormalTo(Vector2.UnitY));
        Assert.False(Vector2.UnitX.IsOrthonormalTo(new Vector2(0, 2)));
    }

    [Fact]
    public void Transform_UsesColumnVectors()
    {
        Assert.True(Vector2.Transform(Vector2.UnitX, Matrix2x2.Rotation(Math.PI / 2)).ApproximatelyEquals(Vector2.UnitY));

        // Regression (issue #6): translations are applied in homogeneous coordinates.
        Assert.Equal(new Vector2(11, 11), Vector2.Transform(Vector2.One, Matrix3x3.Translation(10, 10)));
    }

    [Fact]
    public void Operators_ComputeComponentWise()
    {
        var a = new Vector2(1, 2);
        var b = new Vector2(3, 5);
        Assert.Equal(new Vector2(4, 7), a + b);
        Assert.Equal(new Vector2(-2, -3), a - b);
        Assert.Equal(new Vector2(-1, -2), -a);
        Assert.Equal(new Vector2(2, 4), a * 2);
        Assert.Equal(new Vector2(2, 4), 2 * a);
        Assert.Equal(new Vector2(0.5, 1), a / 2);
    }

    [Fact]
    public void Equality_IsExactAndApproximatelyEquals_IsTolerant()
    {
        var a = new Vector2(0.1 + 0.2, 0);
        var b = new Vector2(0.3, 0);
        Assert.False(a == b);
        Assert.True(a != b);
        Assert.False(a.Equals(b));
        Assert.True(a.ApproximatelyEquals(b));
        Assert.False(a.ApproximatelyEquals(new Vector2(0.31, 0)));
        Assert.True(a.ApproximatelyEquals(new Vector2(0.31, 0), 0.1, 0));

        Assert.True(new Vector2(1, 2).Equals((object) new Vector2(1, 2)));
        Assert.False(new Vector2(1, 2).Equals((object) new Vector3(1, 2, 0)));
        Assert.Equal(new Vector2(1, 2).GetHashCode(), new Vector2(1, 2).GetHashCode());
        Assert.Equal(new Vector2(0.0, 0).GetHashCode(), new Vector2(-0.0, 0).GetHashCode());
    }

    [Fact]
    public void Conversions_AndStrings()
    {
        Assert.Equal(new Point2D(1, 2), new Vector2(1, 2).ToPoint());
        Assert.Equal("Vector2 {X: 1.5, Y: -2}", new Vector2(1.5, -2).ToString());
        Assert.Equal(@"\left( \begin{array}{c} 1 \\ 6 \end{array} \right)", new Vector2(1, 6).ToLaTeXString());
    }
}
