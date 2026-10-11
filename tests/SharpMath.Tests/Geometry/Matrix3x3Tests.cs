using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class Matrix3x3Tests
{
    private static readonly Matrix3x3 A = new(2, 5, 2, 3, -3, 1, 1, 4, -4);
    private static readonly Matrix3x3 B = new(-4, 2.5, 3, 5, 6, 4, 9, 10, -9);

    [Fact]
    public void Determinant_UsesClosedForm()
    {
        Assert.Equal(111, A.Determinant);
        Assert.Equal(566.5, B.Determinant);
        Assert.Equal(-5, A.Trace);
    }

    [Fact]
    public void Multiplication_IsRowByColumn()
    {
        Assert.Equal(new Matrix3x3(35, 55, 8, -18, -0.5, -12, -20, -13.5, 55), A * B);
        Assert.Equal(new Vector3(9, 1, 1), A * Vector3.One);
    }

    [Fact]
    public void RowConstructor_PlacesRowsAsRows()
    {
        // Regression: the vectors used to be placed as columns.
        var matrix = new Matrix3x3(new Vector3(1, 2, 3), new Vector3(4, 5, 6), new Vector3(7, 8, 9));
        Assert.Equal(new Matrix3x3(1, 2, 3, 4, 5, 6, 7, 8, 9), matrix);
        Assert.Equal(2, matrix[0, 1]);
        Assert.Equal(4, matrix[1, 0]);
        Assert.Equal(9, matrix[8]);
        Assert.Throws<ArgumentOutOfRangeException>(() => matrix[0, 3]);
        Assert.Throws<ArgumentOutOfRangeException>(() => matrix[9]);
    }

    [Fact]
    public void Inverse_AdjugateAndCofactors()
    {
        // Regression: Inverse used to return the identity matrix.
        Assert.True((A * A.Inverse()).IsIdentity);
        Assert.True((A.Inverse() * A).IsIdentity);
        Assert.True((A * A.Adjugate()).ApproximatelyEquals(Matrix3x3.Identity * A.Determinant));
        Assert.Equal(A.Adjugate().Transpose(), A.Cofactors());

        var singular = new Matrix3x3(1, 2, 3, 2, 4, 6, 1, 1, 1);
        Assert.True(singular.IsSingular);
        Assert.False(singular.TryInvert(out _));
        Assert.Throws<InvalidOperationException>(() => singular.Inverse());

        // Regression: a permutation matrix used to be reported as not invertible.
        var permutation = new Matrix3x3(0, 1, 0, 1, 0, 0, 0, 0, 1);
        Assert.Equal(permutation, permutation.Inverse());

        // Regression: small but regular matrices used to count as singular.
        Assert.False((Matrix3x3.Identity * 0.001).IsSingular);
    }

    [Fact]
    public void Predicates()
    {
        Assert.True(Matrix3x3.Identity.IsIdentity);
        // Regression: off-diagonal entries used to be ignored.
        Assert.False(new Matrix3x3(1, 5, 0, 0, 1, 0, 0, 0, 1).IsIdentity);
        Assert.True(new Matrix3x3(1, 0, 0, 0, 1, 0, 0, 0, 0).IsDiagonal);
        Assert.True(new Matrix3x3(1, 3, 4, 0, 2, 7, 0, 0, 3).IsTriangular);
        Assert.True(new Matrix3x3(1, 0, 0, 3, 2, 0, 4, 7, 3).IsTriangular);
        Assert.False(A.IsTriangular);
        Assert.True(new Matrix3x3(1, 2, 3, 2, 5, 6, 3, 6, 9).IsSymmetric);
        Assert.False(A.IsSymmetric);
        Assert.True(new Matrix3x3(0, 1, -2, -1, 0, 3, 2, -3, 0).IsSkewSymmetric);
        Assert.True(Matrix3x3.Rotation(1.2).IsOrthogonal);
        Assert.False(A.IsOrthogonal);
    }

    [Fact]
    public void TwoDimensionalTransformations()
    {
        Assert.Equal(new Vector2(11, 11), Matrix3x3.Translation(10, 10) * Vector2.One);
        Assert.Equal(new Vector2(2, 3), Matrix3x3.Scaling(2, 3) * Vector2.One);
        Assert.Equal(new Vector2(2, 2), Matrix3x3.Scaling(2) * Vector2.One);

        // Regression: the old test checked the input vector instead of the rotated one.
        var rotated = Matrix3x3.Rotation(Math.PI) * new Vector2(0, 1);
        Assert.True(rotated.ApproximatelyEquals(new Vector2(0, -1)));

        // Composition from right to left: scale, then rotate, then translate.
        var transform = Matrix3x3.Translation(5, 0) * Matrix3x3.Rotation(Math.PI / 2) * Matrix3x3.Scaling(2);
        Assert.True((transform * Vector2.UnitX).ApproximatelyEquals(new Vector2(5, 2)));

        var projective = new Matrix3x3(1, 0, 0, 0, 1, 0, 0, 0, 2);
        Assert.Equal(new Vector2(0.5, 0.5), projective * Vector2.One);
    }

    [Fact]
    public void ElementWiseOperatorsEqualityAndStrings()
    {
        Assert.Equal(A * 2, A + A);
        Assert.Equal(Matrix3x3.Zero, A - A);
        Assert.Equal(A * -1, -A);
        Assert.Equal(A * 2, 2 * A);
        Assert.Equal(A, A * 2 / 2);
        Assert.Equal(A, Matrix3x3.FromMatrix(MatrixMxN.FromMatrix(A)));
        Assert.Throws<ArgumentException>(() => Matrix3x3.FromMatrix(Matrix2x2.Identity));
        Assert.True(A == new Matrix3x3(2, 5, 2, 3, -3, 1, 1, 4, -4));
        Assert.True(A != B);
        Assert.True(A.Equals((object) new Matrix3x3(2, 5, 2, 3, -3, 1, 1, 4, -4)));
        Assert.Equal(A.GetHashCode(), new Matrix3x3(2, 5, 2, 3, -3, 1, 1, 4, -4).GetHashCode());
        Assert.True(A.ApproximatelyEquals(A + Matrix3x3.Identity * 1e-12));
        Assert.True(A.ApproximatelyEquals(A + Matrix3x3.Identity * 0.01, 0.1, 0));
        Assert.Equal(3, A.RowCount);
        Assert.Equal(3, A.ColumnCount);
        Assert.Equal(3, A.Dimension);
        Assert.Equal("Matrix3x3 [[2, 5, 2], [3, -3, 1], [1, 4, -4]]", A.ToString());
    }
}
