using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class Matrix2x2Tests
{
    private static readonly Matrix2x2 Sample = new(8, 3, 4, 2);

    [Fact]
    public void ConstructorsAndIndexers_AreRowMajor()
    {
        Assert.Equal(Sample, new Matrix2x2(new Vector2(8, 3), new Vector2(4, 2)));
        Assert.Equal(3, Sample[0, 1]);
        Assert.Equal(4, Sample[1, 0]);
        Assert.Equal(3, Sample[1]);
        Assert.Equal(2, Sample[3]);
        Assert.Equal(8, Sample.M11);
        Assert.Equal(2, Sample.RowCount);
        Assert.Equal(2, Sample.ColumnCount);
        Assert.Equal(2, Sample.Dimension);

        // Regression: [0, 2] used to silently return M21.
        Assert.Throws<ArgumentOutOfRangeException>(() => Sample[0, 2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => Sample[2, 0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => Sample[4]);
    }

    [Fact]
    public void DeterminantTraceAndInverse()
    {
        Assert.Equal(4, Sample.Determinant);
        Assert.Equal(10, Sample.Trace);

        // Regression: Inverse used to return the identity matrix.
        var inverse = Sample.Inverse();
        Assert.True(inverse.ApproximatelyEquals(new Matrix2x2(0.5, -0.75, -1, 2)));
        Assert.True((Sample * inverse).IsIdentity);
        Assert.Equal(new Matrix2x2(2, -3, -4, 8), Sample.Adjugate());
        Assert.Equal(new Matrix2x2(2, -4, -3, 8), Sample.Cofactors());
    }

    [Fact]
    public void Inverse_OfSingularMatrixThrows()
    {
        var singular = new Matrix2x2(1, 2, 2, 4);
        Assert.True(singular.IsSingular);
        Assert.False(singular.TryInvert(out var inverse));
        Assert.Equal(Matrix2x2.Zero, inverse);
        Assert.Throws<InvalidOperationException>(() => singular.Inverse());
    }

    [Fact]
    public void IsSingular_IsScaleAware()
    {
        Assert.False((Matrix2x2.Identity * 1e-5).IsSingular);
        Assert.True(Matrix2x2.Zero.IsSingular);
    }

    [Fact]
    public void Predicates()
    {
        // Regression: IsIdentity used to check M22 == 0.
        Assert.True(Matrix2x2.Identity.IsIdentity);
        Assert.False(new Matrix2x2(1, 0, 0, 0).IsIdentity);
        Assert.False(new Matrix2x2(1, 1, 0, 1).IsIdentity);

        Assert.True(new Matrix2x2(1, 0, 0, 0).IsDiagonal);
        Assert.True(Matrix2x2.Zero.IsDiagonal);
        Assert.False(Sample.IsDiagonal);
        Assert.True(new Matrix2x2(1, 2, 0, 3).IsTriangular);
        Assert.False(Sample.IsTriangular);
        Assert.True(new Matrix2x2(1, 2, 2, 3).IsSymmetric);
        Assert.False(Sample.IsSymmetric);
        Assert.True(new Matrix2x2(0, 2, -2, 0).IsSkewSymmetric);
        Assert.False(new Matrix2x2(1, 2, -2, 0).IsSkewSymmetric);
        Assert.True(Matrix2x2.Rotation(0.7).IsOrthogonal);
        Assert.False(Sample.IsOrthogonal);
    }

    [Fact]
    public void Factories()
    {
        Assert.True((Matrix2x2.Rotation(Math.PI / 2) * Vector2.UnitX).ApproximatelyEquals(Vector2.UnitY));
        Assert.Equal(new Vector2(2, 3), Matrix2x2.Scaling(2, 3) * Vector2.One);
        Assert.Equal(new Matrix2x2(4, 0, 0, 4), Matrix2x2.Scaling(4));
        Assert.Equal(Sample, Matrix2x2.FromMatrix(MatrixMxN.FromMatrix(Sample)));
        Assert.Throws<ArgumentException>(() => Matrix2x2.FromMatrix(Matrix3x3.Identity));
        Assert.Throws<ArgumentNullException>(() => Matrix2x2.FromMatrix(null!));
    }

    [Fact]
    public void Operators()
    {
        var other = new Matrix2x2(1, 2, 3, 4);
        Assert.Equal(new Matrix2x2(9, 5, 7, 6), Sample + other);
        Assert.Equal(new Matrix2x2(7, 1, 1, -2), Sample - other);
        Assert.Equal(new Matrix2x2(-8, -3, -4, -2), -Sample);
        Assert.Equal(new Matrix2x2(16, 6, 8, 4), Sample * 2);
        Assert.Equal(new Matrix2x2(16, 6, 8, 4), 2 * Sample);
        Assert.Equal(new Matrix2x2(4, 1.5, 2, 1), Sample / 2);
        Assert.Equal(new Matrix2x2(17, 28, 10, 16), Sample * other);
        Assert.Equal(new Vector2(11, 6), Sample * Vector2.One);
        Assert.Equal(new Matrix2x2(8, 4, 3, 2), Sample.Transpose());
    }

    [Fact]
    public void EqualityAndStrings()
    {
        Assert.True(Sample == new Matrix2x2(8, 3, 4, 2));
        Assert.True(Sample != Matrix2x2.Identity);
        Assert.True(Sample.Equals((object) new Matrix2x2(8, 3, 4, 2)));
        Assert.False(Sample.Equals((object) 1));
        Assert.Equal(Sample.GetHashCode(), new Matrix2x2(8, 3, 4, 2).GetHashCode());
        Assert.True(Sample.ApproximatelyEquals(new Matrix2x2(8, 3, 4, 2 + 1e-12)));
        Assert.False(Sample.ApproximatelyEquals(new Matrix2x2(8, 3, 4, 2.1), 0.01, 0));
        Assert.Equal("Matrix2x2 [[8, 3], [4, 2]]", Sample.ToString());
    }
}
